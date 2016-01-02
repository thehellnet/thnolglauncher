using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ThnOlgLauncher.pinger {
    class Pinger {

        public String address { get; set; }
        public int port { get; set; }

        private UdpClient sck = new UdpClient();

        public void setAddressPort(String text) {
            String[] elems = text.Trim().Split(',');
            if(elems.Count() != 2) {
                return;
            }
            address = elems[0];
            port = Int16.Parse(elems[1]);
        }

        public void start() {
            sck.Connect(address, port);
        }

        public void stop() {

        }
    }
}
