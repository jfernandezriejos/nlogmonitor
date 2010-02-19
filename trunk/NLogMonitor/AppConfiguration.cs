using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace LogMonitor
{
    class AppConfiguration : ConfigurationSection
    {
        static public AppConfiguration Instance = (AppConfiguration) ConfigurationManager.GetSection("AppConfiguration");

        [ConfigurationProperty("Directory", IsRequired = true)]
        public String Directory
        {
            get { return (String)this["Directory"]; }
        }
    }

    class SmtpConfiguration : ConfigurationSection
    {
        static public SmtpConfiguration Instance = (SmtpConfiguration)ConfigurationManager.GetSection("SmtpConfiguration");

        [ConfigurationProperty("Host", IsRequired = true)]
        public String Host
        {
            get { return (String)this["Host"]; }
        }

        [ConfigurationProperty("Port", IsRequired = true)]
        public int Port
        {
            get { return (int)this["Port"]; }
        }

        [ConfigurationProperty("User", IsRequired = true)]
        public String User
        {
            get { return (String)this["User"]; }
        }

        [ConfigurationProperty("Pass", IsRequired = true)]
        public String Pass
        {
            get { return (String)this["Pass"]; }
        }
    }

    class MailConfiguration : ConfigurationSection
    {
        static public MailConfiguration Instance = (MailConfiguration)ConfigurationManager.GetSection("MailConfiguration");

        [ConfigurationProperty("From", IsRequired = true)]
        public String From
        {
            get { return (String)this["From"]; }
        }

        [ConfigurationProperty("To", IsRequired = true)]
        public int To
        {
            get { return (int)this["To"]; }
        }       
    }
}
