using System;
using System.Data.SqlClient;
using System.Linq;
using System.Collections.Generic;
using CSCache.Model;

namespace CSCache.Controlador
{
    public class DAO : IDisponsable
    {
        private static readonly string ConnectionString = 
            "Server=192.168.0.230,1453;Database=CMS_Atlas;User Id=ext_free;Password=D3v_Fr33_SQL14#;";
        private readonly SqlConnection _sharedConnection;

        public DAO()
        {
            _sharedConnection = new SqlConnection(ConnectionString);
        }
        
        public void Dispose()
        {
            _sharedConnection?.Dispose();
        }
        
        public static DateTime ObtenerFechaCache(string id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Conexión exitosa a la base de datos 'cache'");
                    string sql = "SELECT Top 1 FechaInicio FROM caches WHERE IdCache = " + id;

                    using (var command = new SqlCommand(sql, connection))
                    {
                        var result = command.ExecuteScalar().ToDateTime();
                        return result ?? throw new InvalidOperationException("No cache found.");
                    }
                }
                catch (Exception ex)
                {
                    GuardarLog("DAO.ObtenerFechaCache " + ex.ToString(), 1004, "DAO.cs");
                    return DateTime.Now;
                }  
            }

        }

        public static string ObtenerWsInfo()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {

                    connection.Open();

                    string sql = "SELECT TOP 1  wsinfo FROM web_site_options";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        var result = command.ExecuteScalar().ToString();
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    GuardarLog("DAO.ObtenerWsInfo " + ex.ToString(), 1004, "DAO.cs");
                    throw ex;
                    
                }
            }
        }

        public static int ObtenerGrupoComplejosId()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string sql = "SELECT TOP 1  theatreGroup FROM web_site_options";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        var result = command.ExecuteScalar();
                        if(result != null)
                        {
                            return Convert.ToInt32(result);
                        }
                        throw new Exception("theatreGroup is not found");
                          
                    }
                }
                catch (Exception ex)
                {
                    GuardarLog("DAO.ObtenerGrupoComplejosId " + ex.ToString(), 1004, "DAO.cs");
                    throw ex;

                }
            }
        }

        public static string GetParametro(string key)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    
                    string sql = "SELECT  ParamValue FROM Parametros WHERE ParamKey = " + key;

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        var result = command.ExecuteScalar().ToString();
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    GuardarLog("DAO.GetParametro " + ex.ToString(), 1004, "DAO.cs");
                    throw ex;
                }
            }
        }

        public static void GuardarLog(string mensaje, int tipoError, string pagina)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string sql = "INSERT  INTO LogWeb (TipoError, Pagina, Fecha, FechaServidor, Aplicacion, Mensaje) VALUES (@TipoError,@Pagina, @Fecha, @FechaServidor, @Aplicacion, @Mensaje)";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@TipoError",tipoError);
                        command.Parameters.AddWithValue("@Pagina", pagina);
                        command.Parameters.AddWithValue("@Fecha", DateTime.Now);
                        command.Parameters.AddWithValue("@FechaServidor", DateTime.Now);
                        command.Parameters.AddWithValue("@Aplicacion", "CSWebNueva");
                        command.Parameters.AddWithValue("@Mensaje", mensaje);
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    GuardarLog("DAO.GuardarLog " + ex.ToString(), 1004, "DAO.cs");
                    throw ex;

                }
            }
        }

        public static List<Complex_Options> ObtenerComplejos()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    List<Complex_Options> complejos = new List<Complex_Options>();
                    string sql = "SELECT CodComplejo, CodTerminal, Nombre, Email, PassEmail, SMTPServer, EnableSSL, PortNumber, GoogleAnalytics, Allow_Sale, Allow_ConcessionSale, Request_TaxId, Tolerancia,MaxSales, Hourly, ComplexDetails, Request_MinRangeTax, Request_MaxRangeTax, Barcode,PickupCode, AuxEmail, AuxSMTPServer, AuxPort, AuxPassword, AuxEnableSSL, MinimunSeatToSell, Allow_Refund FROM Complex_Options";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var complejo = new Complex_Options();
                                complejo.CodComplejo = reader.GetInt32(0);
                                complejo.CodTerminal = reader.GetInt32(1);
                                complejo.Nombre = reader.GetString(2);
                                complejo.Email = reader.GetString(3);
                                complejo.PassEmail = reader.GetString(4);
                                complejo.SMTPServer = reader.GetString(5);
                                complejo.EnableSSL = reader.GetBoolean(6); //reader.IsDBNull(6) ? null : reader.GetBoolean(6);
                                complejo.PortNumber = reader.GetString(7);
                                complejo.GoogleAnalytics = reader.GetString(8);
                                complejo.Allow_Sale = reader.GetBoolean(9);
                                complejo.Allow_ConcessionSale = reader.GetBoolean(10);
                                complejo.Request_TaxId = reader.GetBoolean(11);
                                complejo.Tolerancia = reader.GetInt32(12);
                                complejo.MaxSales = reader.GetInt32(13);
                                complejo.Hourly = reader.GetString(14);
                                complejo.ComplexDetails = reader.GetString(15);
                                complejo.Request_MinRangeTax = reader.GetInt32(16);
                                complejo.Request_MaxRangeTax = reader.GetInt32(17);
                                complejo.Barcode = reader.GetString(18);
                                complejo.PickupCode = reader.GetInt32(19);
                                complejo.AuxEmail = reader.GetString(20);
                                complejo.AuxSMTPServer = reader.GetString(21);
                                complejo.AuxPort = reader.GetInt32(22);
                                complejo.AuxPassword = reader.GetString(23);
                                complejo.AuxEnableSSL = reader.GetBoolean(24);
                                complejo.MinimunSeatToSell = reader.GetInt32(25);
                                complejo.Allow_Refund = reader.GetBoolean(26);
                                complejo.Direccion = reader.GetString(27);
                                complejo.Cache_Salas = new List<Cache_Salas>();

                                complejos.Add(complejo);
                            }
                           return complejos;
                        }
                    }
                }
                catch (Exception ex)
                {
                    GuardarLog("DAO.ObtenerComplejos " + ex.ToString(), 1004, "DAO.cs");
                    throw ex;
                }
            }
        }

        public static List<Cache_Peliculas> ObtenerPeliculas()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    List<Cache_Peliculas> peliculas = new List<Cache_Peliculas>();

                    string sql = "SELECT CodPelicula, Titulo, TituloOriginal, Subtitulada, Duracion, Estreno, CodClasificacion, Sinopsis, SinopsisCorta, Web1, Web2, UrlTrailer, CodGenero, CodLenguaje, Filename FROM Cache_Peliculas ";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var pelicula = new Cache_Peliculas();
                                pelicula.CodPelicula = reader.GetInt32(0);
                                pelicula.Titulo = reader.GetString(1);
                                pelicula.TituloOriginal = reader.GetString(2);
                                pelicula.Subtitulada = reader.GetBoolean(3);
                                pelicula.Duracion = reader.GetInt32(4);
                                pelicula.Estreno = reader.GetDateTime(5);
                                pelicula.CodClasificacion = reader.GetInt16(6);
                                pelicula.Sinopsis = reader.GetString(7);
                                pelicula.SinopsisCorta = reader.GetString(8);
                                pelicula.Web1 = reader.GetString(9);
                                pelicula.Web2 = reader.GetString(10);
                                pelicula.UrlTrailer = reader.GetString(11);
                                pelicula.CodGenero = reader.GetInt32(12);
                                pelicula.CodLenguaje = reader.GetInt32(13);
                                pelicula.Filename = reader.GetString(14);

                                peliculas.Add(pelicula);
                            }
                            return peliculas;
                        }
                    }
                }
                catch (Exception ex)
                {
                    GuardarLog("DAO.ObtenerPeliculas " + ex.ToString(), 1004, "DAO.cs");
                    throw ex;
                }
            }
        }

        public static void GuardarCache(List<Cache_Peliculas> list, List<Cache_CopiasPelicula> copies, List<Cache_ComplejosGeo> listCompGeo, DateTime fecha)
        {
            GuardarLog("GuardarCache Inicio", 1004, "DAO.cs");

            using (CSWebNuevoEntities db = new CSWebNuevoEntities())
            {
                try
                {
                    db.Cache_Actores.RemoveRange(db.Cache_Actores.ToList());
                    db.Cache_Directores.RemoveRange(db.Cache_Directores.ToList());
                    db.Cache_Funciones.RemoveRange(db.Cache_Funciones.ToList());
                    db.Cache_GruposSemana.RemoveRange(db.Cache_GruposSemana.ToList());
                    db.Cache_Cinesemanas.RemoveRange(db.Cache_Cinesemanas.ToList());
                    db.Cache_Salas.RemoveRange(db.Cache_Salas.ToList());
                    db.Cache_Tecnologias.RemoveRange(db.Cache_Tecnologias.ToList());
                    db.Cache_Peliculas.RemoveRange(db.Cache_Peliculas.ToList());
                    db.Cache_CopiasPelicula.RemoveRange(db.Cache_CopiasPelicula.ToList());
                    db.Cache_Clasificaciones.RemoveRange(db.Cache_Clasificaciones.ToList());
                    db.Cache_Generos.RemoveRange(db.Cache_Generos.ToList());
                    db.Cache_Lenguajes.RemoveRange(db.Cache_Lenguajes.ToList());
                    db.SaveChanges();

                    foreach (Cache_Peliculas peli in list)
                    {
                        GuardarPelicula(db, peli);
                    }

                    db.Cache_CopiasPelicula.AddRange(copies);

                    Caches cache = db.Caches.AsNoTracking().Single(c => c.IdCache == "Peliculas");
                    cache.FechaInicio = fecha;
                    db.Entry(cache).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    GuardarLog("GuardarCache " + ex.ToString() + ex.StackTrace, 1002, "DAO.cs");
                    throw ex;
                }
            }

            GuardarLog("GuardarCache FIN", 1004, "DAO.cs");
        }

        private static void GuardarPelicula(CSWebNuevoEntities db, Cache_Peliculas peli)
        {
            GuardarClasificacion(db, peli.Cache_Clasificaciones);
            GuardarGenero(db, peli.Cache_Generos);
            GuardarLenguaje(db, peli.Cache_Lenguajes);
            GuardarDatosPelicula(db, peli);
            GuardarFunciones(db, peli.Cache_Funciones, peli.CodPelicula);
        }

        private static void GuardarClasificacion(CSWebNuevoEntities db, Cache_Clasificaciones clas)
        {
            Cache_Clasificaciones exist = new CSWebNuevoEntities().Cache_Clasificaciones.AsNoTracking().SingleOrDefault(e => e.CodClasificacion == clas.CodClasificacion);
            if (exist == null)
            {
                db.Cache_Clasificaciones.Add(clas);
                db.SaveChanges();
            }
        }

        private static void GuardarGenero(CSWebNuevoEntities db, Cache_Generos gene)
        {
            Cache_Generos exist = new CSWebNuevoEntities().Cache_Generos.AsNoTracking().SingleOrDefault(e => e.CodGenero == gene.CodGenero);
            if (exist == null)
            {
                db.Cache_Generos.Add(gene);
                db.SaveChanges();
            }
        }

        private static void GuardarLenguaje(CSWebNuevoEntities db, Cache_Lenguajes leng)
        {
            Cache_Lenguajes exist = new CSWebNuevoEntities().Cache_Lenguajes.AsNoTracking().SingleOrDefault(e => e.CodLenguaje == leng.CodLenguaje);
            if (exist == null)
            {
                db.Cache_Lenguajes.Add(leng);
                db.SaveChanges();
            }
        }

        private static void GuardarDatosPelicula(CSWebNuevoEntities db, Cache_Peliculas peli)
        {
            // peli.Cache_Clasificaciones = null;
            // peli.Cache_Generos = null;
            // peli.Cache_Lenguajes = null;

            db.Cache_Peliculas.Add(new Cache_Peliculas()
            {
                CodPelicula = peli.CodPelicula,
                Titulo = peli.Titulo,
                TituloOriginal = peli.TituloOriginal,
                Subtitulada = peli.Subtitulada,
                Duracion = peli.Duracion,
                Estreno = peli.Estreno,
                CodClasificacion = peli.Cache_Clasificaciones.CodClasificacion,
                Sinopsis = peli.Sinopsis,
                SinopsisCorta = peli.SinopsisCorta,
                Web1 = peli.Web1,
                Web2 = peli.Web2,
                UrlTrailer = peli.UrlTrailer,
                CodGenero = peli.Cache_Generos.CodGenero,
                CodLenguaje = peli.Cache_Lenguajes.CodLenguaje,
                Filename = peli.Filename
            });
            db.SaveChanges();

            GuardarActores(db, peli.CodPelicula, peli.Actores);
            GuardarDirectores(db, peli.CodPelicula, peli.Directores);
        }

        private static void GuardarActores(CSWebNuevoEntities db, int codPelicula, List<string> actores)
        {
            foreach (string a in actores)
            {
                if (!db.Cache_Actores.Any(ca => ca.CodPelicula == codPelicula && ca.Actor == a))
                {
                    db.Cache_Actores.Add(new Cache_Actores()
                    {
                        CodPelicula = codPelicula,
                        Actor = a
                    });
                }
            }

            db.SaveChanges();
        }

        private static void GuardarDirectores(CSWebNuevoEntities db, int codPelicula, List<string> directores)
        {
            foreach (string d in directores)
            {
                if (!db.Cache_Directores.Any(cd => cd.CodPelicula == codPelicula && cd.Director == d))
                {
                    db.Cache_Directores.Add(new Cache_Directores()
                    {
                        CodPelicula = codPelicula,
                        Director = d
                    });
                }
            }

            db.SaveChanges();
        }

        private static void GuardarFunciones(CSWebNuevoEntities db, List<Cache_Funciones> funciones, int codPel)
        {
            foreach (Cache_Funciones func in funciones)
            {
                GuardarFuncion(db, func, codPel);
            }
        }

        private static void GuardarFuncion(CSWebNuevoEntities db, Cache_Funciones func, int codPel)
        {
            GuardarTecnologia(db, func.Cache_Tecnologias);
            GuardarCineSemana(db, func.Complex_Options);
            GuardarDatosFuncion(db, func, codPel);
        }

        private static void GuardarTecnologia(CSWebNuevoEntities db, Cache_Tecnologias tecn)
        {
            Cache_Tecnologias exist = new CSWebNuevoEntities().Cache_Tecnologias.AsNoTracking().SingleOrDefault(e => e.CodTecnologia == tecn.CodTecnologia);
            if (exist == null)
            {
                db.Cache_Tecnologias.Add(tecn);
                db.SaveChanges();
            }
        }

        private static void GuardarCineSemana(CSWebNuevoEntities db, Complex_Options comp)
        {
            if (comp.Cache_Cinesemanas != null)
            {
                int cant = new CSWebNuevoEntities().Cache_Cinesemanas.AsNoTracking().Where(c => c.CodComplejo == comp.CodComplejo).Count();
                if (cant == 0)
                {
                    db.Cache_Cinesemanas.Add(new Cache_Cinesemanas()
                    { 
                        CodComplejo = comp.CodComplejo,
                        Desde = comp.Cache_Cinesemanas.Desde,
                        Hasta = comp.Cache_Cinesemanas.Hasta
                    });

                    foreach (Cache_GruposSemana grupo in comp.Cache_Cinesemanas.Cache_GruposSemanas)
                    {
                        db.Cache_GruposSemana.Add(new Cache_GruposSemana()
                        {
                            NomGrupo = grupo.NomGrupo,
                            Orden = grupo.Orden,
                            Desde = grupo.Desde,
                            Hasta = grupo.Hasta,
                            CodComplejo = comp.CodComplejo
                        });
                    }

                    db.SaveChanges();
                }
            }

            foreach (Cache_Salas sala in comp.Cache_Salas)
            {
                GuardarSala(db, sala, comp.CodComplejo);
            }
        }
        
        private static void GuardarSala(SqlConnection connection, Cache_Salas sala, int codComp)
        {
            string checkSql = "SELECT COUNT(1) FROM Cache_Salas WHERE CodSala = @CodSala AND CodComplejo = @CodComplejo";
            using (var checkCommand = new SqlCommand(checkSql, connection))
            {
                checkCommand.Parameters.AddWithValue("@CodSala", sala.CodSala);
                checkCommand.Parameters.AddWithValue("@CodComplejo", codComp);
                int exists = Convert.ToInt32(checkCommand.ExecuteScalar());

                if (exists == 0)
                {
                    // Insert new sala
                    string insertSql = @"
                INSERT INTO Cache_Salas (CodSala, NomSala, CodTipoSala, NomTipoSala, CodComplejo)
                VALUES (@CodSala, @Nombre, @CodTipoSala, @NomTipoSala, @CodComplejo)";
                    using (var insertCommand = new SqlCommand(insertSql, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@CodSala", sala.CodSala);
                        insertCommand.Parameters.AddWithValue("@Nombre", sala.NomSala);
                        insertCommand.Parameters.AddWithValue("@CodTipoSala", sala.CodTipoSala);
                        insertCommand.Parameters.AddWithValue("@NomTipoSala", sala.NomTipoSala);
                        insertCommand.Parameters.AddWithValue("@CodComplejo", codComp);
                        insertCommand.ExecuteNonQuery();
                    }
                }
            }
        }

        private static void GuardarDatosFuncion(CSWebNuevoEntities db, Cache_Funciones func, int codPel)
        {
            func.CodComplejo = func.Complex_Options.CodComplejo;
            func.CodSala = func.Cache_Salas.CodSala;
            func.CodTecnologia = func.Cache_Tecnologias.CodTecnologia;
            func.CodPelicula = codPel;
            db.Cache_Funciones.Add(func);
            db.SaveChanges();
        }

private static void GuardarDatosFuncion(Cache_Funciones func, int codPel)
{
    using (var connection = new SqlConnection(ConnectionString))
    {
        connection.Open();

        func.CodComplejo = func.Complejos?.CodComplejo ?? func.CodComplejo;
        func.CodSala = func.CodSala;
        func.CodTecnologia = func.CodTecnologia;
        func.CodPelicula = codPel;

        string sql = @"
            INSERT INTO Cache_Funciones (
                CodFuncion, CodComplejo, HoraComienzo, Vuelta, Estado, Preestreno, 
                ButacasDisponibles, ButacasHabilitadas, CodDistribucion, Fecha, 
                CodSala, CodTecnologia, CodPelicula, CodCopia
            )
            VALUES (
                @CodFuncion, @CodComplejo, @HoraComienzo, @Vuelta, @Estado, @Preestreno, 
                @ButacasDisponibles, @ButacasHabilitadas, @CodDistribucion, @Fecha, 
                @CodSala, @CodTecnologia, @CodPelicula, @CodCopia
            )";
        using (var command = new SqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@CodFuncion", func.CodFuncion);
            command.Parameters.AddWithValue("@CodComplejo", func.CodComplejo);
            command.Parameters.AddWithValue("@HoraComienzo", func.HoraComienzo ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Vuelta", func.Vuelta);
            command.Parameters.AddWithValue("@Estado", func.Estado ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Preestreno", func.Preestreno);
            command.Parameters.AddWithValue("@ButacasDisponibles", func.ButacasDisponibles);
            command.Parameters.AddWithValue("@ButacasHabilitadas", func.ButacasHabilitadas);
            command.Parameters.AddWithValue("@CodDistribucion", func.CodDistribucion);
            command.Parameters.AddWithValue("@Fecha", func.Fecha);
            command.Parameters.AddWithValue("@CodSala", func.CodSala);
            command.Parameters.AddWithValue("@CodTecnologia", func.CodTecnologia);
            command.Parameters.AddWithValue("@CodPelicula", func.CodPelicula);
            command.Parameters.AddWithValue("@CodCopia", func.CodCopia ?? (object)DBNull.Value);

            command.ExecuteNonQuery();
        }
    }
}

        public static void GuardarCacheProductos(List<Cache_Productos> list, DateTime fecha)
        {
            GuardarLog("GuardarCacheProductos Inicio list.Count: " + list.Count, 1002, "DAO.cs");

            using (CSWebNuevoEntities db = new CSWebNuevoEntities())
            {
                try
                {
                    db.Cache_Productos.RemoveRange(db.Cache_Productos.ToList());
                    db.SaveChanges();

                    foreach (Cache_Productos prod in list)
                    {
                        GuardarProducto(db, prod);
                    }

                    Caches cache = db.Caches.AsNoTracking().Single(c => c.IdCache == "Productos");
                    cache.FechaInicio = fecha;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    GuardarLog("GuardarCacheProductos 2 " + ex.ToString() + ex.StackTrace, 1002, "DAO.cs");
                    throw ex;
                }
            }

            GuardarLog("GuardarCacheProductos Fin", 1002, "DAO.cs");
        }

        private static void GuardarProducto(CSWebNuevoEntities db, Cache_Productos prod)
        {
            db.Cache_Productos.Add(prod);
            db.SaveChanges();
        }
    }
}