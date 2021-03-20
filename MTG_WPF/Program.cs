using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MTG
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 

        //Global Variables
        const String _softwareVersion = "v0.1 beta";

        private static string messageText = "";
        [STAThread]
        static void Main()
        {
            if( GlobalParameters.CheckInternetConnection() )
            {
                messageText =  "Internet connection established";
            }
            else
            {
                messageText = "Internet connection not established.\nOnline card search will not be possible.";
            }

            Console.WriteLine(messageText);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormCardScanner(_softwareVersion));
            //Application.Run(new FormTesting());
        }
    }
}
