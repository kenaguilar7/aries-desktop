using CapaDatos.Daos;
using CapaEntidad.Entidades.Usuarios;
using CapaEntidad.Enumeradores;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CapaLogica
{
    public class CorreoCL
    {
        CorreoDao correoDao = new CorreoDao();
        public DataTable Get()
        {

            return correoDao.Get();
        }
        public Boolean Insert(UsuarioTemporal usuarioTemporal)
        {
            return correoDao.Insert(usuarioTemporal);
        }

        public IEnumerable<UsuarioTemporal> SendMail(IEnumerable<UsuarioTemporal> usuariosTemporales)
        {
            var rejecteds = new List<UsuarioTemporal>();
            foreach (var user in usuariosTemporales)
            {

                if (!Push(user))
                {
                    rejecteds.Add(user);
                    user.EstadoEnvio = EstadoEnvio.Fallido;
                }
                else
                {
                    user.EstadoEnvio = EstadoEnvio.Correcto;
                }

                Insert(user);
            }
            return rejecteds;
        }
        public Boolean Push(UsuarioTemporal usuario)
        {
            try
            {
                var client = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential("ariescontador@gmail.com", "116390867A.ab"),
                    // UseDefaultCredentials = true,
                    // DeliveryMethod = SmtpDeliveryMethod.Network,
                    EnableSsl = true
                };
                MailMessage mail = new MailMessage();
                //Setting From , To and CC
                mail.From = new MailAddress("ariescontador@gmail.com", "Sistemas Aries");
                mail.To.Add(new MailAddress(usuario.CorreoElectronico));
                mail.CC.Add(new MailAddress(usuario.CCopy));
                mail.Subject = usuario.Asunto;
                mail.IsBodyHtml = true;
                mail.Body = $"<html><nav><h1>{usuario.Titulo}</h1><span>{usuario.Cuerpo}</span><h6></h6></nav></html>";
                client.Send(mail);
                //Console.WriteLine("Sent");
                //Console.ReadLine();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
