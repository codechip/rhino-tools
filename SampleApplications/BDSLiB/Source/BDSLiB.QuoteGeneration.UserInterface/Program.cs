using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BDSLiB.QuoteGeneration.UserInterface
{
    using Chapter5.QuoteGeneration;

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            QuoteGenerator.Generate(@"D:\OSS\rhino-tools\SampleApplications\BDSLiB\Chapter5\Chapter5.Scripts\QuoteGenerator\sample.boo");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}