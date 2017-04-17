using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;
using System.Linq;
using System.Text;
using System.Threading;

public class NetworkManager : MonoBehaviour {
    protected Socket tcpSocket; // The TCP socket
    protected UdpClient udpReceiveSocket; // The UDP receive socket
    protected Socket udpSendSocket; // The UDP send socket
    protected IPEndPoint udpBindIPEndPoint; // The address that the udp socket binds to
    protected IPEndPoint udpIPEndPoint; // The address that the udp socket recieves from
    protected string myBomberName; // The name of the offline bomber
    protected bool gameStarted; // Specifies if the game has started or not
    protected List<Thread> threads; // Contains the object's threads

    // Use this for initialization
    void Awake () {
        tcpSocket = new Socket(IPAddress.Any.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        udpReceiveSocket = new UdpClient();
        udpSendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        udpBindIPEndPoint = new IPEndPoint(IPAddress.Parse("0.0.0.0"), 9002);
        gameStarted = false;
        threads = new List<Thread>();
    }

    // Update is called once per frame
    void Update () {
    }

    // Called when the object is destroyed
    private void OnDestroy() {
        tcpSocket.Close();
        udpReceiveSocket.Close();
        foreach (Thread thread in threads) {
            thread.Abort();
        }
    }

    // Receives one packet from a TCP socket
    protected string RecvTcp() {
        string messageHeader = "";
        byte[] oneByteBuffer = new byte[1];
        while (!messageHeader.EndsWith("\r\n\r\n")) {
            tcpSocket.Receive(oneByteBuffer);
            messageHeader += Encoding.UTF8.GetString(oneByteBuffer);
        }
        int payloadSize;
        Protocol.AnalizeTcpHeader(messageHeader, out payloadSize);
        byte[] payloadBuffer = new byte[payloadSize];
        tcpSocket.Receive(payloadBuffer);
        return Encoding.UTF8.GetString(payloadBuffer);
    }

    // Sends a string to a given TCP socket
    protected void SendTcp(string message) {
        tcpSocket.Send(Encoding.UTF8.GetBytes(message));
    }

    // Connects to the given server
    public bool Connect(string name, string serverIp, int serverPort) {
        myBomberName = name;
        tcpSocket.Connect(serverIp, serverPort);
        string[,] messageInfo = new string[1, 2] { { "Name", name } };
        SendTcp(Protocol.CreateTcpPacket(messageInfo));
        messageInfo = Protocol.AnalizeTcpPayload(RecvTcp());
        if (messageInfo[0, 0] == "Connection") {
            if (messageInfo[0, 1] == "Success") {
                if (messageInfo[1, 0] == "Map") {
                    udpReceiveSocket.Client.Bind(udpBindIPEndPoint);
                    GameManager.instance.mapManager.LoadMap(messageInfo[1, 1]);
                    udpIPEndPoint = new IPEndPoint(IPAddress.Parse(serverIp), 9002);
                    udpReceiveSocket.Connect(serverIp, serverPort + 1);
                    Thread listenThread = new Thread(() => ListenUdp());
                    listenThread.IsBackground = true;
                    listenThread.Start();
                    threads.Add(listenThread);
                    Thread updateThread = new Thread(() => UpdateServer());
                    updateThread.IsBackground = true;
                    updateThread.Start();
                    threads.Add(updateThread);
                    return true;
                }
            } else {
                if (messageInfo[1, 0] == "Error") {
                    print(messageInfo[1, 1]);
                }
            }
        }
        return false;
    }

    // Listens to the UDP messages from the server
    protected void ListenUdp() {
        MapManager mm = GameManager.instance.mapManager;
        print("lel");
        while (true) {
            string message = Encoding.UTF8.GetString(udpReceiveSocket.Receive(ref udpIPEndPoint));
            gameStarted = true;
            int[,] newBricksLocations;
            List<int[]> oldBricksLocations = mm.GetTileLocs(typeof(Bricks), true);
            GameObject[] bombsArray;
            Bomber.BomberInfo[] bombersArray;
            Protocol.AnalizeUDPPacket(message, out newBricksLocations, out bombsArray, out bombersArray);
            foreach (int[] curBrickLoc in oldBricksLocations) {
                bool brickFound = false;
                for (int i = 0; i < newBricksLocations.Length; i++) {
                    if (newBricksLocations[i, 0] == curBrickLoc[0] && newBricksLocations[i, 1] == curBrickLoc[1]) {
                        brickFound = true;
                    }
                }
                if (!brickFound) {
                    mm.GetTile(curBrickLoc[0], curBrickLoc[1]).GetComponent<Bricks>().BlowUp(true);
                }
            }
            foreach (Bomber.BomberInfo curBomberInfo in bombersArray) {
                if (curBomberInfo.bomberName == myBomberName) {
                    if (mm.offlineBomber == null) {
                        mm.CreateBomber(curBomberInfo, false);
                    }
                } else {
                    GameObject curBomber = mm.GetOnlineBomber(curBomberInfo.bomberName);
                    if (curBomber == null) {
                        mm.CreateBomber(curBomberInfo, true);
                    } else {
                        curBomber.GetComponent<Bomber>().SetInfo(curBomberInfo);
                    }
                }
            }
        }
    }

    // Sends updates to the server
    protected void UpdateServer() {
        while (!gameStarted) {
            Thread.Sleep(1000 / 64);
        }
        GameManager.instance.timer.StartTimer();
        while (true) {
            string updateData = Protocol.CreateUdpPacket();
            SendUdp(updateData);
            GameManager.instance.timer.Wait(1000 / 64);
        }
    }

    // Sends a UDP message to the server
    protected void SendUdp(string message) {
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
        IPEndPoint IEP = new IPEndPoint(udpIPEndPoint.Address, 9001);
        udpSendSocket.SendTo(messageBytes, IEP);
    }


    // Class in charge of creating and analyzing protocol messages
    protected static class Protocol {
        public static string[] newLine = new string[] { "\r\n" };
        public static string[] nameValueSeperator = new string[] { ": " };

        // Checks if the TCP header is correct and gets the payload size
        public static bool AnalizeTcpHeader(string header, out int payloadSize) {
            payloadSize = 0;
            string[] headerLines = header.Split(newLine, StringSplitOptions.RemoveEmptyEntries);
            if (headerLines[0] != String.Format("Pyromania {0}", "1.0")) {
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

        // Returns bricks locations array, bomb objects array, player location and player's health array analyzed from the given packet
        public static void AnalizeUDPPacket(string packet, out int[,] BricksLocations, out GameObject[] bombsArray, out Bomber.BomberInfo[] bombersArray) {
            MapManager mapManager = GameManager.instance.mapManager;
            string[] packetParts = packet.Split(' ');
            string[] Bricks = packetParts[0].Split('|'); // BricksLocations
            BricksLocations = new int[Bricks.Length, 2];
            for (int i = 0; i < Bricks.Length; i++) {
                string[] brickLoc = Bricks[i].Split(',');
                BricksLocations[i, 0] = int.Parse(brickLoc[0]);
                BricksLocations[i, 1] = int.Parse(brickLoc[1]);
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

        // Creates a server UDP packet from the bricks locations, bomb objects and bomber objects
        public static string CreateUdpPacket() {
            MapManager mm = GameManager.instance.mapManager;
            string blownBricksStr = "";
            List<int[]> offlineGroundLocs = mm.GetTileLocs(typeof(Ground), false);
            List<int[]> onlineBricksLocs = mm.GetTileLocs(typeof(Wall), true);
            for (int i = 0; i < offlineGroundLocs.Count; i++) {
                int[] offlineGroundLoc = offlineGroundLocs.ElementAt(i);
                if (onlineBricksLocs.Contains(offlineGroundLoc)) {
                    blownBricksStr += String.Format("{0},{1}|", offlineGroundLoc[0], offlineGroundLoc[1]);
                }
            }
            blownBricksStr = blownBricksStr.Substring(0, blownBricksStr.Length - 1);
            string bombsStr = "";
            List<string> bombsInfo = mm.GetBombsInfo(false);
            for (int i = 0; i < bombsInfo.Count; i++) {
                bombsStr += String.Format("{0}|", bombsInfo.ElementAt(i));
            }
            bombsStr = bombsStr.Substring(0, bombsStr.Length - 1);
            string bomberPosStr = "";
            float[] bombersInfo = mm.GetOfflineBomber().GetComponent<Bomber>().GetPosition();
            bomberPosStr = String.Format("{0},{1}", bombersInfo[0], bombersInfo[1]);
            string bombersHealthStr = "";
            GameObject[] onlineBombers = mm.GetOnlineBombers();
            for (int i = 0; i < onlineBombers.Length; i++) {
                Bomber curBomber = onlineBombers[i].GetComponent<Bomber>();
                bombersHealthStr += String.Format("{0},{1}|", curBomber.GetName(), curBomber.GetLives());
            }
            bombersHealthStr = bombersHealthStr.Substring(0, bombersHealthStr.Length - 1);
            string packet = String.Format("{0} {1} {2} {3}", blownBricksStr, bombsStr, bomberPosStr, bombersHealthStr);
            return packet;
        }
    }
}
