﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server {
    class Program {
        public static void Main(string[] args) {
            GameManager gameManager = new GameManager();
            gameManager.Run(args);
        }
    }
}
