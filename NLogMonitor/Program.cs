using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data.SQLite;
using System.Data.Common;
using System.Data;
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
                mail_content.Append(process_content(".*LM_ERROR.*", file_name, file_content)); // procesamos errores
                mail_content.Append(process_content(".*LM_WARNING.*", file_name, file_content)); // procesamos errores
                if (mail_content.Length != 0)   // si el fichero ha tenido algun error
                {
                    send_email(mail_content.ToString());   
                }               
            }
        }

        static String process_content(string pattern, string file_name, string file_content)
        {
            Regex reg = new Regex(pattern);

            MatchCollection mc = reg.Matches(file_content);
            StringBuilder line_matched = new StringBuilder();
            foreach (Match m in mc)
            {
                line_matched.AppendLine(m.Value);
            }

            return line_matched.ToString();
        }

        static void send_email(String mail_content)
        {
            MailAddress ma_from = new MailAddress(MailConfiguration.Instance.From);

            MailMessage m = new MailMessage(ma_from, ma_from);
            m.Body = mail_content;

            SmtpClient client = new SmtpClient(SmtpConfiguration.Instance.Host, SmtpConfiguration.Instance.Port);
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(SmtpConfiguration.Instance.User, SmtpConfiguration.Instance.Pass);
            client.Send(m);
        }
    }
}
