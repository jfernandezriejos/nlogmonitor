using System;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Net;

namespace LogMonitor
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] logs_files_name = Directory.GetFiles(AppConfiguration.Instance.Directory);
            
            foreach (string file_name in logs_files_name)
            {
                string file_content = File.ReadAllText(file_name);
                StringBuilder mail_content = new StringBuilder();
                Console.WriteLine("Procesando " + file_name + "...");
                mail_content.Append(process_content(".*LM_ERROR.*|.*LM_WARNING.*", file_name, file_content)); // procesamos errores
                Console.WriteLine(file_name + " procesado");
                
                if (mail_content.Length != 0)   // si el fichero ha tenido algun error
                {
                    Console.WriteLine("Enviando correo ...");
                    send_email(file_name, mail_content.ToString());
                    Console.WriteLine("Correo enviado");
                }               
            }
        }

        static String process_content(string pattern, string file_name, string file_content)
        {
            Regex reg = new Regex(pattern, RegexOptions.Compiled);

            DateTime t = DateTime.Now;
            MatchCollection mc = reg.Matches(file_content);
            Console.WriteLine("{0} matches found. Tardo {1} segundos", mc.Count, DateTime.Now - t);

            StringBuilder line_matched = new StringBuilder();
            foreach (Match m in mc)
            {
                line_matched.AppendLine(m.Value);
            }

            return line_matched.ToString();
        }

        static void send_email(String file_name, String mail_content)
        {
            MailAddress ma_from = new MailAddress(MailConfiguration.Instance.From);

            MailMessage m = new MailMessage(ma_from, ma_from);
            m.Body = mail_content;
            m.Subject = "Hay errores en el fichero " + file_name;

            SmtpClient client = new SmtpClient(SmtpConfiguration.Instance.Host, SmtpConfiguration.Instance.Port);
            client.EnableSsl = SmtpConfiguration.Instance.EnableSsl;
            
            if (client.EnableSsl)
            {
                client.Credentials = new NetworkCredential(SmtpConfiguration.Instance.User, SmtpConfiguration.Instance.Pass);
            }

            client.Send(m);
        }
    }
}
