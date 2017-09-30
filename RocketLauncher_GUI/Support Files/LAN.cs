using Newtonsoft.Json;
using PcapDotNet.Core;
using PcapDotNet.Packets;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace RocketLauncher_GUI.Support_Files
{
    public class Request
    {
        public Filter Filter;
        public int BuildID;
        public bool bHost;
        public string Nonce;
    }

    public class Response
    {
        public Result Result;
        public string ServerID;
        public string MetaData;
        public string Nonce;
    }

    public class Filter
    {
        public string GameTags;
        public string MapName;
        public int GameMode;
        public int MaxPlayerCount;
        public string ServerName;
        public string Password;
        public bool bPublic;
        public Teamsetting[] TeamSettings;
    }

    public class Teamsetting
    {
        public string Name = "";
        public Colors Colors;
    }

    public class Colors
    {
        public int TeamColorID = 0;
        public int CustomColorID = 0;
        public bool bTeamColorSet = false;
        public bool bCustomColorSet = false;
    }

    public class Result
    {
        public string Address;
        public string ServerName;
        public Settings Settings;
    }

    public class Settings
    {
        public string GameTags;
        public string MapName;
        public int GameMode;
        public int MaxPlayerCount;
        public string ServerName;
        public string Password;
        public bool bPublic;
        public Teamsetting[] TeamSettings;
    }


    public class MetaData
    {
        public string OwnerID;
        public string OwnerName;
        public string ServerName;
        public string ServerMap;
        public int ServerGameMode;
        public bool bPassword;
        public int NumPlayers;
        public int MaxPlayers;
    }

    public class LAN
    {
        // response reference from whynotsteven https://hastebin.com/anezitemur.css

        // List of servers to return
        public static List<String> serverList = new List<string>()
        {
            "184.91.128.16:7777"
        };

        /*
         * Intercept all packets going through port 14001
         * DeviceID: id of device from LivePacketDevice.AllLocalMachine
         */
        public static void Intercept(int DeviceID)
        {
            var device = LivePacketDevice.AllLocalMachine[DeviceID];

            using (PacketCommunicator communicator = device.Open(65536, PacketDeviceOpenAttributes.Promiscuous, 1000))
            {
                communicator.ReceivePackets(int.MaxValue, PacketHandler);
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
                // Convert data to useable object
                string json = str.Split(new string[] { "HostQuery_X" }, StringSplitOptions.None)[1];
                Request request = JsonConvert.DeserializeObject<Request>(json);

                // For every server we want to show we will send a response packet back
                foreach (string ServerID in serverList)
                {
                    Response response = craftResponse(ServerID, request);
                    SendResponse(response);
                }
            }
        }

        private static void SendResponse(Response response)
        {
            Console.WriteLine("sending response");

            // Packet header, format = "*something*" + "ProjectX.LanMessage_HostQuery_X" + "*something*" + json
            var bytes = "\x00\x00\x00\x22\x50\x72\x6f\x6a\x65\x63\x74\x58\x2e\x4c\x61\x6e\x4d\x65\x73\x73\x61\x67\x65\x5f\x48\x6f\x73\x74\x52\x65\x73\x70\x6f\x6e\x73\x65\x5f\x58\x00";

            // Send the packet through UDP
            IPEndPoint RemoteEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 14001);
            var Client = new UdpClient();
            var RequestData = Encoding.ASCII.GetBytes(bytes + JsonConvert.SerializeObject(response));
            var ServerEp = new IPEndPoint(IPAddress.Any, 14001);
            Client.EnableBroadcast = true;
            Client.Send(RequestData, RequestData.Length, RemoteEndPoint);
            Client.Close();
        }

        private static Response craftResponse(string ServerID, Request request)
        {
            Response response = new Response()
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
                ServerID = ServerID,
                MetaData = JsonConvert.SerializeObject(new MetaData()
                {
                    OwnerID = "Steam|76561198033133742|0",
                    OwnerName = $"/r/RocketLeagueMods",
                    ServerName = $"RocketLeagueMods {ServerID}",
                    ServerMap = "Labs_Underpass_P",
                    ServerGameMode = 0,
                    bPassword = false,
                    NumPlayers = 1,
                    MaxPlayers = 10
                }),
                Nonce = request.Nonce
            };

            return response;
        }
    }
}
