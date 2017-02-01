using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server {
    class NetworkManager {
        protected static NetworkManager instance; // The instance of the singleton


        protected NetworkManager() { }

        // Returns the instance of the singleton, creates a new one if there isn't one
        public static NetworkManager Instance {
            get {
                if (instance == null) {
                    instance = new NetworkManager();
                }
                return instance;
            }
        }


    }
}
