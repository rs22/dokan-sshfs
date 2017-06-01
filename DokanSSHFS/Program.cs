using System;
using System.Threading;
using System.Windows.Forms;

namespace DokanSSHFS
{
    class DokanSSHFS
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
        }
    }
}
