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
        public static PingResult pingServer(Server server) {
            PingResult pingResult = new PingResult();

            UdpClient sck = new UdpClient();
            sck.Client.ReceiveTimeout = 500;
            try {
                sck.Connect(server.address, server.port);
            } catch(SocketException e) {
                Console.WriteLine(e.StackTrace);
                return pingResult;
            }

            Byte[] sendBytes = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x67, 0x65, 0x74, 0x73, 0x74, 0x61, 0x74, 0x75, 0x73, 0x0A };

            DateTime timeStart = DateTime.Now;
            try {
                sck.Send(sendBytes, sendBytes.Length);
            } catch(Exception e) {
                Console.WriteLine(e.StackTrace);
                return pingResult;
            }

            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] recvData;
            try {
                recvData = sck.Receive(ref RemoteIpEndPoint);
            } catch(Exception e) {
                Console.WriteLine(e.StackTrace);
                return pingResult;
            }

            DateTime timeStop = DateTime.Now;
            string returnData = Encoding.ASCII.GetString(recvData);
            sck.Close();

            Console.WriteLine(returnData);

            TimeSpan timeSpan = timeStop - timeStart;
            pingResult.ping = (int) timeSpan.TotalMilliseconds;

            String[] rows = returnData.Split('\n');
            pingResult.players = rows.Length - 3;

            String[] serverConfig = rows[1].Split('\\');
            for(int i = 1; (i + 1) < serverConfig.Length; i += 2) {
                String param = serverConfig[i];
                String value = serverConfig[i + 1];
                switch(param) {
                    case "sv_maxclients":
                        Console.WriteLine("{0} {1}", param, value);
                        pingResult.maxPlayers = Int32.Parse(value);
                        break;
                    case "g_gametype":
                        pingResult.gametype = value;
                        break;
                    case "mapname":
                        pingResult.map = value;
                        break;
                }
            }

            return pingResult;
        }
    }
}
