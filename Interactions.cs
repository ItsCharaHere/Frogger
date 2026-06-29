using Discord;
using Discord.WebSocket;
namespace Frogger
{
    internal class Interactions
    {
        public static MessageComponent ConstructButtons()
        {
            ComponentBuilder builder = new ComponentBuilder();
            builder.WithButton("Claim Ticket", "claim", ButtonStyle.Success);
            builder.WithButton("Close Ticket", "close", ButtonStyle.Danger);
            return builder.Build();
        }
        public static MessageComponent ConstructDropdown()
        {
            ComponentBuilder builder = new ComponentBuilder();
            SelectMenuBuilder menuBuilder = new SelectMenuBuilder();
            menuBuilder
                .WithPlaceholder("Select a Ticket Type to Open!")
                .WithCustomId("ticketmenu");
            foreach (string Option in Config._config.Categories)
            {
                menuBuilder.AddOption(Option, Option, "Open a Ticket for " +Option);
                
            }
            builder.WithSelectMenu(menuBuilder);
            return builder.Build();
        }

        public static async Task<Task> HandleDropdownSelection(SocketMessageComponent arg)
        {
            var ticketNum = Sqlite.GetRowCount();
            var guild = Bot._client.GetGuild(Config._config.Server);

            List<Overwrite> permissions = new List<Overwrite>
            {
                new Overwrite(guild.EveryoneRole.Id, PermissionTarget.Role,
                    new OverwritePermissions(viewChannel: PermValue.Deny)),
                new Overwrite(Config._config.ManagementRole, PermissionTarget.Role,
                    new OverwritePermissions(viewChannel: PermValue.Allow)),
                new Overwrite(arg.User.Id, PermissionTarget.User,
                    new OverwritePermissions(viewChannel: PermValue.Allow))
            };

            var ticketChannel = await guild.CreateTextChannelAsync("ticket-" + arg.Data.Values.First()+ '-' + ticketNum, options =>
            {
                options.CategoryId = Config._config.UnclaimedTicketCategory;
                options.PermissionOverwrites = permissions;
                options.Topic = $"{arg.Data.Values.First()}||{arg.User.Id}";
            }
            );
            Sqlite.CreateRow(ticketChannel.Id.ToString(), arg.User.Id.ToString());
            await ticketChannel.SendMessageAsync(text: $"Hello <@{arg.User.Id}>! Please describe your problem here! \n <@&{Config._config.ManagementRole}>", components: ConstructButtons());
            await arg.RespondAsync("Done!", ephemeral: true);
            return Task.CompletedTask;
        }

        public static async Task<Task> HandleButtonSelection(SocketMessageComponent arg) {
            var guild = Bot._client.GetGuild(Config._config.Server);
            var role = guild.GetRole(Config._config.ManagementRole);
            var channel = guild.GetTextChannel(arg.Channel.Id);
            var archiveChannel = guild.GetTextChannel(Config._config.ArchiveChannel);

            switch (arg.Data.CustomId)
            {
                case "claim":
                    {
                        if (arg.User is SocketGuildUser user)
                        {
                            if (user.Roles.Contains(role))
                            {
                                await channel.ModifyAsync(x =>
                                {
                                    x.CategoryId = Config._config.ClaimedTicketCategory;
                                });
                                await channel.SendMessageAsync($"<@{arg.User.Id}> Has Claimed this ticket!");
                                await arg.RespondAsync("Done", ephemeral: true);
                            }
                            await arg.RespondAsync("Failed To claim ticket.", ephemeral: true);
                        }
                    }
                    break;
                case "close": 
                    {
                        Sqlite.TicketRow row = Sqlite.RetrieveRowFromChannelId((int)channel.Id);
                        FileAttachment exportPath = await Export.exportChat(arg);
                        if (row.CreatorId != null)
                        {
                            SocketUser ticketAuthor = Bot._client.GetUser((ulong)row.CreatorId);
                            if (ticketAuthor != null)
                            {
                                var tickerAuthorDM = await ticketAuthor.CreateDMChannelAsync();
                                await tickerAuthorDM.SendFileAsync(exportPath, text: "Here is a chat export of your ticket!");
                            }
                        }
                        await archiveChannel.SendFileAsync(exportPath, text: $"Ticket Closed, Message logs avaliable via attached file.");
                        await arg.RespondAsync("Done", ephemeral: true);
                        await channel.DeleteAsync();
                    }
                    break;
                default: break; 
            }
                return Task.CompletedTask;
        }

        public static async Task<Task> HandleWatchMessage(SocketMessage arg)
        {
            if (Config._config.WatchChannels.Contains(arg.Channel.Id))
            {
                var ticketNum = Sqlite.GetRowCount();
                var guild = Bot._client.GetGuild(Config._config.Server);
                

                List<Overwrite> permissions = new List<Overwrite>
                {
                  new Overwrite(guild.EveryoneRole.Id, PermissionTarget.Role,
                      new OverwritePermissions(viewChannel: PermValue.Deny)),
                  new Overwrite(Config._config.ManagementRole, PermissionTarget.Role,
                      new OverwritePermissions(viewChannel: PermValue.Allow)),
                };
                
                var ticketChannel = await guild.CreateTextChannelAsync("ticket-text-message-" + ticketNum, options =>
                    {
                        options.CategoryId = Config._config.UnclaimedTicketCategory;
                        options.PermissionOverwrites = permissions;
                    }
                );
                Sqlite.CreateRow(ticketChannel.Id.ToString(), null);
                await ticketChannel.SendMessageAsync(text: $"<@&{Config._config.ManagementRole}> Watched Channel Ticket: \n {arg.Content}", components: ConstructButtons());
            }
            return Task.CompletedTask;
        }
    }
}
