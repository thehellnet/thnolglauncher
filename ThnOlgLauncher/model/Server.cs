using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThnOlgLauncher.model
{
    class Server {

        public String name { get; set; }
        public String gameTag { get; set; }
        public String address { get; set; }
        public UInt16 port { get; set; }
        public int ping { get; set; }
    }
}
