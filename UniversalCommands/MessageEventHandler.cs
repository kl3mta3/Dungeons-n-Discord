using DisCatSharp;
using DisCatSharp.Entities;
using DisCatSharp.EventArgs;


namespace DnDBot.Character.Commands
{
    [EventHandler]
    public class MessageEventHandler
    {


        [Event]
        public Task MessageCreated(DiscordClient s, MessageCreateEventArgs e)
        {

            var message = e.Message;

            bool serverExists = false;
            if (message.Author.IsBot)
            {
                return Task.CompletedTask;
            }
            if (!e.Channel.IsPrivate)
            {
                 
                serverExists = true;
            }


            if (message.Content.StartsWith("/"))
            {
                var embed = new DiscordEmbedBuilder
                {
                    Color = DiscordColor.Yellow,
                    Title = "Command Recieved",
                };
                if (serverExists)
                {
                    var server = e.Guild;
                    embed.AddField(new DiscordEmbedField("Server", server.Name, true));
                embed.AddField(new DiscordEmbedField("Channel", message.Channel.Name, false));
                }
                embed.AddField(new DiscordEmbedField("Created On", message.CreationTimestamp.ToString(), true));
                embed.AddField(new DiscordEmbedField("Requested By", message.Author.Username, false));
                embed.AddField(new DiscordEmbedField("Command", message.Content, false));

        
                    DnDBot.commandChannel.SendMessageAsync(embed: embed);
               
            }
            else
            {
                var embed = new DiscordEmbedBuilder
                {
                    Color = DiscordColor.SpringGreen,
                    Title = "Message Recieved",
                };

                if (serverExists)
                {
                    var server = e.Guild;
                    embed.AddField(new DiscordEmbedField("Server", server.Name, true));
                    embed.AddField(new DiscordEmbedField("Channel", message.Channel.Name, false));
                }
                embed.AddField(new DiscordEmbedField("Created On", message.CreationTimestamp.ToString(), true));
                embed.AddField(new DiscordEmbedField("Posted By", message.Author.Username, false));
                embed.AddField(new DiscordEmbedField("Message", message.Content, false));
                DnDBot.messageLogChannel.SendMessageAsync(embed: embed);


            }


            return Task.CompletedTask;
        }


        [Event]
        public Task MessageDeleted(DiscordClient s, MessageDeleteEventArgs e)
        {
            var message = e.Message;
            var author = e.Message.Author;
            bool serverExists = false;
            if (!e.Channel.IsPrivate)
            {
                serverExists = true;
            }
            var embed = new DiscordEmbedBuilder
            {
                Color = DiscordColor.DarkRed,
                Title = "Message Deleted",
            };

            string deletedtimeStamp = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");

            if (serverExists)
            {

                var server = e.Guild;
                embed.AddField(new DiscordEmbedField("Server", server.Name, true));
                embed.AddField(new DiscordEmbedField("Channel", message.Channel.Name, false));
            }
            embed.AddField(new DiscordEmbedField("Created On", message.CreationTimestamp.ToString(), true));
            embed.AddField(new DiscordEmbedField("Deleted On", deletedtimeStamp, true));
            embed.AddField(new DiscordEmbedField("OP User", message.Author.Username, false));
            embed.AddField(new DiscordEmbedField("Message", message.Content, false));


            DnDBot.messagesDeleted.SendMessageAsync(embed: embed);


            return Task.CompletedTask;
        }


        [Event(DiscordEvent.GuildMemberAdded)]
        public static async Task UserJoined(DiscordClient discord, GuildMemberAddEventArgs e)
        {
            var user = e.Member;
            var server = e.Guild;
            var channel = e.Guild.Channels.FirstOrDefault(x => x.Value.Name == "general").Value;

            await channel.SendMessageAsync("Welcome to the server " + user.Username + "!");
            await user.SendMessageAsync("Welcome to the server " + user.Username + "!\n" + "Please read the rules and have fun!");

        }



        [Event(DiscordEvent.GuildMemberRemoved)]
        public static async Task UserLeft(DiscordClient discord, GuildMemberRemoveEventArgs e)
        {

            var user = e.Member;
            var server = e.Guild;
            var channel = e.Guild.Channels.FirstOrDefault(x => x.Value.Name == "general").Value;
            await channel.SendMessageAsync("Bye " + user.Username + "!");
            // await user.SendMessageAsync("Bye  Felicia");

        }
        //[Event]
        //public async Task PingEvent(DiscordClient s, MessageDeleteEventArgs e)
        //{
        //    if (e.Message.Content.StartsWith("/") && e.Message.Content.Contains("pingEvent"))

        //    {
        //        await e.Channel.SendMessageAsync($"Pong Next! {s.Ping}ms");
        //    }

          
        //}


        
    }
}
