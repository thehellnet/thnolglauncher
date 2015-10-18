using System.Collections.Generic;

namespace ThnOlgLauncher.model
{
    class DataStore
    {
        public List<Game> games { get; set; }
        public List<Server> servers { get; set; }

        public DataStore() {
            games = new List<Game>();
            servers = new List<Server>();
        }
    }
}
