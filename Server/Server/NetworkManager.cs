using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace Server {
    public class NetworkManager {
        protected static NetworkManager instance; // The instance of the singleton
        protected TcpListener tcpListenerSocket; // The TCP listener socket
        protected UdpClient udpSocket; // The UDP socket
        protected List<IPEndPoint> players; // List of the players ip addresses


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
            Socket tcpClient= tcpListenerSocket.AcceptSocket();
            Thread thread = new Thread(() => HandleTcpClient(tcpClient));
            thread.IsBackground = true;
            thread.Start(tcpClient);
        }

        // Communicates with tcp client
        protected void HandleTcpClient(Socket tcpClient) {
            Bomber newBomber = null;
            string messageHeader = "";
            byte[] oneByteBuffer = new byte[1];
            while (!messageHeader.EndsWith("\r\n\r\n")) {
                tcpClient.Receive(oneByteBuffer);
                messageHeader += Encoding.UTF8.GetChars(oneByteBuffer);
            }
            int payloadSize;
            Protocol.AnalizeTcpHeader(messageHeader, out payloadSize);
            byte[] payloadBuffer = new byte[payloadSize];
            string payload = "" + Encoding.UTF8.GetChars(payloadBuffer);
            string[,] parameters = Protocol.AnalizeTcpPayload(payload);
            for (int i = 0; i < parameters.GetLength(0); i++) {
                switch (parameters[i, 0]) {
                    case "Name": // This is a client who wants to join the game
                        if (MapManager.Instance.GetBomber(parameters[i, 1]) == null) {
                            newBomber = new Bomber(parameters[i, 1]);
                            MapManager.Instance.AddBomber(newBomber);
                            players.Add(tcpClient.RemoteEndPoint as IPEndPoint);
                        }
                        break;
                }
            }
        }

        // Listens to the UDP messages from a given client
        protected void ListenUdp(IPEndPoint IEP) {

        }

        // Sends updates to the clients
        protected void SendUdp() {

        }

        // Class in charge of creating and analizing protocol messages
        protected static class Protocol {
            public static string[] newLine = new string[] { "\r\n" };
            public static string[] nameValueSeperator = new string[] { ": " };

            // Checks if the tcp header is currect and gets the payload size
            public static bool AnalizeTcpHeader(string header, out int payloadSize) {
                payloadSize = 0;
                string[] headerLines = header.Split(newLine, StringSplitOptions.RemoveEmptyEntries);
                if (headerLines[0] != "Pyromania " + Settings.Instance.GetTempSetting("version") + "\r\n") {
                    return false;
                }
                for (int i = 1; i < headerLines.Length; i++) {
                    string[] headerLine = headerLines[i].Split(nameValueSeperator, StringSplitOptions.RemoveEmptyEntries);
                    switch (headerLine[0]) {
                        case "Length":
                            payloadSize = int.Parse(headerLine[1]);
                            break;
                        default:
                            return false;
                    }
                }
                return true;
            }

            // Returns an array of things sent in the payload seperated by name and value
            public static string[,] AnalizeTcpPayload(string payload) {
                string[] linesArray = payload.Split(newLine, StringSplitOptions.RemoveEmptyEntries);
                List<string[]> payloadList = new List<string[]>();
                foreach (string line in linesArray) {
                    payloadList.Add(line.Split(nameValueSeperator, StringSplitOptions.RemoveEmptyEntries));
                }
                string[,] payloadArray = new string[payloadList.Count, 2];
                for (int i = 0; i < payloadList.Count; i++) {
                    try {
                        string[] nameValue = payloadList.ElementAt(i);
                        payloadArray[i, 0] = nameValue[0];
                        payloadArray[i, 1] = nameValue[1];
                    } catch (IndexOutOfRangeException) {
                        Printer.Print("Bad TCP payload parameter");
                    }
                } 
                return payloadArray;
            }
        }
    }
}
