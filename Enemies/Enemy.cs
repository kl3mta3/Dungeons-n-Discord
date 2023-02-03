using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using DisCatSharp.Enums;
using System.Xml.Linq;
using DnDBot;
using DnDBot.Character;
using MySql.Data.MySqlClient;
using Serilog;

namespace DnDBot.Enemies
{

    public enum EnemyType
    {
        Beast = 0,
        Humanoid = 1,
        Undead = 2,
        Aberration = 3,
        Dragon = 4,
        Giant = 5,
        Fey = 6,
        Monstrosity = 7,
        Elemental = 8,
        Construct = 9,
        Ooze = 10,
        Plant = 11,
        SwarmOfTinyBeasts = 12,
        Other = 13
    }
    public enum DamageType
    {
        Physical = 0,
        Bludgeoning = 1,
        Cold = 2,
        Fire = 3,
        Force = 4,
        Lightning = 5,
        Necrotic = 6,
        Piercing = 7,
        Poison = 8,
        Psychic = 9,
        Radiant = 10,
        Slashing = 11,
     
    }


    public enum ClassType
    {
        melee = 0,
        ranged = 1,
        spellcaster = 2,
        flying = 3,
        aquatic = 4,
        underground = 5
    }
    internal class Enemy
    {
        internal ulong ID { get; set; }
        internal string name { get; set; }
        internal int health { get; set; }
        internal int energy { get; set; }
        internal int atkModID { get; set; }
        internal string description { get; set; }

        internal int armor { get; set; }
        internal int level { get; set; }
        
        internal int attackSpeed { get; set; }

        internal ClassType classType { get; set; }

        internal EnemyType enemyType { get; set; }

        internal DamageType damageType { get; set; }



        internal Enemy(ulong ID, string name, int health, int energy, int atkModID, string description, int armorClass, int level, int attackSpeed, ClassType classType, EnemyType enemyType, DamageType damageType)
        {
            this.ID = ID;
            this.name = name;
            this.health = health;
            this.atkModID = atkModID;
            this.description = description;
            this.armor = armorClass;
            this.level = level;
            this.attackSpeed = attackSpeed;
            this.classType = classType;
            this.enemyType = enemyType;
            this.damageType = damageType;
            this.energy = energy;
        }

        public static Enemy GetEnemyFromDB(ulong id)
        {
            Enemy enemy = new Enemy(0, "", 0, 0, 0, "", 0, 0, 0,0, 0, 0);
            try
            {
                using (MySqlConnection connection = new MySqlConnection(DnDBot.connStr))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM enemies WHERE ID = @ID", connection))
                    {
                        cmd.Parameters.AddWithValue("@ID", id);
                        MySqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            enemy.ID = reader.GetUInt64("ID");
                            enemy.name = reader.GetString("name");
                            enemy.health = reader.GetInt32("health");
                            enemy.atkModID = reader.GetInt32("atkModID");
                            enemy.description = reader.GetString("description");
                            enemy.armor = reader.GetInt32("armor");
                            enemy.level = reader.GetInt32("level");
                            enemy.attackSpeed = reader.GetInt32("attackSpeed");
                            enemy.classType = (ClassType)reader.GetInt32("classType");
                            enemy.enemyType = (EnemyType)reader.GetInt32("enemyType");
                            enemy.damageType = (DamageType)reader.GetInt32("damageType");


                        }
                    }
                    connection.Close();


                }

                return enemy;
            }
            catch (Exception ex)
            {


                Log.Logger.Error(ex.Message);
                return null;
            }
        }


        public Task AddEnemyToDB(Enemy enemy)
        {
            try 
            { 

            using (MySqlConnection connection = new MySqlConnection(DnDBot.connStr))
            {
                connection.Open();
                using (MySqlCommand cmd = new MySqlCommand("INSERT INTO enemies (ID, name, description, classType, damageType, atkModID, level, health, energy, enemyType, armor, attackSpeed) VAlUES (@ID, @name, @description, @classType, @damageType, @atkModID, @level, @health, @energy, @enemyType, @armor, @attackSpeed)", connection))
                {
                    cmd.Parameters.AddWithValue("@ID", enemy.ID);
                    cmd.Parameters.AddWithValue("@name", enemy.name);
                    cmd.Parameters.AddWithValue("@level", enemy.level);
                    cmd.Parameters.AddWithValue("@description", enemy.description);
                    cmd.Parameters.AddWithValue("@classType", (int)enemy.classType);
                    cmd.Parameters.AddWithValue("@enemyType", (int)enemy.enemyType);
                    cmd.Parameters.AddWithValue("@damageType", (int)enemy.damageType);
                    cmd.Parameters.AddWithValue("@health", enemy.health);
                    cmd.Parameters.AddWithValue("@armor", enemy.armor);
                    cmd.Parameters.AddWithValue("@attackSpeed", enemy.attackSpeed);
                    cmd.Parameters.AddWithValue("@energy", enemy.energy);
                    cmd.Parameters.AddWithValue("@atkModID", enemy.atkModID);

                    cmd.ExecuteNonQuery();
                }
                connection.Close();
            }

            return Task.CompletedTask;
            }
            catch (Exception ex)
            {


                Log.Logger.Error(ex.Message);
                return null;
            }
        }



    }


    public class AttacksModule
    {
        internal ulong atkModID { get; set; }
        internal string moduleName { get; set; }

        internal string description { get; set; }
        internal int baseAtk1ID { get; set; }
        internal int baseAtk2ID { get; set; }

        internal int medAtk1ID { get; set; }

        internal int medAtk2ID { get; set; }

        internal int bigAtk1ID { get; set; }

        internal int bigAtk2ID { get; set; }

        internal int hugeAtk1ID { get; set; }

        internal int hugeAtk2ID { get; set; }


        internal AttacksModule(ulong atkModID, string moduleName, string description, int baseAtk1ID, int baseAtk2ID, int medAtk1ID, int medAtk2ID, int bigAtk1ID, int bigAtk2ID, int hugeAtk1ID, int hugeAtk2ID)
        {
            this.atkModID = atkModID;
            this.moduleName = moduleName;
            this.description = description;
            this.baseAtk1ID = baseAtk1ID;
            this.baseAtk2ID = baseAtk2ID;
            this.medAtk1ID = medAtk1ID;
            this.medAtk2ID = medAtk2ID;
            this.bigAtk1ID = bigAtk1ID;
            this.bigAtk2ID = bigAtk2ID;
            this.hugeAtk1ID = hugeAtk1ID;
            this.hugeAtk2ID = hugeAtk2ID;
        }






        public static AttacksModule GetAttacksModuleFromDb(ulong _atkModID)
        {
            AttacksModule attacksModule = new AttacksModule(0, "", "", 0, 0, 0, 0, 0, 0, 0, 0);

            try
            { 
            using (MySqlConnection connection = new MySqlConnection(DnDBot.connStr))
            {
                connection.Open();
                using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM enemyattackmodules WHERE atkModID = @atkModID", connection))
                {
                    cmd.Parameters.AddWithValue("@atkModID", _atkModID);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        attacksModule.atkModID = reader.GetUInt64("atkModID");
                        attacksModule.moduleName = reader.GetString("moduleName");
                        attacksModule.baseAtk1ID = reader.GetInt32("baseAtk1ID");
                        attacksModule.baseAtk2ID = reader.GetInt32("baseAtk2ID");
                        attacksModule.medAtk1ID = reader.GetInt32("medAtk1ID");
                        attacksModule.medAtk2ID = reader.GetInt32("medAtk2ID");
                        attacksModule.bigAtk1ID = reader.GetInt32("bigAtk1ID");
                        attacksModule.bigAtk2ID = reader.GetInt32("bigAtk2ID");
                        attacksModule.hugeAtk1ID = reader.GetInt32("hugeAtk1ID");
                        attacksModule.hugeAtk2ID = reader.GetInt32("hugeAtk2ID");
                        attacksModule.description = reader.GetString("description");
                        
                    }
                }
                connection.Close();


            }



            return attacksModule;

        }
            catch (Exception ex)
            {


                Log.Logger.Error(ex.Message);
                return null;
            }
}


        public Task AddAttacksModuleToDB(AttacksModule mod)
        {

            try 
            { 
            using (MySqlConnection connection = new MySqlConnection(DnDBot.connStr))
            {
                connection.Open();
                using (MySqlCommand cmd = new MySqlCommand("INSERT INTO enemyattackmodules (atkModID, moduleName, description, baseAtk1ID, baseAtk2ID, medAtk1ID, medAtk2ID, bigAtk1ID, bigAtk2ID, hugeAtk1ID, hugeAtk2ID) VALUES (@atkModID, @moduleName, @description, @baseAtk1ID, @baseAtk2ID, @medAtk1ID, @medAtk2ID, @bigAtk1ID, @bigAtk2ID, @hugeAtk1ID, @hugeAtk2ID)", connection))
                {
                    cmd.Parameters.AddWithValue("@atkModID", mod.atkModID);
                    cmd.Parameters.AddWithValue("@moduleName", mod.moduleName);
                        cmd.Parameters.AddWithValue("@description", mod.description);
                        cmd.Parameters.AddWithValue("@baseAtk1ID", mod.baseAtk1ID);
                    cmd.Parameters.AddWithValue("@baseAtk2ID", mod.baseAtk2ID);
                    cmd.Parameters.AddWithValue("@medAtk1ID", mod.medAtk1ID);
                    cmd.Parameters.AddWithValue("@medAtk2ID", mod.medAtk2ID);
                    cmd.Parameters.AddWithValue("@bigAtk1ID", mod.bigAtk1ID);
                    cmd.Parameters.AddWithValue("@bigAtk2ID", mod.bigAtk2ID);
                    cmd.Parameters.AddWithValue("@hugeAtk1ID", mod.hugeAtk1ID);
                    cmd.Parameters.AddWithValue("@hugeAtk2ID", mod.hugeAtk2ID);

                    cmd.ExecuteNonQuery();
                }
                connection.Close();
            }

            return Task.CompletedTask;
            }
            catch (Exception ex)
            {


                Log.Logger.Error(ex.Message);
                return null;
            }
        }

    }



    public class EnemyAttack
    {

        internal ulong ID { get; set; }

        internal string attackName { get; set; }

        internal string description { get; set; }

        internal int baseDamage { get; set; }

        internal DamageType damageType { get; set; }


        internal EnemyAttack(ulong id, string attackname, string description, int baseDamage, DamageType damageType)
        {
            this.ID = id;
            this.attackName = attackname;
            this.description = description;
            this.baseDamage = baseDamage;
            this.damageType = damageType;
        }





        public static EnemyAttack GetEnemyAttackFromDB(ulong id)
        {
            EnemyAttack attack = new EnemyAttack(0, "null", "null", 0, DamageType.Physical);


            try
            {
                
                

            using (MySqlConnection connection = new MySqlConnection(DnDBot.connStr))
            {
                connection.Open();
                using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM enemyattacks WHERE ID = @ID", connection))
                {
                    cmd.Parameters.AddWithValue("@ID", id);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        attack.ID = reader.GetUInt64("ID");
                        attack.attackName = reader.GetString("attackName");
                        attack.description = reader.GetString("description");
                        attack.baseDamage = reader.GetInt32("baseDamage");
                        attack.damageType = (DamageType)reader.GetInt32("damageType");

                    }
                }
                connection.Close();


            }

            return attack;
        }
            catch (Exception ex)
            {
                Log.Logger.Error(ex.Message);
                return null;
            }
}


        public Task AddEnemyAttackToDB(EnemyAttack attack)
        {

            try
            {

                using (MySqlConnection connection = new MySqlConnection(DnDBot.connStr))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand("INSERT INTO enemyattacks (ID, attackName, baseDamage, damageType, description) VAlUES (@ID, @attackName, @baseDamage, @damageType, @description)", connection))
                    {
                        cmd.Parameters.AddWithValue("@ID", attack.ID);
                        cmd.Parameters.AddWithValue("@attackName", attack.attackName);
                        cmd.Parameters.AddWithValue("@description", attack.description);
                        cmd.Parameters.AddWithValue("@baseDamage", attack.baseDamage);
                        cmd.Parameters.AddWithValue("@damageType", (int)attack.damageType);

                        cmd.ExecuteNonQuery();
                    }
                    connection.Close();
                }

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex.Message);
                return null;
            }
            
            
        }





        
    }

   
}


    

    


