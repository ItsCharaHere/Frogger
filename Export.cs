using Discord;
using Discord.WebSocket;
using System.Text;

namespace Frogger
{
    internal class Export
    {
        public static async Task<FileAttachment> exportChat(SocketMessageComponent arg)
        {
            using (StreamWriter channelExport = File.AppendText($".\\export\\{arg.ChannelId}.txt"))
            {
                var guild = Bot._client.GetGuild(Config._config.Server);
                var channel = guild.GetTextChannel(arg.Channel.Id);
                var messageBatch = channel.GetMessagesAsync(limit: 500);
                await foreach (var messageGroup in messageBatch)
                {
                    if (messageGroup.Count == 0)
                    {
                        break;
                    }

                    foreach (var message in messageGroup)
                    {
                        channelExport.WriteLine($"{message.Author.Username} ({message.Author.Id}) {message.Timestamp}: {message.Content}");
                    }
                }
            }
            return new FileAttachment($".\\export\\{arg.ChannelId}.txt", $"{arg.ChannelId}.txt");
        }
    }
}
