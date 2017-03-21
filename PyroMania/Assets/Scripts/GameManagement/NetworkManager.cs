using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;
using System.Linq;
using System.Text;

public class NetworkManager : MonoBehaviour {
    protected TcpClient tcpSocket; // The TCP socket
    protected UdpClient udpSocket; // The UDP socket

    // Use this for initialization
    void Start () {
        tcpSocket = new TcpClient();
        udpSocket = new UdpClient();
    }

    // Update is called once per frame
    void Update () {
    }

    // Receives one packet from a TCP socket
    protected string RecvTcp(Socket socket) {
        string messageHeader = "";
        byte[] oneByteBuffer = new byte[1];
        while (!messageHeader.EndsWith("\r\n\r\n")) {
            socket.Receive(oneByteBuffer);
            messageHeader += Encoding.UTF8.GetString(oneByteBuffer);
        }
        int payloadSize;
        Protocol.AnalizeTcpHeader(messageHeader, out payloadSize);
        byte[] payloadBuffer = new byte[payloadSize];
        return Encoding.UTF8.GetString(payloadBuffer);
    }

    // Sends a string to a given TCP socket
    protected void SendTcp(Socket socket, string message) {
        socket.Send(Encoding.UTF8.GetBytes(message));
    }

    // Connects to the given server
    public bool Connect(string serverIp, int serverPort) {
        //TODO
        tcpSocket.Connect(serverIp, serverPort);
        return true;
    }

    // Listens to the UDP messages from a given client
    protected void ListenUdp(IPEndPoint IEP, Bomber bomber) {
        //TODO
        while (true) {
            string message = Encoding.UTF8.GetString(udpSocket.Receive(ref IEP));
            int[,] blownRocksLocations;
            Bomb[] bombsArray;
            float[] playerLocation;
            Dictionary<string, int> playersHealthDict;
            Protocol.AnalizeUDPPacket(message, out blownRocksLocations, out bombsArray, out playerLocation, out playersHealthDict);
            for (int i = 0; i < blownRocksLocations.GetLength(0); i++) {
                int[] bombLoc = new int[] { blownRocksLocations[i, 0], blownRocksLocations[i, 1] };
                MapManager.Instance.DeleteBomb(bombLoc);
            }
            foreach (Bomb bomb in bombsArray) {
                MapManager.Instance.AddBomb(bomb);
            }
            bomber.SetPosition(playerLocation[0], playerLocation[1]);
            foreach (KeyValuePair<string, int> curBomberInfo in playersHealthDict.ToArray()) {
                Bomber curBomber = MapManager.Instance.GetBomber(curBomberInfo.Key);
                if (curBomber.GetHealth() > curBomberInfo.Value) {
                    curBomber.GetHit();
                }
            }
        }
    }

    // Sends updates to the server
    protected void UpdateServer() {
        while (true) {
            string updateData = Protocol.CreateUdpPacket();
            SendUdp(updateData);
            GameManager.instance.timer.Wait(1000 / 64);
        }
    }

    // Sends a UDP message to the server
    protected void SendUdp(string message) {
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
        udpSocket.Send(messageBytes, messageBytes.Length);
    }


    // Class in charge of creating and analyzing protocol messages
    protected static class Protocol {
        public static string[] newLine = new string[] { "\r\n" };
        public static string[] nameValueSeperator = new string[] { ": " };

        // Checks if the TCP header is correct and gets the payload size
        public static bool AnalizeTcpHeader(string header, out int payloadSize) {
            payloadSize = 0;
            string[] headerLines = header.Split(newLine, StringSplitOptions.RemoveEmptyEntries);
            if (headerLines[0] != String.Format("Pyromania {0}\r\n", "1.0")) {
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

        // Returns an array of things sent in the payload separated by name and value
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
                    print("Bad TCP payload parameter");
                }
            }
            return payloadArray;
        }

        // Creates a TCP packet
        public static string CreateTcpPacket(string[,] payloadArray) {
            string payload = "";
            for (int i = 0; i < payloadArray.GetLength(0); i++) {
                payload += String.Format("{0}: {1}\r\n", payloadArray[i, 0], payloadArray[i, 1]);
            }
            string packet = String.Format("Pyromania {0}\r\nLength: {1}\r\n\r\n{2}", "1.0", payload.Length, payload);
            return packet;
        }

        // Returns blown up walls locations array, bomb objects array, player location and player's health array analyzed from the given packet
        public static void AnalizeUDPPacket(string packet, out int[,] blownRocksLocations, out GameObject[] bombsArray, out Bomber.BomberInfo[] bombersArray) {
            MapManager mapManager = GameManager.instance.mapManager.GetComponent<MapManager>();
            string[] packetParts = packet.Split(' ');
            string[] blownRocks = packetParts[0].Split('|'); // blownRocksLocations
            blownRocksLocations = new int[blownRocks.Length, 2];
            for (int i = 0; i < blownRocks.Length; i++) {
                string[] rockLoc = blownRocks[i].Split(',');
                blownRocksLocations[i, 0] = int.Parse(rockLoc[0]);
                blownRocksLocations[i, 1] = int.Parse(rockLoc[1]);
            }
            string[] bombs = packetParts[1].Split('|'); // bombsArray
            bombsArray = new GameObject[bombs.Length];
            for (int i = 0; i < bombs.Length; i++) {
                string[] bombInfo = bombs[i].Split(',');
                int[] parsedBombInfo = new int[3] { int.Parse(bombInfo[0]), int.Parse(bombInfo[1]), int.Parse(bombInfo[2]) };
                if (mapManager.GetBomb(parsedBombInfo[0], parsedBombInfo[1], true) == null) {
                    mapManager.CreateBomb(parsedBombInfo[0], parsedBombInfo[1], parsedBombInfo[2], true);
                }
            }
            string[] bombersInfo = packetParts[2].Split('|'); // bombersDict
            bombersArray = new Bomber.BomberInfo[bombersInfo.Length];
            for (int i = 0; i < bombersInfo.Length; i++) {
                string[] bomberInfo = bombersInfo[i].Split(',');
                bombersArray[i] = new Bomber.BomberInfo(bomberInfo[0], float.Parse(bomberInfo[1]), float.Parse(bomberInfo[2]), int.Parse(bomberInfo[3]));
            }
        }

        // Creates a server UDP packet from the walls locations, bomb objects and bomber objects
        public static string CreateUdpPacket() {
            MapManager mm = GameManager.instance.mapManager;
            string wallsStr = "";
            List<int[]> wallLocs = mm.GetTileLocs(typeof(Wall), false);
            for (int i = 0; i < wallLocs.Count; i++) {
                int[] wallLoc = wallLocs.ElementAt(i);
                wallsStr += String.Format("{0},{1}|", wallLoc[0], wallLoc[1]);
            }
            wallsStr = wallsStr.Substring(0, wallsStr.Length - 1);
            string bombsStr = "";
            List<string> bombsInfo = mm.GetBombsInfo(false);
            for (int i = 0; i < bombsInfo.Count; i++) {
                bombsStr += String.Format("{0}|", bombsInfo.ElementAt(i));
            }
            bombsStr = bombsStr.Substring(0, bombsStr.Length - 1);
            string bombersStr = "";
            List<string> bombersInfo = mm.GetBombersInfo();
            for (int i = 0; i < bombersInfo.Count; i++) {
                bombersStr += String.Format("{0}|", bombersInfo.ElementAt(i));
            }
            bombersStr = bombersStr.Substring(0, bombersStr.Length - 1);
            string packet = String.Format("{0} {1} {2}", wallsStr, bombsStr, bombersStr);
            return packet;
        }
    }
}
