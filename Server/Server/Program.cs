using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server {
    class Program {
        static void Main(string[] args) {
            Settings S = Settings.Instance;
            for (int i = 0; i < args.Length; i += 2) {
                if (args[i].StartsWith("-") && S.ContainsTempSettingKey(args[i].Substring(1))) {
                    S.SetTempSetting(args[i].Substring(1), args[i + 1]);
                } else {
                    Printer.Print("Bad argument: " + args[i]);
                }
            }
            MapManager MM = MapManager.Instance;
            NetworkManager NM = NetworkManager.Instance;
        }
    }
}
