using System;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Net;
using System.Threading;
using System.Configuration;

namespace LogMonitor
{
    class Program
    {
        static AppConfiguration     app_conf;
        static SmtpConfiguration    smtp_conf;
        static MailConfiguration    mail_conf;

        static void Main(string[] args)
        {
            if(args.Length != 1)
            {
                Console.WriteLine("No se ha especificado fichero de configuración");
                Console.WriteLine("Uso:");
                Console.WriteLine("Windows: NLogMonitor <ficheroDeConfiguracion>");
                Console.WriteLine("Linux: mono NLogMonitor <ficheroDeConfiguracion>");
            }
            else
            {
                ExeConfigurationFileMap exe_file = new ExeConfigurationFileMap();
                exe_file.ExeConfigFilename = args[0];
                Configuration conf = ConfigurationManager.OpenMappedExeConfiguration(exe_file, ConfigurationUserLevel.None);
                app_conf = (AppConfiguration) conf.GetSection("AppConfiguration");
                smtp_conf = (SmtpConfiguration) conf.GetSection("SmtpConfiguration");
                mail_conf = (MailConfiguration) conf.GetSection("MailConfiguration");

                string[] logs_files_name = Directory.GetFiles(app_conf.Directory, app_conf.FilePattern);

                foreach (string file_name in logs_files_name)
                {
                    FileStream      fs              = new FileStream(file_name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    StreamReader    reader          = new StreamReader(fs);
                    string          file_content    = reader.ReadToEnd();
                    StringBuilder   mail_content    = new StringBuilder();

                    Console.WriteLine("Procesando " + file_name + "...");
                    mail_content.Append(process_content(app_conf.TextPattern, file_name, file_content)); // procesamos errores
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

                        Thread.Sleep(smtp_conf.TimeBetweenSend * 1000);

                        Console.WriteLine("Correo enviado");
                    }               
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
            MailAddress ma_from = new MailAddress(mail_conf.From);
            MailAddress ma_to   = new MailAddress(mail_conf.To);
            MailMessage m       = new MailMessage(ma_from, ma_to);

            m.Body = mail_content;
            m.Subject = mail_conf.Subject + " " + file_name;

            if (mail_conf.CC != "")
            {
                string[] addresses = mail_conf.CC.Split(',');
                MailAddressCollection ma_cc = new MailAddressCollection();

                foreach (string address in addresses)
                {
                    m.CC.Add(new MailAddress(address));
                }
            }

            SmtpClient client   = new SmtpClient(smtp_conf.Host, smtp_conf.Port);
            client.EnableSsl    = smtp_conf.EnableSsl;
            client.Credentials  = new NetworkCredential(smtp_conf.User, smtp_conf.Pass);
            
            client.Send(m);
        }
    }
}
