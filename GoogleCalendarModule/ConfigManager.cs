using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleCalendarModule
{

    /// <summary>
    /// Singleton container class to store data relative to the calendar, this is a 
    /// mock container in order to exemplify the use of the calendar, feel free to stores 
    /// this in a database, config file of whatever you feel is more appropriate
    /// </summary>
    internal class ConfigManager
    {

        private static ConfigManager instance = null;

        public string GooogleCalendarId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

        private ConfigManager()
        {
            this.GooogleCalendarId = "primary"; // Setting for the default google calendar
         
            // Of course this two credentials are fake and only meant to give an example of how the clientId and ClientSecret look like
            this.ClientId = "526573123428-9ejleevf6ea33o7i0t244aiasblebfbc.apps.googleusercontent.com";
            this.ClientSecret = "pdYxuN4elOGQSqAZSuVr0KgX";
        }

        public static ConfigManager GetInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ConfigManager();
                }
                return instance;
            }
        }

    }
}
