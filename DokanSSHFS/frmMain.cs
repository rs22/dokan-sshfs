using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using DokanNet;
using System.Linq;
using System.Text.RegularExpressions;
using SshFileSystem;
using System.Threading.Tasks;

namespace SshFileSystem.WinForms
{
    public partial class frmMain : Form
    {
        private FileSystem _sshfs;
        private DokanOptions _options;
        private string _mountPoint;
        private StoredPresets _storedPresets = new StoredPresets();
        private Thread _fileSystemThread;
        private bool _isUnmounted = false;
        private bool _isExiting = false;

        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            LoadPresets();
        }

        private void Open_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtPrivateKey.Text = openFileDialog1.FileName;
            }
        }

        private void UsePassword_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtPassword.Checked)
            {
                usePrivateKey.Checked = false;
                txtPrivateKey.Enabled = false;
                passphrase.Enabled = false;
                txtPassword.Enabled = true;
                btnOpenPrivateKey.Enabled = false;
            }
        }

        private void UsePrivateKey_CheckedChanged(object sender, EventArgs e)
        {
            if (usePrivateKey.Checked)
            {
                rbtPassword.Checked = false;
                txtPrivateKey.Enabled = true;
                passphrase.Enabled = true;
                txtPassword.Enabled = false;
                btnOpenPrivateKey.Enabled = true;
            }
        }

        private async void Connect_Click(object sender, EventArgs e)
        {
            int port = 22;
            
            _sshfs = new FileSystem();
            _options = new DokanOptions();

            //if (DokanSSHFS.DokanDebug)
            //    opt |= DokanOptions.DebugMode;

            _options |= DokanOptions.AltStream; // DokanOptions.KeepAlive always enabled.

            _mountPoint = "n:\\";

            var message = "";

            if (txtHost.Text == "")
                message += "Host name is empty\n";

            if (txtUser.Text == "")
                message += "User name is empty\n";


            if (txtPort.Text == "")
                message += "Port is empty\n";
            else
            {
                try
                {
                    port = Int32.Parse(txtPort.Text);
                }
                catch(Exception)
                {
                    message += "Port format error\n";
                }
            }

            if (cmbDriveLetter.Text.Length != 1)
            {
                message += "Drive letter is invalid\n";
            }
            else
            {
                char letter = cmbDriveLetter.Text[0];
                letter = Char.ToLower(letter);
                if (!('e' <= letter && letter <= 'z'))
                    message += "Drive letter is invalid\n";

                _mountPoint = string.Format("{0}:\\", letter);
            }

            //threadCount = DokanSSHFS.DokanThread;

            if (message.Length != 0)
            {
                MessageBox.Show(message);
                return;
            }

            //DokanSSHFS.UseOffline = false;

            var volumeLabel = _storedPresets.Presets.Any(x => x.Name == (string)cmbSelectedPreset.SelectedItem)
                ? (string)cmbSelectedPreset.SelectedItem
                : txtHost.Text;

            if (!Regex.IsMatch(volumeLabel, "^([a-z]|[A-Z])+$"))
                volumeLabel = "SSHFS";

            _sshfs.Initialize(
                txtUser.Text,
                txtHost.Text,
                port,
                usePrivateKey.Checked ? null : txtPassword.Text,
                usePrivateKey.Checked ? txtPrivateKey.Text : null,
                usePrivateKey.Checked ? passphrase.Text : null,
                txtPath.Text,
                volumeLabel);

            lblStatus.Text = "Connecting...";

            if (_sshfs.SSHConnect())
            {
                _isUnmounted = false;
                
                var worker = new MountWorker(_sshfs, _options, _mountPoint, 1);

                _fileSystemThread = new Thread(worker.Start);
                _fileSystemThread.Start();
            }
            else
            {
                lblStatus.Text = "Failed to connect.";
                return;
            }

            lblStatus.Text = "Connection successful.";

            await Task.Delay(500);

            btnMount.Enabled = _isUnmounted;
            btnUnmount.Enabled = !_isUnmounted;

            try
            {
                // Try to open an explorer window (might not be available just after mounting the fs)
                Process.Start(_mountPoint);
            }
            catch { }
        }
        
        private void Unmount()
        {
            if (_sshfs != null)
            {
                Debug.WriteLine(string.Format("SSHFS Trying unmount : {0}", _mountPoint));

                try
                {
                    Dokan.RemoveMountPoint(_mountPoint);
                    Debug.WriteLine("DokanReveMountPoint success");
                }
                catch (DokanException ex)
                {
                    Debug.WriteLine("DokanRemoveMountPoint failed: " + ex.Message);
                }

                // This should be called from Dokan, but not called.
                // Call here explicitly.
                _sshfs.Unmounted(null);
            }
        }
        
        private void btnExit_Click(object sender, EventArgs e)
        {
            _isExiting = true;
            Application.Exit();
        }
        
        private void btnUnmount_Click(object sender, EventArgs e)
        {
            Unmount();
            _isUnmounted = true;

            btnMount.Enabled = _isUnmounted;
            btnUnmount.Enabled = !_isUnmounted;

            lblStatus.Text = "Ready";
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cmbSelectedPreset.Text))
            {
                MessageBox.Show("Please enter a preset name");
                return;
            }

            var preset = _storedPresets.LoadOrCreatePreset(cmbSelectedPreset.Text);
            _storedPresets.AddPreset(preset);

            preset.Host = txtHost.Text;
            preset.User = txtUser.Text;
            if (int.TryParse(txtPort.Text, out int portNumber))
                preset.Port = portNumber;
            else
                preset.Port = 22;

            preset.PrivateKey = txtPrivateKey.Text;
            preset.UsePassword = rbtPassword.Checked;
            if (preset.UsePassword && !string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                preset.EncryptedPassword = EncryptionHelper.EncryptToBase64String(txtPassword.Text);
            }

            preset.Drive = cmbDriveLetter.Text;
            preset.ServerRoot = txtPath.Text;

            _storedPresets.Save();

            if (cmbSelectedPreset.Items.Count == _storedPresets.Presets.Count)
                cmbSelectedPreset.Items.Add(_storedPresets.GetNewName());
        }

        private void cmbSelectedPreset_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadPreset((string)cmbSelectedPreset.SelectedItem);
        }

        private void LoadPreset(string name)
        {
            if (string.IsNullOrEmpty(name))
                name = _storedPresets.GetNewName();

            var preset = _storedPresets.LoadOrCreatePreset(name);

            txtHost.Text = preset.Host;
            txtUser.Text = preset.User;
            txtPort.Text = preset.Port.ToString();
            txtPrivateKey.Text = preset.PrivateKey;
            txtPassword.Text = string.IsNullOrEmpty(preset.EncryptedPassword) ? string.Empty : EncryptionHelper.DecryptBase64String(preset.EncryptedPassword);
            rbtPassword.Checked = preset.UsePassword;
            usePrivateKey.Checked = !preset.UsePassword;
            UsePassword_CheckedChanged(null, null);
            UsePrivateKey_CheckedChanged(null, null);
            
            cmbDriveLetter.Text = preset.Drive;
            txtPath.Text = preset.ServerRoot;
        }

        private void LoadPresets()
        {
            _storedPresets.Load();
            cmbSelectedPreset.Items.Clear();
            cmbSelectedPreset.Items.AddRange(_storedPresets.Presets.Select(x => x.Name).ToArray());
            var placeholder = _storedPresets.GetNewName();
            cmbSelectedPreset.Items.Add(placeholder);
            cmbSelectedPreset.SelectedItem = placeholder;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_storedPresets.Delete(cmbSelectedPreset.Text))
            {
                _storedPresets.Save();
                LoadPresets();
            }
        }

        private void btnMount_Click(object sender, EventArgs e)
        {
            Show();
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!_isUnmounted)
            {
                Unmount();
                _isUnmounted = true;
            }

            Debug.WriteLine("SSHFS Thread Waiting");

            if (_fileSystemThread != null && _fileSystemThread.IsAlive)
            {
                Debug.WriteLine("doka.Join");
                _fileSystemThread.Join();
            }

            Debug.WriteLine("SSHFS Thread End");
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_isUnmounted && !_isExiting)
            {
                e.Cancel = true;
                WindowState = FormWindowState.Minimized;
            }
        }
    }
}