using System.Web.Script.Serialization;

namespace ThnOlgLauncher.model
{
    internal class Server
    {
        public string Name { get; set; }
        public string GameTag { get; set; }
        public string Address { get; set; }
        public ushort Port { get; set; }

        [ScriptIgnore] public int Ping { get; set; }

        [ScriptIgnore] public string Players { get; set; }

        [ScriptIgnore] public string Map { get; set; }
    }
}