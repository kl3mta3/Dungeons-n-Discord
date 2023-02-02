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

namespace DnDBot.UniversalCommands
{
    internal class SystemCommands: BaseCommandModule
    {
        [Command("system-createworld"), Aliases("world-create"), Description("Builds the WorldGridSpace")]
        public  Task CreateWorld(CommandContext ctx, int worldSize)
        {
            ctx.Client.Logger.LogInformation($"ctx Guild.ID si {ctx.Guild.Id}- homeserver id is {DnDBot.homeServer.Id}");
            ctx.Client.Logger.LogInformation($"ctx user is {ctx.User.Id}- god is {DnDBot.god.Id}");
            if (ctx.Guild.Id == DnDBot.homeServer.Id && ctx.User.Id == DnDBot.god.Id)
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


            if (ctx.Guild.Id == DnDBot.homeServer.Id && ctx.User == DnDBot.god)
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

        [Command("system-test"), Aliases("test"), Description("test update stat to Server to database.")]
        public async Task TestUpdate(CommandContext ctx)
        {
            ctx.Client.Logger.LogInformation($"test update started");
            await DnDBot.testUpdateStat();
            ctx.Client.Logger.LogInformation($"test update finished");
            return;

        }


    }
}
