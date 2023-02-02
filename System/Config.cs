using DndBot.System;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDBot.System
{
    /// <summary>
	/// The config.
	/// </summary>
	public partial class Config
    {
        /// <summary>
        /// Gets or sets the bot config.
        /// </summary>
        [JsonProperty("discord")]
        public static DiscordConfig DiscordConfig { get; set; }

        /// <summary>
        /// Gets or sets the database config.
        /// </summary>
        [JsonProperty("database")]
        public static DatabaseConfig DatabaseConfig { get; set; }

        /// <summary>
        /// Gets or sets the phabricator conduit config.
        /// </summary>
        [JsonProperty("system")]
        public static SystemCommandConfig SystemCommandConfig { get; set; }
    }
}
