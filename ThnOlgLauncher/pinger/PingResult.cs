using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThnOlgLauncher.pinger {
    public class PingResult {
        public int ping { get; set; }
        public int players { get; set; }

        public PingResult() {
            this.ping = 0;
            this.players = 0;
        }
    }
}
