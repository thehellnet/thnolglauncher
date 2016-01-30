using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ThnOlgLauncher.model;

namespace ThnOlgLauncher.pinger {
    class Pinger {
        public static int pingServer(Server server) {
            UdpClient sck = new UdpClient();
            sck.Client.ReceiveTimeout = 500;
            sck.Connect(server.address, server.port);

            Byte[] sendBytes = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x67, 0x65, 0x74, 0x73, 0x74, 0x61, 0x74, 0x75, 0x73, 0x0A };

            DateTime timeStart = DateTime.Now;
            sck.Send(sendBytes, sendBytes.Length);

            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] recvData;
            try {
                recvData = sck.Receive(ref RemoteIpEndPoint);
            } catch(Exception e) {
                Console.WriteLine(e.StackTrace);
                return 0;
            }

            DateTime timeStop = DateTime.Now;
            string returnData = Encoding.ASCII.GetString(recvData);
            sck.Close();

            TimeSpan timeSpan = timeStop - timeStart;
            return (int) timeSpan.TotalMilliseconds;
        }
    }
}
