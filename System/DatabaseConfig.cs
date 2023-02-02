using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDBot.System
{
    /// <summary>
	/// The database config.
	/// </summary>
	public partial  class DatabaseConfig
    {
        /// <summary>
        /// Gets or sets the hostname.
        /// </summary>
        [JsonProperty("host")]
        public  string hostname { get; set; }


        /// <summary>
        /// Gets or sets the hostname.
        /// </summary>
        [JsonProperty("port")]
        public string port { get; set; }
        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        [JsonProperty("user")]
        public  string user { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [JsonProperty("password")]
        public  string password { get; set; }

        /// <summary>
        /// Gets or sets the database.
        /// </summary>
        [JsonProperty("db")]
        public  string dbName { get; set; }
    }
}
