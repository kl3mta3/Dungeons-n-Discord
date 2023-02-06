using DnDBot.Character;
using Google.Protobuf.WellKnownTypes;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Utilities;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DnDBot.Items
{
    public enum ItemRarity
    {

        Common=0,
        Uncommon = 1,
        Rare = 2,
        Epic = 3,
        Legendary = 4,

    }

    public enum ItemType
    {
        Weapon=1,
        Equipment=2,
        Consumable=3,
        Misc =4,
    }

    public enum WeaponType
    {
        NA = 0,
        Sword=1,
        Axe = 2,
        Mace = 3,
        Spear = 4,
        Bow = 5,
        Crossbow = 6,
        Dagger = 7,
        Staff = 8,
        Wand = 9,
        SpellBook = 10,
        Unarmed = 11,
    }

    public enum EquipmentType
    {

        Na = 0,
        Head = 1,
        Neck = 2,
        Shoulders = 3,
        Chest = 4,
        Back = 5,
        Bracers = 6,
        Gloves = 7,
        Belt = 8,
        Legs = 9,
        Feet = 10,
        Ring = 11,
        Bag = 12,
    }

    public enum ConsumableType
    {
        NA = 0,
        Potion = 1,
        Scroll = 2,
        Food = 3,
        Drink = 4,
        Herb = 5,
        Ore =6,
        
    }

    public class Item
    {
        public ulong itemID { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int weight { get; set; }
        public int value { get; set; }

        public bool isTradable;
        internal ItemType itemType { get; set; }

        internal ItemRarity itemRarity {get;set;}
        public static Item CreateItem(ItemType type, ItemRarity rarity, ulong itemID, string name, string description, int weight, int value, bool tradable)
        {
            Item item = new Item();
            switch (type)
            {
                case ItemType.Weapon:
                    item = new Weapon();
                    break;
                case ItemType.Equipment:
                    item=  new Equipment();
                    break;
                case ItemType.Consumable:
                    item = new Consumable();
                    break;
                case ItemType.Misc:
                    item = new Misc();
                    break;
                default:
                    item = null;
                    break;
            }
            item.itemID = itemID;
            item.name = name;
            item.description = description;
            item.weight = weight;
            item.value = value;
            item.isTradable = tradable;
            return item;
        }


        public static Item GetItemFromDB(ulong itemID)
        {
            //get the item from the database
            Item dbItem = new Item();
            dbItem.itemID = itemID;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(DnDBot.connStr))
                {

                    connection.Open();
                    string query = $"SELECT * FROM items WHERE itemID = @itemID";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {

                        cmd.Parameters.AddWithValue("@itemID", itemID);
                        MySqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {

                            dbItem.isTradable = reader.GetBoolean("isBanned");
                            dbItem.weight = reader.GetInt32("weight");
                            dbItem.value = reader.GetInt32("weight");
                            dbItem.name = reader.GetString("name");
                            dbItem.description = reader.GetString("description");
                            dbItem.itemType = (ItemType)reader.GetInt32("itemType");
                            dbItem.itemRarity= (ItemRarity)reader.GetInt32("itemRarity");
                            Item item = CreateItem(ItemType.Weapon, dbItem.itemRarity,  dbItem.itemID, dbItem.name, dbItem.description, dbItem.weight, dbItem.value, dbItem.isTradable);



                            switch (dbItem.itemType)
                            {
                                case ItemType.Weapon:

                                    Weapon weapon = (Weapon)item;

                                    weapon.health = reader.GetInt32("bonusHealth");
                                    weapon.armor=   reader.GetInt32("bonusArmor");
                                    weapon.stamina = reader.GetInt32("bonusStamina");
                                    weapon.strength = reader.GetInt32("bonusStrength");
                                    weapon.agility = reader.GetInt32("bonusAgility");
                                    weapon.energy = reader.GetInt32("bonusEnergy");
                                    weapon.intellect = reader.GetInt32("bonusIntellect");
                                    weapon.range = reader.GetInt32("range");
                                    weapon.isRepareable  = reader.GetBoolean("repairable");
                                    weapon.weaponType = (WeaponType)reader.GetInt32("weaponType");
                                    weapon.damage = reader.GetInt32("damage");
                                    weapon.durability = reader.GetInt32("durability");
                                    item = weapon;
                                    Console.WriteLine($"{item.name} returned of type {item.itemType}");
                                    return (Weapon)item;

                                case ItemType.Equipment:

                                    Equipment equipment = (Equipment)item;


                                    equipment.health = reader.GetInt32("bonusHealth");
                                    equipment.armor = reader.GetInt32("bonusArmor");
                                    equipment.stamina = reader.GetInt32("bonusStamina");
                                    equipment.strength = reader.GetInt32("bonusStrength");
                                    equipment.agility = reader.GetInt32("bonusAgility");
                                    equipment.energy = reader.GetInt32("bonusEnergy");
                                    equipment.intellect = reader.GetInt32("bonusIntellect");
                                    equipment.isRepareable = reader.GetBoolean("repairable");
                                    equipment.equipmentType = (EquipmentType)reader.GetInt32("equipmentType");

                                    equipment.durability = reader.GetInt32("durability");

                                    item = equipment;
                                    Console.WriteLine($"{item.name} returned of type {item.itemType}");
                                    return (Equipment)item;

                                case ItemType.Consumable:

                                    Consumable consumable = (Consumable)item;


                                    consumable.health = reader.GetInt32("bonusHealth");
                                    consumable.armor = reader.GetInt32("bonusArmor");
                                    consumable.stamina = reader.GetInt32("bonusStamina");
                                    consumable.strength = reader.GetInt32("bonusStrength");
                                    consumable.agility = reader.GetInt32("bonusAgility");
                                    consumable.energy = reader.GetInt32("bonusEnergy");
                                    consumable.intellect = reader.GetInt32("bonusIntellect");
                                    consumable.useDelay = reader.GetInt32("useDelay");
                                    consumable.consumableType = (ConsumableType)reader.GetInt32("consumableType");
                                    consumable.maxQuantity= reader.GetInt32("mazQuantity");

                                    item = consumable;
                                    Console.WriteLine($"{item.name} returned of type {item.itemType}");
                                    return (Consumable)item;

                                case ItemType.Misc:
                                    Misc misc = (Misc)item;

                                    misc.health = reader.GetInt32("bonusHealth");
                                    misc.armor = reader.GetInt32("bonusArmor");
                                    misc.stamina = reader.GetInt32("bonusStamina");
                                    misc.strength = reader.GetInt32("bonusStrength");
                                    misc.agility = reader.GetInt32("bonusAgility");
                                    misc.energy = reader.GetInt32("bonusEnergy");
                                    misc.intellect = reader.GetInt32("bonusIntellect");
                                    misc.useDelay = reader.GetInt32("useDelay");
                                    
                                    misc.maxQuantity = reader.GetInt32("mazQuantity");

                                    item = misc;

                                     Console.WriteLine($"{item.name} returned of type {item.itemType}");
                                    return (Misc)item;


                            }

                        }
                            return null;


                    } 

                    connection.Close();
                }
            }
            catch (Exception ex)
            {

                Log.Logger.Error(ex.Message);
                return null;

            }

           

            // create the item based on the type

            
        }

       


    }

    public class Weapon : Item
    {
        internal WeaponType weaponType { get; set; }
        public int damage { get; set; }
        public bool isRepareable { get; set; }
        public int range { get; set; }

        public int durability { get; set; }

        public int health { get; set; }
        public int energy { get; set; }
        public int strength { get; set; }
        public int stamina { get; set; }
        public int intellect { get; set; }
        public int agility { get; set; }
        public int armor { get; set; }



        public int atk1ID { get; set; }
        public int atk2ID { get; set; }
        public int atk3ID { get; set; }
        public int atk4ID { get; set; }


        public Weapon()
        {
            itemType = ItemType.Weapon;
        }


    }

    public class Equipment : Item
    {
        internal EquipmentType equipmentType { get; set; }
   
        public int durability { get; set; }
        public bool isRepareable { get; set; }
        public int health { get; set; }
        public int energy { get; set; }
        public int strength { get; set; }
        public int stamina { get; set; }
        public int intellect { get; set; }
        public int agility { get; set; }
        public int armor{ get; set; }
        public int inventoryMax { get; set; }

        public Equipment()
        {
            itemType = ItemType.Equipment;
        }

    }

    public class Consumable : Item
    {
        internal ConsumableType consumableType { get; set; }
        public int useDelay { get; set; }
        public int maxQuantity { get; set; }
        public int health { get; set; }
        public int energy { get; set; }
        public int strength { get; set; }
        public int stamina { get; set; }
        public int intellect { get; set; }
        public int agility { get; set; }
        public int armor { get; set; }

        public Consumable()
        {
            itemType = ItemType.Consumable;
        }
    }

    public class Misc : Item
    {
        public int durability { get; set; }
        public int useDelay { get; set; }
        public int maxQuantity { get; set; }
        public int health { get; set; }
        public int energy { get; set; }
        public int strength { get; set; }
        public int stamina { get; set; }
        public int intellect { get; set; }
        public int agility { get; set; }
        public int armor { get; set; }

        public Misc()
        {
            this.itemType = ItemType.Misc;
        }
    }

   public class PlayerEquipment
    {
        public ulong playerID { get; set; }
        
        public Equipment head { get; set; }

        public Equipment  neck { get; set; }

        public Equipment shoulders { get; set; }

        public Equipment chest { get; set; }

        public Equipment back { get; set; }

        public Equipment bracers { get; set; }

        public Equipment gloves { get; set; }

        public Equipment belt { get; set; }

        public Equipment legs { get; set; }

        public Equipment feet { get; set; }

        public Equipment ring1 { get; set; }

        public Equipment ring2 { get; set; }

        public Equipment bag1 { get; set; }

        public Equipment bag2 { get; set; }

        public Equipment bag3 { get; set; }

        public Equipment bag4 { get; set; }

        public Weapon mainHandWeapon { get; set; }

        public Weapon offHandWeapon { get; set; }

        public PlayerEquipment(ulong _playerID, Equipment _head, Equipment _neck, Equipment _shoulders, Equipment _chest, Equipment _back, Equipment _bracers, Equipment _gloves, Equipment _belt, Equipment _legs, Equipment _feet, Equipment _ring1, Equipment _ring2, Equipment _bag1, Equipment _bag2, Equipment _bag3, Equipment _bag4, Weapon _mainHandWeapon, Weapon _offHandWeapon)
        {
            playerID = _playerID;
            head = _head;
            neck = _neck;
            shoulders = _shoulders;
            chest = _chest;
            back = _back;
            bracers = _bracers;
            gloves = _gloves;
            belt = _belt;
            legs = _legs;
            feet = _feet;
            ring1 = _ring1;
            ring2 = _ring2;
            bag1 = _bag1;
            bag2 = _bag2;
            bag3 = _bag3;
            bag4 = _bag4;
            mainHandWeapon = _mainHandWeapon;
            offHandWeapon = _offHandWeapon;
        }

        public ulong GetSlotItemIDFromDB(ulong playerID, string slotLocation)
        {
            ulong itemID = new ulong();
            string query = $"SELECT @slotLocation FROM equipment WHERE playerID= @playerID";
            try
            {
                using (MySqlConnection connection = new MySqlConnection(DnDBot.connStr))
                {
                    connection.Open();

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@slotLocation", slotLocation);
                        cmd.Parameters.AddWithValue("@playerID", playerID);

                        MySqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {

                            itemID = reader.GetUInt64($"{slotLocation}");
                        }


                    }

                    connection.Close();

                }
                if ( itemID>0)
                {
                    return itemID;
                }
                else
                {
                    return 0;
                }

            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex.Message);
                return 0;
            }

        }


        public Task AddItemToPlayerEquipment(ulong playerID, string slotLocation, ulong itemID)
        {
            Item item = Item.GetItemFromDB(itemID);
            ulong currentSlotItemID = GetSlotItemIDFromDB(playerID, slotLocation);
            if (currentSlotItemID > 0)
            {
                //here is where the inventory reAdd will be
            }

            string query = $"SELECT * FROM equipment SET @slotLocation = @itmeID WHERE playerID= @playerID";
            try
            {

                using (MySqlConnection connection = new MySqlConnection(DnDBot.connStr))
                {

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@slotLocation", slotLocation);
                        cmd.Parameters.AddWithValue("@itemID", itemID);
                        cmd.Parameters.AddWithValue("@playerID", playerID);

                        cmd.ExecuteNonQuery();
                    }
                }
                return Task.CompletedTask;

            }
            catch(Exception ex)
            {
                Log.Logger.Error(ex.Message);
                return Task.CompletedTask;

            }

        }



    }

    public class InventoryItem
    {
        public ulong playerID { get; set; }

        public ulong itemID { get; set; }

        public int quantity { get; set; }

        public string itemName { get; set; }

        public InventoryItem(ulong _playerID, ulong _itemID, int _quantity, string _itemName)
        {
            playerID = _playerID;
            itemID = _itemID;
            quantity = _quantity;
            itemName = _itemName;
        }


        public InventoryItem GetInventoryItemFromDB(ulong playerID, ulong itemID)
        {

            InventoryItem item = new InventoryItem(playerID,itemID,0,"");


            string query = $"SELECT * FROM inventory WHERE playerID= @playerID AND itemID= @itemID";
            try
            {
                using (MySqlConnection connection = new MySqlConnection(DnDBot.connStr))
                {
                    connection.Open();

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@itemID", itemID);
                        cmd.Parameters.AddWithValue("@playerID", playerID);

                        MySqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {

                            item.quantity = reader.GetInt32($"quantity");
                            item.itemName = reader.GetString("itemName");
                        }
                    }

                    connection.Close();

                }
                    return item;
               
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex.Message);
                return null;
               
            }
        }

        public List<InventoryItem> GetPlayerInventoryFromDB(ulong playerID)
        {

            List<InventoryItem> items = new List<InventoryItem>();

            string query = $"SELECT * FROM inventory WHERE playerID= @playerID";
            try
            {
                using (MySqlConnection connection = new MySqlConnection(DnDBot.connStr))
                {
                    connection.Open();

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        
                        cmd.Parameters.AddWithValue("@playerID", playerID);

                        MySqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            InventoryItem item = new InventoryItem(playerID,reader.GetUInt64("itemID"),reader.GetInt32("quantity"),reader.GetString("itemName"));
                            items.Add(item);
                        }


                    }

                    connection.Close();

                }
                return items;


            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex.Message);
                return null;

            }




        }

        public bool InventoryItemForPlayerExists(ulong playerID, ulong itemID)

        {
            string query = $"SELECT * FROM inventory WHERE playerID= @playerID AND itemID= @itemID";
            try
            {
                using (MySqlConnection connection = new MySqlConnection(DnDBot.connStr))
                {
                    connection.Open();

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@itemID", itemID);
                        cmd.Parameters.AddWithValue("@playerID", playerID);

                        MySqlDataReader reader = cmd.ExecuteReader();

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

                    }

                }
            


            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex.Message);
                return false;

            }







        }
        public async Task AddInventoryItemToDB(InventoryItem _inventoryItem)
        {
            
            Player player = Player.GetPlayerFromDataBase(_inventoryItem.playerID);
            int inventorySize = player.inventorySize;
            int inventoryMax = player.inventoryMax;
            bool reachedMaxStack = false;
            int _itemMaxStackSize = 0;
            Item _item= Item.GetItemFromDB(_inventoryItem.itemID);
            if(_item.itemType==ItemType.Consumable)
            {
                Consumable con = (Consumable)_item;
                _itemMaxStackSize = con.maxQuantity;

            }
            else if(_item.itemType == ItemType.Misc)
            {

                Misc misc = (Misc)_item;
                _itemMaxStackSize = misc.maxQuantity;


            }
            else
            {
                _itemMaxStackSize = 1;

            }

            if(player.inventorySize < player.inventoryMax)
            {
                if (InventoryItemForPlayerExists(player.playerID, _inventoryItem.itemID))  //item exists in global inventory
                {
                    InventoryItem inventoryItemOnDB = GetInventoryItemFromDB(playerID, itemID);
                    int currentQuantity = inventoryItemOnDB.quantity;

                    if (_itemMaxStackSize > 1) //item is stackable
                    {
                        if (currentQuantity+_inventoryItem.quantity >= _itemMaxStackSize)
                        {
                           

                            string query = $"UPDATE inventory SET quantity= {currentQuantity + _inventoryItem.quantity} ";
                            try
                            {
                                using (MySqlConnection connection = new MySqlConnection(DnDBot.connStr))
                                {
                                    connection.Open();

                                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                                    {

                                        cmd.Parameters.AddWithValue("@playerID", playerID);
                                        cmd.Parameters.AddWithValue("@itemID", itemID);
                                        cmd.Parameters.AddWithValue("@itemName", itemName);
                                        cmd.Parameters.AddWithValue("@quantity", quantity);

                                        MySqlDataReader reader = cmd.ExecuteReader();

                                        while (reader.Read())
                                        {
                                            InventoryItem item = new InventoryItem(playerID, reader.GetUInt64("itemID"), reader.GetInt32("quantity"), reader.GetString("itemName"));

                                        }


                                    }

                                    connection.Close();

                                }

                                await player.UpdatePlayerStatInDataBase(player.playerID, "inventorySize", 1); //adds 1 to playerinventory size because "newStack" was Created
                                return;


                            }
                            catch (Exception ex)
                            {
                                Log.Logger.Error(ex.Message);
                                return;

                            }

                        }   // current quantity + new Quantity is more than max size  add quantitys and add 1 to player inventorySIze
                        else
                        {


                            string query = $"Update inventory Set quantity= {currentQuantity + _inventoryItem.quantity} WHERE playerID = @playerID AND itemID = @itemID)";
                            try
                            {
                                using (MySqlConnection connection = new MySqlConnection(DnDBot.connStr))
                                {
                                    connection.Open();

                                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                                    {

                                        cmd.Parameters.AddWithValue("@playerID", playerID);
                                        cmd.Parameters.AddWithValue("@itemID", itemID);
                                        cmd.Parameters.AddWithValue("@quantity", quantity);

                                        cmd.ExecuteNonQuery();


                                    }

                                    connection.Close();

                                }

                              
                                return;


                            }
                            catch (Exception ex)
                            {
                                Log.Logger.Error(ex.Message);
                                return;

                            }








                        }    // item is stackable but doesnt need new stack just add quantity
                    }
                    else
                    {
                        string query = $"Update inventory Set quantity= {currentQuantity + _inventoryItem.quantity} WHERE playerID = @playerID AND itemID = @itemID)";
                        try
                        {
                            using (MySqlConnection connection = new MySqlConnection(DnDBot.connStr))
                            {
                                connection.Open();

                                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                                {

                                    cmd.Parameters.AddWithValue("@playerID", playerID);
                                    cmd.Parameters.AddWithValue("@itemID", itemID);
                                    cmd.Parameters.AddWithValue("@itemName", itemName);
                                    cmd.Parameters.AddWithValue("@quantity", quantity);

                                    cmd.ExecuteNonQuery();


                                }

                                connection.Close();

                            }

                            await player.UpdatePlayerStatInDataBase(player.playerID, "inventorySize", 1); //adds 1 to playerinventory size because "newStack" was Created
                            return;


                        }
                        catch (Exception ex)
                        {
                            Log.Logger.Error(ex.Message);
                            return;

                        }


                    }  // item isnt stackable  add to DB Quantity and add 1 to player InventorySize

                }
                else
                {
                    
                        string query = $"INSERT INTO inventory (playerID, itemID, itemName, quantity) VALUES ( @playerID, @itemID, @itemName, @quantity)";
                        try
                        {
                            using (MySqlConnection connection = new MySqlConnection(DnDBot.connStr))
                            {
                                connection.Open();

                                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                                {

                                    cmd.Parameters.AddWithValue("@playerID", playerID);
                                    cmd.Parameters.AddWithValue("@itemID", itemID);
                                    cmd.Parameters.AddWithValue("@itemName", itemName);
                                    cmd.Parameters.AddWithValue("@quantity", quantity);

                                    cmd.ExecuteNonQuery();


                            }

                                connection.Close();

                            }

                            await player.UpdatePlayerStatInDataBase(player.playerID, "inventorySize", 1); //adds 1 to playerinventory size because "newStack" was Created
                            return;


                        }
                        catch (Exception ex)
                        {
                            Log.Logger.Error(ex.Message);
                            return;

                        }

                } // item doesnt exist currently for the player in the Database
            }
             else
              {
                    //build EMbed Later
                    await player.discordMember.SendMessageAsync($"{player.name} you can't Pick-Up that {_inventoryItem.itemName} your inventory is full");
                    return;
              } //player at max Inventory size reutrn cant Pick Up


            
        }
    }

    public class WeaponAttack
    {






    }

}
