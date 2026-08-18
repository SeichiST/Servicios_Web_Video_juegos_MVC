using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Servicios_Web_Video_juegos_MVC.Helpers
{
    public static class EmailHelper
    {
        private const string CorreoEmisor = "0123cloud3210@gmail.com"; //Gmail
        private const string ClaveAplicacion = "qdcx zeuf dgpv qaqv"; //clave16

        ///
        /// plantilla de correo de confirmación en formato HTML
        ///
        public static string GenerarPlantillaConfirmacion(string nombreUsuario, string mensajeOriginal) {
            return $@"
            <div style='font-family: Arial, sans-serif; color: #333; line-height: 1.6; max-width: 600px; margin: auto; border: 1px solid #e0e0e0; border-radius: 8px; padding: 20px;'>
                <h2 style='color: #2c3e50; border-bottom: 2px solid #3498db; padding-bottom: 10px;'>
                    ¡Gracias por comunicarte con nosotros, {nombreUsuario}!
                </h2>
                <p>Hemos recibido tu consulta satisfactoriamente. Nuestro equipo de soporte la revisará a la brevedad posible.</p>
                
                <div style='background-color: #f8f9fa; border-left: 4px solid #3498db; padding: 15px; margin: 20px 0;'>
                    <h4 style='margin-top: 0; color: #555;'>Copia de tu mensaje enviado:</h4>
                    <p style='font-style: italic; color: #666; margin-bottom: 0;'>""{mensajeOriginal}""</p>
                </div>

                <p>Si deseas agregar información adicional, puedes responder directamente a este correo.</p>
                
                <hr style='border: none; border-top: 1px solid #eee; margin-top: 30px;' />
                <p style='font-size: 12px; color: #888; text-align: center;'>
                    Este es un mensaje automático de confirmación. Por favor no elimines esta referencia.
                </p>
            </div>";
        }

        ///
        /// Envía un correo electrónico asíncrono utilizando el servidor SMTP de Gmail
        ///
        public static async Task EnviarCorreoAsync(string correoDestino, string asunto, string cuerpoHtml) {
            using (MailMessage mail = new MailMessage()) {
                mail.From = new MailAddress(CorreoEmisor, "Soporte Videojuegos MVC");
                mail.To.Add(correoDestino);
                mail.Subject = asunto;
                mail.Body = cuerpoHtml;
                mail.IsBodyHtml = true;

                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587)) {
                    smtp.Credentials = new NetworkCredential(CorreoEmisor, ClaveAplicacion);
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(mail);
                }
            }
        }
    }
}