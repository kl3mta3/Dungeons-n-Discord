using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDBot.System
{


    /// <summary>
    /// The discord config.
    /// </summary>
    public partial class DiscordConfig
    {
        /// <summary>
        /// Gets or sets the bot token.
        /// </summary>
        [JsonProperty("bot_token")]
        public string BotToken { get; set; }

        /// <summary>
        /// Gets or sets the prefix.
        /// </summary>
        [JsonProperty("prefix")]
        public string Prefix { get; set; }







    }
}
