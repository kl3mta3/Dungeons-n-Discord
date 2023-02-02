using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DisCatSharp.CommandsNext.Attributes;
using DnDBot;
using DnDBot.Character;
using MySql.Data.MySqlClient;
using Serilog;
using static Google.Protobuf.Reflection.SourceCodeInfo.Types;

namespace DnDBot.WorldSystem
{

    
    
    public  class WorldGridSpace
    {
       internal string name = "";
        internal int currentPlayerCount;
        internal List< WorldGrid> worldgrid = new List<WorldGrid>();
        internal List<WorldZone> zones = new List<WorldZone>();
        internal int worldSize;
        internal int worldMaxXValue;
        internal int worldMaxYValue;
    }


    public class WorldGrid
    {
        internal ulong gridID;
        internal int zoneID;
        internal Vector2 gridLocation = new Vector2();
        internal bool onPath;
        internal bool isWater;
        internal bool isMountain;
        internal bool isCity;
        internal bool isWilds;
        internal bool isInstance;
        internal bool isMine;



        public static WorldGrid GetGridbyID(int gridID)
        {

            MySqlConnection connection = new MySqlConnection(DnDBot.connStr);
            connection.Open();

            Log.Logger.Information($"Get Grid From Database started.");
            WorldGrid grid = new WorldGrid();


            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            MySqlCommand cmd = new MySqlCommand("SELECT * FROM worldspace WHERE gridID = @gridID", connection);
            cmd.Parameters.AddWithValue("@gridID", gridID);
            MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {

                grid.gridLocation = new Vector2(reader.GetFloat("gridLocationX"), reader.GetFloat("gridLocationY"));
                grid.zoneID = reader.GetInt32("zoneID");
                grid.gridID = reader.GetUInt64("gridID");
                grid.onPath = reader.GetBoolean("onPath");
                grid.isWater = reader.GetBoolean("isWater");
                grid.isMountain = reader.GetBoolean("isMountain");
                grid.isCity = reader.GetBoolean("isSafe");
                grid.isWilds = reader.GetBoolean("isWilds");
                grid.isInstance = reader.GetBoolean("isInstance");
                grid.isMine = reader.GetBoolean("isMine");



            }
            connection.Close();
            Console.WriteLine($"Returning {grid.gridID}");
            return grid;

        }

        public static WorldGrid GetGridByLocation(Vector2 location)
        {
            
            foreach (WorldGrid grid in DnDBot.worldSpace.worldgrid)
            {

                if (grid.gridLocation.X == location.X && grid.gridLocation.Y == location.Y)
                {
                    return grid;
                }
            }
            return null;
        }

        
        public static WorldGrid GetNearestCityGrid(Vector2 currentLocation)
        {
            List<WorldGrid>cityGrids = new List<WorldGrid>();
            Vector2 closestCityGrid = Vector2.Zero;
            float minDistance = float.MaxValue;
            foreach (WorldGrid grid in DnDBot.worldSpace.worldgrid)
            {

                if (grid.isCity)
                {
                    float distance = Vector2.Distance(currentLocation, grid.gridLocation);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestCityGrid = grid.gridLocation;
                    }
                }
                    return grid;
            }
            
            return null;
        }


        public bool CanMoveToGrid()
        {
            if (isWater || isMountain || gridLocation.Y > DnDBot.worldSpace.worldMaxYValue || gridLocation.X > DnDBot.worldSpace.worldMaxXValue || gridLocation.X <= 0 || gridLocation.Y <= 0)
            {
                return false;
            }
            else
            {
                return true;
            }
           

        }

        public static WorldGrid GetGridByLocationDB(int x,int y)
        {

            MySqlConnection connection = new MySqlConnection(DnDBot.connStr);
            connection.Open();

            Log.Logger.Information($"Get Grid From Database started.");
            WorldGrid grid = new WorldGrid();


            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            MySqlCommand cmd = new MySqlCommand("SELECT * FROM worldspace WHERE gridPositionX = @gridPositionX AND gridPositionY = @gridPositionY", connection);
            cmd.Parameters.AddWithValue("@gridPositionX", x);
            cmd.Parameters.AddWithValue("@gridPositionY", y);
            MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {

                grid.gridLocation = new Vector2(reader.GetFloat("gridLocationX"), reader.GetFloat("gridLocationY"));
                grid.zoneID = reader.GetInt32("zoneID");
                grid.gridID = reader.GetUInt64("gridID");
                grid.onPath = reader.GetBoolean("onPath");
                grid.isWater = reader.GetBoolean("isWater");
                grid.isMountain = reader.GetBoolean("isMountain");
                grid.isCity = reader.GetBoolean("isSafe");
                grid.isWilds = reader.GetBoolean("isWilds");
                grid.isInstance = reader.GetBoolean("isInstance");
                grid.isMine = reader.GetBoolean("isMine");



            }
            connection.Close();
            Console.WriteLine($"Returning {grid.gridID}");
            return grid;

        }


        public static List<WorldGrid> GetGridsFromDatabase()
        {
            Log.Logger.Information($"Get Grid From Database started.");
            int indexLoaded = 0;
            List<WorldGrid> grids = new List<WorldGrid>();
            
            using (MySqlConnection connection = new MySqlConnection(DnDBot.connStr))
            {
                ;
                    connection.Open();
                try
                {


                    MySqlCommand cmd = new MySqlCommand("SELECT * FROM worldspace", connection);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        WorldGrid grid = new WorldGrid();
                        grid.gridLocation = new Vector2(reader.GetFloat("gridPositionX"), reader.GetFloat("gridPositionY"));
                        grid.zoneID = reader.GetInt32("zoneID");
                        grid.gridID = reader.GetUInt64("gridID");
                        grid.onPath = reader.GetBoolean("onPath");
                        grid.isWater = reader.GetBoolean("isWater");
                        grid.isMountain = reader.GetBoolean("isMountain");
                        grid.isCity = reader.GetBoolean("isCity");
                        grid.isWilds = reader.GetBoolean("isWilds");
                        grid.isInstance = reader.GetBoolean("isInstance");
                        grid.isMine = reader.GetBoolean("isMine");

                        grids.Add(grid);
                        indexLoaded++;
                    }
                    connection.Close();
                    Log.Information($"Loaded {indexLoaded} grids from database.");
                    return grids;

                }
                catch (Exception e)
                {
                    connection.Close();
                    Log.Error($"Error loading grids from database. {e}");
                    return null;
                }


            }
        }


        public  bool GridDiscovered(ulong _playerID, int _gridID)
        {
            var playerID = _playerID;
            var gridID = _gridID;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(DnDBot.connStr))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM playerdiscoveredgrids WHERE playerID = @playerID AND gridID = @gridID", connection))
                    {
                            cmd.Parameters.AddWithValue("@playerID", playerID);
                        cmd.Parameters.AddWithValue("@gridID", gridID);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {

                            if (reader.HasRows)
                            {
                                connection.Close();
                                return true;
                            }
                            else
                            {
                                connection.Close();
                                return false;
                            }

                        };
                        
                    };
                }
            }
            catch (Exception e)
            {
                Log.Error($"Error inserting discovered grid into database. {e}");
                return true;
            }



           
        }

        public Task InsertDiscoveredGirdDataBase(ulong _playerID, int _gridID)
        {
            var playerID = _playerID;
            var gridID = _gridID;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(DnDBot.connStr))
                {
                    connection.Open();
                    MySqlCommand cmd = new MySqlCommand("INSERT INTO playerdiscoveredgrids (playerID,gridID) VALUES (@playerID,@gridID)", connection);
                    cmd.Parameters.AddWithValue("@playerID", playerID);
                    cmd.Parameters.AddWithValue("@gridID", gridID);
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception e)
            {
                Log.Error($"Error inserting discovered grid into database. {e}");
            }
        
            return Task.CompletedTask;
        }





    }
        public class WorldZone
        {
            internal ulong zoneID = 0;
            internal string zoneName = "";
            internal int zoneSize { get; set; }
            internal bool isCity;
            internal bool isDungeon;
            internal List<WorldGrid> zoneGrids = new List<WorldGrid>();

        }
      
}
