
using System.Reflection;

using Serilog;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using DisCatSharp;
using DisCatSharp.Entities;

using DisCatSharp.CommandsNext;

using DisCatSharp.Net;
using DisCatSharp.ApplicationCommands;

using DisCatSharp.Interactivity;
using DisCatSharp.Interactivity.Extensions;

using DnDBot.Character.Commands;
using Newtonsoft.Json;
using DnDBot.WorldSystem;
using DnDBot.Character;
using System.Numerics;
using DisCatSharp.CommandsNext.Attributes;
using DnDBot.UniversalCommands;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using MySqlX.XDevAPI;
using System.Data;
using DnDBot.System;
namespace DnDBot
{
    public class DnDBot
    {
        public static Task Main(string[] args) => new DnDBot().MainAsync();

        private string messageLogName = "dndbot-message-log";
        private string commandLogName = "dndbot-command-log";
        private string deleteLogName = "dndbot-deleted-log";

        public static Config Config { get; internal set; }
        public static DiscordClient DiscordClient { get; internal set; }

        internal static string connStr = "server="+Config.DatabaseConfig.hostname+";userid="+Config.DatabaseConfig.user+";password="+Config.DatabaseConfig.password+";database="+Config.DatabaseConfig.dbName;
        internal static  string tolken = @$"{Config.DiscordConfig.BotToken}";

        public static DiscordGuild? homeServer;
        public static DiscordChannel? messagesDeleted;
        public static DiscordChannel? messageLogChannel;
        public static DiscordChannel? commandChannel;

        public static List<dnDGuild> masterGuildList = new List<dnDGuild>();


        public static WorldGridSpace worldSpace = new WorldGridSpace();

        public static DiscordClient? discord;
        public static DiscordUser? god;

        public static MySqlConnection? mYSQlconn;

        public async Task MainAsync()
        {
            Console.WriteLine("Starting bot...");
            Config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(@"config.json"));
            List<string> prefixes = new List<string>();

            prefixes.Add(@$"{Config.DiscordConfig.Prefix}");

            var services = new ServiceCollection()
                .AddSingleton<Random>()
                .BuildServiceProvider();

            Log.Logger = new LoggerConfiguration()
                 .MinimumLevel.Debug()
                 .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                 .CreateLogger();

           
            DiscordConfiguration discCofig = new DiscordConfiguration()
            {
                Token = tolken,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged | DiscordIntents.All,
                MinimumLogLevel = LogLevel.Information,
            };

            discord = new DiscordClient(discCofig);



            discord.UseInteractivity(new InteractivityConfiguration
            {
                PollBehaviour = DisCatSharp.Interactivity.Enums.PollBehaviour.KeepEmojis,
                Timeout = TimeSpan.FromSeconds(30)
            });


            ///commandNext start///
           
            var nextCommands = discord.UseCommandsNext(new CommandsNextConfiguration
            {

                StringPrefixes = prefixes,
                ServiceProvider = services

            });

            nextCommands.RegisterCommands<CharacterCommandHandler>();
            nextCommands.RegisterCommands<ListCommandHandler>();
            nextCommands.RegisterCommands<SystemCommands>();
            discord.RegisterEventHandlers(Assembly.GetExecutingAssembly());


            ///commandNext end///


            ///shash commands start///
            Type appCommandModule = typeof(ApplicationCommandsModule);
            var commands = Assembly.GetExecutingAssembly().GetTypes().Where(t => appCommandModule.IsAssignableFrom(t) && !t.IsNested).ToList();
            var appCommandExt = discord.UseApplicationCommands();

            foreach (var command in commands)
            {
                appCommandExt.RegisterGlobalCommands(command);
            }
            discord.Logger.LogInformation("Application commands registered successfully");
            appCommandExt.RegisterGlobalCommands<RandomSlashCommands>();
            ///shash commands end///


            await discord.ConnectAsync();

            
            
            ///System Home Server Info Start///
            homeServer = discord.GetGuildAsync(Config.SystemCommandConfig.systemGuildID).Result;
            god = discord.GetUserAsync(Config.SystemCommandConfig.godUserID).Result;
            
            foreach (var item in homeServer.Channels)
            {
                if (item.Value.Name == messageLogName)
                {
                    messageLogChannel = item.Value;
                }
                if (item.Value.Name == commandLogName)
                {
                    commandChannel = item.Value;
                }
                if (item.Value.Name == deleteLogName)
                {
                    messagesDeleted = item.Value;
                }
            }
            
            discord.Logger.LogInformation($"God User Set to {god.Username}");
            discord.Logger.LogInformation($"Connection success! Logged in as {discord.CurrentUser.Username}#{discord.CurrentUser.Discriminator} ({discord.CurrentUser.Id})");
            ///System Home Server Info Start///

            
            /// Database Connecting stuff Start ///

            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            
                //Player player = new Player();
                //player.InsertPlayerIntoDataBase(conn);
          
            string databaseName = conn.Database;
            Console.WriteLine("Database Name: " + databaseName);
            if (conn.Ping())
            {
                Console.WriteLine("Connection is  open");
            }
            else
            {
                Console.WriteLine("Connection is closed");
            }
            conn.Close();
            /// Database Connecting stuff End///


            Console.WriteLine("Starting WorldGrid Load");
            worldSpace.worldgrid = WorldGrid.GetGridsFromDatabase();
            float maxValue = MathF.Sqrt(worldSpace.worldgrid.Count);
            worldSpace.worldMaxYValue = (int)maxValue;
            worldSpace.worldMaxXValue = (int)maxValue;
            Console.WriteLine("Finished WorldGrid Load");

            await Task.Delay(-1);
        }


       
        public static  Task CreateWorld( int _worldSize)
        {

            worldSpace.name = "New World Name";
            worldSpace.worldSize = _worldSize;
            ulong  gridId = 0;
            float xMaxValue = MathF.Sqrt(_worldSize);
            float yMaxValue = MathF.Sqrt(_worldSize);
            worldSpace.worldMaxYValue = (int)yMaxValue;
            worldSpace.worldMaxXValue = (int)xMaxValue;
            float xValue = 0;
            float yvalue = yMaxValue;


            for (int i = 0; i < _worldSize; i++)
            {
            WorldGrid newGrid = new WorldGrid();
                gridId++;
                
                newGrid.gridID = gridId;
                newGrid.onPath = false;
                newGrid.isWater = false;
                newGrid.isWilds = false;
                if (xValue >=xMaxValue)
                {
                    xValue = 1;
                    yvalue--;
                }
                else
                {
                    xValue++;



                }
                newGrid.gridLocation = new Vector2(xValue, yvalue);
                newGrid.zoneID = 1;
                //Console.Write($"Grid:{gridId} was Created with X value");

                worldSpace.worldgrid.Add(newGrid);
                discord.Logger.LogInformation($"Grid with ID {worldSpace.worldgrid[i].gridID} ({worldSpace.worldgrid[i].gridLocation.X}, {worldSpace.worldgrid[i].gridLocation.Y}) created.");
            }

            discord.Logger.LogInformation($"World Created with a size of {worldSpace.worldSize} and a row count of{worldSpace.worldMaxXValue}.");


            return Task.CompletedTask;
        }

       
        public async static  Task FillCharacterDatabase()
        {
           
           
            Console.Write($"fill database started ");
            foreach (KeyValuePair<ulong, DiscordGuild> guild in DnDBot.discord.Guilds)
            {
                    dnDGuild newGuild = new dnDGuild();
                    newGuild.dndGuildMembers = new List<Player>();
                    newGuild.serverID = guild.Key;
                    newGuild.serverLevel = 0;
                    newGuild.serverXp = 1;
                    masterGuildList.Add(newGuild);

            }
           
            
            foreach (dnDGuild g in masterGuildList)
            {
              
                var _guild = discord.GetGuildAsync(g.serverID).Result;
                foreach (KeyValuePair<ulong,DiscordMember> u in _guild.Members)
                {
                    
                    if (u.Value.IsBot == false)
                    {

                        var user = u.Value;
                        Player newPlayer = new Player();
                        newPlayer.discordMember = user;

                        if (user.Id == god.Id)
                        {
                            newPlayer.playerType = PlayerType.God;
                        }
                        else
                        {
                            newPlayer.playerType = PlayerType.Player;
                        }

                        newPlayer.playerID = u.Key;
                        newPlayer.homeGuildID = user.Guild.Id;
                        newPlayer.homeGuildName = user.Guild.Name;
                        newPlayer.playerLevel = 1;
                        newPlayer.xP = 1;
                        newPlayer.cash = 1;
                        newPlayer.currentPosition = new Vector2(1, 1);
                        newPlayer.name = user.Username;
                       
                        
                        
                        MySqlConnection sqconn = new MySqlConnection(connStr);
                        await sqconn.OpenAsync();

                       bool exists = Player.CheckPlayerIdExists(sqconn,newPlayer.playerID);
                        await sqconn.CloseAsync();

                        if (exists)
                        {
                            Log.Logger.Information($"New player {newPlayer.name} already in Database in guild {newPlayer.homeGuildName}.");
                        }
                        else
                        {
                            
                            await sqconn.OpenAsync();
                            newPlayer.InsertPlayerIntoDataBase(sqconn);
                            await sqconn.CloseAsync();
                            Log.Logger.Information($"New player {newPlayer.name} added from guild {newPlayer.homeGuildName} to DataBase");
                          
                        }
                    }
                }
            }
            return ;
        }

        
    }

            
     





    public class dnDGuild
    {
        internal ulong serverID;

        internal float serverXp;

        internal float serverLevel;

        internal string serverName;

        public List<Player> dndGuildMembers = new List<Player>();

    }



}