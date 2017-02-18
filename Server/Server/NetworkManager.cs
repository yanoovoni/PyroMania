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

        // Recieves one packet from a tcp socket
        protected string RecvTcp(Socket socket) {
            string messageHeader = "";
            byte[] oneByteBuffer = new byte[1];
            while (!messageHeader.EndsWith("\r\n\r\n")) {
                socket.Receive(oneByteBuffer);
                messageHeader += Encoding.UTF8.GetChars(oneByteBuffer);
            }
            int payloadSize;
            Protocol.AnalizeTcpHeader(messageHeader, out payloadSize);
            byte[] payloadBuffer = new byte[payloadSize];
            return "" + Encoding.UTF8.GetChars(payloadBuffer);
        }

        // Sends a string to a given tcp socket
        protected void SendTcp(Socket socket, string message) {
            socket.Send(Encoding.UTF8.GetBytes(message));
        }

        // Communicates with tcp client
        protected void HandleTcpClient(Socket tcpClient) {
            string[,] parameters = Protocol.AnalizeTcpPayload(RecvTcp(tcpClient));
            for (int i = 0; i < parameters.GetLength(0); i++) {
                switch (parameters[i, 0]) {
                    case "Name": // This is a client who wants to join the game
                        AddPlayer(tcpClient, new Bomber(parameters[i, 1]));
                        break;
                }
            }
        }

        // Listens to the UDP messages from a given client
        protected void ListenUdp(IPEndPoint IEP) {
            //TODO
        }

        // Sends updates to all of the clients
        protected void UpdateClients() {
            string updateData = Protocol.CreateUdpPacket();
            foreach (IPEndPoint player in players.ToArray()) {
                SendUdp(updateData, player);
            }
        }

        // Sends a udp message to the client
        protected void SendUdp(string message, IPEndPoint client) {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            udpSocket.Send(messageBytes, messageBytes.Length, client);
        }

        // Adds a player to the network manager's list of players and the bomber to the map manager. If the bomber's name is taken does not add him. returns if successful and notifies the player
        protected bool AddPlayer(Socket playerSocket, Bomber bomber) {
            IPEndPoint player = playerSocket.RemoteEndPoint as IPEndPoint;
            string[,] responseMessage;
            string errMessage = null;
            if (MapManager.Instance.SlotsLeft() <= 0) {
                errMessage = "Server full";
            } else if (MapManager.Instance.GetBomber(bomber.GetName()) == null) {
                errMessage = "Name taken";
            }
            if (errMessage == null) { // Bomber with that name does not exist so add this player to the game
                MapManager.Instance.AddBomber(bomber);
                players.Add(player);
                ListenUdp(player);
                responseMessage = new string[2, 2] { { "Connection", "Success" }, { "Map", MapManager.Instance.GetMap() } };
                SendTcp(playerSocket, Protocol.CreateTcpPacket(responseMessage));
                return true;
            } else { // Bomber with that name exists so tell player to fuck off
                responseMessage = new string[2, 2] { { "Connection", "Failure" }, { "Error", errMessage } };
                SendTcp(playerSocket, Protocol.CreateTcpPacket(responseMessage));
                return false;
            }
        }

        // Class in charge of creating and analizing protocol messages
        protected static class Protocol {
            public static string[] newLine = new string[] { "\r\n" };
            public static string[] nameValueSeperator = new string[] { ": " };

            // Checks if the tcp header is currect and gets the payload size
            public static bool AnalizeTcpHeader(string header, out int payloadSize) {
                payloadSize = 0;
                string[] headerLines = header.Split(newLine, StringSplitOptions.RemoveEmptyEntries);
                if (headerLines[0] != String.Format("Pyromania {0}\r\n", Settings.Instance.GetTempSetting("version"))) {
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

            // Creates a tcp packet
            public static string CreateTcpPacket(string[,] payloadArray) {
                string payload = "";
                for (int i = 0; i < payloadArray.GetLength(0); i++) {
                    payload += String.Format("{0}: {1}\r\n", payloadArray[i, 0], payloadArray[i, 1]);
                }
                string packet = String.Format("Pyromania {0}\r\nLength: {1}\r\n\r\n{2}", Settings.Instance.GetTempSetting("version"), payload.Length, payload);
                return packet;
            }

            // Creates a server udp packet from the rocks locations, bomb objects and bomber objects
            public static string CreateUdpPacket() {
                MapManager mm = MapManager.Instance;
                string rocksStr = "";
                List<int[]> rockLocs = mm.GetRockLocs();
                for (int i = 0; i < rockLocs.Count; i++) {
                    int[] rockLoc = rockLocs.ElementAt(i);
                    rocksStr += String.Format("{0},{1}|", rockLoc[0], rockLoc[1]);
                }
                rocksStr = rocksStr.Substring(0, rocksStr.Length - 1);
                string bombsStr = "";
                List<Bomb> bombs = mm.GetBombs();
                for (int i = 0; i < bombs.Count; i++) {
                    bombsStr += String.Format("{0}|", bombs.ElementAt(i).ToString());
                }
                bombsStr = bombsStr.Substring(0, bombsStr.Length - 1);
                string bombersStr = "";
                List<Bomber> bombers = mm.GetBombers();
                for (int i = 0; i < bombers.Count; i++) {
                    bombersStr += String.Format("{0}|", bombers.ElementAt(i).ToString());
                }
                bombersStr = bombersStr.Substring(0, bombersStr.Length - 1);
                string packet = String.Format("{0} {1} {2}", rocksStr, bombsStr, bombersStr);
                return packet;
            }
        }
    }
}
