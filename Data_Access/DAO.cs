using System;
using System.Data.SqlClient;
using System.Linq;
using System.Collections.Generic;
using CSCache.Model;

namespace CSCache.Controlador
{
    public class DAO : IDisposable
    {
        private static string connectionString = 
            "Server=192.168.0.230,1453;Database=CMS_Atlas;User Id=ext_free;Password=D3v_Fr33_SQL14#;";
        private readonly SqlConnection _sharedConnection;

        public DAO()
        {
            _sharedConnection = new SqlConnection(connectionString);
        }
        
        public void Dispose()
        {
            _sharedConnection?.Dispose();
        }
        
        public static DateTime ObtenerFechaCache(string id)
        {
            using (var connection = new SqlConnection())
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Conexión exitosa a la base de datos 'cache'");
                    string sql = "SELECT Top 1 FechaInicio FROM caches WHERE IdCache = " + id;

                    using (var command = new SqlCommand(sql, connection))
                    {
                        var result = command.ExecuteScalar();
                        if (result != null)
                        {
                            return Convert.ToDateTime(result);
                        }

                        return DateTime.Now;
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
        public static List<DeviceImageSettings> ConfiguracionesDispositivosImagenes()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    List<DeviceImageSettings> dispositivos = new List<DeviceImageSettings>();
                    string sql = "SELECT Id, ImageTypeCode, DesktopWidth, TabletWidth, SmartphoneWidth, CellphoneWidth";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var dispositivo = new DeviceImageSettings();
                                dispositivo.Id = reader.GetInt32(0);
                                dispositivo.ImageTypeCode = reader.GetString(1);
                                dispositivo.DesktopWidth = reader.GetInt32(2);
                                dispositivo.TabletWidth = reader.GetInt32(3);
                                dispositivo.SmartphoneWidth = reader.GetInt32(4);
                                dispositivo.CellphoneWidth = reader.GetInt32(5);

                                dispositivos.Add(dispositivo);
                            }
                            return dispositivos;
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

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    // Paso 1: Eliminar datos existentes de las tablas
                    string[] tablesToClear = {
                        "Cache_Actores", "Cache_Directores", "Cache_Funciones", "Cache_GruposSemana",
                        "Cache_Cinesemanas", "Cache_Salas", "Cache_Tecnologias", "Cache_Peliculas",
                        "Cache_CopiasPelicula", "Cache_Clasificaciones", "Cache_Generos", "Cache_Lenguajes"
                    };
                    foreach (var table in tablesToClear)
                    {
                        using (var command = new SqlCommand($"DELETE FROM {table}", connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                    //Paso 2: Insertar peliculas
                    foreach (Cache_Peliculas peli in list)
                    {
                        GuardarPelicula(connection, peli);
                    }
                    //Paso 3: Insertar copias de la peliculas
                    foreach (var copia in copies)
                    {
                        string insertCopiaSql = @"
                            INSERT INTO Cache_CopiasPelicula (CodPelicula, CodCopia, IdTecnologia, Titulo, CodIdioma, Subtitulada, Doblada )
                            VALUES (@CodPelicula, @CodCopia, @IdTecnologia, @Titulo, @CodIdioma, @Subtitulada, @Doblada )";
                        using (var command = new SqlCommand(insertCopiaSql, connection))
                        {
                            command.Parameters.AddWithValue("@CodPelicula", copia.CodPelicula);
                            command.Parameters.AddWithValue("@CodCopia", copia.CodCopia);
                            command.Parameters.AddWithValue("@IdTecnologia", copia.IdTecnologia);
                            command.Parameters.AddWithValue("@Titulo", copia.Titulo);
                            command.Parameters.AddWithValue("@CodIdioma", copia.CodIdioma);
                            command.Parameters.AddWithValue("@Subtitulada", copia.Subtitulada);
                            command.Parameters.AddWithValue("@Doblada", copia.Doblada);

                            command.ExecuteNonQuery();
                        }
                    }
                    //Paso 4: Actualizar tabla de cache
                    string updateCacheSql = @"
                        UPDATE Caches 
                        SET FechaInicio = @Fecha 
                        WHERE IdCache = 'Peliculas'";
                    using (var command = new SqlCommand(updateCacheSql, connection))
                    {
                        command.Parameters.AddWithValue("@Fecha", fecha);
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    GuardarLog("GuardarCache " + ex.ToString() + ex.StackTrace, 1002, "DAO.cs");
                    throw ex;
                }
            }

            GuardarLog("GuardarCache FIN", 1004, "DAO.cs");
        }

        private static void GuardarPelicula(SqlConnection connection, Cache_Peliculas peli)
        {
            GuardarClasificacion(connection, peli.Cache_Clasificaciones);
            GuardarGenero(connection, peli.Cache_Generos);
            GuardarLenguaje(connection, peli.Cache_Lenguajes);
            GuardarDatosPelicula(connection, peli);
            GuardarFunciones(connection, peli.Cache_Funciones, peli.CodPelicula);
        }

        private static void GuardarClasificacion(SqlConnection connection, Cache_Clasificaciones clas)
        {
            try
            {
                string checkSql = "SELECT COUNT(1) FROM Cache_Clasificaciones WHERE CodClasificacion = @CodClasificacion";
                bool exist;
                using (var command = new SqlCommand(checkSql, connection))
                {
                    command.Parameters.AddWithValue("@CodClasificacion", clas.CodClasificacion );
                    exist = (int)command.ExecuteScalar() > 0;
                }

                if (exist == false)
                {
                    string insertSql = "INSERT INTO Cache_Clasificaciones(CodClasificacion, NomClasificacion) VALUES (@CodClasificacion, @NomClasificacion)";
                    using (var command = new SqlCommand(insertSql, connection))
                    {
                        command.Parameters.AddWithValue("@CodClasificacion", clas.CodClasificacion);
                        command.Parameters.AddWithValue("@NomClasificacion", clas.NomClasificacion);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                GuardarLog("GuardarClasificacion" + ex.ToString() + ex.StackTrace, 1002, "DAO.cs");
                throw ex;
            }
}

        private static void GuardarGenero(SqlConnection connection, Cache_Generos gene)
        {
            try
            {
                string checkSql = "SELECT COUNT(1) FROM Cache_Generos WHERE CodGenero = @CodGenero";
                bool exist;
                using (var command = new SqlCommand(checkSql, connection))
                {
                    command.Parameters.AddWithValue("@CodGenero", gene.CodGenero);
                    exist = (int)command.ExecuteScalar() > 0;
                }
                if (exist == false)
                {
                    string insertSql = "INSERT INTO Cache_Generos(CodGenero, NomGenero) VALUES (@CodGenero, @NomGenero)";
                    using (var command = new SqlCommand(insertSql, connection))
                    {
                        command.Parameters.AddWithValue("@CodGenero", gene.CodGenero);
                        command.Parameters.AddWithValue("@NomGenero", gene.NomGenero);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                GuardarLog("GuardarGenero" + ex.ToString() + ex.StackTrace, 1002, "DAO.cs");
                throw ex;
            }
}

        private static void GuardarLenguaje(SqlConnection connection, Cache_Lenguajes leng)
        {
            try
            {
                string checkSql = "SELECT COUNT(1) FROM Cache_Lenguajes WHERE CodLenguaje = @CodLenguaje";
                bool exist;
                using (var command = new SqlCommand(checkSql, connection))
                {
                    command.Parameters.AddWithValue("@CodLenguaje", leng.CodLenguaje);
                    exist = (int)command.ExecuteScalar() > 0;
                }
                if (exist == false)
                {
                    string insertSql = "INSERT INTO Cache_Lenguajes(CodLenguaje, NomLenguaje) VALUES (@CodLenguaje, @NomLenguaje)";
                    using (var command = new SqlCommand(insertSql, connection))
                    {
                        command.Parameters.AddWithValue("@CodLenguaje", leng.CodLenguaje);
                        command.Parameters.AddWithValue("@NomLenguaje", leng.NomLenguaje);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                GuardarLog("GuardarLenguaje" + ex.ToString() + ex.StackTrace, 1002, "DAO.cs");
                throw ex;
            }
        }

        private static void GuardarDatosPelicula(SqlConnection connection, Cache_Peliculas peli)
        {
            try
            {
                string insertSql = @"
                    INSERT INTO Cache_Peliculas (
                        CodPelicula, Titulo, TituloOriginal, Subtitulada, Duracion, Estreno, 
                        CodClasificacion, Sinopsis, SinopsisCorta, Web1, Web2, UrlTrailer, 
                        CodGenero, CodLenguaje, Filename
                    ) VALUES (
                        @CodPelicula, @Titulo, @TituloOriginal, @Subtitulada, @Duracion, @Estreno, 
                        @CodClasificacion, @Sinopsis, @SinopsisCorta, @Web1, @Web2, @UrlTrailer, 
                        @CodGenero, @CodLenguaje, @Filename
                    )";
                using (var command = new SqlCommand(insertSql, connection))
                {
                    command.Parameters.AddWithValue("@CodPelicula", peli.CodPelicula);
                    command.Parameters.AddWithValue("@Titulo", peli.Titulo);
                    command.Parameters.AddWithValue("@TituloOriginal", peli.TituloOriginal);
                    command.Parameters.AddWithValue("@Subtitulada", peli.Subtitulada);
                    command.Parameters.AddWithValue("@Duracion", peli.Duracion);
                    command.Parameters.AddWithValue("@Estreno", peli.Estreno);
                    command.Parameters.AddWithValue("@CodClasificacion", peli.Cache_Clasificaciones?.CodClasificacion);
                    command.Parameters.AddWithValue("@Sinopsis", peli.Sinopsis);
                    command.Parameters.AddWithValue("@SinopsisCorta", peli.SinopsisCorta);
                    command.Parameters.AddWithValue("@Web1", peli.Web1 );
                    command.Parameters.AddWithValue("@Web2", peli.Web2 );
                    command.Parameters.AddWithValue("@UrlTrailer", peli.UrlTrailer);
                    command.Parameters.AddWithValue("@CodGenero", peli.Cache_Generos?.CodGenero );
                    command.Parameters.AddWithValue("@CodLenguaje", peli.Cache_Lenguajes?.CodLenguaje);
                    command.Parameters.AddWithValue("@Filename", peli.Filename);

                    command.ExecuteNonQuery();
                }

                GuardarActores(connection, peli.CodPelicula, peli.Actores);
                GuardarDirectores(connection, peli.CodPelicula, peli.Directores);
            }
            catch (Exception ex)
            {
                GuardarLog("GuardarDatosPelicula" + ex.ToString() + ex.StackTrace, 1002, "DAO.cs");
                throw ex;
            }
        }

        private static void GuardarActores(SqlConnection connection, int codPelicula, List<string> actores)
        {
            try
            {
                if (actores == null || actores.Count == 0) return;

                string checkSql = "SELECT COUNT(1) FROM Cache_Actores WHERE codPelicula = @codPelicula AND Actor = @Actor";
                bool exist;
                foreach (string a in actores)
                {
                    using (var command = new SqlCommand(checkSql, connection))
                    {
                        command.Parameters.AddWithValue("@codPelicula", codPelicula);
                        command.Parameters.AddWithValue("@Actor", a);
                        exist = (int)command.ExecuteScalar() > 0;
                    }
                    if (exist == false)
                    {
                        string insertSql = "INSERT INTO Cache_Actores(codPelicula, Actor) VALUES (@codPelicula, @Actor)";
                        using (var command = new SqlCommand(insertSql, connection))
                        {
                            command.Parameters.AddWithValue("@codPelicula", codPelicula);
                            command.Parameters.AddWithValue("@Actor", a);
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                GuardarLog("GuardarActores" + ex.ToString() + ex.StackTrace, 1002, "DAO.cs");
                throw ex;
            }
        }

        private static void GuardarDirectores(SqlConnection connection, int codPelicula, List<string> directores)
        {
            try
            {
                if (directores == null || directores.Count == 0) return;

                string checkSql = "SELECT COUNT(1) FROM Cache_Directores WHERE codPelicula = @codPelicula AND Director = @Director";
                bool exist;
                foreach (string d in directores)
                {
                    using (var command = new SqlCommand(checkSql, connection))
                    {
                        command.Parameters.AddWithValue("@codPelicula", codPelicula);
                        command.Parameters.AddWithValue("@Director", d);
                        exist = (int)command.ExecuteScalar() > 0;
                    }
                    if (exist == false)
                    {
                        string insertSql = "INSERT INTO Cache_Directores(codPelicula, Director) VALUES (@codPelicula, @Director)";
                        using (var command = new SqlCommand(insertSql, connection))
                        {
                            command.Parameters.AddWithValue("@codPelicula", codPelicula);
                            command.Parameters.AddWithValue("@Director", d);
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                GuardarLog("GuardarDirectores" + ex.ToString() + ex.StackTrace, 1002, "DAO.cs");
                throw ex;
            }
}

        private static void GuardarFunciones(SqlConnection connection, List<Cache_Funciones> funciones, int codPel)
        {
            foreach (Cache_Funciones func in funciones)
            {
                GuardarFuncion(connection, func, codPel);
            }
        }

        private static void GuardarFuncion(SqlConnection connection, Cache_Funciones func, int codPel)
        {
            GuardarTecnologia(connection, func.Cache_Tecnologias);
            GuardarCineSemana(connection, func.Complex_Options);
            GuardarDatosFuncion(connection, func, codPel);
        }

        private static void GuardarTecnologia(SqlConnection connection, Cache_Tecnologias tecn)
        {
            try
            {
                string checkSql = "SELECT COUNT(1) FROM Cache_Tecnologias WHERE CodTecnologia = @CodTecnologia ";
                bool exist;
            
                    using (var command = new SqlCommand(checkSql, connection))
                    {
                        command.Parameters.AddWithValue("@CodTecnologia", tecn.CodTecnologia);
                        exist = (int)command.ExecuteScalar() > 0;
                    }
                    if (exist == false)
                    {
                        string insertSql = "INSERT INTO Cache_Tecnologias(CodTecnologia, NomTecnologia) VALUES (@CodTecnologia, @NomTecnologia)";
                        using (var command = new SqlCommand(insertSql, connection))
                        {
                            command.Parameters.AddWithValue("@codPelicula", tecn.CodTecnologia);
                            command.Parameters.AddWithValue("@NomTecnologia", tecn.NomTecnologia);
                            command.ExecuteNonQuery();
                        }
                    }
            }
            catch (Exception ex)
            {
                GuardarLog("GuardarTecnologia" + ex.ToString() + ex.StackTrace, 1002, "DAO.cs");
                throw ex;
            }
}

        private static void GuardarCineSemana(SqlConnection connection, Complex_Options comp)
        {
            try
            {
                if (comp.Cache_Cinesemanas != null)
                {
                    string checkSql = "SELECT COUNT(1) FROM Cache_Cinesemanas WHERE CodComplejo = @CodComplejo ";
                    bool exist;
                
                    using (var command = new SqlCommand(checkSql, connection))
                    {
                        command.Parameters.AddWithValue("@CodComplejo", comp.CodComplejo);
                        exist = (int)command.ExecuteScalar() > 0;
                    }
                    if (exist == false)
                    {
                        string insertSql = "INSERT INTO Cache_Cinesemanas(CodComplejo, Desde, Hasta) VALUES (@CodComplejo, @Desde, @Hasta)";
                        using (var command = new SqlCommand(insertSql, connection))
                        {
                            command.Parameters.AddWithValue("@CodComplejo", comp.CodComplejo);
                            command.Parameters.AddWithValue("@Desde", comp.Cache_Cinesemanas.Desde);
                            command.Parameters.AddWithValue("@Hasta", comp.Cache_Cinesemanas.Hasta);
                            command.ExecuteNonQuery();
                        }
                    }

                    foreach (Cache_GruposSemana grupo in comp.Cache_Cinesemanas.Cache_GruposSemana)
                    {
                        string insertCopiaSql = @"
                                INSERT INTO Cache_GruposSemana (NomGrupo, Orden, Desde, Hasta, CodComplejo )
                                VALUES (@NomGrupo, @Orden, @Desde, @Hasta, @CodComplejo)";
                        using (var command = new SqlCommand(insertCopiaSql, connection))
                        {
                            command.Parameters.AddWithValue("@NomGrupo", grupo.NomGrupo);
                            command.Parameters.AddWithValue("@Orden", grupo.Orden);
                            command.Parameters.AddWithValue("@Desde", grupo.Desde);
                            command.Parameters.AddWithValue("@Hasta", grupo.Hasta);
                            command.Parameters.AddWithValue("@CodComplejo", grupo.CodComplejo);

                            command.ExecuteNonQuery();
                        }
                    }

                    foreach (Cache_Salas sala in comp.Cache_Salas)
                    {
                        GuardarSala(connection, sala, comp.CodComplejo);
                    }
                }
            }
            catch (Exception ex)
            {
                GuardarLog("GuardarCineSemana" + ex.ToString() + ex.StackTrace, 1002, "DAO.cs");
                throw ex;
            }
}

           
        
        
        private static void GuardarSala(SqlConnection connection, Cache_Salas sala, int codComp)
        {
            try
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
                    VALUES (@CodSala, @NomSala, @CodTipoSala, @NomTipoSala, @CodComplejo)";
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
            catch (Exception ex)
            {
                GuardarLog("GuardarSala" + ex.ToString() + ex.StackTrace, 1002, "DAO.cs");
                throw ex;
            }
}

    private static void GuardarDatosFuncion(SqlConnection connection, Cache_Funciones func, int codPel)
    {
            try
            {
                func.CodComplejo = func.Complex_Options.CodComplejo;
                func.CodSala = func.Cache_Salas.CodSala;
                func.CodTecnologia = func.Cache_Tecnologias.CodTecnologia;
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
            catch (Exception ex)
            {
                GuardarLog("GuardarDatosFuncion" + ex.ToString() + ex.StackTrace, 1002, "DAO.cs");
                throw ex;
            }
        }

        public static void GuardarCacheProductos(List<Cache_Productos> list, DateTime fecha)
        {
            GuardarLog("GuardarCacheProductos Inicio list.Count: " + list.Count, 1002, "DAO.cs");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    using (var command = new SqlCommand($"DELETE FROM Cache_Productos", connection))
                    {
                        command.ExecuteNonQuery();
                    }
                        string sql = @"
                    INSERT INTO Cache_Productos (
                        CodProducto, CodComplejo, NomProducto, Precio, Posicion, NombreArchivo, 
                    )
                    VALUES (
                        @CodProducto, @CodComplejo, @NomProducto, @Precio, @Posicion, @NombreArchivo
                    )";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        foreach (var lista in list)
                        {
                            command.Parameters.AddWithValue("@CodProducto", lista.CodProducto);
                            command.Parameters.AddWithValue("@CodComplejo", lista.CodComplejo);
                            command.Parameters.AddWithValue("@NomProducto", lista.NomProducto);
                            command.Parameters.AddWithValue("@Precio", lista.Precio ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Posicion", lista.Posicion);
                            command.Parameters.AddWithValue("@NombreArchivo", lista.NombreArchivo); ;

                            command.ExecuteNonQuery();
                        }
                    }

                    foreach (Cache_Productos prod in list)
                    {
                        GuardarProducto(connection, prod);
                    }

                    string updateCacheSql = @"
                        UPDATE Caches 
                        SET FechaInicio = @Fecha 
                        WHERE IdCache = 'Productos'";
                    using (var command = new SqlCommand(updateCacheSql, connection))
                    {
                        command.Parameters.AddWithValue("@Fecha", fecha);
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    GuardarLog("GuardarCacheProductos 2 " + ex.ToString() + ex.StackTrace, 1002, "DAO.cs");
                    throw ex;
                }
            }

            GuardarLog("GuardarCacheProductos Fin", 1002, "DAO.cs");
        }

        private static void GuardarProducto(SqlConnection connection, Cache_Productos prod)
        {
            try
            {
                string sql = @"
            INSERT INTO Cache_Productos (
                CodProducto, CodComplejo, NomProducto, Precio, Posicion, NombreArchivo, 
            )
            VALUES (
                @CodProducto, @CodComplejo, @NomProducto, @Precio, @Posicion, @NombreArchivo
            )";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@CodProducto", prod.CodProducto);
                    command.Parameters.AddWithValue("@CodComplejo", prod.CodComplejo);
                    command.Parameters.AddWithValue("@NomProducto", prod.NomProducto);
                    command.Parameters.AddWithValue("@Precio", prod.Precio ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Posicion", prod.Posicion);
                    command.Parameters.AddWithValue("@NombreArchivo", prod.NombreArchivo); ;

                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                GuardarLog("GuardarProducto " + ex.ToString() + ex.StackTrace, 1002, "DAO.cs");
                throw ex;
            }

        }
    }
}