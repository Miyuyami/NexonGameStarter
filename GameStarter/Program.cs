﻿using System;
using System.Windows.Forms;

namespace GameStarter
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                Arguments.GameId = args[1];
            }
            if (args.Length > 2)
            {
                Arguments.StartLocale = args[2];
            }
            if (args.Length > 3)
            {
                Arguments.LoginUrl = args[3];
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new BrowserForm());
        }
    }
}
