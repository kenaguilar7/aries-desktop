using CapaDatos.Daos;
using CapaEntidad.Entidades.Compañias;
using CapaEntidad.Entidades.Usuarios;
using CapaEntidad.Enumeradores;
using CapaEntidad.Verificaciones;
using System;
using System.Collections.Generic;
using System.Data;


namespace CapaLogica
{
    public class CompañiaCL
    {
        CompañiaDao compañiaDao = new CompañiaDao();
        /// <summary>
        /// Se inserta la compañia en la base de datos
        /// </summary>
        /// <param name="t"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public Boolean Insert(Compañia t, Usuario user, Compañia copiarDe, out String mensaje)
        {

            try
            {

                ///Mandemos estas verificaciones a la capa entida
                if (!VerificaString.VerificarID(t.NumeroCedula, t.TipoId, out mensaje))
                {
                    return false;
                }
                if (!VerificaString.IsNullOrWhiteSpace(t.Nombre, "Nombre", out mensaje))
                {
                    return false;
                }
                if (!VerificaString.ValidarEmail(t.Correo))
                {
                    mensaje = "Formato de correo invalido";
                    return false;
                }

                if (compañiaDao.Insert(t, user, copiarDe, out mensaje))
                {
                    return true;
                }
                else
                {
                    return false;
                }



            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return false;
            }

        }
        public Boolean Update(Compañia t, Usuario user, out String mensaje)
        {
            try
            {
                if (!VerificaString.VerificarID(t.NumeroCedula, t.TipoId, out mensaje))
                {
                    return false;
                }
                if (!VerificaString.IsNullOrWhiteSpace(t.Nombre, "Nombre", out mensaje))
                {
                    return false;
                }
                if (!VerificaString.ValidarEmail(t.Correo))
                {
                    mensaje = "Formato de correo invalido";
                    return false;
                }

                if (compañiaDao.Update(t, user, out mensaje))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return false;
            }


            
        }
        public string NuevoCodigo()
        {
            return compañiaDao.NuevoCodigo();
        }
        /// <summary>
        /// Devuleve la lista con todas las compañias, Esta lista trae en 
        /// las primeras posiciones las personas fisicas y despues las juridicas 
        /// puede ordenarlas. 
        /// </summary>
        /// <returns></returns>
        public List<Compañia> GetAll(Usuario usuario)
        {
            return compañiaDao.GetAll(usuario);
        }
    }
}
