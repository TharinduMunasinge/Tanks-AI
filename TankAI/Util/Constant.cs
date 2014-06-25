using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace TankAI.Util
{
    /// <summary>
    /// Defined Constants of the Game
    /// </summary>
    public class Constant
    {
        public const byte defualtCost = 7;
        public const byte defualtEnemyCost = 225;
        public const byte defultCoinCost=1;
        public const byte defaultLPCost=3;
        public const byte defaultBrickCost=200;
        public const byte defaultWaterCost = 0;//don't use
        public const byte defaultStoneCost = 0;//don't use

        #region "C2S - Client To Server"
        public const string C2S_INITIALREQUEST = "JOIN#";
        public const string UP = "UP#";
        public const string DOWN = "DOWN#";
        public const string LEFT = "LEFT#";
        public const string RIGHT = "RIGHT#";
        public const string SHOOT = "SHOOT#";
        #endregion

        #region "S2C - Server To Client"
        public const string S2C_DEL = "#";

        public const string S2C_GAMESTARTED = "GAME_ALREADY_STARTED";
        public const string S2C_NOTSTARTED = "GAME_NOT_STARTED_YET";
        public const string S2C_GAMEOVER = "GAME_HAS_FINISHED";
        public const string S2C_GAMEJUSTFINISHED = "GAME_FINISHED";

        public const string S2C_CONTESTANTSFULL = "PLAYERS_FULL";
        public const string S2C_ALREADYADDED = "ALREADY_ADDED";

        public const string S2C_INVALIDCELL = "INVALID_CELL";
        public const string S2C_NOTACONTESTANT = "NOT_A_VALID_CONTESTANT";
        public const string S2C_TOOEARLY = "TOO_QUICK";
        public const string S2C_CELLOCCUPIED = "CELL_OCCUPIED";
        public const string S2C_HITONOBSTACLE = "OBSTACLE";//Penalty should be added.
        public const string S2C_FALLENTOPIT = "PITFALL";
       
        public const string S2C_NOTALIVE = "DEAD";

        public const string S2C_REQUESTERROR = "REQUEST_ERROR";
        public const string S2C_SERVERERROR = "SERVER_ERROR";
        #endregion

        #region "Communicatioin Configurations"

        public static string SERVICE = "TCP";
        public static bool GUIOFF = true;
        public static string SERVER_IP = ConfigurationManager.AppSettings.Get("ServerIP");
        public static int SERVER_PORT = int.Parse(ConfigurationManager.AppSettings.Get("ServerPort"));
        public static int CLIENT_PORT = int.Parse(ConfigurationManager.AppSettings.Get("ClientPort"));
        public static int MAP_SIZE = int.Parse(ConfigurationManager.AppSettings.Get("MapSize"));
        public static string GUI_IP = ConfigurationManager.AppSettings.Get("GuiIP");
        public static int GUI_PORT = int.Parse(ConfigurationManager.AppSettings.Get("GuiPort"));
        
        #endregion
    
    }
}
