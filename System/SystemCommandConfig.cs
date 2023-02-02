using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DndBot.System
{
    /// <summary>
    /// The application command config.
    /// </summary>
    public partial class SystemCommandConfig
    {
        /// <summary>
        /// Gets or sets the system Guild
        /// </summary>
        [JsonProperty("godUserID")]
        public ulong godUserID { get; set; }
        
        [JsonProperty("systemGuildID")]
        public ulong systemGuildID { get; set; }
        

    }
    
}
