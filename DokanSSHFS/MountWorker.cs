using System.Windows.Forms;
using System.Diagnostics;
using DokanNet;

namespace DokanSSHFS
{
    class MountWorker
    {
        private IDokanOperations _sshfs;
        private DokanOptions _opt;
        private string _mountPoint;
        private int _threadCount;

        public MountWorker(IDokanOperations sshfs, DokanOptions opt, string mountPoint, int threadCount)
        {
            _sshfs = sshfs;
            _opt = opt;
            _mountPoint = mountPoint;
            _threadCount = threadCount;
        }

        public void Start()
        {
            System.IO.Directory.SetCurrentDirectory(Application.StartupPath);
            try
            {
                _sshfs.Mount(_mountPoint, _opt, _threadCount);
            }
            catch (DokanException ex)
            {
                MessageBox.Show(ex.Message, "Error");
                Application.Exit();
            }
            Debug.WriteLine("DokanNet.Main end");
        }
    }
}