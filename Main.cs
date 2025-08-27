using Discord;
using Discord.WebSocket;

namespace Frogger
{
    
    internal class Bot
    {
        public static DiscordSocketClient _client;

        public static async Task Main(string[] args)
        {
            _client = new DiscordSocketClient(
                new DiscordSocketConfig
                {
                    GatewayIntents =
                        GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
                }
            );
            Config.ParseJson();
            await _client.LoginAsync(TokenType.Bot, Config._config.Token);
            await _client.StartAsync();
            await Startup.Boot();
            _client.Log += Log;
            _client.ButtonExecuted += Interactions.HandleButtonSelection;
            _client.SelectMenuExecuted += Interactions.HandleDropdownSelection;
            _client.MessageReceived += Interactions.HandleWatchMessage;
            Frog();
            await Task.Delay(-1);
        }

        public static Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        public static void Frog()
        {
            Console.WriteLine("Frogger Online!");
            Console.WriteLine("            .--._.--.           ");
            Console.WriteLine("           ( O     O )          ");
            Console.WriteLine("           /   . .   \\          ");
            Console.WriteLine("          .`._______.'.         ");
            Console.WriteLine("         /(           )\\        ");
            Console.WriteLine("       _/  \\  \\   /  /  \\_      ");
            Console.WriteLine("    .~   `  \\  \\ /  /  '   ~.   ");
            Console.WriteLine("   {    -.   \\  V  /   .-    }  ");
            Console.WriteLine(" _ _`.    \\  |  |  |  /    .'_ _");
            Console.WriteLine(" >_       _} |  |  | {_       _<");
            Console.WriteLine("  /. - ~ ,_-'  .^.  `-_, ~ - .\\ ");
            Console.WriteLine("          '-'|/   \\|`-`       ");
        }
    }
}
