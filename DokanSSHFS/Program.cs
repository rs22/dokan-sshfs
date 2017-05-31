using System;
using System.Threading;
using System.Windows.Forms;

namespace DokanSSHFS
{
    class DokanSSHFS
    {
        public static bool DokanDebug = false;
        public static bool SSHDebug = false;
        public static ushort DokanThread = 0;
        public static bool UseOffline = true;

        [STAThread]
        static void Main()
        {
            string[] args = Environment.GetCommandLineArgs();
            foreach (string arg in args)
            {
                if (arg == "-sd")
                {
                    SSHDebug = true;
                }
                if (arg == "-dd")
                {
                    DokanDebug = true;
                }
                if (arg.Length >= 3 &&
                    arg[0] == '-' &&
                    arg[1] == 't')
                {
                    DokanThread = ushort.Parse(arg.Substring(2));
                }
                if (arg == "-no")
                {
                    UseOffline = false;
                }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SettingForm());
        }
    }
}
