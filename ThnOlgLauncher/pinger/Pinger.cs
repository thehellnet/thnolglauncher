using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ThnOlgLauncher.model;

namespace ThnOlgLauncher.pinger
{
    internal class Pinger
    {
        public static PingResult PingServer(Server server)
        {
            var pingResult = new PingResult();

            var sck = new UdpClient {Client = {ReceiveTimeout = 500}};
            try
            {
                sck.Connect(server.Address, server.Port);
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.StackTrace);
                return pingResult;
            }

            var sendBytes = new byte[]
                {0xFF, 0xFF, 0xFF, 0xFF, 0x67, 0x65, 0x74, 0x73, 0x74, 0x61, 0x74, 0x75, 0x73, 0x0A};

            var timeStart = DateTime.Now;
            try
            {
                sck.Send(sendBytes, sendBytes.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return pingResult;
            }

            var remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] recvData;
            try
            {
                recvData = sck.Receive(ref remoteIpEndPoint);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return pingResult;
            }

            var timeStop = DateTime.Now;
            var returnData = Encoding.ASCII.GetString(recvData);
            sck.Close();

            Console.WriteLine(returnData);

            var timeSpan = timeStop - timeStart;
            pingResult.Ping = (int) timeSpan.TotalMilliseconds;

            var rows = returnData.Split('\n');
            pingResult.Players = rows.Length - 3;

            var serverConfig = rows[1].Split('\\');
            for (var i = 1; (i + 1) < serverConfig.Length; i += 2)
            {
                var param = serverConfig[i];
                var value = serverConfig[i + 1];
                switch (param)
                {
                    case @"sv_maxclients":
                        Console.WriteLine(@"{0} {1}", param, value);
                        pingResult.MaxPlayers = int.Parse(value);
                        break;
                    case @"g_gametype":
                        pingResult.Gametype = value;
                        break;
                    case @"mapname":
                        pingResult.Map = value;
                        break;
                    default:
                        continue;
                }
            }

            return pingResult;
        }
    }
}