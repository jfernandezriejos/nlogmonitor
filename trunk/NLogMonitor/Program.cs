using System;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Net;
using System.Threading;

namespace LogMonitor
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] logs_files_name = Directory.GetFiles(AppConfiguration.Instance.Directory, 
                                                          AppConfiguration.Instance.FilePattern);
            
            foreach (string file_name in logs_files_name)
            {
                FileStream      fs              = new FileStream(file_name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader    reader          = new StreamReader(fs);
                string          file_content    = reader.ReadToEnd();
                StringBuilder   mail_content    = new StringBuilder();

                Console.WriteLine("Procesando " + file_name + "...");
                mail_content.Append(process_content(AppConfiguration.Instance.TextPattern, file_name, file_content)); // procesamos errores
                Console.WriteLine(file_name + " procesado");
                
                if (mail_content.Length != 0)   // si el fichero ha tenido algun error
                {
                    Console.WriteLine("Enviando correo ...");
                    try
                    {
                        send_email(file_name, mail_content.ToString());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Se produjo una excepción en el envio del correo");
                        Console.WriteLine(ex.StackTrace);
                    }

                    Thread.Sleep(SmtpConfiguration.Instance.TimeBetweenSend * 1000);

                    Console.WriteLine("Correo enviado");
                }               
            }
        }

        static String process_content(string pattern, string file_name, string file_content)
        {
            Regex           reg         = new Regex(pattern, RegexOptions.Compiled);            
            StringBuilder   sb_matched  = new StringBuilder();
            DateTime        t           = DateTime.Now;
            MatchCollection mc          = reg.Matches(file_content);

            Console.WriteLine("{0} matches found. Tardo {1} segundos", mc.Count, DateTime.Now - t);

            foreach (Match m in mc)
            {
                sb_matched.AppendLine(m.Value);
            }

            return sb_matched.ToString();
        }

        static void send_email(String file_name, String mail_content)
        {
            MailAddress ma_from = new MailAddress(MailConfiguration.Instance.From);
            MailAddress ma_to   = new MailAddress(MailConfiguration.Instance.To);
            MailMessage m       = new MailMessage(ma_from, ma_to);

            m.Body = mail_content;
            m.Subject = MailConfiguration.Instance.Subject + " " + file_name;

            if (MailConfiguration.Instance.CC != "")
            {
                string[] addresses = MailConfiguration.Instance.CC.Split(',');
                MailAddressCollection ma_cc = new MailAddressCollection();

                foreach (string address in addresses)
                {
                    m.CC.Add(new MailAddress(address));
                }
            }
            
            SmtpClient client = new SmtpClient(SmtpConfiguration.Instance.Host, SmtpConfiguration.Instance.Port);
            client.EnableSsl = SmtpConfiguration.Instance.EnableSsl;
            client.Credentials = new NetworkCredential(SmtpConfiguration.Instance.User, SmtpConfiguration.Instance.Pass);
            
            client.Send(m);
        }
    }
}
