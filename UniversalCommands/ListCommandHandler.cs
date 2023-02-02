using Microsoft.Extensions.Logging;

using DisCatSharp.Entities;

using DisCatSharp.CommandsNext;
using DisCatSharp.CommandsNext.Attributes;


using DisCatSharp.Interactivity.Extensions;


namespace DnDBot.Character.Commands
{
    public class GuildListItem
    {

        public string itemName = "";
        public string itemDescription = "";

    }
    public class GuildList
    {
        public string Name = "";
        public ulong guildId = 0;
        public string creator = "";
        public string timeStamp = "";

        public List<GuildListItem> Items = new List<GuildListItem>();
    }


    public class ListCommandHandler : BaseCommandModule
    {

        //---------------------------List Database----------------------------//
        public static Dictionary<ulong, List<GuildList>> listDatabase = new Dictionary<ulong, List<GuildList>>();



        //-------------------------Create List--------------------------------//
        [Command("list-create"), Aliases("list-c", "createlist"), Description("Create a list with a given Name")]
        public async Task CreateNewListCommand(CommandContext ctx, [RemainingText] string listname)
        {
            ulong guildId = ctx.Guild.Id;
            Console.WriteLine($"Guild id= {guildId}");
            foreach (KeyValuePair<ulong, List<GuildList>> guildlist in listDatabase)//loop through master list for list 
            {
                if (guildlist.Key == guildId)// Guildlist with found    
                {
                    for (int i = 0; i < guildlist.Value.Count; i++)
                    {
                        if (guildlist.Value[i].Name == listname)   //list with list name found
                        {


                            ctx.Client.Logger.LogInformation($" {ctx.User.Username} Tried to Create a list called {listname} in Guild {ctx.Guild.Name} but it already existed.");
                            await ctx.RespondAsync($"  {ctx.Member.Mention} List {listname} already Exists in your Servers lists.");
                            return;

                        }

                    }

                }
            }

            Console.WriteLine($"No List with name {listname} found");

            foreach (KeyValuePair<ulong, List<GuildList>> guildlist in listDatabase)//loop through Database for Guildlist 
            {

                if (guildlist.Key == guildId)// Guildlist with ID found    
                {

                    GuildList _newlist = new GuildList();
                    List<GuildListItem> _guildListItems = new List<GuildListItem>();
                    Console.WriteLine($"created new GuildList");
                    _newlist.Name = listname;
                    _newlist.guildId = guildId;
                    _newlist.Items = _guildListItems;
                    _newlist.creator = ctx.User.Username;
                    _newlist.timeStamp = DateTime.Now.ToString();
                    guildlist.Value.Add(_newlist);
                    Console.WriteLine($"new list created with name {_newlist.Name}");
                    ctx.Client.Logger.LogInformation($" {ctx.User.Username} Created a new list called {listname} in Guild {ctx.Guild.Name}.");
                    await ctx.RespondAsync($"  {ctx.Member.Mention} List {listname} created and added Successfully.");
                    return;
                }
            }
            List<GuildList> newGuildList = new List<GuildList>();
            List<GuildListItem> guildListItems = new List<GuildListItem>();
            GuildList newlist = new GuildList();
            Console.WriteLine($"created new GuildList");
            newlist.Name = listname;
            newlist.guildId = guildId;
            newlist.Items = guildListItems;
            newlist.creator = ctx.User.Username;
            newlist.timeStamp = DateTime.Now.ToString();
            newGuildList.Add(newlist);
            Console.WriteLine($"new list created with name {newlist.Name}");
            listDatabase.Add(guildId, newGuildList);
            ctx.Client.Logger.LogInformation($" {ctx.User.Username} Created a new list called {listname} in Guild {ctx.Guild.Name}.");
            await ctx.RespondAsync($"  {ctx.Member.Mention} A new Database entry and List {listname} was created Successfully.");
            return;
        }

        //-------------------------Delete List--------------------------------//

        [Command("list-delete"), Aliases("list-d", "deletelist"), Description("Deletes a list with a given Name")]
        public async Task DeleteListCommand(CommandContext ctx, [RemainingText] string listname)
        {
            ulong guildId = ctx.Guild.Id;
            foreach (KeyValuePair<ulong, List<GuildList>> list in listDatabase)//loop through master list for list 
            {

                if (list.Key == guildId)// list found    
                {
                    for (int i = 0; i < list.Value.Count; i++)
                    {
                        if (list.Value[i].Name == listname)
                        {
                            listDatabase.Remove(guildId);
                            ctx.Client.Logger.LogInformation($" {ctx.User.Username} Deleted a list called {listname} in Guild {ctx.Guild.Name}.");
                            await ctx.RespondAsync($"  {ctx.Member.Mention} List {listname} Deleted Successfully.");
                            return;
                        }
                    }
                }
            }

        }
        //------------------------Display-List--------------------------------//

        [Command("list-display"), Aliases("displaylist"), Description("Display a list with a given name")]
        public async Task DisplayListCommand(CommandContext ctx, [RemainingText] string listname)
        {
            ulong guildId = ctx.Guild.Id;


            foreach (KeyValuePair<ulong, List<GuildList>> list in listDatabase)//loop through master list for list 
            {

                if (list.Key == guildId)// list found    
                {
                    for (int i = 0; i < list.Value.Count; i++)
                    {
                        if (list.Value[i].Name == listname)
                        {
                            var embed = new DiscordEmbedBuilder
                            {
                                Color = DiscordColor.CornflowerBlue,
                                Title = $"{list.Value[i].Name}",
                                Description = $"Created by {list.Value[i].creator} ",
                            };

                            for (int r = 0; r < list.Value[i].Items.Count; r++)
                            {

                                embed.AddField(new DiscordEmbedField($"{list.Value[i].Items[r].itemName}", $"{list.Value[i].Items[r].itemDescription}", false));
                            }

                            embed.Build();
                            await ctx.Channel.SendMessageAsync(embed: embed);
                            return;

                        }
                    }
                    await ctx.Channel.SendMessageAsync("No List Found To Display");
                    return;
                }


            }
        }

        //------------------------List-Lists--------------------------------//

        [Command("list-list"), Aliases("listlist", "list-l"), Description("Displys all lists on server")]
        public async Task ListListsCommand(CommandContext ctx, [RemainingText] string listname)
        {
            ulong guildId = ctx.Guild.Id;


            foreach (KeyValuePair<ulong, List<GuildList>> list in listDatabase)//loop through master list for list 
            {

                if (list.Key == guildId)// list found    
                {
                    var embed = new DiscordEmbedBuilder
                    {
                        Color = DiscordColor.CornflowerBlue,
                        Title = $"{ctx.Guild.Name}",
                        Description = "Master List",
                    };
                    for (int i = 0; i < list.Value.Count; i++)
                    {
                        embed.AddField(new DiscordEmbedField($"{list.Value[i].Name}", $"Created by {list.Value[i].creator} on {list.Value[i].timeStamp}", false));


                        embed.Build();
                        await ctx.Channel.SendMessageAsync(embed: embed);
                        return;


                    }
                    await ctx.Channel.SendMessageAsync("No List Found To Display");
                    return;
                }


            }
        }


        //-------------------------Add Item--------------------------------//

        [Command("item-add"), Aliases("item-a", "additem"), Description("Addes an item to a list with a given Name, item name and description must be seperated by a | ")]
        public async Task AddItemToListCommand(CommandContext ctx, [RemainingText] string mixedString)
        {
            ulong guildId = ctx.Guild.Id;
            ctx.Client.Logger.LogInformation($"mixedString = {mixedString}.");
            if (mixedString.Contains(","))
            {
                char[] delimiterChars = { ',' };
                string[] strArray = mixedString.Split(delimiterChars);
                string listname = strArray[0].ToString();
                ctx.Client.Logger.LogInformation($"listname = {listname}.");
                string itemname = strArray[1].ToString();
                ctx.Client.Logger.LogInformation($"itemname = {itemname}.");
                string itemdescription = strArray[2].ToString();
                ctx.Client.Logger.LogInformation($"itemdescription = {itemdescription}.");
                GuildListItem item = new GuildListItem();
                Console.WriteLine($"Guild item Created");
                item.itemName = itemname.ToString();
                Console.WriteLine($"itemname set");
                item.itemDescription = itemdescription.ToString();
                Console.WriteLine($"Item Description set");
                ctx.Client.Logger.LogInformation($"New List Item Created.");
                ctx.Client.Logger.LogInformation($"looping through masterlist for list {listname}.");
                foreach (KeyValuePair<ulong, List<GuildList>> list in listDatabase)//loop through master list for list 
                {

                    if (list.Key == guildId)// list found    
                    {

                        for (int i = 0; i < list.Value.Count; i++)
                        {
                            if (list.Value[i].Name == listname)
                            {
                                ctx.Client.Logger.LogInformation($"list found.");
                                list.Value[i].Items.Add(item);
                                ctx.Client.Logger.LogInformation($" {ctx.User.Username} Added item {itemname}to list {listname} in Guild {ctx.Guild.Name}.");
                                await ctx.RespondAsync($"  {ctx.Member.Mention} Added {itemname} to  {listname} with a description of {itemdescription}.");

                                return;
                            }
                        }
                    }
                }

            }
            else
            {
                ctx.Client.Logger.LogInformation($" {ctx.User.Username} failed to add item to list  in Guild {ctx.Guild.Name} reason improper format missing *.");
                await ctx.RespondAsync("Improper format used  use /additem list name*item name*item description");

            }
        }

        //-------------------------Remove Item--------------------------------//

        [Command("item-delete"), Aliases("item-d", "delitem"), Description("Deletes an item from a list with a given list Name and ItemName")]
        public async Task DeleteItemFromListCommand(CommandContext ctx, [RemainingText] string mixedString)
        {
            ulong guildId = ctx.Guild.Id;
            if (mixedString.Contains("*"))
            {
                if (mixedString.Length >= 2 && mixedString.Length < 4)
                {
                    var listname = mixedString[0];
                    var itemname = mixedString[1];
                    var itemdescription = mixedString[2];

                    foreach (KeyValuePair<ulong, List<GuildList>> list in listDatabase)//loop through master list for list 
                    {

                        if (list.Key == guildId)// list found    
                        {

                            for (int i = 0; i < list.Value.Count; i++)
                            {
                                if (list.Value[i].Name == listname.ToString())
                                {
                                    var item = new GuildListItem() { itemName = itemname.ToString() };
                                    for (int r = 0; r < list.Value[i].Items.Count; r++)
                                    {
                                        if (list.Value[i].Items[r].itemName == itemname.ToString())
                                        {
                                            list.Value[i].Items.Remove(list.Value[i].Items[r]);
                                            ctx.Client.Logger.LogInformation($" {ctx.User.Username} Deleted a list called {listname} in Guild {ctx.Guild.Name}.");
                                            await ctx.RespondAsync($"  {ctx.Member.Mention} List {listname} Deleted Successfully.");
                                            return;
                                        }
                                    }

                                }
                            }
                        }
                    }
                }
                else
                {
                    ctx.Client.Logger.LogInformation($" {ctx.User.Username} failed to add item to list  in Guild {ctx.Guild.Name} reason improper format.");
                    await ctx.RespondAsync("Improper format used  use /additem list name*item name*item description");
                }
            }
            else
            {
                ctx.Client.Logger.LogInformation($" {ctx.User.Username} failed to add item to list  in Guild {ctx.Guild.Name} reason improper format missing *.");
                await ctx.RespondAsync("Improper format used  use /additem list name*item name*item description");
            }
        }

    }
}















