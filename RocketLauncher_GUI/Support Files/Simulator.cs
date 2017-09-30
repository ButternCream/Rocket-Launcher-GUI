using Newtonsoft.Json;
using PcapDotNet.Core;
using PcapDotNet.Packets;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace RocketLauncher_GUI.Support_Files
{
    public class Simulator
    {
        public static List<String> ServerList = new List<string>();
        public static List<PacketCommunicator> OpenCommunicators = new List<PacketCommunicator>();
        public static List<Thread> OpenThreads = new List<Thread>();

        /*
         * Intercept all packets going through port 14001
         * DeviceID: id of device from LivePacketDevice.AllLocalMachine
         */
        public static void Intercept(List<string> serverList)
        {
            // Set new server list
            ServerList = serverList;

            // Close old communicators and threads
            foreach (var openCommunicator in OpenCommunicators)
                openCommunicator.Dispose();
            foreach (var openThread in OpenThreads)
                openThread.Abort();

            // Start new intercepting communicators
            foreach (var device in LivePacketDevice.AllLocalMachine)
            {
                Thread thread = new Thread(InterceptDevice);
                thread.IsBackground = true;
                thread.Start(device);
                OpenThreads.Add(thread);
            }
        }

        private static void InterceptDevice(object data)
        {
            LivePacketDevice device = (LivePacketDevice)data;
            using (PacketCommunicator communicator = device.Open(65536, PacketDeviceOpenAttributes.Promiscuous, 1000))
            {
                using (BerkeleyPacketFilter filter = communicator.CreateFilter("udp port 14001"))
                {
                    communicator.SetFilter(filter);
                    communicator.ReceivePackets(int.MaxValue, PacketHandler);
                }
            }
        }

        /*
         * Handles intercepted packets
         */
        private static void PacketHandler(Packet packet)
        {
            byte[] data = new byte[packet.Count];
            packet.CopyTo(data, 0);
            string str = Encoding.Default.GetString(data);

            // If this is a hostquery message we will respond
            if (str.Contains("HostQuery_X"))
            {
                Console.WriteLine(str);
                // Convert data to useable object
                string json = str.Split(new string[] { "HostQuery_X" }, StringSplitOptions.None)[1];
                try
                {
                    HostQuery_X query = JsonConvert.DeserializeObject<HostQuery_X>(json);

                    // For every server we want to show we will send a response packet back
                    foreach (string ServerID in ServerList)
                    {
                        HostResponse_X response = CraftResponse(ServerID, query);
                        SendResponse(response);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        private static void SendResponse(HostResponse_X response)
        {
            Console.WriteLine("sending response");

            // Packet header, format = "*something*" + "ProjectX.LanMessage_HostQuery_X" + "*something*" + json
            var bytes = "\x00\x00\x00\x22\x50\x72\x6f\x6a\x65\x63\x74\x58\x2e\x4c\x61\x6e\x4d\x65\x73\x73\x61\x67\x65\x5f\x48\x6f\x73\x74\x52\x65\x73\x70\x6f\x6e\x73\x65\x5f\x58\x00";
            var RequestData = Encoding.ASCII.GetBytes(bytes + JsonConvert.SerializeObject(response));

            // Send the packet through UDP
            IPEndPoint RemoteEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 14001);
            var Client = new UdpClient();
            var ServerEp = new IPEndPoint(IPAddress.Any, 14001);
            Client.EnableBroadcast = true;
            Client.Send(RequestData, RequestData.Length, RemoteEndPoint);
            Client.Close();
        }

        private static HostResponse_X CraftResponse(string serverID, HostQuery_X query)
        {
            HostResponse_X response = new HostResponse_X()
            {
                Result = new Result
                {
                    Settings = new Settings()
                    {
                        GameTags = "",
                        MapName = "None",
                        GameMode = 0,
                        MaxPlayerCount = 0,
                        ServerName = "",
                        Password = "",
                        bPublic = false,
                        TeamSettings = new Teamsetting[]
                        {
                            new Teamsetting(),
                            new Teamsetting()
                        }
                    }
                },
                ServerID = serverID,
                MetaData = JsonConvert.SerializeObject(new MetaData()
                {
                    OwnerID = "Steam|76561198033133742|0",
                    OwnerName = $"/r/RocketLeagueMods",
                    ServerName = $"RocketLeagueMods {serverID}",
                    ServerMap = "Labs_Underpass_P",
                    ServerGameMode = 0,
                    bPassword = false,
                    NumPlayers = 1,
                    MaxPlayers = 10
                }),
                Nonce = query.Nonce
            };

            return response;
        }
    }
}
