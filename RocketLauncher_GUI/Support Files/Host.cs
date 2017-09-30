using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketLauncher_GUI.Support_Files
{
    public class HostQuery_X
    {
        public Filter Filter;
        public int BuildID;
        public bool bHost;
        public string Nonce;
    }

    public class HostResponse_X
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
        public int TeamColorID;
        public int CustomColorID;
        public bool bTeamColorSet;
        public bool bCustomColorSet;
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
}
