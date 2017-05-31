using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;

using DokanNet;
using System.Security.AccessControl;
using Renci.SshNet;
using Renci.SshNet.Common;

namespace DokanSSHFS
{
    public class SshFS : IDokanOperations
    {
        private object _sessionLock = new Object();
        private SftpClient _client;

        private string _user;
        private string _host;
        private int _port;
        private string _identity;
        private bool _debug;
        private string _root;
        private string _passphrase;
        private string _password;

        private TextWriter _tw;

        private int _trycount = 0;
        private bool _connectionError = false;
        private object _reconnectLock = new object();

        public void Initialize(string user, string host, int port, string password, string identity, string passphrase, string root, bool debug)
        {
            _user = user;
            _host = host;
            _port = port;
            _identity = identity;
            _password = password;
            _passphrase = passphrase;

            _root = root;

            _debug = debug;

            if (_debug && _tw != null)
            {
                StreamWriter sw = new StreamWriter(Application.UserAppDataPath + "\\error.txt")
                {
                    AutoFlush = true
                };
                _tw = TextWriter.Synchronized(sw);
                Console.SetError(_tw);
            }

        }

        private void Debug(string format, params object[] args)
        {
            if (_debug)
            {
                Console.Error.WriteLine("SSHFS: " + format, args);
                System.Diagnostics.Debug.WriteLine(string.Format("SSHFS: " + format, args));
            }
        }

        internal bool SSHConnect()
        {
            try
            {
                var credentials = new PasswordAuthenticationMethod(_user, _password);
                var connectionInfo = new ConnectionInfo(_host, _port, _user, credentials);
                
                _client = new SftpClient(connectionInfo);

                _client.Connect();

                //_channels = new Dictionary<int, ChannelSftp>();

                //jsch_ = new JSch();
                //Hashtable config = new Hashtable();
                //config["StrictHostKeyChecking"] = "no";

                //if (_identity != null)
                //    jsch_.addIdentity(_identity, _passphrase);

                //session_ = jsch_.getSession(_user, _host, _port);
                //session_.setConfig(config);
                //session_.setUserInfo(new DokanUserInfo(_password, _passphrase));
                //session_.setPassword(_password);

                //session_.connect();

                return true;
            }
            catch (Exception e)
            {
                Debug(e.ToString());
                return false;
            }
        }

        private bool Reconnect()
        {
            lock (_reconnectLock)
            {
                if (!_connectionError)
                    return true;

                Debug("Disconnect current sessions\n");
                try
                {
                    GetClient().Disconnect();

                    //session_.disconnect();
                }
                catch (Exception e)
                {
                    Debug(e.ToString());
                }

                Debug("Reconnect {0}\n", _trycount);

                _trycount++;

                if (SSHConnect())
                {
                    Debug("Reconnect success\n");
                    _connectionError = false;
                    return true;
                }
                else
                {
                    Debug("Reconnect failed\n");
                    return false;
                }
            }
        }

        private string GetPath(string filename)
        {
            string path = _root + filename.Replace('\\', '/');
            Debug("GetPath : {0} thread {1}", path, Thread.CurrentThread.ManagedThreadId);
            //Debug("  Stack {0}", new System.Diagnostics.StackTrace().ToString());
            return path;
        }

        private SftpClient GetClient()
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            return _client;

            //ChannelSftp channel;
            //try
            //{
            //    channel = channels_[threadId];
            //}
            //catch(KeyNotFoundException)
            //{

            //    lock (sessionLock_)
            //    {
            //        channel = (ChannelSftp)session_.openChannel("sftp");
            //        channel.connect();
            //        channels_[threadId] = channel;
            //    }
            //}
            //return channel;
        }

        private bool PathExists(string path, DokanFileInfo info)
        {
            try
            {
                var channel = GetClient();
                var attr = channel.GetAttributes(path);
                if (attr.IsDirectory)
                    info.IsDirectory = true;
                return true;
            }
            catch (SftpPathNotFoundException)
            {
                return false;
            }
        }

        //private string ReadPermission(string path)
        //{
        //    try
        //    {
        //        var attr = GetClient().GetAttributes(path);
        //        return Convert.ToString(attr.own() & 0xFFF, 8) + "\n";
        //    }
        //    catch (SftpException)
        //    {
        //        return "";
        //    }
        //    catch (Exception)
        //    {
        //        _connectionError = true;
        //        Reconnect();
        //        return "";
        //    }
        //}

        private bool CheckAltStream(string filename)
        {
            if (filename.Contains(":"))
            {
                string[] tmp = filename.Split(new char[] { ':' }, 2);

                if (tmp.Length != 2)
                    return false;

                if (tmp[1].StartsWith("SSHFSProperty."))
                    return true;

                return false;
            }
            else
            {
                return false;
            }
        }


        public NtStatus CreateFile(
            string filename,
            DokanNet.FileAccess access,
            FileShare share,
            FileMode mode,
            FileOptions options,
            FileAttributes attributes,
            DokanFileInfo info)
        {

            var path = GetPath(filename);

            if (info.IsDirectory)
            {
                switch (mode)
                {
                    case FileMode.Open:
                        Debug("OpenDirectory {0}", filename);
                        try
                        {
                            var attr = GetClient().GetAttributes(path);
                            if (attr.IsDirectory)
                            {
                                return NtStatus.Success;
                            }
                            else
                            {
                                return NtStatus.ObjectPathNotFound; // TODO: return not directory?
                            }
                        }
                        catch (SftpPathNotFoundException e)
                        {
                            Debug(e.ToString());
                            return NtStatus.ObjectPathNotFound;
                        }
                        catch (SshConnectionException e)
                        {
                            _connectionError = true;
                            Debug(e.ToString());
                            Reconnect();
                            return NtStatus.ObjectPathNotFound;
                        }


                    case FileMode.CreateNew:
                        Debug("CreateDirectory {0}", filename);
                        try
                        {
                            var client = GetClient();

                            client.CreateDirectory(path);
                            return NtStatus.Success;
                        }
                        catch (SftpPermissionDeniedException e)
                        {
                            Debug(e.ToString());
                            return NtStatus.Error;
                        }
                        catch (SshConnectionException e)
                        {
                            _connectionError = true;
                            Debug(e.ToString());
                            Reconnect();
                            return NtStatus.Error; // TODO: more appropriate error code
                        }
                    default:
                        Debug("Error FileMode invalid for directory {0}", mode);
                        return NtStatus.Error;

                }
            }
            else
            {
                Debug("CreateFile {0}", filename);
                try
                {
                    var client = GetClient();

                    if (CheckAltStream(path))
                        return NtStatus.Success;

                    switch (mode)
                    {
                        case FileMode.Open:
                            {
                                Debug("Open");
                                if (PathExists(path, info))
                                    return NtStatus.Success;
                                else
                                    return NtStatus.ObjectNameNotFound;
                            }
                        case FileMode.CreateNew:
                            {
                                Debug("CreateNew");
                                if (PathExists(path, info))
                                    return NtStatus.ObjectNameCollision;

                                Debug("CreateNew put 0 byte");
                                client.Create(path).Close();
                                return NtStatus.Success;
                            }
                        case FileMode.Create:
                            {
                                Debug("Create put 0 byte");
                                client.Create(path).Close();
                                return NtStatus.Success;
                            }
                        case FileMode.OpenOrCreate:
                            {
                                Debug("OpenOrCreate");

                                if (!PathExists(path, info))
                                {
                                    Debug("OpenOrCreate put 0 byte");
                                    client.Create(path).Close();
                                }
                                return NtStatus.Success;
                            }
                        case FileMode.Truncate:
                            {
                                Debug("Truncate");

                                if (!PathExists(path, info))
                                    return NtStatus.ObjectNameNotFound;

                                Debug("Truncate put 0 byte");
                                client.Create(path).Close();
                                return NtStatus.Success;
                            }
                        case FileMode.Append:
                            {
                                Debug("Append");

                                if (PathExists(path, info))
                                    return NtStatus.Success;

                                Debug("Append put 0 byte");
                                client.Create(path).Close();
                                return NtStatus.Success;
                            }
                        default:
                            Debug("Error unknown FileMode {0}", mode);
                            return NtStatus.Error;
                    }

                }
                // Don't know what this is for
                //catch (SftpException e)
                //{
                //    Debug(e.ToString());
                //    return NtStatus.ObjectNameNotFound;
                //}
                catch (Exception e)
                {
                    _connectionError = true;
                    Debug(e.ToString());
                    Reconnect();
                    return NtStatus.ObjectNameNotFound;
                }
            }
        }
        public void Cleanup(
            string filename,
            DokanFileInfo info)
        {
        }

        public void CloseFile(
            string filename,
            DokanFileInfo info)
        {
        }


        public NtStatus ReadFile(
            string filename,
            Byte[] buffer,
            out int readBytes,
            long offset,
            DokanFileInfo info)
        {
            string path = GetPath(filename);

            readBytes = 0;
            //if (path.Contains(":SSHFSProperty.Permission"))
            //{
            //    if (offset == 0)
            //    {
            //        string[] tmp = path.Split(new char[] { ':' });
            //        path = tmp[0];
            //        string str = ReadPermission(path);
            //        byte[] bytes = System.Text.Encoding.ASCII.GetBytes(str);
            //        int min = (buffer.Length < bytes.Length ? buffer.Length : bytes.Length);
            //        Array.Copy(bytes, buffer, min);
            //        readBytes = min;
            //        return NtStatus.Success;
            //    }
            //    else
            //    {
            //        return NtStatus.Success;
            //    }
            //}

            if (info.IsDirectory)
                return NtStatus.Error;

            Debug("ReadFile {0} bufferLen {1} Offset {2}", filename, buffer.Length, offset);
            try
            {
                var client = GetClient();
                using (var stream = client.OpenRead(path))
                {
                    var position = stream.Seek(offset, SeekOrigin.Begin);
                    readBytes = stream.Read(buffer, 0, buffer.Length);
                }
                //GetMonitor monitor = new GetMonitor(offset + buffer.Length);
                //GetStream stream = new GetStream(buffer);
                //client.get(path, stream, monitor, ChannelSftp.RESUME, offset);
                //readBytes = stream.RecievedBytes;
                Debug("  ReadFile readBytes: {0}", readBytes);
                return NtStatus.Success;
            }
            //catch (SftpException)
            //{
            //    return NtStatus.Error;
            //}
            catch (Exception e)
            {
                _connectionError = true;
                Debug(e.ToString());
                Reconnect();
                return NtStatus.Error;
            }
        }

        //private bool WritePermission(
        //    string path,
        //    int permission)
        //{
        //    try
        //    {
        //        Debug("WritePermission {0}:{1}", path, Convert.ToString(permission, 8));
        //        var channel = GetClient();
        //        var attr = channel.GetAttributes(path);
        //        attr.setPERMISSIONS(permission);
        //        channel.setStat(path, attr);
        //    }
        //    catch (SftpException)
        //    {
        //    }
        //    catch (Exception e)
        //    {
        //        _connectionError = true;
        //        Debug(e.ToString());
        //        Reconnect();
        //    }
        //    return true;
        //}

        public NtStatus WriteFile(
            string filename,
            byte[] buffer,
            out int writtenBytes,
            long offset,
            DokanFileInfo info)
        {
            Debug("WriteFile {0} bufferLen {1} Offset {2}", filename, buffer.Length, offset);

            string path = GetPath(filename);

            writtenBytes = 0;
            //if (path.Contains(":SSHFSProperty.Permission"))
            //{
            //    if (offset == 0)
            //    {
            //        string[] tmp = path.Split(new char[] { ':' });
            //        path = tmp[0];
            //        int permission = 0;
            //        permission = Convert.ToInt32(System.Text.Encoding.ASCII.GetString(buffer), 8);
            //        WritePermission(path, permission);
            //        writtenBytes = buffer.Length;
            //        return NtStatus.Success;
            //    }
            //    else
            //    {
            //        return NtStatus.Success;
            //    }
            //}

            try
            {
                var channel = GetClient();

                //GetMonitor monitor = new GetMonitor(buffer.Length);
                //Tamir.SharpSsh.java.io.OutputStream stream = channel.put(path, null, 3 /*HACK: ‘¶Ý‚µ‚È‚¢ƒ‚[ƒh */, offset);
                //stream.Write(buffer, 0, buffer.Length);
                //stream.Close();
                writtenBytes = buffer.Length;
                return NtStatus.Success;
            }
            catch (IOException)
            {
                return NtStatus.Success;
            }
            catch (Exception e)
            {
                Debug(e.ToString());
                return NtStatus.Error;
            }
        }

        public NtStatus FlushFileBuffers(
            string filename,
            DokanFileInfo info)
        {
            return NtStatus.Success;
        }

        public NtStatus GetFileInformation(
            string filename,
            out FileInformation fileinfo,
            DokanFileInfo info)
        {
            fileinfo = new FileInformation();
            try
            {
                string path = GetPath(filename);
                fileinfo.FileName = path;
                var attr = GetClient().GetAttributes(path);

                fileinfo.Attributes = attr.IsDirectory ?
                    FileAttributes.Directory :
                    FileAttributes.Normal;

                if (DokanSSHFS.UseOffline)
                    fileinfo.Attributes |= FileAttributes.Offline;

                fileinfo.CreationTime = attr.LastWriteTime;
                fileinfo.LastAccessTime = attr.LastAccessTime;
                fileinfo.LastWriteTime = attr.LastWriteTime;
                fileinfo.Length = attr.Size;

                return NtStatus.Success;
            }
            catch (SftpPathNotFoundException)
            {
                return NtStatus.Error;
            }
            catch (Exception e)
            {
                _connectionError = true;
                Debug(e.ToString());
                Reconnect();
                return NtStatus.Error;
            }
        }

        public NtStatus FindFilesWithPattern(
            string fileName,
            string searchPattern,
            out IList<FileInformation> files,
            DokanFileInfo info)
        {
            files = new List<FileInformation>();
            return NtStatus.NotImplemented;
        }

        public NtStatus FindFiles(
            string filename,
            out IList<FileInformation> files,
            DokanFileInfo info)
        {
            Debug("FindFiles {0}", filename);

            files = new List<FileInformation>();
            try
            {
                string path = GetPath(filename);
                var entries = GetClient().ListDirectory(path);

                foreach (var entry in entries)
                {
                    FileInformation fi = new FileInformation()
                    {
                        Attributes = entry.IsDirectory ?
                            FileAttributes.Directory :
                            FileAttributes.Normal,
                        CreationTime = entry.LastWriteTime,
                        LastAccessTime = entry.LastAccessTime,
                        LastWriteTime = entry.LastWriteTime,
                        Length = entry.Length,
                        FileName = entry.Name
                    };

                    if (fi.FileName.StartsWith("."))
                    {
                        fi.Attributes |= FileAttributes.Hidden;
                    }

                    if (DokanSSHFS.UseOffline)
                        fi.Attributes |= FileAttributes.Offline;


                    files.Add(fi);
                }
                return NtStatus.Success;

            }
            catch (SftpPermissionDeniedException)
            {
                return NtStatus.Error;
            }
            catch (Exception e)
            {
                _connectionError = true;
                Debug(e.ToString());
                Reconnect();
                return NtStatus.Error;
            }
        }

        public NtStatus FindStreams(string fileName, out IList<FileInformation> streams, DokanFileInfo info)
        {
            streams = new List<FileInformation>();
            return NtStatus.NotImplemented;
        }

        public NtStatus SetFileAttributes(
            string filename,
            FileAttributes attr,
            DokanFileInfo info)
        {
            Debug("SetFileAttributes {0}", filename);
            try
            {
                // Nothing to do here?
                string path = GetPath(filename);
                var channel = GetClient();
                var sattr = channel.GetAttributes(path);

                //int permissions = sattr.getPermissions();
                //Debug(" permissons {0} {1}", permissions, sattr.getPermissionsString());
                //sattr.setPERMISSIONS(permissions);
                //channel.setStat(path, sattr);
                return NtStatus.Success;
            }
            catch (SftpPathNotFoundException)
            {
                return NtStatus.Error;
            }
            catch (SshConnectionException e)
            {
                _connectionError = true;
                Debug(e.ToString());
                Reconnect();
                return NtStatus.Error;
            }
        }

        public NtStatus SetFileTime(
            string filename,
            DateTime? ctime,
            DateTime? atime,
            DateTime? mtime,
            DokanFileInfo info)
        {
            Debug("SetFileTime {0}", filename);
            try
            {
                Debug(" filetime {0} {1} {2}", ctime.HasValue ? ctime.ToString() : "-", atime.HasValue ? atime.ToString() : "-", mtime.HasValue ? mtime.ToString() : "-");

                string path = GetPath(filename);
                var client = GetClient();

                if (atime.HasValue)
                {
                    // Not yet implemented
                    // client.SetLastAccessTime(path, atime.Value);
                }

                if (mtime.HasValue)
                {
                    // Not yet implemented
                    // client.SetLastWriteTime(path, atime.Value);
                }

                return NtStatus.Success;
            }
            catch (SftpPathNotFoundException)
            {
                return NtStatus.Error;
            }
            catch (SshConnectionException e)
            {
                _connectionError = true;
                Debug(e.ToString());
                Reconnect();
                return NtStatus.Error;
            }
        }

        public NtStatus DeleteFile(
            string filename,
            DokanFileInfo info)
        {
            Debug("DeleteFile {0}", filename);
            try
            {
                string path = GetPath(filename);
                var client = GetClient();
                client.DeleteFile(path);
                return NtStatus.Success;
            }
            catch (SftpPathNotFoundException)
            {
                return NtStatus.Error;
            }
            catch (SftpPermissionDeniedException)
            {
                return NtStatus.Error;
            }
            catch (SshConnectionException e)
            {
                _connectionError = true;
                Debug(e.ToString());
                Reconnect();
                return NtStatus.Error;
            }
        }

        public NtStatus DeleteDirectory(
            string filename,
            DokanFileInfo info)
        {
            Debug("DeleteDirectory {0}", filename);
            try
            {
                string path = GetPath(filename);
                var channel = GetClient();
                channel.DeleteDirectory(path);
                return NtStatus.Success;
            }
            catch (SftpPathNotFoundException)
            {
                return NtStatus.Error;
            }
            catch (SftpPermissionDeniedException)
            {
                return NtStatus.Error;
            }
            catch (SshConnectionException e)
            {
                _connectionError = true;
                Debug(e.ToString());
                Reconnect();
                return NtStatus.Error;
            }
        }

        public NtStatus MoveFile(
            string filename,
            string newname,
            bool replace,
            DokanFileInfo info)
        {
            Debug("MoveFile {0}", filename);
            try
            {
                string oldPath = GetPath(filename);
                string newPath = GetPath(newname);
                var client = GetClient();
                client.RenameFile(oldPath, newPath);
                return NtStatus.Success;
            }
            catch (SftpPermissionDeniedException)
            {
                return NtStatus.Error;
            }
            catch (SshConnectionException e)
            {
                _connectionError = true;
                Debug(e.ToString());
                Reconnect();
                return NtStatus.Error;
            }
        }

        public NtStatus SetEndOfFile(
            string filename,
            long length,
            DokanFileInfo info)
        {
            try
            {
                string path = GetPath(filename);
                var channel = GetClient();
                var attr = channel.GetAttributes(path);
                attr.Size = length;

                return NtStatus.Success;
            }
            catch (SftpPathNotFoundException)
            {
                return NtStatus.Error;
            }
            catch (SshConnectionException e)
            {
                _connectionError = true;
                Debug(e.ToString());
                Reconnect();
                return NtStatus.Error;
            }
        }

        public NtStatus SetAllocationSize(string filename, long length, DokanFileInfo info)
        {
            try
            {
                string path = GetPath(filename);
                var channel = GetClient();
                var attr = channel.GetAttributes(path);
                if (attr.Size < length)
                {
                    attr.Size = length;
                }
            }
            catch (SftpPathNotFoundException)
            {
                return NtStatus.Error;
            }
            catch (SshConnectionException e)
            {
                _connectionError = true;
                Debug(e.ToString());
                Reconnect();
                return NtStatus.Error;
            }
            return NtStatus.Success;
        }

        public NtStatus LockFile(
            string filename,
            long offset,
            long length,
            DokanFileInfo info)
        {
            return NtStatus.Success;
        }

        public NtStatus UnlockFile(
            string filename,
            long offset,
            long length,
            DokanFileInfo info)
        {
            return NtStatus.Success;
        }

        public NtStatus GetDiskFreeSpace(
                     out long freeBytesAvailable,
                     out long totalBytes,
                     out long totalFreeBytes,
                     DokanFileInfo info)
        {
            freeBytesAvailable = 1024L * 1024 * 1024 * 10;
            totalBytes = 1024L * 1024 * 1024 * 20;
            totalFreeBytes = 1024L * 1024 * 1024 * 10;
            return NtStatus.Success;
        }

        public NtStatus Mounted(
            DokanFileInfo info)
        {
            return NtStatus.NotImplemented;
        }

        public NtStatus Unmounted(
            DokanFileInfo info)
        {
            try
            {
                Debug("disconnection...");

                GetClient().Disconnect();

                Thread.Sleep(1000 * 1);

                //session_.disconnect();

                Debug("disconnected");
            }
            catch (Exception e)
            {
                Debug(e.ToString());
            }
            return NtStatus.Success;
        }

        public NtStatus GetFileSecurity(string fileName, out FileSystemSecurity security, AccessControlSections sections, DokanFileInfo info)
        {
            security = null;
            return NtStatus.Error;
        }

        public NtStatus SetFileSecurity(string fileName, FileSystemSecurity security, AccessControlSections sections, DokanFileInfo info)
        {
            return NtStatus.Error;
        }

        public NtStatus GetVolumeInformation(out string volumeLabel, out FileSystemFeatures features, out string fileSystemName, DokanFileInfo info)
        {
            volumeLabel = String.Empty;
            features = FileSystemFeatures.None;
            fileSystemName = String.Empty;
            return NtStatus.Error;
        }
    }
}
