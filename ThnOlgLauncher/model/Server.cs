using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThnOlgLauncher.model
{
    class Server
    {
        public String name { get; set; }
        public Game game { get; set; }
        public String address { get; set; }
        public UInt16 port { get; set; }
    }
}
