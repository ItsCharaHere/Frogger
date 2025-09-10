using Discord;

namespace Frogger
{
    internal class Startup
    {
        public static async Task<bool> Boot() 
        {
            var MasterChannel = await Bot._client.GetChannelAsync(Config._config.MasterChannel);
            if (!Directory.Exists(".\\export")) 
            {
                Directory.CreateDirectory(".\\export");
            }
            if (MasterChannel != null && MasterChannel is IMessageChannel msgChannel)
            {
                var messages = msgChannel.GetMessagesAsync(2).FlattenAsync();
                if (messages.Result.Count() < 1)
                {
                    await msgChannel.SendMessageAsync(text: "Please Select from the dropdown what Kind of Ticket you would like to open!", components: Interactions.ConstructDropdown());
                }
            }
            return true;
        }
    }
}
