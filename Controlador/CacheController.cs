using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net;
using System.Threading;
using System.Xml;
using NLog;
using System.Web.Caching;
using CSCache.Model;
using System.Security.Cryptography.X509Certificates;
using System.Configuration;
using System.Data.OleDb;
using ImageResizer;
using System.Xml.Linq;
using System.Web.Configuration;

namespace CSCache.Controlador
{
    public class CacheController
    {
        #region Properties
        public const string SALE_RESULT_AUTHORIZED = "1";
        public const string SALE_RESULT_REJECTED = "2";
        public const string GENERIC_CLAVE_VENTA = "SaleByCinestar";

        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static CacheItemRemovedCallback callback = new CacheItemRemovedCallback(OnRemove);
        private static Mutex mutexCacheProductos = new Mutex();
        private static Mutex mutexCachePeliculas = new Mutex();
        private static Mutex mutexCacheEstrenos = new Mutex();
        private static Mutex mutexCacheSemanal = new Mutex();
        private static Mutex mutexDatosTrans = new Mutex();
        private static Mutex mutexComplexGeo = new Mutex();
        private static Mutex mutexProductos = new Mutex();
        private static Mutex mutexPeliculas = new Mutex();
        private static List<Cache_Productos> productos = null;
        private static List<Cache_Peliculas> peliculas = null;
        private static List<Cache_ComplejosGeo> listComplejosGeo = null;
        // private static List<FuncionTrans> datosTrans = null; // AC 06/08/2024
        private static DateTime fechaActualizacion;

        // TODO MF prueba para semaforo de Cache Peliculas
        private static bool iniciandoPeliculas = false;

        //MF 12/2024
        //Cargo el directorio del config
        private static String strDirImg = WebConfigurationManager.AppSettings["ImageDirectory"].ToString();

        #endregion

        public static void InitCache()
        {
            Thread newThread1 = new Thread(InitCachePeliculas2);
            Thread newThread2 = new Thread(InitCacheProductos2);
            newThread1.Start();
            newThread2.Start();
        }

        public static void InitCachePeliculas2()
        {
            // DAO.GuardarLog("InitCachePeliculas2 INICIO", 1003, "Controlador.cs");
            // inicio Semaforo
            iniciandoPeliculas = true;
            List<Cache_Peliculas> list = null;

            try
            {

                mutexCachePeliculas.WaitOne();
                DateTime fechaCacheActualizada = DAO.ObtenerFechaCache("Peliculas");
                DateTime actualDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(DAO.GetParametro("TimeZoneId")));
                var tiempoExpiracionCache = int.Parse(DAO.GetParametro("TiempoExpiracionCache"));

                if (fechaCacheActualizada.AddMinutes(tiempoExpiracionCache) < actualDateTime)
                {
                    int grupoId = new CacheController().getGrupoComplejosId();

                    List<Complex_Options> complejosAux = new List<Complex_Options>();
                    List<Cache_Tecnologias> tecnologiasAux = new List<Cache_Tecnologias>();

                    // MF se agrega para validar los certificados al usar los ws con ssl.
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(ValidateServerCertificate);
                    Info.Info inf = new CSCache.Info.Info();
                    inf.Url = DAO.ObtenerWsInfo();

                    List<Complex_Options> listComplejos = DAO.ObtenerComplejos();
                    foreach (Complex_Options com in listComplejos)
                    {
                        XmlNode nodoSemanas = null;
                        try
                        {
                            nodoSemanas = inf.InfoWeeklyShowTime(com.CodComplejo, actualDateTime.ToString("yyyyMMdd"));
                            // MF 11/2020
                            // Comento los return ya que cortan la carga del resto de los complejos.
                            if (nodoSemanas.InnerXml.Contains("<Error>"))
                            {
                                com.Cache_Cinesemanas = null;
                            }
                            else
                            {
                                com.Cache_Cinesemanas = new Cache_Cinesemanas();
                                com.Cache_Cinesemanas.Desde = DateTime.ParseExact(nodoSemanas.ChildNodes[0]["DateFrom"].InnerText, "yyyyMMdd", null);
                                com.Cache_Cinesemanas.Hasta = DateTime.ParseExact(nodoSemanas.ChildNodes[0]["DateTo"].InnerText, "yyyyMMdd", null);
                                com.Cache_Cinesemanas.Cache_GruposSemanas = new List<Cache_GruposSemana>();
                                while (nodoSemanas.ChildNodes[0]["WeekGroup"] != null)
                                {
                                    Cache_GruposSemana gs = new Cache_GruposSemana();
                                    gs.Desde = DateTime.ParseExact(nodoSemanas.ChildNodes[0]["WeekGroup"]["DateFrom"].InnerText, "yyyyMMdd", null);
                                    gs.Hasta = DateTime.ParseExact(nodoSemanas.ChildNodes[0]["WeekGroup"]["DateTo"].InnerText, "yyyyMMdd", null);
                                    gs.NomGrupo = nodoSemanas.ChildNodes[0]["WeekGroup"]["Name"].InnerText;
                                    gs.Orden = Convert.ToInt32(nodoSemanas.ChildNodes[0]["WeekGroup"]["Order"].InnerText);
                                    nodoSemanas.ChildNodes[0].RemoveChild(nodoSemanas.ChildNodes[0]["WeekGroup"]);
                                    com.Cache_Cinesemanas.Cache_GruposSemanas.Add(gs);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            // MF 2017/04/26
                            //    if (nodoSemanas.ChildNodes[0]["Mensaje"].InnerText != "InfoWeeklyShowTime doesn't exist")
                            //        DAO.GuardarLog("InitCachePeliculas2  916 No hay Cache_Cinesemanas" , 1001, "Controlador.cs");
                            // throw ex;

                            // GR 3/1/2018
                            string msgLog = string.Empty;
                            if (nodoSemanas == null)
                            {
                                msgLog = "InitCachePeliculas2 - inf.InfoWeeklyShowTime retorna null. Complejo: {0} - Actual Date Time: {1}";
                                DAO.GuardarLog(string.Format(msgLog, com.CodComplejo, actualDateTime.ToString("yyyyMMdd")), 3, "Controlador.cs");
                            }
                            else
                            {
                                msgLog = "InitCachePeliculas2 - Error mapeo Complejo con xml nodoSemanas. Complejo: {0} - XmlNodoSemanas: {1}";
                                DAO.GuardarLog(string.Format(msgLog, com.CodComplejo, nodoSemanas.OuterXml), 3, "Controlador.cs");
                            }

                            DAO.GuardarLog("InitCachePeliculas2 Error en Cache_Cinesemanas" + ex.ToString() + ex.StackTrace + " - " + nodoSemanas, 1001, "Controlador.cs");
                            com.Cache_Cinesemanas = null;

                            //throw ex;
                        }
                    }

                    // if (nodo.InnerXml.Contains("<error>"))
                    // return;

                    XmlNode nodo = inf.Movies(grupoId, "");
                    list = new List<Cache_Peliculas>();

                    for (int i = 0; i < nodo.ChildNodes.Count; i++)
                    {
                        Cache_Peliculas peli = new Cache_Peliculas();
                        XmlNode nodoPel = inf.MovieDetail(grupoId, Convert.ToInt32(nodo.ChildNodes[i]["FeatureId"].InnerText));
                        if (nodoPel.InnerXml.Contains("<Error>"))
                            return;

                        peli.CodPelicula = Convert.ToInt32(nodoPel.ChildNodes[0]["FeatureId"].InnerText);
                        peli.Titulo = nodoPel.ChildNodes[0]["Title"].InnerText;
                        peli.TituloOriginal = nodoPel.ChildNodes[0]["OriginalTitle"].InnerText;
                        peli.Subtitulada = nodoPel.ChildNodes[0]["SubTitled"].InnerText.Trim() == "1";
                        peli.Duracion = Convert.ToInt32(nodoPel.ChildNodes[0]["TotalRuntime"].InnerText);
                        string auxDate = nodoPel.ChildNodes[0]["PremierDate"].InnerText.Trim();
                        peli.Estreno = new DateTime(Convert.ToInt32(auxDate.Substring(0, 4)), Convert.ToInt32(auxDate.Substring(4, 2)), Convert.ToInt32(auxDate.Substring(6, 2)));
                        peli.Sinopsis = nodoPel.ChildNodes[0]["Synopsis"].InnerText;
                        peli.SinopsisCorta = nodoPel.ChildNodes[0]["ShortSynopsis"].InnerText;
                        peli.Web1 = nodoPel.ChildNodes[0]["WebSite1"].InnerText;
                        peli.Web2 = nodoPel.ChildNodes[0]["WebSite2"].InnerText;
                        peli.UrlTrailer = nodoPel.ChildNodes[0]["URLTrailer"].InnerText;
                        short aux = Convert.ToInt16(nodoPel.ChildNodes[0]["RatingID"].InnerText);

                        Cache_Clasificaciones clas = null;
                        Cache_Peliculas pelAux = list.Find((Predicate<Cache_Peliculas>)(x => x.Cache_Clasificaciones != null && x.Cache_Clasificaciones.CodClasificacion == aux));

                        if (pelAux != null)
                        {
                            peli.Cache_Clasificaciones = pelAux.Cache_Clasificaciones;
                        }
                        else
                        {
                            clas = new Cache_Clasificaciones();
                            clas.CodClasificacion = aux;
                            clas.NomClasificacion = nodoPel.ChildNodes[0]["Rating"].InnerText;
                            peli.Cache_Clasificaciones = clas;
                        }

                        aux = Convert.ToInt16(nodoPel.ChildNodes[0]["GenreID"].InnerText);
                        pelAux = list.Find(x => x.CodGenero == aux);
                        
                        if (pelAux != null)
                        {
                            peli.Cache_Generos = pelAux.Cache_Generos;
                        }
                        else
                        {
                            Cache_Generos gene = new Cache_Generos();
                            gene.CodGenero = aux;
                            gene.NomGenero = nodoPel.ChildNodes[0]["Genre"].InnerText;
                            peli.Cache_Generos = gene;
                        }

                        aux = Convert.ToInt16(nodoPel.ChildNodes[0]["LanguageID"].InnerText);
                        Cache_Lenguajes leng = null;
                        pelAux = list.Find(x => x.CodLenguaje == aux);
                        
                        if (pelAux != null)
                        {
                            peli.Cache_Lenguajes = pelAux.Cache_Lenguajes;
                        }
                        else
                        {
                            leng = new Cache_Lenguajes();
                            leng.CodLenguaje = aux;
                            leng.NomLenguaje = nodoPel.ChildNodes[0]["Language"].InnerText;
                            peli.Cache_Lenguajes = leng;
                        }

                        peli.Actores = new List<string>();
                        peli.Directores = new List<string>();

                        while (nodoPel.ChildNodes[0]["Actor"] != null)
                        {
                            peli.Actores.Add(nodoPel.ChildNodes[0]["Actor"]["ActorName"].InnerText);
                            nodoPel.ChildNodes[0].RemoveChild(nodoPel.ChildNodes[0]["Actor"]);
                        }

                        while (nodoPel.ChildNodes[0]["Director"] != null)
                        {
                            peli.Directores.Add(nodoPel.ChildNodes[0]["Director"]["DirectorName"].InnerText);
                            nodoPel.ChildNodes[0].RemoveChild(nodoPel.ChildNodes[0]["Director"]);
                        }

                        peli.Cache_Funciones = new List<Cache_Funciones>();
                        XmlNode nodoFunciones = inf.ShowTimeByDateAndMovie(grupoId, "", Convert.ToInt32(nodoPel.ChildNodes[0]["FeatureId"].InnerText), "");

                        // if (nodoFunciones.InnerXml.Contains("<error>"))
                        // return;

                        for (int j = 0; j < nodoFunciones.ChildNodes.Count; j++)
                        {
                            Cache_Funciones func = new Cache_Funciones();
                            aux = Convert.ToInt16(nodoFunciones.ChildNodes[j]["TheatreId"].InnerText);
                            Complex_Options comp = listComplejos.Find(x => x.CodComplejo == aux);
                            
                            if (comp != null)
                            {
                                func.Complex_Options = comp;
                            }
                            else
                            {
                                // Este error se da si dentro del Grupo de complejos del wsInfo hay uno que no está configurado en la tabla complejos de la base CMS_WEB. 
                                throw new Exception("Funcion sin complejo en el grupo." + "CodComplejo de la funcion:" + aux);
                            }

                            func.CodFuncion = Convert.ToInt32(nodoFunciones.ChildNodes[j]["ScheduleId"].InnerText);
                            func.HoraComienzo = nodoFunciones.ChildNodes[j]["StartTime"].InnerText.Trim();
                            func.Vuelta = Convert.ToInt16(nodoFunciones.ChildNodes[j]["Performance"].InnerText);
                            func.Estado = nodoFunciones.ChildNodes[j]["Status"].InnerText.Trim();
                            func.Preestreno = nodoFunciones.ChildNodes[j]["Preview"].InnerText.Trim() == "1";
                            func.ButacasDisponibles = Convert.ToInt32(nodoFunciones.ChildNodes[j]["Seats"].InnerText);
                            func.CodDistribucion = Convert.ToInt32(nodoFunciones.ChildNodes[j]["Distribution"].InnerText);
                            // func.ButacasHabilitadas = Convert.ToInt32(nodoFunciones.ChildNodes[j]["Habilitadas"].InnerText);
                            aux = Convert.ToInt16(nodoFunciones.ChildNodes[j]["ScreenID"].InnerText);
                            func.CodCopia = Convert.ToInt32(nodoFunciones.ChildNodes[j]["FeatureCopy"].InnerText);
                            Cache_Salas sala = null;
                            sala = comp.Cache_Salas.Find(x => x.CodSala == aux);

                            if (sala != null)
                            {
                                func.Cache_Salas = sala;
                            }
                            else
                            {
                                sala = new Cache_Salas();
                                sala.CodSala = aux;
                                sala.NomSala = nodoFunciones.ChildNodes[j]["ScreenName"].InnerText;
                                sala.CodTipoSala = Convert.ToInt32(nodoFunciones.ChildNodes[j]["ScreenTypeID"].InnerText);
                                sala.NomTipoSala = nodoFunciones.ChildNodes[j]["ScreenType"].InnerText;
                                func.Cache_Salas = sala;
                                comp.Cache_Salas.Add(sala);
                            }

                            aux = Convert.ToInt16(nodoFunciones.ChildNodes[j]["TechnologyID"].InnerText);
                            Cache_Tecnologias tecn = tecnologiasAux.Find(x => x.CodTecnologia == aux);

                            if (tecn != null)
                            {
                                func.Cache_Tecnologias = tecn;
                            }
                            else
                            {
                                tecn = new Cache_Tecnologias();
                                tecn.CodTecnologia = aux;
                                tecn.NomTecnologia = nodoFunciones.ChildNodes[j]["TechnologyName"].InnerText;
                                func.Cache_Tecnologias = tecn;
                            }

                            string auxFecha = nodoFunciones.ChildNodes[j]["ScheduleDate"].InnerText.Trim();

                            try
                            {
                                func.Fecha = DateTime.ParseExact(auxFecha, "yyyyMMdd", null);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.ToString());
                                func.Fecha = DateTime.ParseExact(auxFecha, "dd/MM/yyyy", null);
                            }

                            peli.Cache_Funciones.Add(func);
                        }

                        list.Add(peli);

                        try
                        {
                            bool isPosterReq = true;
								if (System.IO.File.Exists(strDirImg + "/posters/" + grupoId + "_" + peli.CodPelicula + ".jpg"))
                            {
								DateTime utcModificado = File.GetLastWriteTimeUtc(strDirImg + "/posters/" + grupoId + "_" + peli.CodPelicula + ".jpg");
                                DateTime modificado = TimeZoneInfo.ConvertTimeFromUtc(utcModificado, TimeZoneInfo.FindSystemTimeZoneById(DAO.GetParametro("TimeZoneId")));
                                isPosterReq = inf.IsPosterRequired(grupoId, peli.CodPelicula, modificado.Year.ToString() + modificado.Month.ToString().PadLeft(2, '0') + modificado.Day.ToString().PadLeft(2, '0'), modificado.Hour.ToString().PadLeft(2, '0') + modificado.Minute.ToString().PadLeft(2, '0')) == "1";
                            }

                            var filename = grupoId + "_" + peli.CodPelicula + ".jpg";

                            #region Update Filename field of the film
                            foreach (var p in list)
                            {
                                if (p.CodPelicula == peli.CodPelicula)
                                {
                                    p.Filename = filename;
                                    break;
                                }
                            }
                            #endregion

                            if (isPosterReq) // Agustin Martinez - bckp
                            {
                                Object obj1 = inf.GetPoster(grupoId, peli.CodPelicula); // Agustin Martinez - bckp
                                byte[] arrAfiche = (byte[])obj1;

                                //if (arrAfiche != null)
                                if (arrAfiche.Length > 0)
                                {
                                    try
                                    {
                                        var context = new CSWebNuevoEntities();
                                        var deviceImageSettings = context.DeviceImageSettings.ToList();
                                        var poster = DeviceImageSettingsResolution.Poster.ToString();
                                        var posterImgSettings = deviceImageSettings.Single(x => x.ImageTypeCode.Trim() == poster);

                                        #region Create Desktop image
										var desktopFileName = strDirImg + "/posters/" + filename;
                                        if (File.Exists(desktopFileName))
                                            File.Delete(desktopFileName);

                                        ImageBuilder.Current.Build(arrAfiche, desktopFileName, new ResizeSettings("width=" + posterImgSettings.DesktopWidth.Value));
                                        #endregion

                                        #region Create Tablet image
										var tableFileName = strDirImg + "/posters/" + grupoId + "_" + peli.CodPelicula + "-" + DevicesTypes.Tablet.ToString().ToLower() + ".jpg";
                                        if (File.Exists(tableFileName))
                                            File.Delete(tableFileName);

                                        ImageBuilder.Current.Build(arrAfiche, tableFileName, new ResizeSettings("width=" + posterImgSettings.TabletWidth.Value));
                                        #endregion

                                        #region Create Smartphone image
										var smartphoneFileName = strDirImg + "/posters/" + grupoId + "_" + peli.CodPelicula + "-" + DevicesTypes.Smartphone.ToString().ToLower() + ".jpg";
                                        if (File.Exists(smartphoneFileName))
                                            File.Delete(smartphoneFileName);

                                        ImageBuilder.Current.Build(arrAfiche, smartphoneFileName, new ResizeSettings("width=" + posterImgSettings.SmartphoneWidth.Value));
                                        #endregion

                                        #region Create Cellphone image
										var cellphoneFileName = strDirImg + "/posters/" + grupoId + "_" + peli.CodPelicula + "-" + DevicesTypes.Cellphone.ToString().ToLower() + ".jpg";
                                        if (File.Exists(cellphoneFileName))
                                            File.Delete(cellphoneFileName);

                                        ImageBuilder.Current.Build(arrAfiche, cellphoneFileName, new ResizeSettings("width=" + posterImgSettings.CellphoneWidth.Value));
                                        #endregion
                                    }
                                    catch (Exception ex)
                                    {
                                        DAO.GuardarLog("InitCachePeliculas2 ActualizarImágenes Crear Device Images" + ex.ToString() + ex.StackTrace, 1001, "Controlador.cs");
                                        // throw ex;
                                    }
                                }
                                else
                                {
                                    #region Poster dummy image
									using (FileStream fs = new FileStream(strDirImg + "/NOPOSTER.jpg", FileMode.Open, FileAccess.Read))
                                    {
                                        using (Image original = Image.FromStream(fs))
                                        {
                                            Graphics posterAMano = Graphics.FromImage(original);
                                            StringFormat stringFormat = new StringFormat();
                                            stringFormat.Alignment = StringAlignment.Center;
                                            stringFormat.LineAlignment = StringAlignment.Center;
                                            posterAMano.DrawString(peli.Titulo, new Font("Comic Sans", 7, FontStyle.Bold), Brushes.Black, new RectangleF(20, 0, original.Width - 40, original.Height), stringFormat);

											original.Save(strDirImg + "/posters/" + grupoId + "_" + peli.CodPelicula + ".jpg");
                                        }
                                    }
                                    #endregion
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            DAO.GuardarLog("InitCachePeliculas2 ActualizarImágenes " + ex.ToString() + ex.StackTrace, 1001, "Controlador.cs");
                        }
                    }

                    #region Classifications
                    try
                    {
                        DataSet ds = inf.GetRatingsImages(grupoId);
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            int cod = Convert.ToInt32(ds.Tables[0].Rows[i][0]);
                            byte[] img = (byte[])ds.Tables[0].Rows[i][1];
							System.IO.FileStream newFile = new System.IO.FileStream(strDirImg + "/clasificaciones/" + cod + ".jpg", System.IO.FileMode.Create);
                            newFile.Write(img, 0, img.Length);
                            newFile.Close();
                        }

                    }
                    catch (Exception ex)
                    {
                        DAO.GuardarLog("InitCachePeliculas2 GetRatingsImages " + ex.ToString() + ex.StackTrace, 1001, "Controlador.cs");
                    }
                    #endregion

                    #region Generos
                    try
                    {
                        DataSet ds = inf.GetGenresImages(grupoId);
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            int cod = Convert.ToInt32(ds.Tables[0].Rows[i][0]);
                            byte[] img = (byte[])ds.Tables[0].Rows[i][1];
							System.IO.FileStream newFile = new System.IO.FileStream(strDirImg + "/generos/" + cod + ".jpg", System.IO.FileMode.Create);
                            newFile.Write(img, 0, img.Length);
                            newFile.Close();
                        }

                    }
                    catch (Exception ex)
                    {
                        DAO.GuardarLog("InitCachePeliculas2 Generos" + ex.ToString() + ex.StackTrace, 1001, "Controlador.cs");
                    }
                    #endregion

                    // AJUSTES AC 06/11/2024
                    List<Cache_Peliculas> listAux = list.GroupBy(p => p.CodPelicula).Select(g => g.First()).OrderBy(p => p.CodPelicula).ToList();
                    List<Cache_CopiasPelicula> listCopy = new List<Cache_CopiasPelicula>();
                    foreach (Cache_Peliculas peli in listAux)
                        listCopy.AddRange(parseXML(inf.MovieDetailv2(grupoId, peli.CodPelicula)));

                    SetPeliculas(listAux, actualDateTime);
                    DAO.GuardarCache(listAux, listCopy, listComplejosGeo, actualDateTime);
                }
            }
            catch (Exception ex)
            {
                if (list != null)
                {
                    List<int> codePeliculas = list.Select(x => x.CodPelicula).ToList();
                    string codPeliculasLog = string.Join(", ", codePeliculas);
                    string logLine = "InitCachePeliculas2 - CodPeliculas {0}";
                    DAO.GuardarLog(string.Format(logLine, codPeliculasLog), 3, "Controlador.cs");
                }

                DAO.GuardarLog("InitCachePeliculas2 cod 1482 " + ex.ToString() + ex.StackTrace, 1001, "Controlador.cs");
                // throw ex;
            }
            finally
            {
                mutexCachePeliculas.ReleaseMutex();
            }

            iniciandoPeliculas = false;
            // DAO.GuardarLog("InitCachePeliculas2 FIN", 1003, "Controlador.cs");
        }

        public static void InitCacheProductos2()
        {
            mutexCacheProductos.WaitOne();
            List<Cache_Productos> listprod = new List<Cache_Productos>();

            try
            {
                // MF obtengo la cache por el id
                DateTime fechaCacheActualizada = DAO.ObtenerFechaCache("Productos");
                DateTime actualDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(DAO.GetParametro("TimeZoneId")));
                var tiempoExpiracionCache = int.Parse(DAO.GetParametro("TiempoExpiracionCache"));
                if (fechaCacheActualizada.AddMinutes(tiempoExpiracionCache) < actualDateTime)
                {
                    int grupoId = new CacheController().getGrupoComplejosId();
                    List<Complex_Options> complejosAux = new List<Complex_Options>();

                    // MF se agrega para validar los certificados al usar los ws con ssl.
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(ValidateServerCertificate);
                    Info.Info inf = new CSCache.Info.Info();
                    inf.Url = DAO.ObtenerWsInfo();
                    List<Complex_Options> listComplejos = DAO.ObtenerComplejos();

                    // primero se obtiene los productos asociadas al KeyLayout ATM de la terminal web
                    // segundo obtengo los productos vendibles para el ATM y actualizo los datos que faltan en la lista de productos (nombre y precio)
                    // tercero guardo la lista filtrada que va a actualizar la cache.
                    // cuarto verifico si exite o se debe actualizar la imagen del producto.
                    int auxcodcomplejo = 0;

                    foreach (Complex_Options com in listComplejos)
                    {
                        auxcodcomplejo = com.CodComplejo;
                        XmlNode nodoKeyLayout = null;
                        try
                        {
                            int auxCol = 0;
                            nodoKeyLayout = inf.GetKeyLayouts(auxcodcomplejo, com.CodTerminal, "KLATM");
                            int auxRow = 0;

                            XmlNodeList lista = ((XmlElement)nodoKeyLayout).GetElementsByTagName("Detail");
                            foreach (XmlElement nodo in lista)
                            {
                                XmlNodeList nItem = nodo.GetElementsByTagName("CodProducto");
                                Cache_Productos prod = new Cache_Productos();
                                prod.CodComplejo = auxcodcomplejo;
                                prod.CodProducto = Convert.ToInt32(nItem[0].InnerText);
                                prod.NomProducto = "";
                                prod.Precio = 0;
                                prod.NombreArchivo = string.Format("{0}_{1}.jpg", auxcodcomplejo, prod.CodProducto); // AC 13/11/2024
                                nItem = nodo.GetElementsByTagName("ColDetalleKeyLayout");
                                auxCol = Convert.ToInt32(nItem[0].InnerText);
                                nItem = nodo.GetElementsByTagName("RowDetalleKeyLayout");
                                auxRow = Convert.ToInt32(nItem[0].InnerText);

                                // con las coordenadas se establece la posición para ordenar los productos
                                prod.Posicion = AsignarPosicion(auxCol, auxRow);
                                listprod.Add(prod);
                            }

                            if (listprod.Any())
                            {
                                XmlNode nodoProductos = null;
                                int auxcodprod = 0;

                                try
                                {
                                    nodoProductos = inf.GetProducts(auxcodcomplejo, "CATM");
                                    for (int i = 0; i < nodoProductos.ChildNodes.Count; i++)
                                    {
                                        auxcodprod = Convert.ToInt32(nodoProductos.ChildNodes[i]["CodProducto"].InnerText);

                                        foreach (var pItem in listprod.Where(x => x.CodComplejo == auxcodcomplejo))
                                        {
                                            if (auxcodprod == pItem.CodProducto)
                                            {
                                                pItem.NomProducto = nodoProductos.ChildNodes[i]["NomProducto"].InnerText;
                                                pItem.Precio = decimal.Parse(nodoProductos.ChildNodes[i]["ImpPreciosXProducto"].InnerText, CultureInfo.InvariantCulture);
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    DAO.GuardarLog("InitCacheProductos 2 - Actualizo Nombre y Precio" + ex.ToString() + ex.StackTrace, 1001, "Controlador.cs");
                                    // throw ex;
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            DAO.GuardarLog("InitCacheProductos 2 - For each complejos " + ex.ToString() + ex.StackTrace, 1001, "Controlador.cs");
                            // throw ex;
                        }
                    }

                    if (listprod.Any())
                    {
                        foreach (var pItem in listprod)
                        {
                            try
                            {
                                bool isPosterReq = true;
									if (System.IO.File.Exists(strDirImg + "/products/" + pItem.CodComplejo + "_" + pItem.CodProducto + ".jpg"))
                                {
									DateTime utcModificado = System.IO.File.GetLastWriteTimeUtc(strDirImg + "/products/" + pItem.CodComplejo + "_" + pItem.CodProducto + ".jpg");
                                    DateTime modificado = TimeZoneInfo.ConvertTimeFromUtc(utcModificado, TimeZoneInfo.FindSystemTimeZoneById(DAO.GetParametro("TimeZoneId")));
                                    isPosterReq = inf.IsProductImageRequired(pItem.CodComplejo, pItem.CodProducto, modificado.Year.ToString() + modificado.Month.ToString().PadLeft(2, '0') + modificado.Day.ToString().PadLeft(2, '0'), modificado.Hour.ToString().PadLeft(2, '0') + modificado.Minute.ToString().PadLeft(2, '0')) == "1";
                                }
                                if (isPosterReq)
                                {
                                    XmlNode nodoImagen = inf.GetProductImage(pItem.CodComplejo, pItem.CodProducto);
                                    byte[] arrAfiche = Convert.FromBase64String(nodoImagen.ChildNodes[0]["Imagen"].InnerText);
                                    //if (arrAfiche != null)
                                    if (arrAfiche.Length > 0)
                                    {
										System.IO.FileStream newFile = new System.IO.FileStream(strDirImg + "/products/" + pItem.CodComplejo + "_" + pItem.CodProducto + ".jpg", System.IO.FileMode.Create);
                                        newFile.Write(arrAfiche, 0, arrAfiche.Length);
                                        newFile.Close();
                                    }
                                    else
                                    {
										using (FileStream fs = new FileStream(strDirImg + "/NOPRODUCT.jpg", FileMode.Open, FileAccess.Read))
                                        {
                                            using (Image original = Image.FromStream(fs))
                                            {
                                                Graphics posterAMano = Graphics.FromImage(original);
                                                StringFormat stringFormat = new StringFormat();
                                                stringFormat.Alignment = StringAlignment.Center;
                                                stringFormat.LineAlignment = StringAlignment.Center;
                                                posterAMano.DrawString(pItem.NomProducto, new Font("Comic Sans", 7, FontStyle.Bold), Brushes.Black, new RectangleF(20, 0, original.Width - 40, original.Height), stringFormat);

												original.Save(strDirImg + "/products/" + pItem.CodComplejo + "_" + pItem.CodProducto + ".jpg");
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                DAO.GuardarLog("InitCacheProductos2 ActualizarImágenes " + ex.ToString() + ex.StackTrace, 1001, "Controlador.cs");
                                // throw ex;
                            }
                        }
                    }
                }
                // TODO MF evito error que se borran los productos, cuando no se pueden obtener del ws.
                // falta encontrar el error.
                if (listprod.Count > 0)
                {
                    mutexProductos.WaitOne();
                    productos = listprod;
                    DAO.GuardarCacheProductos(listprod, actualDateTime);
                    mutexProductos.ReleaseMutex();
                }
            }
            catch (Exception ex)
            {
                DAO.GuardarLog("InitCacheProductos2 GuardarCacheProductos 1419 " + ex.ToString() + ex.StackTrace, 1001, "Controlador.cs");
            }
            finally
            {
                mutexCacheProductos.ReleaseMutex();
            }
        }

        public static void OnRemove(string key, object cacheItem, CacheItemRemovedReason reason)
        {
            if (key == "Peliculas" && reason == CacheItemRemovedReason.Expired)
            {
                DAO.GuardarLog("OnRemove InitCachePeliculas2", 8001, "controlador.cs");
                InitCachePeliculas2();
            }
        }

        public static bool ValidateServerCertificate(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public int getGrupoComplejosId()
        {
            return DAO.ObtenerGrupoComplejosId();
        }

        public static void SetPeliculas(List<Cache_Peliculas> list, DateTime fecha)
        {
            mutexPeliculas.WaitOne();
            peliculas = list;
            fechaActualizacion = fecha;
            mutexPeliculas.ReleaseMutex();
        }

        private static short AsignarPosicion(int col, int fila)
        {
            //MF se pasa la columna y fila del DetalleKeyLayout a un único valor para determinar la posición de los productos
            short auxPos = 1;

            if (col == 0 & fila == 0)
                auxPos = 1;
            if (col == 1 & fila == 0)
                auxPos = 2;
            if (col == 2 & fila == 0)
                auxPos = 3;
            if (col == 0 & fila == 1)
                auxPos = 4;
            if (col == 1 & fila == 1)
                auxPos = 5;
            if (col == 2 & fila == 1)
                auxPos = 6;
            if (col == 0 & fila == 2)
                auxPos = 7;
            if (col == 1 & fila == 2)
                auxPos = 8;
            if (col == 2 & fila == 2)
                auxPos = 9;

            return auxPos;
        }
    
        private static List<Cache_CopiasPelicula> parseXML(XmlNode xmlResult)
        {
            List<Cache_CopiasPelicula> copies = new List<Cache_CopiasPelicula>();

            XDocument doc = XDocument.Parse(xmlResult.OuterXml);
            foreach (var featureCopy in doc.Descendants("FeatureCopy"))
            {
                Cache_CopiasPelicula copia = new Cache_CopiasPelicula();
                copia.CodPelicula = Convert.ToInt32(featureCopy.Element("FeatureId").Value);
                copia.CodCopia = Convert.ToInt32(featureCopy.Element("CopyID").Value);
                copia.IdTecnologia = Convert.ToInt32(featureCopy.Element("TechnologyID").Value);
                copia.Titulo = featureCopy.Element("Title").Value;
                copia.CodIdioma = Convert.ToInt32(featureCopy.Element("LanguageID").Value);
                copia.Subtitulada = Convert.ToInt32(featureCopy.Element("SubTitled").Value) == 1;
                copia.Doblada = Convert.ToInt32(featureCopy.Element("Dubbed").Value) == 1;

                copies.Add(copia);
            }

            return copies;
        }
    }
}