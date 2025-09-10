using Newtonsoft.Json;

namespace Frogger
{
    internal class Config
    {
        public static Config _config;
        public string Token { get; set; }
        public ulong Server { get; set; }
        public ulong ManagementRole { get; set; }
        public ulong MasterChannel { get; set; }
        public ulong[] WatchChannels { get; set; }
        public ulong ArchiveChannel { get; set; }
        public ulong UnclaimedTicketCategory { get; set; }
        public ulong ClaimedTicketCategory { get; set; }
        public ulong ClosedTicketCategory { get; set; }
        public string[] Categories { get; set; }

        

        public static void ParseJson()
        {
            try
            {
                string jsonString = File.ReadAllText("config.json");

                Config data = JsonConvert.DeserializeObject<Config>(jsonString);
                _config = data;
            }
            catch (Exception ex)
            {
                Console.WriteLine("File \"config.json\" not found! \n" + ex);
            }

        }
    }

}
