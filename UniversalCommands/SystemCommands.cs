using DisCatSharp.CommandsNext;
using DisCatSharp.CommandsNext.Attributes;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DnDBot;
using System.Threading.Channels;
using DnDBot.System;
using DnDBot.Enemies;
using static Mysqlx.Notice.Warning.Types;
using System.Xml.Linq;
using DisCatSharp.Entities;
using DnDBot.Character;

namespace DnDBot.UniversalCommands
{
    internal class SystemCommands: BaseCommandModule
    {
        [Command("system-createworld"), Aliases("world-create"), Description("Builds the WorldGridSpace")]
        public  Task CreateWorld(CommandContext ctx, int worldSize)
        {
            ctx.Client.Logger.LogInformation($"ctx Guild.ID si {ctx.Guild.Id}- homeserver id is {DnDBot.homeServer.Id}");
            ctx.Client.Logger.LogInformation($"ctx user is {ctx.User.Id}- god is {DnDBot.god.Id}");
            if (CanUseSystemCommand(ctx))
            {

                DnDBot.CreateWorld(worldSize);
                ctx.Client.Logger.LogInformation($"WorldGridSpace created by {ctx.User.Username}");
                return ctx.Member.SendMessageAsync("WorldGridSpace created");
            }
            else
            {
                ctx.Client.Logger.LogWarning($"Unauthorized attempted to create WorldGridSpace attempted by {ctx.User.Username} in {ctx.Channel} from guild {ctx.Guild.Name}");
                ctx.Member.SendMessageAsync($"Warning: Unauthorized attempted to create WorldGridSpace attempted by {ctx.User.Username} in {ctx.Channel} from guild {ctx.Guild.Name}");
                return ctx.Member.SendMessageAsync($"{ctx.Member.Mention} Continued Attempts to used commands beyond your user access will result in punisment and possibly result in your Ban from Dungeons and Disc.");
            }


        }
        [Command("system-fillDatabase"), Aliases("world-fill"), Description("Addes and Creates Players from Discord Users on Server to database.")]
        public Task FillDatabase(CommandContext ctx)
        {

            ctx.Client.Logger.LogInformation($"ctx Guild.ID si {ctx.Guild.Id}- homeserver id is {DnDBot.homeServer.Id}");
            ctx.Client.Logger.LogInformation($"ctx user is {ctx.User.Id}- god is {DnDBot.god.Id}");


            if (CanUseSystemCommand(ctx))
            {

                DnDBot.FillCharacterDatabase();
                ctx.Client.Logger.LogInformation($"DataBase Successfully Updated by {ctx.User.Username}");
                return ctx.Member.SendMessageAsync("Database Updated");
            }
            else
            {
                ctx.Client.Logger.LogInformation($"Unauthorized attempted to Update The Database attempted by {ctx.User.Username} in {ctx.Channel} from guild {ctx.Guild.Name}");
                ctx.Member.SendMessageAsync($"Warning: Unauthorized attempted to Fill The Database attempted by {ctx.User.Username} in {ctx.Channel} from guild {ctx.Guild.Name}");
                return ctx.Member.SendMessageAsync($"{ctx.Member.Mention} Continued Attempts to used commands beyond your user access will result in punisment and possibly result in your Ban from Dungeons and Disc.");
            }
        }



        [Command("system-addEnemy"), Aliases("ae"), Description("Format: ID, name,  health,  energy, atkModID, description, armor, level, attackSpeed, classType, enemyType, damageType.")]
        public Task AddNewEnemyToDB(CommandContext ctx, [RemainingText] string _enemy)
        {

            if (CanUseSystemCommand(ctx))
            {
                string[] enemyData = _enemy.Split(',');
                ulong ID = Convert.ToUInt64(enemyData[0]);
                string name = enemyData[1];
                int health = Convert.ToInt32(enemyData[2]);
                int energy = Convert.ToInt32(enemyData[3]);
                int atkModID = Convert.ToInt32(enemyData[4]);
                string description = enemyData[5];
                int armor = Convert.ToInt32(enemyData[6]);
                int level = Convert.ToInt32(enemyData[7]);
                int attackSpeed = Convert.ToInt32(enemyData[8]);
                ClassType classType = (ClassType)Convert.ToInt32(enemyData[9]);
                EnemyType enemyType = (EnemyType)Convert.ToInt32(enemyData[10]);
                DamageType damageType = (DamageType)Convert.ToInt32(enemyData[11]);
                
                Enemy enemy = new Enemy(ID, name,  health,  energy, atkModID, description, armor, level, attackSpeed, classType, enemyType, damageType);

                enemy.AddEnemyToDB(enemy);

                ctx.Client.Logger.LogInformation($"Enemy Succfully Added by {ctx.User.Username}");
                return ctx.Channel.SendMessageAsync("Enemy Updated");
            }
            else
            {
                ctx.Client.Logger.LogInformation($"Unauthorized attempt to add Enemy to the Database attempted by {ctx.User.Username} in {ctx.Channel} from guild {ctx.Guild.Name}");
                ctx.Member.SendMessageAsync($"Warning: Unauthorized attempt to add Enemy to the Database attempted by {ctx.User.Username} in {ctx.Channel} from guild {ctx.Guild.Name}");
                return ctx.Member.SendMessageAsync($"{ctx.Member.Mention} Continued Attempts to used commands beyond your user access will result in punisment and possibly result in your Ban from Dungeons and Disc.");
            }
        }



        [Command("system-addEnemyAtk"), Aliases("aek"), Description("Format: ID, attackName, description, baseDamage, damageType.")]
        public Task AddNewEnemyAttackToDB(CommandContext ctx, [RemainingText] string _enemy)
        {

            if (CanUseSystemCommand(ctx))
            {
                string[] enemyData = _enemy.Split(',');
                ulong ID = Convert.ToUInt64(enemyData[0]);
                string attackName = enemyData[1];
                string description = enemyData[2];
                int baseDamage = Convert.ToInt32(enemyData[3]);
                DamageType damageType = (DamageType)Convert.ToInt32(enemyData[4]);

                EnemyAttack attack = new EnemyAttack(ID, attackName, description, baseDamage, damageType);

                attack.AddEnemyAttackToDB(attack);

                ctx.Client.Logger.LogInformation($"EnemyAttack Succfully Added by {ctx.User.Username}");
                return ctx.Channel.SendMessageAsync("EnemyAttack Updated");
            }
            else
            {
                ctx.Client.Logger.LogInformation($"Unauthorized attempt to add Enemy to the Database attempted by {ctx.User.Username} in {ctx.Channel} from guild {ctx.Guild.Name}");
                ctx.Member.SendMessageAsync($"Warning: Unauthorized attempt to add Enemy to the Database attempted by {ctx.User.Username} in {ctx.Channel} from guild {ctx.Guild.Name}");
                return ctx.Member.SendMessageAsync($"{ctx.Member.Mention} Continued Attempts to used commands beyond your user access will result in punisment and possibly result in your Ban from Dungeons and Disc.");
            }
        }


        [Command("system-addEnemyAtkMod"), Aliases("aeam"), Description("Format: atkModID, moduleName, description, baseAtk1ID, baseAtk2ID, medAtk1ID, medAtk2ID, bigAtk1ID, bigAtk2ID, hugeAtk1ID, hugeAtk2ID")]
        public Task AddNewEnemyAttackModuleToDB(CommandContext ctx, [RemainingText] string _enemy)
        {

            if (CanUseSystemCommand(ctx))
            {
                string[] enemyData = _enemy.Split(',');
                ulong atkModID = Convert.ToUInt64(enemyData[0]);
                string moduleName = enemyData[1];
                string description = enemyData[2];
                int baseAtk1ID = Convert.ToInt32(enemyData[3]);
                int baseAtk2ID = Convert.ToInt32(enemyData[4]);
                int medAtk1ID = Convert.ToInt32(enemyData[5]);
                int medAtk2ID = Convert.ToInt32(enemyData[6]);
                int bigAtk1ID = Convert.ToInt32(enemyData[7]);
                int bigAtk2ID = Convert.ToInt32(enemyData[8]);
                int hugeAtk1ID = Convert.ToInt32(enemyData[9]);
                int hugeAtk2ID = Convert.ToInt32(enemyData[10]);


                AttacksModule attack = new AttacksModule(atkModID, moduleName, description, baseAtk1ID, baseAtk2ID, medAtk1ID, medAtk2ID, bigAtk1ID, bigAtk2ID, hugeAtk1ID, hugeAtk2ID);

                attack.AddAttacksModuleToDB(attack);

                ctx.Client.Logger.LogInformation($"EnemyAttackMod Succfully Added by {ctx.User.Username}");
                return ctx.Channel.SendMessageAsync("EnemyAttackMod Updated");
            }
            else
            {
                ctx.Client.Logger.LogInformation($"Unauthorized attempt to add Enemy to the Database attempted by {ctx.User.Username} in {ctx.Channel} from guild {ctx.Guild.Name}");
                ctx.Member.SendMessageAsync($"Warning: Unauthorized attempt to add Enemy to the Database attempted by {ctx.User.Username} in {ctx.Channel} from guild {ctx.Guild.Name}");
                return ctx.Member.SendMessageAsync($"{ctx.Member.Mention} Continued Attempts to used commands beyond your user access will result in punisment and possibly result in your Ban from Dungeons and Disc.");
            }
        }

        [Command("system-displayenemy"), Aliases("de"), Description("Format: atkModID, moduleName, description, baseAtk1ID, baseAtk2ID, medAtk1ID, medAtk2ID, bigAtk1ID, bigAtk2ID, hugeAtk1ID, hugeAtk2ID")]
        public Task DisplayEnemyByID(CommandContext ctx, [RemainingText] string _enemyId)
        {
            if (CanUseSystemCommand(ctx))
            {

                Enemy enemy =  Enemy.GetEnemyFromDB(Convert.ToUInt64(_enemyId));

                AttacksModule attack = AttacksModule.GetAttacksModuleFromDb(Convert.ToUInt64(enemy.atkModID));

                var enemyInfoEmbed = new DiscordEmbedBuilder
                {
                    Color = DiscordColor.Magenta,
                    Title = $"             Dungeons n Disc",
                    Description = $"      Enemy Info for {enemy.name}",
                };

                enemyInfoEmbed.AddField(new DiscordEmbedField("Name", $"{enemy.name}", true));
                enemyInfoEmbed.AddField(new DiscordEmbedField("Level", $"{enemy.level}", true));
                enemyInfoEmbed.AddField(new DiscordEmbedField("EnemyType", $"{enemy.enemyType.ToString()}", true));
                
                enemyInfoEmbed.AddField(new DiscordEmbedField("Description", $"{enemy.description}", false));
                
                enemyInfoEmbed.AddField(new DiscordEmbedField("ClassType", $"{enemy.classType.ToString()}", true));
                enemyInfoEmbed.AddField(new DiscordEmbedField("Damage Type", $"{enemy.damageType}", true));

                enemyInfoEmbed.AddField(new DiscordEmbedField("Enemy ID", $"{enemy.ID}", false));
                enemyInfoEmbed.AddField(new DiscordEmbedField($"Stats", "-------------------------", false));
                enemyInfoEmbed.AddField(new DiscordEmbedField("Health", $"{enemy.health}", true));
                enemyInfoEmbed.AddField(new DiscordEmbedField("Armor", $"{enemy.armor}", true));
                enemyInfoEmbed.AddField(new DiscordEmbedField("AttackSpeed", $"{enemy.attackSpeed}", true));
                enemyInfoEmbed.AddField(new DiscordEmbedField("-", $"-", true));

                enemyInfoEmbed.AddField(new DiscordEmbedField($"Basic Attacks", "-------------------------", false));

                if (attack.baseAtk1ID != 0)
                {
                    string attackname = EnemyAttack.GetEnemyAttackFromDB(Convert.ToUInt64(attack.baseAtk1ID)).attackName;
                    enemyInfoEmbed.AddField(new DiscordEmbedField($"{attackname}", $"{EnemyAttack.GetEnemyAttackFromDB(Convert.ToUInt64(attack.baseAtk1ID)).description}", false));
                    enemyInfoEmbed.AddField(new DiscordEmbedField($"Damage Type", $"{EnemyAttack.GetEnemyAttackFromDB(Convert.ToUInt64(attack.baseAtk1ID)).damageType}", true));
                    enemyInfoEmbed.AddField(new DiscordEmbedField("Damage", $"{EnemyAttack.GetEnemyAttackFromDB(Convert.ToUInt64(attack.baseAtk1ID)).baseDamage}", true));

                    enemyInfoEmbed.AddField(new DiscordEmbedField(".", $".", false));
                }

                if (attack.baseAtk2ID != 0)
                {
                    string attackname = EnemyAttack.GetEnemyAttackFromDB(Convert.ToUInt64(attack.baseAtk2ID)).attackName;
                    enemyInfoEmbed.AddField(new DiscordEmbedField($"{attackname}", $"{EnemyAttack.GetEnemyAttackFromDB(Convert.ToUInt64(attack.baseAtk2ID)).description}", false));
                    enemyInfoEmbed.AddField(new DiscordEmbedField($"Damage Type", $"{EnemyAttack.GetEnemyAttackFromDB(Convert.ToUInt64(attack.baseAtk2ID)).damageType}", true));
                    enemyInfoEmbed.AddField(new DiscordEmbedField("Damage", $"{EnemyAttack.GetEnemyAttackFromDB(Convert.ToUInt64(attack.baseAtk2ID)).baseDamage}", true));
                    enemyInfoEmbed.AddField(new DiscordEmbedField(".", $".", false));
                }

                enemyInfoEmbed.AddField(new DiscordEmbedField($"Medium Attacks", "-------------------------", false));
                if (attack.medAtk1ID != 0)
                {
                    string attackname = EnemyAttack.GetEnemyAttackFromDB(Convert.ToUInt64(attack.medAtk1ID)).attackName;
                    enemyInfoEmbed.AddField(new DiscordEmbedField($"{attackname}", $"{EnemyAttack.GetEnemyAttackFromDB(Convert.ToUInt64(attack.medAtk1ID)).description}", false));
                    enemyInfoEmbed.AddField(new DiscordEmbedField($"Damage Type", $"{EnemyAttack.GetEnemyAttackFromDB(Convert.ToUInt64(attack.medAtk1ID)).damageType}", true));
                    enemyInfoEmbed.AddField(new DiscordEmbedField("Damage", $"{EnemyAttack.GetEnemyAttackFromDB(Convert.ToUInt64(attack.medAtk1ID)).baseDamage}", true));
                    enemyInfoEmbed.AddField(new DiscordEmbedField(".", $".", false));
                }
                if (attack.medAtk2ID != 0)
                {
                    string attackname = EnemyAttack.GetEnemyAttackFromDB(Convert.ToUInt64(attack.medAtk2ID)).attackName;
                    enemyInfoEmbed.AddField(new DiscordEmbedField($"{attackname}", $"{EnemyAttack.GetEnemyAttackFromDB(Convert.ToUInt64(attack.medAtk2ID)).description}", false));
                    enemyInfoEmbed.AddField(new DiscordEmbedField($"Damage Type", $"{EnemyAttack.GetEnemyAttackFromDB(Convert.ToUInt64(attack.medAtk2ID)).damageType}", true));
                    enemyInfoEmbed.AddField(new DiscordEmbedField("Damage", $"{EnemyAttack.GetEnemyAttackFromDB(Convert.ToUInt64(attack.medAtk2ID)).baseDamage}", true));
                    enemyInfoEmbed.AddField(new DiscordEmbedField(".", $".", false));
                }
                enemyInfoEmbed.AddField(new DiscordEmbedField($"Big Attacks", "-------------------------", false));

                if (attack.bigAtk1ID != 0)
                {
                    string attackname = EnemyAttack.GetEnemyAttackFromDB(Convert.ToUInt64(attack.bigAtk1ID)).attackName;
                    enemyInfoEmbed.AddField(new DiscordEmbedField($"{attackname}", $"{EnemyAttack.GetEnemyAttackFromDB(Convert.ToUInt64(attack.bigAtk1ID)).description}", false));
                    enemyInfoEmbed.AddField(new DiscordEmbedField("Description", $"{EnemyAttack.GetEnemyAttackFromDB(Convert.ToUInt64(attack.bigAtk1ID)).description}", false));
                    enemyInfoEmbed.AddField(new DiscordEmbedField($"Damage Type", $"{EnemyAttack.GetEnemyAttackFromDB(Convert.ToUInt64(attack.bigAtk1ID)).damageType}", true));
                    enemyInfoEmbed.AddField(new DiscordEmbedField("Damage", $"{EnemyAttack.GetEnemyAttackFromDB(Convert.ToUInt64(attack.bigAtk1ID)).baseDamage}", true));
                    enemyInfoEmbed.AddField(new DiscordEmbedField(".", $".", false));
                }

                if (attack.bigAtk2ID != 0)
                {
                    string attackname = EnemyAttack.GetEnemyAttackFromDB(Convert.ToUInt64(attack.bigAtk2ID)).attackName;
                    enemyInfoEmbed.AddField(new DiscordEmbedField($"{attackname}", $"{EnemyAttack.GetEnemyAttackFromDB(Convert.ToUInt64(attack.bigAtk2ID)).description}", false));
                    enemyInfoEmbed.AddField(new DiscordEmbedField($"Damage Type", $"{EnemyAttack.GetEnemyAttackFromDB(Convert.ToUInt64(attack.bigAtk2ID)).damageType}", true));
                    enemyInfoEmbed.AddField(new DiscordEmbedField("Damage", $"{EnemyAttack.GetEnemyAttackFromDB(Convert.ToUInt64(attack.bigAtk2ID)).baseDamage}", true));
                    enemyInfoEmbed.AddField(new DiscordEmbedField(".", $".", false));
                }
                enemyInfoEmbed.AddField(new DiscordEmbedField($"Huge Attacks", "-------------------------", false));

                if (attack.hugeAtk1ID != 0)
                {
                    string attackname = EnemyAttack.GetEnemyAttackFromDB(Convert.ToUInt64(attack.hugeAtk1ID)).attackName;
                    enemyInfoEmbed.AddField(new DiscordEmbedField($"{attackname}", $"{EnemyAttack.GetEnemyAttackFromDB(Convert.ToUInt64(attack.hugeAtk1ID)).description}", false));
                    enemyInfoEmbed.AddField(new DiscordEmbedField($"Damage Type", $"{EnemyAttack.GetEnemyAttackFromDB(Convert.ToUInt64(attack.hugeAtk1ID)).damageType}", true));
                    enemyInfoEmbed.AddField(new DiscordEmbedField("Damage", $"{EnemyAttack.GetEnemyAttackFromDB(Convert.ToUInt64(attack.hugeAtk1ID)).baseDamage}", true));
                    enemyInfoEmbed.AddField(new DiscordEmbedField(".", $".",false));
                }
                if (attack.hugeAtk2ID != 0)
                {
                    string attackname = EnemyAttack.GetEnemyAttackFromDB(Convert.ToUInt64(attack.hugeAtk2ID)).attackName;
                    enemyInfoEmbed.AddField(new DiscordEmbedField($"{attackname}", $"{EnemyAttack.GetEnemyAttackFromDB(Convert.ToUInt64(attack.hugeAtk2ID)).description}", false));
                    enemyInfoEmbed.AddField(new DiscordEmbedField($"Damage Type", $"{EnemyAttack.GetEnemyAttackFromDB(Convert.ToUInt64(attack.hugeAtk2ID)).damageType}", true));
                    enemyInfoEmbed.AddField(new DiscordEmbedField("Damage", $"{EnemyAttack.GetEnemyAttackFromDB(Convert.ToUInt64(attack.hugeAtk2ID)).baseDamage}", true));
                    enemyInfoEmbed.AddField(new DiscordEmbedField(".", $".", false));
                }

                enemyInfoEmbed.Build();
                ctx.Client.Logger.LogInformation($"EnemyAttackMod Succfully Added by {ctx.User.Username}");
                return ctx.Channel.SendMessageAsync(embed: enemyInfoEmbed);
            }
            else
            {
                return ctx.Member.SendMessageAsync($"{ctx.Member.Mention} Continued Attempts to used commands beyond your user access will result in punisment and possibly result in your Ban from Dungeons and Disc.");
            }
        }
        public bool CanUseSystemCommand(CommandContext ctx)
        {
            if (ctx.Guild.Id == Config.SystemCommandConfig.systemGuildID && ctx.User.Id == Config.SystemCommandConfig.godUserID)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
       
        
    }
}
