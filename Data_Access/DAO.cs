using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using CSCache.Model;

namespace CSCache.Controlador
{
    public class DAO : IDisposable
    {
        private static readonly string connectionString = ConfigurationManager.ConnectionStrings["CMS_AtlasConnection"].ConnectionString;
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
            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string sql = "SELECT TOP 1 FechaInicio FROM Caches WHERE IdCache = @IdCache";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@IdCache", id);
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
                    ActualizarCacheProceso(id,"Iniciado", 0, $"Error en ObtenerFechaCache: {ex.Message}", DateTime.Now);
                    return DateTime.Now;
                }
            }
        }

        public static void ActualizarCacheProceso(string idCache, string estado, int? detalle, string informe, DateTime? horaFin = null)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string sql = @"
                        UPDATE Caches
                        SET Estado = @Estado, Detalle = @Detalle, Informe = @Informe, HoraFin = @HoraFin
                        WHERE ID = @ID";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ID", idCache);
                        command.Parameters.AddWithValue("@Estado", estado);
                        command.Parameters.AddWithValue("@Detalle", detalle ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Informe", informe ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@HoraFin", horaFin ?? (object)DBNull.Value);
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al actualizar Caches: {ex.Message}");
                    throw;
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
                    
                    string sql = "SELECT ParamValue FROM Parametros WHERE ParamKey = @Key";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Key", key);
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
                    string sql = "SELECT CodComplejo, CodTerminal, Nombre, Email, PassEmail, SMTPServer, EnableSSL, PortNumber, GoogleAnalytics, Allow_Sale, Allow_ConcessionSale, Request_TaxId, Tolerancia,MaxSales, Hourly, ComplexDetails, Request_MinRangeTax, Request_MaxRangeTax, Barcode,PickupCode, AuxEmail, AuxSMTPServer, AuxPort, AuxPassword, AuxEnableSSL, MinimunSeatToSell, Allow_Refund, Direccion FROM Complex_Options";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var complejo = new Complex_Options();
                                complejo.CodComplejo = reader.GetInt32(0);
                                complejo.CodTerminal = reader.GetInt32(1);
                                complejo.Nombre = reader.IsDBNull(2) ? null : reader.GetString(2);
                                complejo.Email = reader.IsDBNull(3) ? null : reader.GetString(3);
                                complejo.PassEmail = reader.IsDBNull(4) ? null : reader.GetString(4);
                                complejo.SMTPServer = reader.IsDBNull(5) ? null : reader.GetString(5);
                                complejo.EnableSSL = reader.IsDBNull(6) ? (bool?)null : reader.GetBoolean(6);
                                complejo.PortNumber = reader.IsDBNull(7) ? null : reader.GetString(7);
                                complejo.GoogleAnalytics = reader.IsDBNull(8) ? null : reader.GetString(8);
                                complejo.Allow_Sale = reader.GetBoolean(9);
                                complejo.Allow_ConcessionSale = reader.GetBoolean(10);
                                complejo.Request_TaxId = reader.IsDBNull(11) ? (bool?)null : reader.GetBoolean(11);
                                complejo.Tolerancia = reader.IsDBNull(12) ? (int?)null : reader.GetInt32(12);
                                complejo.MaxSales = reader.IsDBNull(13) ? (int?)null : reader.GetInt32(13);
                                complejo.Hourly = reader.IsDBNull(14) ? null : reader.GetString(14);
                                complejo.ComplexDetails = reader.IsDBNull(15) ? null : reader.GetString(15);
                                complejo.Request_MinRangeTax = reader.IsDBNull(16) ? (int?)null : reader.GetInt32(16);
                                complejo.Request_MaxRangeTax = reader.IsDBNull(17) ? (int?)null : reader.GetInt32(17);
                                complejo.Barcode = reader.IsDBNull(18) ? null : reader.GetString(18);
                                complejo.PickupCode = reader.IsDBNull(19) ? (int?)null : reader.GetInt32(19);
                                complejo.AuxEmail = reader.IsDBNull(20) ? null : reader.GetString(20);
                                complejo.AuxSMTPServer = reader.IsDBNull(21) ? null : reader.GetString(21);
                                complejo.AuxPort = reader.IsDBNull(22) ? (int?)null : reader.GetInt32(22);
                                complejo.AuxPassword = reader.IsDBNull(23) ? null : reader.GetString(23);
                                complejo.AuxEnableSSL = reader.IsDBNull(24) ? (bool?)null : reader.GetBoolean(24);
                                complejo.MinimunSeatToSell = reader.IsDBNull(25) ? (int?)null : reader.GetInt32(25);
                                complejo.Allow_Refund = reader.IsDBNull(26) ? (bool?)null : reader.GetBoolean(26);
                                complejo.Direccion = reader.IsDBNull(27) ? null : reader.GetString(27);
                                complejo.Cache_Salas = new List<Cache_Salas>();
                                complejo.Cache_Cinesemanas = new Cache_Cinesemanas();

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
            SqlTransaction transaction = null;

            try
            {
                ActualizarCacheProceso("Peliculas", "En Proceso", 0, "Procesando caché de películas");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    
                    connection.Open();

                    // Iniciar la transacción
                    transaction = connection.BeginTransaction();

                    // Paso 1: Eliminar datos existentes de las tablas
                    string[] tablesToClear = {
                "Cache_Actores", "Cache_Directores", "Cache_Funciones", "Cache_GruposSemana",
                "Cache_Cinesemanas", "Cache_Salas", "Cache_Tecnologias", "Cache_Peliculas",
                "Cache_CopiasPelicula", "Cache_Clasificaciones", "Cache_Generos", "Cache_Lenguajes"
            };
                    foreach (var table in tablesToClear)
                    {
                        using (var command = new SqlCommand($"DELETE FROM {table}", connection, transaction))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                    //Paso 2: Insertar peliculas
                    foreach (Cache_Peliculas peli in list)
                    {
                        GuardarPelicula(connection, peli, transaction);
                    }
                    //Paso 3: Insertar copias de la peliculas
                    foreach (var copia in copies)
                    {
                        string insertCopiaSql = @"
                    INSERT INTO Cache_CopiasPelicula (CodPelicula, CodCopia, IdTecnologia, Titulo, CodIdioma, Subtitulada, Doblada )
                    VALUES (@CodPelicula, @CodCopia, @IdTecnologia, @Titulo, @CodIdioma, @Subtitulada, @Doblada )";
                        using (var command = new SqlCommand(insertCopiaSql, connection, transaction))
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
                    using (var command = new SqlCommand(updateCacheSql, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@Fecha", fecha);
                        command.ExecuteNonQuery();
                    }

                    // Si todo sale bien, confirmamos la transacción
                    transaction.Commit();
                    ActualizarCacheProceso("Peliculas", "Finalizado", list.Count, "Proceso ejecutado correctamente", DateTime.Now);
                }
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                ActualizarCacheProceso("Peliculas", "Iniciado", 0, $"Error en GuardarCache: {ex.Message}", DateTime.Now);
                throw;
            }
        }

        private static void GuardarPelicula(SqlConnection connection, Cache_Peliculas peli, SqlTransaction transaction)
        {
            GuardarClasificacion(connection, peli.Cache_Clasificaciones, transaction);
            GuardarGenero(connection, peli.Cache_Generos, transaction);
            GuardarLenguaje(connection, peli.Cache_Lenguajes, transaction);
            GuardarDatosPelicula(connection, peli, transaction);
            GuardarFunciones(connection, peli.Cache_Funciones, peli.CodPelicula, transaction);
        }

        private static void GuardarClasificacion(SqlConnection connection, Cache_Clasificaciones clas, SqlTransaction transaction)
        {
                try
                {
                    string checkSql = "SELECT COUNT(1) FROM Cache_Clasificaciones WHERE CodClasificacion = @CodClasificacion";
                    bool exist;
                    using (var command = new SqlCommand(checkSql, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@CodClasificacion", clas.CodClasificacion );
                        exist = (int)command.ExecuteScalar() > 0;
                    }

                    if (exist == false)
                    {
                        string insertSql = "INSERT INTO Cache_Clasificaciones(CodClasificacion, NomClasificacion) VALUES (@CodClasificacion, @NomClasificacion)";
                        using (var command = new SqlCommand(insertSql, connection, transaction))
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

        private static void GuardarGenero(SqlConnection connection, Cache_Generos gene, SqlTransaction transaction)
        {
            try
            {
                string checkSql = "SELECT COUNT(1) FROM Cache_Generos WHERE CodGenero = @CodGenero";
                bool exist;
                using (var command = new SqlCommand(checkSql, connection, transaction))
                {
                    command.Parameters.AddWithValue("@CodGenero", gene.CodGenero);
                    exist = (int)command.ExecuteScalar() > 0;
                }
                if (exist == false)
                {
                    string insertSql = "INSERT INTO Cache_Generos(CodGenero, NomGenero) VALUES (@CodGenero, @NomGenero)";
                    using (var command = new SqlCommand(insertSql, connection, transaction))
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

        private static void GuardarLenguaje(SqlConnection connection, Cache_Lenguajes leng, SqlTransaction transaction)
        {
            try
            {
                string checkSql = "SELECT COUNT(1) FROM Cache_Lenguajes WHERE CodLenguaje = @CodLenguaje";
                bool exist;
                using (var command = new SqlCommand(checkSql, connection, transaction))
                {
                    command.Parameters.AddWithValue("@CodLenguaje", leng.CodLenguaje);
                    exist = (int)command.ExecuteScalar() > 0;
                }
                if (exist == false)
                {
                    string insertSql = "INSERT INTO Cache_Lenguajes(CodLenguaje, NomLenguaje) VALUES (@CodLenguaje, @NomLenguaje)";
                    using (var command = new SqlCommand(insertSql, connection, transaction))
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

        private static void GuardarDatosPelicula(SqlConnection connection, Cache_Peliculas peli, SqlTransaction transaction)
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
                using (var command = new SqlCommand(insertSql, connection, transaction))
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

                GuardarActores(connection, peli.CodPelicula, peli.Actores, transaction);
                GuardarDirectores(connection, peli.CodPelicula, peli.Directores, transaction);
            }
            catch (Exception ex)
            {
                GuardarLog("GuardarDatosPelicula" + ex.ToString() + ex.StackTrace, 1002, "DAO.cs");
                throw ex;
            }
        }

        private static void GuardarActores(SqlConnection connection, int codPelicula, List<string> actores, SqlTransaction transaction)
        {
            try
            {
                if (actores == null || actores.Count == 0) return;

                string checkSql = "SELECT COUNT(1) FROM Cache_Actores WHERE codPelicula = @codPelicula AND Actor = @Actor";
                bool exist;
                foreach (string a in actores)
                {
                    using (var command = new SqlCommand(checkSql, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@codPelicula", codPelicula);
                        command.Parameters.AddWithValue("@Actor", a);
                        exist = (int)command.ExecuteScalar() > 0;
                    }
                    if (exist == false)
                    {
                        string insertSql = "INSERT INTO Cache_Actores(codPelicula, Actor) VALUES (@codPelicula, @Actor)";
                        using (var command = new SqlCommand(insertSql, connection, transaction))
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

        private static void GuardarDirectores(SqlConnection connection, int codPelicula, List<string> directores, SqlTransaction transaction)
        {
            try
            {
                if (directores == null || directores.Count == 0) return;

                string checkSql = "SELECT COUNT(1) FROM Cache_Directores WHERE codPelicula = @codPelicula AND Director = @Director";
                bool exist;
                foreach (string d in directores)
                {
                    using (var command = new SqlCommand(checkSql, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@codPelicula", codPelicula);
                        command.Parameters.AddWithValue("@Director", d);
                        exist = (int)command.ExecuteScalar() > 0;
                    }
                    if (exist == false)
                    {
                        string insertSql = "INSERT INTO Cache_Directores(codPelicula, Director) VALUES (@codPelicula, @Director)";
                        using (var command = new SqlCommand(insertSql, connection, transaction))
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

        private static void GuardarFunciones(SqlConnection connection, List<Cache_Funciones> funciones, int codPel, SqlTransaction transaction)
        {
            foreach (Cache_Funciones func in funciones)
            {
                GuardarFuncion(connection, func, codPel, transaction);
            }
        }

        private static void GuardarFuncion(SqlConnection connection, Cache_Funciones func, int codPel, SqlTransaction transaction)
        {
            GuardarTecnologia(connection, func.Cache_Tecnologias, transaction);
            GuardarCineSemana(connection, func.Complex_Options, transaction);
            GuardarDatosFuncion(connection, func, codPel, transaction);
        }

        private static void GuardarTecnologia(SqlConnection connection, Cache_Tecnologias tecn, SqlTransaction transaction)
        {
            try
            {
                string checkSql = "SELECT COUNT(1) FROM Cache_Tecnologias WHERE CodTecnologia = @CodTecnologia ";
                bool exist;
            
                    using (var command = new SqlCommand(checkSql, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@CodTecnologia", tecn.CodTecnologia);
                        exist = (int)command.ExecuteScalar() > 0;
                    }
                    if (exist == false)
                    {
                        string insertSql = "INSERT INTO Cache_Tecnologias(CodTecnologia, NomTecnologia) VALUES (@CodTecnologia, @NomTecnologia)";
                        using (var command = new SqlCommand(insertSql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@CodTecnologia", tecn.CodTecnologia);
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

        private static void GuardarCineSemana(SqlConnection connection, Complex_Options comp, SqlTransaction transaction)
        {
            try
            {
                if (comp.Cache_Cinesemanas != null)
                {
                    string checkSql = "SELECT COUNT(1) FROM Cache_Cinesemanas WHERE CodComplejo = @CodComplejo ";
                    bool exist;
                
                    using (var command = new SqlCommand(checkSql, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@CodComplejo", comp.CodComplejo);
                        exist = (int)command.ExecuteScalar() > 0;
                    }
                    if (exist == false)
                    {
                        string insertSql = "INSERT INTO Cache_Cinesemanas(CodComplejo, Desde, Hasta) VALUES (@CodComplejo, @Desde, @Hasta)";
                        using (var command = new SqlCommand(insertSql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@CodComplejo", comp.CodComplejo);
                            command.Parameters.AddWithValue("@Desde", comp.Cache_Cinesemanas.Desde);
                            command.Parameters.AddWithValue("@Hasta", comp.Cache_Cinesemanas.Hasta);
                            command.ExecuteNonQuery();
                        }
                    }

                    foreach (var grupo in comp.Cache_Cinesemanas.Cache_GruposSemana)
                    {
                        string checkExistsSql = @"
                            SELECT COUNT(1)
                            FROM Cache_GruposSemana
                            WHERE Orden = @Orden AND CodComplejo = @CodComplejo";

                        using (var checkCommand = new SqlCommand(checkExistsSql, connection, transaction))
                        {
                            checkCommand.Parameters.AddWithValue("@Orden", grupo.Orden);
                            checkCommand.Parameters.AddWithValue("@CodComplejo", comp.CodComplejo);

                            int count = (int)checkCommand.ExecuteScalar();

                            if (count == 0)
                            {
                                string insertCopiaSql = @"
                                    INSERT INTO Cache_GruposSemana (NomGrupo, Orden, Desde, Hasta, CodComplejo)
                                    VALUES (@NomGrupo, @Orden, @Desde, @Hasta, @CodComplejo)";

                                using (var insertCommand = new SqlCommand(insertCopiaSql, connection, transaction))
                                {
                                    insertCommand.Parameters.AddWithValue("@NomGrupo", grupo.NomGrupo);
                                    insertCommand.Parameters.AddWithValue("@Orden", grupo.Orden);
                                    insertCommand.Parameters.AddWithValue("@Desde", grupo.Desde);
                                    insertCommand.Parameters.AddWithValue("@Hasta", grupo.Hasta);
                                    insertCommand.Parameters.AddWithValue("@CodComplejo", comp.CodComplejo);

                                    insertCommand.ExecuteNonQuery();
                                }
                            }
                        }
                    }

                    foreach (Cache_Salas sala in comp.Cache_Salas)
                    {
                        GuardarSala(connection, sala, comp.CodComplejo, transaction);
                    }
                }
            }
            catch (Exception ex)
            {
                GuardarLog("GuardarCineSemana" + ex.ToString() + ex.StackTrace, 1002, "DAO.cs");
                throw ex;
            }
}
        
        private static void GuardarSala(SqlConnection connection, Cache_Salas sala, int codComp, SqlTransaction transaction)
        {
            try
            {
                string checkSql = "SELECT COUNT(1) FROM Cache_Salas WHERE CodSala = @CodSala AND CodComplejo = @CodComplejo";
                using (var checkCommand = new SqlCommand(checkSql, connection, transaction))
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
                        using (var insertCommand = new SqlCommand(insertSql, connection, transaction))
                        {
                            insertCommand.Parameters.AddWithValue("@CodSala", sala.CodSala);
                            insertCommand.Parameters.AddWithValue("@NomSala", sala.NomSala);
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

        private static void GuardarDatosFuncion(SqlConnection connection, Cache_Funciones func, int codPel, SqlTransaction transaction)
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
                    using (var command = new SqlCommand(sql, connection, transaction))
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
            try
            {
                ActualizarCacheProceso("Productos", "En Proceso", 0, "Procesando caché de productos");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new SqlCommand($"DELETE FROM Cache_Productos", connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    foreach (var prod in list)
                    {
                        GuardarProducto(connection, prod);
                    }

                    string updateCacheSql = @"
                UPDATE Caches 
                SET HoraInicio = @Fecha 
                WHERE IdCache = 'Productos'";
                    using (var command = new SqlCommand(updateCacheSql, connection))
                    {
                        command.Parameters.AddWithValue("@Fecha", fecha);
                        command.ExecuteNonQuery();
                    }

                    ActualizarCacheProceso("Productos", "Finalizado", list.Count, "Proceso ejecutado correctamente", DateTime.Now);
                }
            }
            catch (Exception ex)
            {
                ActualizarCacheProceso("Productos", "Iniciado", 0, $"Error en GuardarCacheProductos: {ex.Message}", DateTime.Now);
                throw;
            }
        }

        private static void GuardarProducto(SqlConnection connection, Cache_Productos prod)
        {
            try
            {
                string checkExistsSql = @"
                    SELECT COUNT(1)
                    FROM Cache_Productos
                    WHERE CodProducto = @CodProducto AND CodComplejo = @CodComplejo";

                using (var checkCommand = new SqlCommand(checkExistsSql, connection))
                {
                    checkCommand.Parameters.AddWithValue("@CodProducto", prod.CodProducto);
                    checkCommand.Parameters.AddWithValue("@CodComplejo", prod.CodComplejo);
                    int count = (int)checkCommand.ExecuteScalar();

                    if (count == 0)
                    {
                        string sql = @"
                    INSERT INTO Cache_Productos (
                        CodProducto, CodComplejo, NomProducto, Precio, Posicion, NombreArchivo
                    )
                    VALUES (
                        @CodProducto, @CodComplejo, @NomProducto, @Precio, @Posicion, @NombreArchivo
                    )";
                        using (var command = new SqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@CodProducto", prod.CodProducto);
                            command.Parameters.AddWithValue("@CodComplejo", prod.CodComplejo);
                            command.Parameters.AddWithValue("@NomProducto", prod.NomProducto ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Precio", prod.Precio ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Posicion", prod.Posicion);
                            command.Parameters.AddWithValue("@NombreArchivo", prod.NombreArchivo ?? (object)DBNull.Value);

                            command.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        GuardarLog($"Producto con CodProducto={prod.CodProducto} y CodComplejo={prod.CodComplejo} ya existe, no se insertó.", 1002, "DAO.cs");
                    }
                }
            }
            catch (Exception ex)
            {
                GuardarLog("GuardarProducto Error: " + ex.ToString() + ex.StackTrace, 1002, "DAO.cs");
                throw;
            }
        }
    }
}