namespace ThnOlgLauncher.pinger
{
    public class PingResult
    {
        public int Ping { get; set; }
        public int Players { get; set; }
        public int MaxPlayers { get; set; }
        public string Gametype { get; set; }
        public string Map { get; set; }

        public PingResult()
        {
            Ping = 0;
            Players = 0;
            MaxPlayers = 0;
            Gametype = "";
            Map = "";
        }
    }
}