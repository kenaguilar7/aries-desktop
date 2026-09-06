using AriesContador.Core.Models.Companies;
using CapaDatos.Daos;
using CapaEntidad.Entidades.Usuarios;
using CapaEntidad.Verificaciones;
using System;
using System.Collections.Generic;


namespace CapaLogica
{
    public class CompañiaCL
    {
        private CompañiaDao _compañiaDao;
        private CompañiaDao compañiaDao
        {
            get
            {
                if (_compañiaDao == null) _compañiaDao = new CompañiaDao();
                return _compañiaDao;
            }
        }
        /// <summary>
        /// Se inserta la compañia en la base de datos
        /// </summary>
        /// <param name="t"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public Boolean Insert(Company t, Usuario user, Company copiarDe, out String mensaje)
        {

            try
            {

                ///Mandemos estas verificaciones a la capa entida
                if (!VerificaString.VerificarID(t.NumberId, t.IdType, out mensaje))
                {
                    return false;
                }
                if (!VerificaString.IsNullOrWhiteSpace(t.Name, "Nombre", out mensaje))
                {
                    return false;
                }
                if (!VerificaString.ValidarEmail(t.Mail))
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


        public Boolean Update(Company t, Usuario user, out String mensaje)
        {
            try
            {
                if (!VerificaString.VerificarID(t.IdNumber, t.IdType, out mensaje))
                {
                    return false;
                }
                if (!VerificaString.IsNullOrWhiteSpace(t.Name, "Nombre", out mensaje))
                {
                    return false;
                }
                if (!VerificaString.ValidarEmail(t.Mail))
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

        /// <summary>
        /// Devuleve la lista con todas las compañias, Esta lista trae en 
        /// las primeras posiciones las personas fisicas y despues las juridicas 
        /// puede ordenarlas. 
        /// </summary>
        /// <returns></returns>
        public List<Company> GetAll(Usuario usuario)
        {
            return compañiaDao.GetAll(usuario);
        }
    }
}
