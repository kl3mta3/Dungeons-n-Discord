using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using DnDBot.WorldSystem;
using DnDBot.Character.Commands;
using Serilog;
using DisCatSharp.Entities;
using MySql.Data.MySqlClient;

using System.Xml.Linq;
using System.Data;
using System.Collections;
using Mysqlx.Crud;
using Org.BouncyCastle.Utilities.Collections;
using DisCatSharp.CommandsNext;
using Microsoft.VisualBasic;
using System.Linq.Expressions;
using System.Reflection.PortableExecutable;

namespace DnDBot.Character
{
    public class CharacterSystem
    {

        //public static Dictionary<ulong, Character> characterList = new Dictionary<ulong, Character>();
        public static Dictionary<ulong, Player> characterDadabase = new Dictionary<ulong, Player>();



        public static Player GetPlayer(ulong id)
        {
            if (characterDadabase.ContainsKey(id))
            {
                return characterDadabase[id];
            }
            else
            {
                return null;
            }
        }

    }
    
    public enum PlayerType
    {
        God=0,
        PowerUser=1,
        Mod=2,
        Player=3,
        
    }
    public class Player
    {

        internal bool isBanned = false;
        internal PlayerType playerType;

        internal DiscordMember discordMember;

        internal WorldGrid grid = new WorldGrid();
        internal Vector2 currentPosition;
        internal string name = "";
        internal int cash;
        internal int xP;
        internal int playerLevel;
        internal ulong homeGuildID;
        internal string homeGuildName = "";
        internal ulong playerID;
        internal int xPToNextLevel;
        internal int maxLevel;
        internal int deaths;
        internal Vector2 deathPosition;
        //Base Stats


        internal int baseStamina = 50;
        internal int baseHealth = 50;
        internal int baseEnergy = 50;
        internal int baseArmor = 1;
        internal int baseStrength = 1;
        internal int baseIntellect = 1;
        internal int baseAgility = 1;

        internal int baseHerbalism = 1;
        internal int baseMining = 1;
        internal int gridsDiscovered = 0;

        internal int inventorySize = 0;
        internal int inventoryMax = 8;

        //additem array for inventory when item class is made

        public async Task MovePlayerTo(ulong player, Vector2 position)
        {
            Player p = Player.GetPlayerFromDataBase(player);
            await p.UpdatePlayerStatInDataBase(player, "worldPositionX", (int)position.X);
            await p.UpdatePlayerStatInDataBase(player, "worldPositionY", (int)position.Y);
            
            Log.Logger.Information($"{p.name} moved to {p.currentPosition.X},{p.currentPosition.X}d");
            await discordMember.SendMessageAsync($"You have moved to {p.currentPosition.X},{p.currentPosition.X}");
        }
        
        public void InsertPlayerIntoDataBase(MySqlConnection connection)
        {


            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            //connection.ChangeDatabase("dnddatabase");
            MySqlCommand command = new MySqlCommand("INSERT INTO players (name, playerId, xP, cash, playerLevel, worldPositionX, worldPositionY, homeGuildId, homeGuildName, baseStamina , baseHealth ,baseEnergy , baseArmor, baseStrength, baseIntellect , baseAgility, playerType, isBanned, gridsDiscovered, xPToNextLevel, maxLevel, deaths, deathPositionX, deathPositionY, baseHerbalism, baseMining, inventorySize, InventoryMax) VALUES (@name, @playerId, @xP, @cash, @playerLevel, @worldPositionX, @worldPositionY, @homeGuildId, @homeGuildName, @baseStamina , @baseHealth , @baseEnergy , @baseArmor, @baseStrength, @baseIntellect ,@baseAgility, @playerType, @isBanned, @gridsDiscovered, @xPToNextLevel, @maxLevel, @deaths, @deathPositionX, @deathPositionY, @baseHerbalism, @baseMining, @inventorySize, @InventoryMax)", connection);
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@playerId", playerID);
            command.Parameters.AddWithValue("@xP", xP);
            command.Parameters.AddWithValue("@cash", cash);
            command.Parameters.AddWithValue("@playerLevel", playerLevel);
            command.Parameters.AddWithValue("@worldPositionX", currentPosition.X);
            command.Parameters.AddWithValue("@worldPositionY", currentPosition.Y);
            command.Parameters.AddWithValue("@homeGuildId", homeGuildID);
            command.Parameters.AddWithValue("@homeGuildName", homeGuildName);
            command.Parameters.AddWithValue("@baseStamina", baseStamina);
            command.Parameters.AddWithValue("@baseHealth", baseHealth);
            command.Parameters.AddWithValue("@baseEnergy", baseEnergy);
            command.Parameters.AddWithValue("@baseArmor", baseArmor);
            command.Parameters.AddWithValue("@baseStrength", baseStrength);
            command.Parameters.AddWithValue("@baseIntellect", baseIntellect);
            command.Parameters.AddWithValue("@baseAgility", baseAgility);
            command.Parameters.AddWithValue("@isBanned", isBanned);
            command.Parameters.AddWithValue("@playerType", (int)playerType);
            command.Parameters.AddWithValue("@gridsDiscovered", gridsDiscovered);
            command.Parameters.AddWithValue("@xPToNextLevel", xPToNextLevel);
            command.Parameters.AddWithValue("@maxLevel", maxLevel);
            command.Parameters.AddWithValue("@deaths", deaths);
            command.Parameters.AddWithValue("@deathPosition.X", deathPosition.X);
            command.Parameters.AddWithValue("@deathPosition.Y", deathPosition.Y);
            command.Parameters.AddWithValue("@baseHerbalism", baseHerbalism);
            command.Parameters.AddWithValue("baseMining", baseMining);
            command.Parameters.AddWithValue("@inventorySize", inventorySize);
            command.Parameters.AddWithValue("@InventoryMax", inventoryMax);


            command.ExecuteNonQuery();
            connection.Close();////

        }//works

        

        public static bool CheckPlayerIdExists(MySqlConnection connection, ulong playerid)//works
        {
  
            MySqlCommand command = new MySqlCommand("SELECT COUNT(*) FROM players WHERE playerid = @playerID", connection);
            command.Parameters.AddWithValue("@playerID", playerid);
            command.ExecuteNonQuery();
            int count = Convert.ToInt32(command.ExecuteScalar());
   
            return count > 0;
        }




        // method to retrieve all players from the MySQL table
        public static List<Player> GetAllPlayersFromDataBase(MySqlConnection connection)//doesnt work
        {
            List<Player> players = new List<Player>();
            MySqlCommand command = new MySqlCommand("SELECT * FROM players", connection);
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {

                Player player = new Player();
                player.name = reader.GetString("name");
                player.playerID = (ulong)reader.GetInt32("playerID");
                player.xP = reader.GetInt32("xP");
                player.cash = reader.GetInt32("cash");
                player.playerLevel = reader.GetInt32("playerLevel");
                player.currentPosition.X = reader.GetInt32("worldPosition.X");
                player.currentPosition.Y = reader.GetInt32("worldPosition.Y");
                player.homeGuildID = (ulong)reader.GetInt32("homeGuildID");

                player.homeGuildName = reader.GetString("homeGuildName");
                player.baseStamina = reader.GetInt32("baseStamina");
                player.baseHealth = reader.GetInt32("baseHealth");
                player.baseEnergy = reader.GetInt32("baseEnergy");
                player.baseArmor = reader.GetInt32("baseArmor");
                player.baseStrength = reader.GetInt32("baseStrength");
                player.baseIntellect = reader.GetInt32("baseIntellect");
                player.baseAgility = reader.GetInt32("baseAgility");
                player.gridsDiscovered = reader.GetInt32("gridsDiscovered");
                player.xPToNextLevel = reader.GetInt32("xPToNextLevel");
                player.maxLevel = reader.GetInt32("maxLevel");
                player.isBanned = reader.GetBoolean("isBanned");
                player.playerType = (PlayerType)reader.GetInt32("playerType");
                player.deaths = reader.GetInt32("deaths");
                player.deathPosition.X = reader.GetInt32("deathPosition.X");
                player.deathPosition.Y = reader.GetInt32("deathPosition.Y");
                player.baseHerbalism= reader.GetInt32("baseHerbalism");
                player.baseMining= reader.GetInt32("baseMining");
                player.inventorySize = reader.GetInt32("inventorySize");
                player.inventoryMax = reader.GetInt32("inventoryMax");
                player.discordMember = GetDiscordMember(player.homeGuildID, player.playerID);

                if (reader.GetInt32("playerType") == 0)
                {
                    player.playerType = PlayerType.God;
                }
                else if (reader.GetInt32("playerType") == 1)
                {
                    player.playerType = PlayerType.PowerUser;
                }
                else if (reader.GetInt32("playerType") == 2)
                {
                    player.playerType = PlayerType.Mod;
                }
                else if (reader.GetInt32("playerType") == 3)
                {
                    player.playerType = PlayerType.Player;
                }
                else
                {
                    player.playerType = PlayerType.Player;
                }

                if (reader.GetInt32("isBanned") == 1)
                {
                    player.isBanned = true;
                }
                else
                {
                    player.isBanned = false;


                }

            }

            return players;



        }


        public async Task LevelPlayer(ulong playerID, int xP)
        {

            Player p = Player.GetPlayerFromDataBase(playerID);
            var currentXp =p.xP;
            var neededXp = p.xPToNextLevel;
            var currentLevel = p.playerLevel;

            if (currentXp + xP >= neededXp)
            {
                var newNeedeXP = (p.xPToNextLevel * (int)1.5f);
                await p.UpdatePlayerStatInDataBase(playerID, "currentLevel", 1);
                await p.UpdatePlayerStatInDataBase(playerID, "xP", xP);
                await p.UpdatePlayerStatInDataBase(playerID, "baseStrength", 2);
                await p.UpdatePlayerStatInDataBase(playerID, "baseIntellect",2);
                await p.UpdatePlayerStatInDataBase(playerID, "baseAgility",2);
                await p.UpdatePlayerStatInDataBase(playerID, "baseEnergy", 5);
                await p.UpdatePlayerStatInDataBase(playerID, "xPToNextLevel", newNeedeXP);
                await p.discordMember.SendMessageAsync("You have leveled up to level " + p.playerLevel + "!");
                return;
            }
            else
            {
                
                await p.UpdatePlayerStatInDataBase(playerID, "xP", xP);
                return;
            }
    
        }


        // get player from database
        public static Player GetPlayerFromDataBase(ulong playerid)//works
        {
             MySqlConnection connection = new MySqlConnection(DnDBot.connStr);
            connection.Open();


            Player player = new Player();


            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            MySqlCommand cmd = new MySqlCommand("SELECT * FROM players WHERE playerID = @playerID", connection);
            cmd.Parameters.AddWithValue("@playerID", playerid);
            MySqlDataReader reader = cmd.ExecuteReader();
           
                if (reader.Read())
                {
                    player.isBanned = reader.GetBoolean("isBanned");
                    player.playerType = (PlayerType)reader.GetInt32("playerType");
                    player.currentPosition = new Vector2(reader.GetFloat("worldPositionX"), reader.GetFloat("worldPositionY"));
                    player.name = reader.GetString("name");
                    player.cash = reader.GetInt32("cash");
                    player.xP = reader.GetInt32("xP");
                    player.playerLevel = reader.GetInt32("playerLevel");        
                    player.homeGuildID = reader.GetUInt64("homeGuildID");
                    player.homeGuildName = reader.GetString("homeGuildName");
                    player.playerID = reader.GetUInt64("playerID");
                    player.baseStamina = reader.GetInt32("baseStamina");
                    player.baseHealth = reader.GetInt32("baseHealth");
                    player.baseEnergy = reader.GetInt32("baseEnergy");
                    player.baseArmor = reader.GetInt32("baseArmor");
                    player.baseStrength = reader.GetInt32("baseStrength");
                    player.baseIntellect = reader.GetInt32("baseIntellect");
                    player.baseAgility = reader.GetInt32("baseAgility");
                    player.gridsDiscovered = reader.GetInt32("gridsDiscovered");
                    player.xPToNextLevel = reader.GetInt32("xPToNextLevel");
                    player.maxLevel = reader.GetInt32("maxLevel");
                    player.deaths = reader.GetInt32("deaths");
                    player.deathPosition.X = reader.GetInt32("deathPosition.X");
                    player.deathPosition.Y = reader.GetInt32("deathPosition.Y");
                    player.baseHerbalism = reader.GetInt32("baseHerbalism");
                    player.baseMining = reader.GetInt32("baseMining");
                player.inventorySize = reader.GetInt32("inventorySize");
                player.inventoryMax = reader.GetInt32("inventoryMax");
                player.discordMember = GetDiscordMember(player.homeGuildID, player.playerID);
                }
            connection.Close();
            return player;
        }


        
        //get the discord member associated with a certain player.
         public static DiscordMember GetDiscordMember(ulong homeGuildId, ulong playerId)//works
        {
        DiscordMember member = DnDBot.discord.TryGetGuildAsync(homeGuildId).Result.GetMemberAsync(playerId).Result;

        return member;
            
        }


        //update the players stats in the database. 
        public async Task UpdatePlayerStatInDataBase ( ulong playerId, string playerStat, int newValue)//works
        {
            MySqlConnection connT = new MySqlConnection(DnDBot.connStr);


            try
            {
                Player player = GetPlayerFromDataBase(playerId);
            
                int _playerStat = player.UpdateStatValueByName(playerStat, newValue);

                var var1 = playerId;
                var var2 = _playerStat;
                var var3 = playerStat;
                string query = $"UPDATE players SET {playerStat} =@playerStat WHERE playerID =@playerID ";
                //string query1 = "UPDATE players SET baseHealth =150 WHERE playerID =179578819672408064 ";
            
                MySqlCommand command = new MySqlCommand(query, connT);
                command.Parameters.AddWithValue("@playerID", var1);
                command.Parameters.AddWithValue("@statName", var3);
                command .Parameters.AddWithValue("@playerStat", var2);
                await connT.OpenAsync();



                await command.ExecuteNonQueryAsync();
                await connT.CloseAsync();

            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Error in UpdatePlayerStatInDataBase: {ex.Message}");
            }
            return ;


        }

        public int UpdateStatValueByName(string statName, int newValue)//works
        {
            int stat = 0;
            switch (statName)
            {
                case "baseHealth":
                    stat = baseHealth+= newValue;
                    break;
                case "baseEnergy":
                    stat = baseEnergy += newValue;
                    break;
                case "baseStamina":
                    stat = baseStamina += newValue;
                    break;
                case "baseArmor":
                    stat = baseArmor += newValue;
                    break;
                case "baseStrength":
                    stat = baseStrength += newValue;
                    break;
                case "baseIntenllect":
                    stat = baseIntellect += newValue;
                    break;
                case "baseAgility":
                    stat = baseAgility += newValue;
                    break;
                case "worldPositionX":
                    stat = (int)(currentPosition.X += newValue);
                    break;
                case "worldPositionY":
                    stat = (int)(currentPosition.Y += newValue);
                    break;
                case "deathPositionX":
                    stat = (int)(deathPosition.X = newValue);
                    break;
                case "deathPositionY":
                    stat = (int)(deathPosition.Y = newValue);
                    break;
                case "cash":
                    stat = cash += newValue;
                    break;
                case "xP":
                    stat = xP += newValue;
                    break;
                case "playerLevel":
                    stat = playerLevel += newValue;
                    break;
                case "gridsDiscovered":
                    stat = gridsDiscovered += newValue;
                    break;
                case "xPToNextLevel":
                    stat = xPToNextLevel += newValue;
                    break;
                case "deaths":
                    stat = deaths += newValue;
                    break;
                case "baseHerbalism":
                    stat = baseHerbalism += newValue;
                    break;
                case "baseMining":
                    stat = baseMining += newValue;
                    break;
                case "inventorySize":
                    stat = inventorySize += newValue;
                    break;
                case "inventoryMax":
                    stat = inventoryMax += newValue;
                    break;
                    
                    
            }

            return stat;

        }

        public async Task UpdatePlayerPosition(ulong playerID, Vector2 newPosition)//works
        {
            await UpdatePlayerStatInDataBase(playerID, "worldPositionX", (int)newPosition.X);
            await UpdatePlayerStatInDataBase(playerID, "worldPositionY", (int)newPosition.Y);

            return;
        }
    }

 }

          