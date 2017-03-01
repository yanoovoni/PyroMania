using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;
using System.Linq;

public class NetworkManager : MonoBehaviour {
    TcpClient tcpSocket; // The TCP socket
    UdpClient udpSocket; // The UDP socket


    // Use this for initialization
    void Start () {
        tcpSocket = new TcpClient();
        udpSocket = new UdpClient();
    }

    // Update is called once per frame
    void Update () {
    }

    // Connects to the given server
    public bool Connect(string serverIp, int serverPort) {
        //TODO
        tcpSocket.Connect(serverIp, serverPort);
        return true;
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

        /*
        // Returns blown up rocks locations array, bomb objects array, player location and player's health array analyzed from the given packet
        public static void AnalizeUDPPacket(string packet, out int[,] blownRocksLocations, out Bomb[] bombsArray, out double[] playerLocation, out Dictionary<string, int> playersHealthDict) {
            string[] packetParts = packet.Split(' ');
            string[] blownRocks = packetParts[0].Split('|'); // blownRcoksLocations
            blownRocksLocations = new int[blownRocks.Length, 2];
            for (int i = 0; i < blownRocks.Length; i++) {
                string[] rockLoc = blownRocks[i].Split(',');
                blownRocksLocations[i, 0] = int.Parse(rockLoc[0]);
                blownRocksLocations[i, 1] = int.Parse(rockLoc[1]);
            }
            string[] bombs = packetParts[1].Split('|'); // bombsArray
            bombsArray = new Bomb[bombs.Length];
            for (int i = 0; i < bombs.Length; i++) {
                string[] bombInfo = bombs[i].Split(',');
                bombsArray[i] = new Bomb(int.Parse(bombInfo[0]), int.Parse(bombInfo[1]), int.Parse(bombInfo[2]));
            }
            string[] playerLoc = packetParts[2].Split(','); // playerLocation
            playerLocation = new double[2] { double.Parse(playerLoc[0]), double.Parse(playerLoc[1]) };
            string[] playersHealth = packetParts[3].Split('|'); // playersHealthDict
            playersHealthDict = new Dictionary<string, int>();
            for (int i = 0; i < playersHealth.Length; i++) {
                string[] playerInfo = playersHealth[i].Split(',');
                playersHealthDict.Add(playerInfo[0], int.Parse(playerInfo[1]));
            }
        }

/*
    <list of rocks locations><space>
    <list of bombs locations and times><space>
    <player locations and health>
*/

        /*
        // Creates a server UDP packet from the rocks locations, bomb objects and bomber objects
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
        */
    }
}
