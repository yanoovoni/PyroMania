using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server {
    class GameManager {
        bool continueRunning;

        public void Run(string[] args) {
            continueRunning = true;
            Settings S = Settings.Instance;
            for (int i = 0; i < args.Length; i += 2) {
                if (args[i].StartsWith("-") && S.ContainsTempSettingKey(args[i].Substring(1))) {
                    S.SetTempSetting(args[i].Substring(1), args[i + 1]);
                } else {
                    Printer.Print("Bad argument: " + args[i]);
                }
            }
            MapManager MM = MapManager.Instance;
            MM.LoadMap(S.GetTempSetting("map"));
            NetworkManager NM = NetworkManager.Instance;
            while (continueRunning) {
                string input = Console.ReadLine().ToLower();
                switch (input) {
                    case "stop":
                        Stop();
                        break;
                    default:
                        Printer.Print("dafaq?");
                        break;
                }
            }
        }

        public void Stop() {
            continueRunning = false;
        }
    }
}
