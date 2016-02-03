using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace ThnOlgLauncher.model
{
    class Server {

        public String name { get; set; }
        public String gameTag { get; set; }
        public String address { get; set; }
        public UInt16 port { get; set; }

        [ScriptIgnore]
        public int ping { get; set; }

        [ScriptIgnore]
        public String players { get; set; }

        [ScriptIgnore]
        public String map { get; set; }
    }
}
