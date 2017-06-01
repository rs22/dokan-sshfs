using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;

using DokanNet;
using System.Security.AccessControl;
using Renci.SshNet;
using Renci.SshNet.Common;
using System.Diagnostics;

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
        private string _root;
        private string _passphrase;
        private string _password;

        private int _trycount = 0;
        private bool _connectionError = false;
        private object _reconnectLock = new object();

        public void Initialize(string user, string host, int port, string password, string identity, string passphrase, string root)
        {
            _user = user;
            _host = host;
            _port = port;
            _identity = identity;
            _password = password;
            _passphrase = passphrase;

            _root = root;
        }

        private void DebugWrite(string format, params object[] args)
        {
            if (true)
            {
                Debug.WriteLine(string.Format("SSHFS: " + format, args));
            }
        }

        internal bool SSHConnect()
        {
            try
            {
                var credentials = !string.IsNullOrEmpty(_password) 
                    ? (AuthenticationMethod)new PasswordAuthenticationMethod(_user, _password)
                    : new PrivateKeyAuthenticationMethod(_user, new PrivateKeyFile(_identity, _passphrase));
                var connectionInfo = new ConnectionInfo(_host, _port, _user, credentials);
                
                _client = new SftpClient(connectionInfo);
                _client.Connect();
                return true;
            }
            catch (Exception e)
            {
                DebugWrite(e.ToString());
                return false;
            }
        }

        private bool Reconnect()
        {
            lock (_reconnectLock)
            {
                if (!_connectionError)
                    return true;

                DebugWrite("Disconnect current sessions\n");
                try
                {
                    GetClient().Disconnect();
                }
                catch (Exception e)
                {
                    DebugWrite(e.ToString());
                }

                DebugWrite("Reconnect {0}\n", _trycount);

                _trycount++;

                if (SSHConnect())
                {
                    DebugWrite("Reconnect success\n");
                    _connectionError = false;
                    return true;
                }
                else
                {
                    DebugWrite("Reconnect failed\n");
                    return false;
                }
            }
        }

        private string GetPath(string filename)
        {
            string path = _root + filename.Replace('\\', '/');
            DebugWrite("GetPath : {0} thread {1}", path, Thread.CurrentThread.ManagedThreadId);
            //Debug("  Stack {0}", new System.Diagnostics.StackTrace().ToString());
            return path;
        }

        private SftpClient GetClient()
        {
            return _client;
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
                        DebugWrite("OpenDirectory {0}", filename);
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
                            DebugWrite(e.ToString());
                            return NtStatus.ObjectPathNotFound;
                        }
                        catch (SshConnectionException e)
                        {
                            _connectionError = true;
                            DebugWrite(e.ToString());
                            Reconnect();
                            return NtStatus.ObjectPathNotFound;
                        }


                    case FileMode.CreateNew:
                        DebugWrite("CreateDirectory {0}", filename);
                        try
                        {
                            var client = GetClient();

                            client.CreateDirectory(path);
                            return NtStatus.Success;
                        }
                        catch (SftpPermissionDeniedException e)
                        {
                            DebugWrite(e.ToString());
                            return NtStatus.Error;
                        }
                        catch (SshConnectionException e)
                        {
                            _connectionError = true;
                            DebugWrite(e.ToString());
                            Reconnect();
                            return NtStatus.Error; // TODO: more appropriate error code
                        }
                    default:
                        DebugWrite("Error FileMode invalid for directory {0}", mode);
                        return NtStatus.Error;

                }
            }
            else
            {
                DebugWrite("CreateFile {0}", filename);
                try
                {
                    var client = GetClient();

                    if (CheckAltStream(path))
                        return NtStatus.Success;

                    switch (mode)
                    {
                        case FileMode.Open:
                            {
                                DebugWrite("Open");
                                if (PathExists(path, info))
                                    return NtStatus.Success;
                                else
                                    return NtStatus.ObjectNameNotFound;
                            }
                        case FileMode.CreateNew:
                            {
                                DebugWrite("CreateNew");
                                if (PathExists(path, info))
                                    return NtStatus.ObjectNameCollision;

                                DebugWrite("CreateNew put 0 byte");
                                client.Create(path).Close();
                                return NtStatus.Success;
                            }
                        case FileMode.Create:
                            {
                                DebugWrite("Create put 0 byte");
                                client.Create(path).Close();
                                return NtStatus.Success;
                            }
                        case FileMode.OpenOrCreate:
                            {
                                DebugWrite("OpenOrCreate");

                                if (!PathExists(path, info))
                                {
                                    DebugWrite("OpenOrCreate put 0 byte");
                                    client.Create(path).Close();
                                }
                                return NtStatus.Success;
                            }
                        case FileMode.Truncate:
                            {
                                DebugWrite("Truncate");

                                if (!PathExists(path, info))
                                    return NtStatus.ObjectNameNotFound;

                                DebugWrite("Truncate put 0 byte");
                                client.Create(path).Close();
                                return NtStatus.Success;
                            }
                        case FileMode.Append:
                            {
                                DebugWrite("Append");

                                if (PathExists(path, info))
                                    return NtStatus.Success;

                                DebugWrite("Append put 0 byte");
                                client.Create(path).Close();
                                return NtStatus.Success;
                            }
                        default:
                            DebugWrite("Error unknown FileMode {0}", mode);
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
                    DebugWrite(e.ToString());
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

            DebugWrite("ReadFile {0} bufferLen {1} Offset {2}", filename, buffer.Length, offset);
            try
            {
                var client = GetClient();
                using (var stream = client.OpenRead(path))
                {
                    var position = stream.Seek(offset, SeekOrigin.Begin);
                    readBytes = stream.Read(buffer, 0, buffer.Length);
                }
                DebugWrite("  ReadFile readBytes: {0}", readBytes);
                return NtStatus.Success;
            }
            //catch (SftpException)
            //{
            //    return NtStatus.Error;
            //}
            catch (Exception e)
            {
                _connectionError = true;
                DebugWrite(e.ToString());
                Reconnect();
                return NtStatus.Error;
            }
        }

        public NtStatus WriteFile(
            string filename,
            byte[] buffer,
            out int writtenBytes,
            long offset,
            DokanFileInfo info)
        {
            DebugWrite("WriteFile {0} bufferLen {1} Offset {2}", filename, buffer.Length, offset);

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
                //Tamir.SharpSsh.java.io.OutputStream stream = channel.put(path, null, 3 /*HACK: 存在しないモード */, offset);
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
                DebugWrite(e.ToString());
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

                //if (DokanSSHFS.UseOffline)
                //    fileinfo.Attributes |= FileAttributes.Offline;

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
                DebugWrite(e.ToString());
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
            DebugWrite("FindFiles {0}", filename);

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

                    //if (DokanSSHFS.UseOffline)
                    //    fi.Attributes |= FileAttributes.Offline;
                    
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
                DebugWrite(e.ToString());
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
            DebugWrite("SetFileAttributes {0}", filename);
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
                DebugWrite(e.ToString());
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
            DebugWrite("SetFileTime {0}", filename);
            try
            {
                DebugWrite(" filetime {0} {1} {2}", ctime.HasValue ? ctime.ToString() : "-", atime.HasValue ? atime.ToString() : "-", mtime.HasValue ? mtime.ToString() : "-");

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
                DebugWrite(e.ToString());
                Reconnect();
                return NtStatus.Error;
            }
        }

        public NtStatus DeleteFile(
            string filename,
            DokanFileInfo info)
        {
            DebugWrite("DeleteFile {0}", filename);
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
                DebugWrite(e.ToString());
                Reconnect();
                return NtStatus.Error;
            }
        }

        public NtStatus DeleteDirectory(
            string filename,
            DokanFileInfo info)
        {
            DebugWrite("DeleteDirectory {0}", filename);
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
                DebugWrite(e.ToString());
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
            DebugWrite("MoveFile {0}", filename);
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
                DebugWrite(e.ToString());
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
                DebugWrite(e.ToString());
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
                DebugWrite(e.ToString());
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
                DebugWrite("disconnection...");

                GetClient().Disconnect();

                DebugWrite("disconnected");
            }
            catch (Exception e)
            {
                DebugWrite(e.ToString());
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
