using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace Server {
    public class NetworkManager {
        protected static NetworkManager instance; // The instance of the singleton
        protected TcpListener tcpListenerSocket; // The TCP listener socket
        protected UdpClient udpSocket; // The UDP socket

        protected NetworkManager() {
            Settings s = Settings.Instance;
            tcpListenerSocket = new TcpListener(new IPEndPoint(IPAddress.Parse(s.GetTempSetting("ip")), int.Parse(s.GetTempSetting("tcp_port")))); // Creates a new TCP listener socket according to the temp settings
            udpSocket = new UdpClient(new IPEndPoint(IPAddress.Parse(s.GetTempSetting("ip")), int.Parse(s.GetTempSetting("udp_port")))); // Creates a new UDP socket according to the temp settings
            
        }

        // Returns the instance of the singleton, creates a new one if there isn't one
        public static NetworkManager Instance {
            get {
                if (instance == null) {
                    instance = new NetworkManager();
                }
                return instance;
            }
        }

        // Listens to the TCP socket to accept new connections
        protected void ListenTcp() {

        }

        // Listens to the UDP messages from a given client
        protected void ListenUdp(IPEndPoint IEP) {

        }
    }
}
