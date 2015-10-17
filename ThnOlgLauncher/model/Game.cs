using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThnOlgLauncher.model
{
    class Game
    {
        public String name { get; set; }
        public String tag { get; set; }
        public String executable { get; set; }
        public bool runAsAdmin { get; set; }
    }
}
