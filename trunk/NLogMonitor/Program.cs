using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data.SQLite;
using System.Data.Common;
using System.Data;
using System.Text.RegularExpressions;

namespace LogMonitor
{
    class Program
    {
        static void Main(string[] args)
        {
            IDbConnection conn = new SQLiteConnection("Data Source=NLogMonitor.sqlite3;Version=3;");
            conn.Open();
            
            string[] logs_files_name = Directory.GetFiles("../../logs");
            
            foreach (string file_name in logs_files_name)
            {
                string file_content = File.ReadAllText(file_name);
                process_content(file_name, file_content, conn);                
            }
        }

        static void process_content(string file_name, string file_content, IDbConnection conn)
        {
            Regex reg = new Regex(".*LM_DEBUG.*");

            MatchCollection mc = reg.Matches(file_content);
            foreach (Match m in mc)
            {
                IDbCommand cmd = conn.CreateCommand();
                cmd.CommandText = String.Format( "INSERT INTO Errors (file_name, text) VALUES ('{0}', '{1}')", file_name, m.Value );
                cmd.ExecuteNonQuery();
            }            
        }
    }
}
