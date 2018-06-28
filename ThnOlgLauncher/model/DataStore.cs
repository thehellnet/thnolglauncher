using System.Collections.Generic;

namespace ThnOlgLauncher.model
{
    internal class DataStore
    {
        public List<Game> Games { get; set; }
        public List<Server> Servers { get; set; }

        public DataStore()
        {
            Games = new List<Game>();
            Servers = new List<Server>();
        }
    }
}