
using Microsoft.Extensions.Logging;
using DisCatSharp.Entities;
using DisCatSharp.CommandsNext;
using DisCatSharp.CommandsNext.Attributes;
using DisCatSharp.Interactivity.Extensions;
using DisCatSharp.Interactivity;
using DisCatSharp.Interactivity.Enums;
using DisCatSharp.EventArgs;
using DisCatSharp.ApplicationCommands;
using DisCatSharp.ApplicationCommands.Attributes;
using DisCatSharp;
using DnDBot.Character;
using DnDBot;
using System.Numerics;
using MySql.Data.MySqlClient;
using System.Data;
using System.Xml.Linq;
using DisCatSharp.ApplicationCommands.Context;
using DisCatSharp.Enums;
using Mysqlx.Crud;
using Newtonsoft.Json;
using static Mysqlx.Crud.Order.Types;
using static System.Net.Mime.MediaTypeNames;
using DnDBot.WorldSystem;

namespace DnDBot.Character.Commands
{
    //[SlashCommandGroup("random", "Random commands")]
    public class RandomSlashCommands : ApplicationCommandsModule
    {


        [SlashCommand("hello", "Say Hello")]

        public static async Task HelloCommand(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder());
            ctx.Client.Logger.LogInformation($"/hello used by {ctx.User.Username} in {ctx.Channel.Name} on {ctx.Guild.Name}");
            var em = new DiscordEmbedBuilder();
            em.WithColor(DiscordColor.Blurple);
            em.WithTitle($"Greetings! {ctx.Member.Username} Nice to meet you... I hope!");
            //em.WithDescription($"it came up Heads!");
            DiscordWebhookBuilder builder = new DiscordWebhookBuilder();
            builder.AddEmbed(em);
            await ctx.EditResponseAsync(builder);
            //await ctx.Channel.SendMessageAsync($"Greetings!  {ctx.Member.Mention}  Nice to meet you... I hope!");

        }

        [SlashCommand("8ball", "Yes? No? Maybe?")]
        public static async Task EightBallAsync(InteractionContext ctx, [Option("text", "Text to modify")] string text)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder());
            var responses = new[] { "It is certain", "It is decidedly so", "Without a doub.", "Yes - definitely", "You may rely on it", "As I see it, yes", "Most likely", "Outlook good", "Yes", "Signs point to yes", "Reply hazy, try again", "Ask again later", "Better not tell you now", "Cannot predict now.", "Concentrate and ask again", "Don't count on it", "My reply is no", "My sources say no", "Outlook not so good", "Very doubtful", "No" };


            var em = new DiscordEmbedBuilder();
            em.WithColor(DiscordColor.Blurple);
            em.WithTitle($"> {text}\n\n{responses[new Random().Next(0, responses.Length)]}!");
            //em.WithDescription($"it came up Heads!");
            DiscordWebhookBuilder builder = new DiscordWebhookBuilder();
            builder.AddEmbed(em);
            await ctx.EditResponseAsync(builder);

           
        }

        [SlashCommand("roll", "Rolls a random number between 1 and 100")]

        public static async Task RollCommand(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder());
            var roll = new Random().Next(1, 100);
            ctx.Client.Logger.LogInformation($"/roll used by {ctx.User.Username} in {ctx.Channel.Name} on {ctx.Guild.Name} and they rolled a {roll}.");
            // await ctx.Channel.SendMessageAsync($" {ctx.Member.Mention} rolled a {roll}!");
            var em = new DiscordEmbedBuilder();
            em.WithColor(DiscordColor.Blurple);
            em.WithTitle($"{ctx.Member.Username}  rolled a {roll}!");
            //em.WithDescription($"it came up Heads!");
            DiscordWebhookBuilder builder = new DiscordWebhookBuilder();
            builder.AddEmbed(em);
            await ctx.EditResponseAsync(builder);
        }




        [SlashCommand("flip", "Flips a coin")]
        public static async Task FlipCommand(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder());
            int flip = new Random().Next(1, 2);

            if (flip == 1)
            {
                ctx.Client.Logger.LogInformation($"/flip used by {ctx.User.Username} in {ctx.Channel.Name} on {ctx.Guild.Name} and got heads.");
                //await ctx.Channel.SendMessageAsync($"  {ctx.Member.Mention} flipped a Coin, it came up Heads!");
                var em = new DiscordEmbedBuilder();
                em.WithColor(DiscordColor.Blurple);
                em.WithTitle($" {ctx.Member.Username}  flipped a Coin");
                em.WithDescription($"it came up Heads!");
                DiscordWebhookBuilder builder = new DiscordWebhookBuilder();
                builder.AddEmbed(em);
                await ctx.EditResponseAsync(builder);
            }
            else
            {
                ctx.Client.Logger.LogInformation($"/flip used by {ctx.User.Username} in {ctx.Channel.Name} on {ctx.Guild.Name} and got tails.");
                //await ctx.Channel.SendMessageAsync($"  {ctx.Member.Mention} flipped a Coin, it came up Tails!");
                var em = new DiscordEmbedBuilder();
                em.WithColor(DiscordColor.Blurple);
                em.WithTitle($" {ctx.Member.Username} flipped a Coin");
                em.WithDescription($"it came up Tails!");
                DiscordWebhookBuilder builder = new DiscordWebhookBuilder();
                builder.AddEmbed(em);
                await ctx.EditResponseAsync(builder);
            }
        }


        [SlashCommand("rolldie", "Rolls a die with the number of sides you specify")]

        public static async Task RollDieCommand(InteractionContext ctx, [Option("text", "Text to modify")] int sides)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder());
            ctx.Client.Logger.LogInformation($"/rolldie used by {ctx.User.Username} in {ctx.Channel.Name} on {ctx.Guild.Name} with a {sides} sided die.");
            var roll = new Random().Next(1, sides);
            //await ctx.Channel.SendMessageAsync($" {ctx.Member.Mention} rolled a {sides} sided die and rolled a {roll}!");
            var em = new DiscordEmbedBuilder();
            em.WithColor(DiscordColor.Blurple);
            em.WithTitle($"{ctx.Member.Username} Rolled a {sides}'d Die");
            em.WithDescription($"And rolled a {roll}!");
            DiscordWebhookBuilder builder = new DiscordWebhookBuilder();
            builder.AddEmbed(em);
            await ctx.EditResponseAsync(builder);
        }

        [SlashCommand("rps", "Play rock paper scissors!")]
        public static async Task RPSAsync(InteractionContext ctx, [Option("rps", "Your rock paper scissor choice")] string rps)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder());
            var rock = new[] { $"Rock {DiscordEmoji.FromName(ctx.Client, ":black_circle:")}", $"Paper {DiscordEmoji.FromName(ctx.Client, ":pencil:")}", $"Scissors {DiscordEmoji.FromName(ctx.Client, ":scissors:")}" };

            var em = new DiscordEmbedBuilder();
            em.WithColor(DiscordColor.Blurple);
            em.WithTitle($"{ctx.Member.Username} choose {rps}!\n\nI choose {rock[new Random().Next(0, rock.Length)]}");
            //em.WithDescription($"And rolled a {roll}!");
            DiscordWebhookBuilder builder = new DiscordWebhookBuilder();
            builder.AddEmbed(em);
            await ctx.EditResponseAsync(builder);

            //await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"{ctx.Member.Mention} choose {rps}!\n\nI choose {rock[new Random().Next(0, rock.Length)]}"));
        }

        [SlashCommand("move", "Move in a given Direction")]
        public static async Task MoveCommand(InteractionContext ctx, [Option("Direction", "Direction to move")] string _direction)
          {
            string direction = _direction.ToLower();

                await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder());
                // Player player = CharacterSystem.GetPlayer(ctx.Member.Id);
                Player player = Player.GetPlayerFromDataBase(ctx.Member.Id);
                Console.WriteLine(player.name + "was gotten from database");
                DiscordMember member = ctx.Member;
            Vector2 currentLocation = new Vector2(player.worldPosition.X, player.worldPosition.Y);
                
                WorldGrid newGrid= new WorldGrid();

            switch (direction)
                {



                    case "north":

                    newGrid = WorldGrid.GetGridByLocation(new Vector2(currentLocation.X, currentLocation.Y + 1));
                    if (newGrid.CanMoveToGrid())
                        {


                        string message = $"";

                        if (newGrid.onPath)
                        {
                            message = $"You moved North along the Path!";

                        }
                        else if (newGrid.isWilds)
                        {

                            message = $"You moved North, in the Wilds. Be careful!";

                        }
                        else if (newGrid.isMine)
                        {

                            message = $"You moved North, in a Mine. It's dark, not like that matters";

                        }
                        else if (newGrid.isInstance)
                        {

                            message = $"You moved North, in the Dungeon. It's wise to Leave!";

                        }
                        else if (newGrid.isCity)
                        {

                            message = $"You moved North, in A City. It's Safe Here!";

                        }

                        else
                        {
                            message = $"You moved North!";

                        }

                        await player.UpdatePlayerPosition(player.playerID, new Vector2(0, 1));
                        if (!newGrid.GridDiscovered(player.playerID, (int)newGrid.gridID))
                        {
                            await player.UpdatePlayerStatInDataBase(player.playerID,"gridsDiscovered",1);
                            await player.LevelPlayer(player.playerID,5);
                            await newGrid.InsertDiscoveredGirdDataBase(player.playerID, (int)newGrid.gridID);
                            

                        }
                            //await ctx.Member.SendMessageAsync($" {ctx.Member.Mention} you moved North!");

                            DiscordWebhookBuilder builder = new DiscordWebhookBuilder();
                            var em = new DiscordEmbedBuilder();
                            em.WithColor(DiscordColor.Blurple);
                            em.WithTitle($"{member.Nickname} {message}");
                            builder.AddEmbed(em);
                            await ctx.EditResponseAsync(builder);
                            return;

                        }
                        else
                        {
                            string message = $"";
                      
                            if (newGrid.isMountain)
                            {
                                message = $"You can't move North, Mountains block your way!";

                            }
                            else if(newGrid.isWater)
                            {

                            message = $"You can't move North, Water is too Deep to Cross!";

                            }
                            else if(newGrid.gridLocation.X>DnDBot.worldSpace.worldMaxXValue)
                            {

                            message = $"You can't move North, You've Reached the end of the world, as you know it!";
                            }
                            else
                            {
                            message = $"You can't move North!";

                            }

                            DiscordWebhookBuilder builder = new DiscordWebhookBuilder();
                            var em = new DiscordEmbedBuilder();
                            em.WithColor(DiscordColor.Blurple);
                            em.WithTitle($"{member.Nickname}, {message}");
                            //em.WithDescription($"it came up Heads!");
                            builder.AddEmbed(em);
                            await ctx.EditResponseAsync(builder);
                            return;
                        }

                    case "south":

                    newGrid = WorldGrid.GetGridByLocation(new Vector2(currentLocation.X, currentLocation.Y - 1));
                    if (newGrid.CanMoveToGrid())
                    {
                        string message = $"";

                        if (newGrid.onPath)
                        {
                            message = $"You moved South along the Path!";

                        }
                        else if (newGrid.isWilds)
                        {

                            message = $"You moved South, in the Wilds. Be careful!";

                        }
                        else if (newGrid.isMine)
                        {

                            message = $"You moved South, in a Mine. It's dark, not like that matters";

                        }
                        else if (newGrid.isInstance)
                        {

                            message = $"You moved South, in the Dungeon. It's wise to Leave!";

                        }
                        else if (newGrid.isCity)
                        {

                            message = $"You moved South, in A City. It's Safe Here!";

                        }

                        else
                        {
                            message = $"You moved South!";

                        }
                        //await player.MoveSouth();
                        await player.UpdatePlayerPosition(player.playerID, new Vector2(0, -1));
                        if (!newGrid.GridDiscovered(player.playerID, (int)newGrid.gridID))
                        {
                            await player.UpdatePlayerStatInDataBase(player.playerID, "gridsDiscovered", 1);
                            await player.LevelPlayer(player.playerID, 5);
                            await newGrid.InsertDiscoveredGirdDataBase(player.playerID, (int)newGrid.gridID);


                        }
                        ctx.Client.Logger.LogInformation($"{player.name} moved {direction} To {player.worldPosition.X}, {player.worldPosition.Y}");
                        DiscordWebhookBuilder builder = new DiscordWebhookBuilder();
                        var em = new DiscordEmbedBuilder();
                        em.WithColor(DiscordColor.Blurple);
                        em.WithTitle($"{ctx.Member.DisplayName} {message}");
                        builder.AddEmbed(em);
                        await ctx.EditResponseAsync(builder);
                        return;
                    }
                    else
                    {
                        string message = $"";

                        if (newGrid.isMountain)
                        {
                            message = $"You can't move South, Mountains block your way!";

                        }
                        else if (newGrid.isWater)
                        {

                            message = $"You can't move South, Water is too Deep to Cross!";

                        }
                        else if (newGrid.gridLocation.X > DnDBot.worldSpace.worldMaxXValue)
                        {

                            message = $"You can't move South, You've Reached the end of the world, as you know it!";
                        }
                        else
                        {
                            message = $"You can't move South!";

                        }

                        DiscordWebhookBuilder builder = new DiscordWebhookBuilder();
                        var em = new DiscordEmbedBuilder();
                        em.WithColor(DiscordColor.Blurple);
                        em.WithTitle($"{ctx.Member.DisplayName}, {message}");
                        //em.WithDescription($"it came up Heads!");
                        builder.AddEmbed(em);
                        await ctx.EditResponseAsync(builder);
                        return;
                    }

                    case "east":
                    newGrid = WorldGrid.GetGridByLocation(new Vector2(currentLocation.X + 1, currentLocation.Y));
                    if (newGrid.CanMoveToGrid())
                        {


                        string message = $"";

                        if (newGrid.onPath)
                        {
                            message = $"You moved East along the Path!";

                        }
                        else if (newGrid.isWilds)
                        {

                            message = $"You moved East, in the Wilds. Be careful!";

                        }
                        else if (newGrid.isMine)
                        {

                            message = $"You moved East, in a Mine. It's dark, not like that matters";

                        }
                        else if (newGrid.isInstance)
                        {

                            message = $"You moved East, in the Dungeon. It's wise to Leave!";

                        }
                        else if (newGrid.isCity)
                        {

                            message = $"You moved East, in A City. It's Safe Here!";

                        }

                        else
                        {
                            message = $"You moved East!";

                        }







                        //await player.MoveEast();
                        await player.UpdatePlayerPosition(player.playerID, new Vector2(1, 0));
                        if (!newGrid.GridDiscovered(player.playerID, (int)newGrid.gridID))
                        {
                            await player.UpdatePlayerStatInDataBase(player.playerID, "gridsDiscovered", 1);
                            await player.LevelPlayer(player.playerID, 5);
                            await newGrid.InsertDiscoveredGirdDataBase(player.playerID, (int)newGrid.gridID);


                        }
                        ctx.Client.Logger.LogInformation($"{player.name} moved {direction} To {player.worldPosition.X}, {player.worldPosition.Y}");

                        DiscordWebhookBuilder builder = new DiscordWebhookBuilder();
                        var em = new DiscordEmbedBuilder();
                        em.WithColor(DiscordColor.Blurple);
                        em.WithTitle($"{ctx.Member.DisplayName} {message}");
                        //em.WithDescription($"it came up Heads!");
                        builder.AddEmbed(em);
                        await ctx.EditResponseAsync(builder);
                        return;
                        }
                        else
                        {

                        string message = $"";

                        if (newGrid.isMountain)
                        {
                            message = $"You can't move East, Mountains block your way!";

                        }
                        else if (newGrid.isWater)
                        {

                            message = $"You can't move East, Water is too Deep to Cross!";

                        }
                        else if (newGrid.gridLocation.X > DnDBot.worldSpace.worldMaxXValue)
                        {

                            message = $"You can't move East, You've Reached the end of the world, as you know it!";
                        }
                        else
                        {
                            message = $"You can't move East, You've Reached the end of the world, as you know it!";

                        }

                        DiscordWebhookBuilder builder = new DiscordWebhookBuilder();
                        var em = new DiscordEmbedBuilder();
                        em.WithColor(DiscordColor.Blurple);
                        em.WithTitle($"{ctx.Member.DisplayName}, {message}");
                        //em.WithDescription($"it came up Heads!");
                        builder.AddEmbed(em);
                        await ctx.EditResponseAsync(builder);
                        return;
                        }

                    case "west":

                        newGrid = WorldGrid.GetGridByLocation(new Vector2(currentLocation.X - 1, currentLocation.Y));

                    if (newGrid.CanMoveToGrid())
                        {
                        string message = $"";

                        if (newGrid.onPath)
                        {
                            message = $"You moved West along the Path!";

                        }
                        else if (newGrid.isWilds)
                        {

                            message = $"You moved West, in the Wilds. Be careful!";

                        }
                        else if (newGrid.isMine)
                        {

                            message = $"You moved West, in a Mine. It's dark, not like that matters";

                        }
                        else if (newGrid.isInstance)
                        {

                            message = $"You moved West, in the Dungeon. It's wise to Leave!";

                        }
                        else if (newGrid.isCity)
                        {

                            message = $"You moved West, in A City. It's Safe Here!";

                        }

                        else
                        {
                            message = $"You moved West!";

                        }
                        //await player.MoveWest();
                        await player.UpdatePlayerPosition(player.playerID, new Vector2(-1, 0));
                        if (!newGrid.GridDiscovered(player.playerID, (int)newGrid.gridID))
                        {
                            await player.UpdatePlayerStatInDataBase(player.playerID, "gridsDiscovered", 1);
                            await player.LevelPlayer(player.playerID, 5);
                            await newGrid.InsertDiscoveredGirdDataBase(player.playerID, (int)newGrid.gridID);


                        }
                        ctx.Client.Logger.LogInformation($"{player.name} moved {direction} To {player.worldPosition.X}, {player.worldPosition.Y}");
                           // await ctx.Member.SendMessageAsync($" {ctx.Member.DisplayName} you moved West!");

                                DiscordWebhookBuilder builder = new DiscordWebhookBuilder();
                                var em = new DiscordEmbedBuilder();
                                 em.WithColor(DiscordColor.Blurple);
                                 em.WithTitle($"{ctx.Member.DisplayName} {message}");
                                //em.WithDescription($"it came up Heads!");
                                 builder.AddEmbed(em);
                                await ctx.EditResponseAsync(builder);
                            return;

                       

                        }
                        else
                        {

                        string message = $"";

                        if (newGrid.isMountain)
                        {
                            message = $"You can't move West, Mountains block your way!";

                        }
                        else if (newGrid.isWater)
                        {

                            message = $"You can't move West, Water is too Deep to Cross!";

                        }
                        else if (newGrid.gridLocation.X > DnDBot.worldSpace.worldMaxXValue)
                        {

                            message = $"You can't move West, You've Reached the end of the world, as you know it!";
                        }
                        else
                        {
                            message = $"You can't move West, You've Reached the end of the world, as you know it!";

                        }

                        DiscordWebhookBuilder builder = new DiscordWebhookBuilder();
                        var em = new DiscordEmbedBuilder();
                        em.WithColor(DiscordColor.Blurple);
                        em.WithTitle($"{ctx.Member.Mention}, {message}");
                        //em.WithDescription($"it came up Heads!");
                        builder.AddEmbed(em);
                        await ctx.EditResponseAsync(builder);
                        return;
                        }
            }
                   
                 


        }

        [SlashCommand("commands", "display help")]

        public static async Task SendCommandsToUser(InteractionContext ctx)
        {
            ctx.Client.Logger.LogInformation($"/bothelp used by {ctx.User.Username} in {ctx.Channel.Name} on {ctx.Guild.Name}");
            var guild = ctx.Guild;
            var user = ctx.Member;

            var genEmbed = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Yellow,
                Title = $"Dungeons n Disc",
                Description = $"GENERAL COMMANDS"
            };



            genEmbed.AddField(new DiscordEmbedField($"/roll ", "Roll a random number between 1-100", false));
            genEmbed.AddField(new DiscordEmbedField($"/rollmax ", "Roll a random number with given max number", false));
            genEmbed.AddField(new DiscordEmbedField($"/flip ", "Pause the current Track.", false));



            genEmbed.Build();
            await user.SendMessageAsync(embed: genEmbed);
        }


        [SlashCommand("character", "sends the user their Character info")]

        public static async Task DisplayCharacter(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder());
            Player player = Player.GetPlayerFromDataBase(ctx.User.Id);
            ctx.Client.Logger.LogInformation($"Player {player.name} found.");
            var characterInfoEmbed = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Magenta,
                Title = $"             Dungeons n Disc",
                Description = $"      Character Info for {ctx.User.Username}",
        };
               characterInfoEmbed.WithThumbnail(ctx.Client.CurrentUser.AvatarUrl);

            characterInfoEmbed.AddField(new DiscordEmbedField("Name", player.name, true));
            characterInfoEmbed.AddField(new DiscordEmbedField("Level", player.playerLevel.ToString(), true));
            characterInfoEmbed.AddField(new DiscordEmbedField("Xp", player.xP.ToString(), true));
            characterInfoEmbed.AddField(new DiscordEmbedField("Current Position", $"Located at Grid({player.worldPosition.X}, {player.worldPosition.Y})", true));
            characterInfoEmbed.AddField(new DiscordEmbedField("Cash", player.cash.ToString(), true));
            characterInfoEmbed.AddField(new DiscordEmbedField("Home Guild", player.homeGuildName, true));
            characterInfoEmbed.AddField(new DiscordEmbedField("Player ID", player.playerID.ToString(), false));

            characterInfoEmbed.AddField(new DiscordEmbedField($"Stats", "-------------------------", false));
            characterInfoEmbed.AddField(new DiscordEmbedField("Health", player.baseHealth.ToString(), false));
            characterInfoEmbed.AddField(new DiscordEmbedField("Energy", player.baseEnergy.ToString(), true));
            characterInfoEmbed.AddField(new DiscordEmbedField("Stamina", player.baseStamina.ToString(), false));
            characterInfoEmbed.AddField(new DiscordEmbedField("Armor", player.baseArmor.ToString(), true));
            characterInfoEmbed.AddField(new DiscordEmbedField("Strength", player.baseStrength.ToString(), false));
            characterInfoEmbed.AddField(new DiscordEmbedField("Intellect", player.baseIntellect.ToString(), true));
            characterInfoEmbed.AddField(new DiscordEmbedField("Agility", player.baseAgility.ToString(), false));
            characterInfoEmbed.AddField(new DiscordEmbedField("Summary", $"{player.name} of {player.homeGuildName} is currently located at ({player.worldPosition.X}), ({player.worldPosition.Y}), \n is level {player.playerLevel} with {player.xP} Xp currently, \n and has {player.cash} cash in their pocket.", false));
            characterInfoEmbed.Build();

            DiscordWebhookBuilder builder = new DiscordWebhookBuilder();
            builder.AddEmbed(characterInfoEmbed);
            ctx.Client.Logger.LogInformation($"{player.name} of {player.homeGuildName} asked for and was sent Character info.");
            await ctx.EditResponseAsync(builder);



        }


        //[SlashCommand("modals", "A modal!")]
        //public async Task SendModalAsync(InteractionContext ctx)
        //{
        //    DiscordInteractionModalBuilder builder = new DiscordInteractionModalBuilder();
        //    builder.WithCustomId("modal_test");
        //    builder.WithTitle("Modal Test");
        //    builder.AddTextComponent(new DiscordTextComponent(TextComponentStyle.Paragraph, label: "Some input", required: false));

        //    await ctx.CreateModalResponseAsync(builder);
        //    var res = await ctx.Client.GetInteractivity().WaitForModalAsync(builder.CustomId, TimeSpan.FromMinutes(1));

        //    if (res.TimedOut)
        //        return;

        //    await res.Result.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordWebhookBuilder().WithContent(res.Result.Interaction.Data.Components?.First()?.Value ?? "Nothing was submitted.")) ;
        //}

        [SlashCommand("pingslash", "Ping Slash Command")]
            //await ctx.Member.SendMessageAsync($"{player.name} of {player.homeGuildName} is currently located at ({player.worldPosition.X}), ({player.worldPosition.Y}), \n is level {player.playerLevel} with {player.xP} Xp currently, \n and has {player.cash} cash in their pocket.");
           // await ctx.Member.SendMessageAsync(embed: characterInfoEmbed);

            //var em = new DiscordEmbedBuilder();
            //em.WithColor(DiscordColor.Blurple);
            //em.WithDescription($"{ctx.Member.DisplayName} your Character info was sent to you in a DM.");
        public async Task pingSlashCommand(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder());
            var em = new DiscordEmbedBuilder();
            em.WithColor(DiscordColor.Blurple);
            em.WithTitle($"Pong  Slash! {ctx.Client.Ping}ms");
            //em.WithDescription($"And rolled a {roll}!");
            DiscordWebhookBuilder builder = new DiscordWebhookBuilder();
            builder.AddEmbed(em);
            await ctx.EditResponseAsync(builder);
        }



        [SlashCommand("scout", "Scount the area around you")]
        public async Task ScoutArea(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder());
            Player player = Player.GetPlayerFromDataBase(ctx.Member.Id);
            Vector2 playerPosition = player.worldPosition;
            WorldGrid northGrid = WorldGrid.GetGridByLocation(new Vector2(playerPosition.X, playerPosition.Y + 1));
            WorldGrid southGrid = WorldGrid.GetGridByLocation(new Vector2(playerPosition.X, playerPosition.Y - 1));
            WorldGrid eastGrid = WorldGrid.GetGridByLocation(new Vector2(playerPosition.X + 1, playerPosition.Y));
            WorldGrid westGrid = WorldGrid.GetGridByLocation(new Vector2(playerPosition.X - 1, playerPosition.Y));
            WorldGrid closetCityGrid = WorldGrid.GetNearestCityGrid(playerPosition);
            float distance = Vector2.Distance(playerPosition, closetCityGrid.gridLocation);
            string northString;
            string southString;
            string eastString;
            string westString;

            if (northGrid.isWilds)
            {
                northString = $"North of you is off the path it looks wild and dangerous.\n";

            }
            else if (northGrid.isCity)
            {
                northString = $"North of you is a city seems safe and inviting.\n";
            }
            else if (northGrid.isInstance)
            {
                northString = $"North of you is a dungeon it looks dangerous and scar.y\n";
            }
            else if (northGrid.onPath)
            {
                northString = $"North of you is on the path it appears well worn and hazard-free.\n";
            }
            else if (northGrid.isMountain)
            {
                northString = $"North of you is a mountain. It's much too tall to scale, unless you're a Sherpa.\n";
            }
            else if (northGrid.isWater)
            {
                northString = $"North of you is a body of water. It's too deep to swim across.\n";
            }
            else if (northGrid.isMine)
            {
                northString = $"North of you is a mine. It's dark and full if Deadends.\n";
            }
            else
            {
                northString = $"North of you is a Dark Black Void.\n" +
                    $" Staring into it brigns a sense of fear and dread to all that gaze into it.\n";
            }

            if (southGrid.isWilds)
            {
                southString = $"South of you is off the path it looks wild and dangerous.\n";
            }
            else if (southGrid.isCity)
            {
                southString = $"South of you is a city seems safe and inviting.\n";
            }
            else if (southGrid.isInstance)
            {
                southString = $"South of you is a dungeon it looks dangerous and scar.y\n";
            }
            else if (southGrid.onPath)
            {
                southString = $"South of you is on the path it appears well worn and hazard-free.\n";
            }
            else if (southGrid.isMountain)
            {
                southString = $"South of you is a mountain. It's much too tall to scale, unless you're a Sherpa.\n";
            }
            else if (southGrid.isWater)
            {
                southString = $"South of you is a body of water. It's too deep to swim across.\n";
            }
            else if (southGrid.isMine)
            {
                southString = $"South of you is a mine. It's dark and full if Deadends.\n";
            }
            else
            {
                southString = $"South of you is a Dark Black Void.\n" +
                    $" Staring into it brigns a sense of fear and dread to all that gaze into it.\n";
            }

            if (eastGrid.isWilds)
            {
                eastString = $"East of you is off the path it looks wild and dangerous.\n";
            }
            else if (eastGrid.isCity)
            {
                eastString = $"East of you is a city seems safe and inviting.\n";
            }
            else if (eastGrid.isInstance)
            {
                eastString = $"East of you is a dungeon it looks dangerous and scar.y\n";
            }
            else if (eastGrid.onPath)
            {
                eastString = $"East of you is on the path it appears well worn and hazard-free.\n";
            }
            else if (eastGrid.isMountain)
            {
                eastString = $"East of you is a mountain. It's much too tall to scale, unless you're a Sherpa.\n";
            }
            else if (eastGrid.isWater)
            {
                eastString = $"East of you is a body of water. It's too deep to swim across.\n";
            }
            else if (eastGrid.isMine)
            {
                eastString = $"East of you is a mine. It's dark and full if Deadends.\n";
            }
            else
            {
                eastString = $"East of you is a Dark Black Void.\n" +
                    $" Staring into it brigns a sense of fear and dread to all that gaze into it.\n";
            }

            if (westGrid.isWilds)
            {
                westString = $"West of you is off the path it looks wild and dangerous.\n";
            }
            else if (westGrid.isCity)
            {
                westString = $"West of you is a city seems safe and inviting.\n";
            }
            else if (westGrid.isInstance)
            {
                westString = $"West of you is a dungeon it looks dangerous and scar.y\n";
            }
            else if (westGrid.onPath)
            {
                westString = $"West of you is on the path it appears well worn and hazard-free.\n";
            }
            else if (westGrid.isMountain)
            {
                westString = $"West of you is a mountain. It's much too tall to scale, unless you're a Sherpa.\n";
            }
            else if (westGrid.isWater)
            {
                westString = $"West of you is a body of water. It's too deep to swim across.\n";
            }
            else if (westGrid.isMine)
            {
                westString = $"West of you is a mine. It's dark and full if Deadends.\n";
            }
            else
            {
                westString = $"West of you is a Dark Black Void.\n" +
                    $" Staring into it brigns a sense of fear and dread to all that gaze into it.\n";
            }

            string message = $"You are currently located at ({playerPosition.X}, {playerPosition.Y}) \n"
                + $"The nearest city grid is located at ({closetCityGrid.gridLocation.X}, {closetCityGrid.gridLocation.Y})"
                + $" {Math.Round(distance)} grids away from you.\n";


            var em = new DiscordEmbedBuilder
            {
                Color=DiscordColor.PhthaloGreen,
                Title=$"------O.O------",
                Description=$"You take a look Around"
            };
            
            em.AddField(new DiscordEmbedField($"{northString}","-^-",false));
            em.AddField(new DiscordEmbedField($"{southString}", "-v-", false));
            em.AddField(new DiscordEmbedField($"{eastString}", "->-", false));
            em.AddField(new DiscordEmbedField($"{westString}", "-<-", false));
            em.AddField(new DiscordEmbedField("Summary", $"{message}", false));
            em.Build();
            //em.WithDescription($"And rolled a {roll}!");
            DiscordWebhookBuilder builder = new DiscordWebhookBuilder();
            builder.AddEmbed(em);
            await ctx.EditResponseAsync(builder);

        }




    }
    //[SlashCommandGroup("emotes", "Emote commands")]

    public class EmoteCommands : ApplicationCommandsModule
    {

        [SlashCommand("hug", "Hug someone!")]
        public static async Task HugAsync(InteractionContext ctx, [Option("user", "The user to execute the action with")] DiscordUser user)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder());
            var em = new DiscordEmbedBuilder();
            em.WithColor(DiscordColor.Chartreuse);
            em.WithDescription($"{ctx.User.Mention} hugs {user.Mention} uwu");

            DiscordWebhookBuilder builder = new();
            builder.AddEmbed(em.Build());
            await ctx.EditResponseAsync(builder);
        }

        [SlashCommand("kiss", "Kiss someone!")]
        public static async Task KissAsync(InteractionContext ctx, [Option("user", "The user to execute the action with")] DiscordUser user)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder());
            var em = new DiscordEmbedBuilder();
            em.WithColor(DiscordColor.Chartreuse);
            em.WithDescription($"{ ctx.User.Mention} kisses { user.Mention} > ~< ");

            DiscordWebhookBuilder builder = new();
            builder.AddEmbed(em.Build());
            await ctx.EditResponseAsync(builder);
        }

        [SlashCommand("lick", "Lick someone!")]
        public static async Task LickAsync(InteractionContext ctx, [Option("user", "The user to execute the action with")] DiscordUser user)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder());
            var em = new DiscordEmbedBuilder();
            em.WithColor(DiscordColor.Chartreuse);
            em.WithDescription($"{ctx.User.Mention} licks {user.Mention} owo");

            DiscordWebhookBuilder builder = new();
            builder.AddEmbed(em.Build());
            await ctx.EditResponseAsync(builder);
        }

        [SlashCommand("pat", "Pat someone!")]
        public static async Task PatAsync(InteractionContext ctx, [Option("user", "The user to execute the action with")] DiscordUser user)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder());
            var em = new DiscordEmbedBuilder();
            em.WithColor(DiscordColor.Chartreuse);
            em.WithDescription($"{ctx.User.Mention} pats {user.Mention} #w#");

            DiscordWebhookBuilder builder = new();
            builder.AddEmbed(em.Build());
            await ctx.EditResponseAsync(builder);
        }

        [SlashCommand("poke", "Poke someone!")]
        public static async Task PokeAsync(InteractionContext ctx, [Option("user", "The user to execute the action with")] DiscordUser user)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder());
            var em = new DiscordEmbedBuilder();
            em.WithDescription($"{ctx.User.Mention} pokes {user.Mention} ÓwÒ");
            em.WithColor(DiscordColor.Chartreuse);

            DiscordWebhookBuilder builder = new();
     
            builder.AddEmbed(em.Build());
            await ctx.EditResponseAsync(builder);
        }

        [SlashCommand("slap", "Slap someone!")]
        public static async Task SlapAsync(InteractionContext ctx, [Option("user", "The user to execute the action with")] DiscordUser user)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder());
           
            var em = new DiscordEmbedBuilder();
            em.WithColor(DiscordColor.Chartreuse);
            em.WithDescription($"{ctx.User.Mention} slaps {user.Mention} ÒwÓ");


            DiscordWebhookBuilder builder = new();
            builder.AddEmbed(em.Build());
            await ctx.EditResponseAsync(builder);
        }






    }
    

    
    

    public class CharacterCommandHandler : BaseCommandModule
    {
        
        //[Command("move"), Description("Move in a given Direction")]
        //public  async Task MoveCommand(CommandContext ctx, [RemainingText] string direction)
        //{

        //   // Player player = CharacterSystem.GetPlayer(ctx.Member.Id);
        //    Player player = Player.GetPlayerFromDataBase(ctx.Member.Id);
        //    Console.WriteLine(player.name +"was gotten from database");

        //    bool moved = false;

        //    switch (direction)
        //    {



        //        case "north":

        //            if (player.worldPosition.Y < DnDBot.worldSpace.worldMaxYValue)
        //            {
        //                moved = true;
        //               // await player.MoveNorth();
        //                await player.UpdatePlayerPosition(player.playerID, new Vector2(0, 1));
        //                await ctx.Member.SendMessageAsync($" {ctx.Member.Mention} you moved North!");
        //                ctx.Client.Logger.LogInformation($"{player.name} moved {direction} To {player.worldPosition.X}, {player.worldPosition.Y}");
        //                break;

        //            }
        //            else
        //            {
        //                await ctx.Member.SendMessageAsync($" {ctx.Member.Mention} You can't move North!");
        //                return;
        //            }

        //        case "south":

        //            if (player.worldPosition.Y >= 2)
        //            {
        //                moved = true;
        //                //await player.MoveSouth();
        //                await player.UpdatePlayerPosition(player.playerID, new Vector2(0, -1));
        //                ctx.Client.Logger.LogInformation($"{player.name} moved {direction} To {player.worldPosition.X}, {player.worldPosition.Y}");
        //                await ctx.Member.SendMessageAsync($" {ctx.Member.Mention} you moved South!");
        //                break;
        //            }
        //            else
        //            {
        //                await ctx.Member.SendMessageAsync($" {ctx.Member.Mention} You can't move South!");
        //                return;
        //            }

        //        case "east":

        //            if (player.worldPosition.X < DnDBot.worldSpace.worldMaxXValue)
        //            {
        //                moved = true;
        //                //await player.MoveEast();
        //                await player.UpdatePlayerPosition(player.playerID, new Vector2(1, 0));
        //                ctx.Client.Logger.LogInformation($"{player.name} moved {direction} To {player.worldPosition.X}, {player.worldPosition.Y}");
        //                await ctx.Member.SendMessageAsync($" {ctx.Member.Mention} you moved East!");
        //                break;
        //            }
        //            else
        //            {
        //                await ctx.Member.SendMessageAsync($" {ctx.Member.Mention} You can't move East!");
        //                return;
        //            }

        //        case "west":

        //            if (player.worldPosition.X >= 2)
        //            {
        //                moved = true;
        //                //await player.MoveWest();
        //                await player.UpdatePlayerPosition(player.playerID, new Vector2(-1, 0));
        //                ctx.Client.Logger.LogInformation($"{player.name} moved {direction} To {player.worldPosition.X}, {player.worldPosition.Y}");
        //                await ctx.Member.SendMessageAsync($" {ctx.Member.Mention} you moved West!");
        //                break;

        //            }
        //            else
        //            {
        //                await ctx.Member.SendMessageAsync($" {ctx.Member.Mention} You can't move West!");
        //                return;
        //            }

        //    }

        //}


        [Command("pingnext"), Description("Ping Command Next")]
        public async Task pingNextCommand(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync($"Pong Next! {ctx.Client.Ping}ms");
        }
       

    }



}




