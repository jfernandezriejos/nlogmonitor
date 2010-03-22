using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace LogMonitor
{
    class AppConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("Directory", IsRequired = true)]
        public String Directory
        {
            get { return (String)this["Directory"]; }
        }

        [ConfigurationProperty("FilePattern", IsRequired = true)]
        public String FilePattern
        {
            get { return (String)this["FilePattern"]; }
        }

        [ConfigurationProperty("TextPattern", IsRequired = true)]
        public String TextPattern
        {
            get { return (String)this["TextPattern"]; }
        }
    }

    class SmtpConfiguration : ConfigurationSection
    {
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

        [ConfigurationProperty("EnableSsl", IsRequired = true)]
        public bool EnableSsl
        {
            get { return (bool)this["EnableSsl"]; }
        } 
        
        [ConfigurationProperty("TimeBetweenSend", IsRequired = true)]
        public int TimeBetweenSend
        {
            get { return (int)this["TimeBetweenSend"]; }
        } 
    }

    class MailConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("From", IsRequired = true)]
        public String From
        {
            get { return (String)this["From"]; }
        }

        [ConfigurationProperty("To", IsRequired = true)]
        public String To
        {
            get { return (String)this["To"]; }
        }

        [ConfigurationProperty("CC", IsRequired = true)]
        public String CC
        {
            get { return (String)this["CC"]; }
        }

        [ConfigurationProperty("Subject", IsRequired = true)]
        public String Subject
        {
            get { return (String)this["Subject"]; }
        } 
    }
}
