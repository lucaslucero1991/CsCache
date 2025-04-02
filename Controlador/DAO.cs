using System;
using System.Linq;
using System.Collections.Generic;
using CSCache.Model;
using Microsoft.Ajax.Utilities;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;

namespace CSCache.Controlador
{
    public class DAO
    {
        public static DateTime ObtenerFechaCache(string id)
        {
            using (CSWebNuevoEntities db = new CSWebNuevoEntities())
            {
                try
                {
                    DateTime fecha = db.Caches.AsNoTracking().Single(c => c.IdCache == id).UltimaActualizacion;
                    return fecha;
                }
                catch (Exception)
                {
                    db.Caches.Add(new Caches() { IdCache = "Peliculas", UltimaActualizacion = DateTime.Now });
                    db.SaveChanges();
                    return DateTime.Now;
                }
            }
        }

        public static string ObtenerWsInfo()
        {
            using (CSWebNuevoEntities db = new CSWebNuevoEntities())
            {
                try
                {
                    string res = db.web_site_options.AsNoTracking().First().wsinfo;
                    return res;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public static int ObtenerGrupoComplejosId()
        {
            using (CSWebNuevoEntities db = new CSWebNuevoEntities())
            {
                try
                {
                    int res = db.web_site_options.AsNoTracking().First().theatreGroup;
                    return res;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public static string GetParametro(string key)
        {
            using (CSWebNuevoEntities db = new CSWebNuevoEntities())
            {
                try
                {
                    string res = db.Parametros.AsNoTracking().Single(p => p.ParamKey == key).ParamValue;
                    return res;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public static void GuardarLog(string mensaje, int tipoError, string pagina)
        {
            using (CSWebNuevoEntities db = new CSWebNuevoEntities())
            {
                try
                {
                    db.LogWeb.Add(new LogWeb()
                    {
                        TipoError = tipoError,
                        Pagina = pagina,
                        Fecha = DateTime.Now,
                        FechaServidor = DateTime.Now,
                        Aplicacion = "CSWebNueva",
                        Mensaje = mensaje,
                    });
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public static List<Complex_Options> ObtenerComplejos()
        {
            using (CSWebNuevoEntities db = new CSWebNuevoEntities())
            {
                try
                {

                    List<Complex_Options> complejos = db.Complex_Options.AsNoTracking().ToList();
                    foreach (Complex_Options comp in complejos)
                        comp.Cache_Salas = new List<Cache_Salas>();

                    return complejos;
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
            using (CSWebNuevoEntities db = new CSWebNuevoEntities())
            {
                try
                {
                    List<Cache_Peliculas> peliculas = db.Cache_Peliculas.AsNoTracking().ToList();
                    return peliculas;
                }
                catch (Exception ex)
                {
                    GuardarLog("DAO.ObtenerPeliculas " + ex.ToString(), 1004, "DAO.cs");
                    throw ex;
                }
            }
        }

        public static List<Cache_Peliculas> ObtenerPeliculas2D3D(List<Cache_Peliculas> peliculas, string tec)
        {
            using (CSWebNuevoEntities db = new CSWebNuevoEntities())
            {
                Cache_Tecnologias tecn = db.Cache_Tecnologias.AsNoTracking().ToList().Find(t => t.NomTecnologia.ToUpper().Contains(tec));
                return peliculas.Where(cp => db.Cache_Funciones.Any(cf => cf.CodPelicula == cp.CodPelicula && cf.CodTecnologia == tecn.CodTecnologia)).ToList();
            }
        }

        public static List<Cache_Peliculas> ObtenerPeliculasComplejo(string comp)
        {
            using (CSWebNuevoEntities db = new CSWebNuevoEntities())
            {
                int id_comp = int.Parse(comp);
                return db.Cache_Peliculas.Where(cp => db.Cache_Funciones.Any(cf => cf.CodPelicula == cp.CodPelicula && cf.CodComplejo == id_comp)).ToList();
            }
        }

        public static List<Cache_Peliculas> ObtenerPeliculasEstreno(List<Cache_Peliculas> peliculas)
        {
            using (CSWebNuevoEntities db = new CSWebNuevoEntities())
            {
                var resultado = (
                    from p in peliculas
                    join cs in db.Cache_Cinesemanas
                    on true equals true
                    into csGroup
                    from cs in csGroup.DefaultIfEmpty()
                    where p.Estreno >= cs.Desde && p.Estreno <= cs.Hasta && cs.CodComplejo != 0 // null
                    orderby p.Estreno, cs.CodComplejo
                    select new
                    {
                        Pelicula = p,
                        cs.CodComplejo,
                        cs.Desde,
                        cs.Hasta
                    }
                )
                .DistinctBy(x => x.Pelicula.CodPelicula)
                .Select(x => x.Pelicula)
                .ToList();

                return resultado;
            }
        }

        public static List<Cache_Peliculas> ObtenerPeliculasPreventa(List<Cache_Peliculas> peliculas)
        {
            using (CSWebNuevoEntities db = new CSWebNuevoEntities())
            {
                var resultado = (
                    from p in peliculas
                    join f in db.Cache_Funciones
                    on p.CodPelicula equals f.CodPelicula
                    join cs in (
                        from cinesemana in db.Cache_Cinesemanas
                        group cinesemana by cinesemana.CodComplejo into g
                        select new {
                            CodComplejo = g.Key,
                            UltimaFecha = g.Max(x => x.Hasta)
                        }
                    )
                    on f.CodComplejo equals cs.CodComplejo
                    where f.Fecha > cs.UltimaFecha
                    orderby p.CodPelicula
                    select p
                )
                .Distinct()
                .ToList();

                return resultado;
            }
        }

        /*
        public static List<Feature> ObtenerPeliculasNuevo()
        {
            List<Feature> result = new MovieController().GetMoviesAsync(190, "").GetAwaiter().GetResult();
            return result;
        }
        */

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
                    cache.UltimaActualizacion = fecha;
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

        private static void GuardarSala(CSWebNuevoEntities db, Cache_Salas sala, int codComp)
        {
            Cache_Salas exist = new CSWebNuevoEntities().Cache_Salas.AsNoTracking().SingleOrDefault(e => e.CodSala == sala.CodSala && e.CodComplejo == codComp);
            if (exist == null)
            {
                sala.CodComplejo = codComp;
                db.Cache_Salas.Add(sala);
                db.SaveChanges();
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
                    cache.UltimaActualizacion = fecha;
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