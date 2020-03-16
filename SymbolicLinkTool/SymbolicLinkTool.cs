using System.IO;
using System.Diagnostics;
using UnityEditor;
using System;
using System.Text;

namespace ysk
{
    using Debug = UnityEngine.Debug;
    public class SymbolicLinkTool
    {
        // NOTE:
        // WindowsはCMDを管理者権限付きで開いたうえでbat実行を行わなければいけない
        private readonly static string COMMAND_MAKE_SYMLINK =
#if UNITY_EDITOR_WIN
            Environment.GetEnvironmentVariable("ComSpec");
#else
            "bash";
#endif

        // NOTE:
        // Mac、Unix系はディレクトリ間でもファイル間でもコマンドは同一
        private const string ARGS_MAKE_SYMLINK_DIR =
#if UNITY_EDITOR_WIN
            "/c {2}\\CreateSymbolicLink.bat /D \"{0}\" \"{1}\"";
#else
             "./CreateSymbolicLink.sh \"{1}\" \"{0}\"";
#endif
        private const string ARGS_MAKE_SYMLINK_FILE =
#if UNITY_EDITOR_WIN
            "/c {2}\\CreateSymbolicLink.bat \"{0}\" \"{1}\"";
#else
             "./CreateSymbolicLink.sh \"{1}\" \"{0}\"";
#endif
        
        public static bool CreateSymbolicLinkBetweenFolders(string from, string to)
        {
            var dirInfo = new DirectoryInfo(from);
            if (dirInfo.Exists)
            {
                Debug.LogError(from + " is existed. please retry after removing directory.");
                return false;
            }

            dirInfo = new DirectoryInfo(to);
            if (!dirInfo.Exists)
            {
                Debug.LogError(to + " is not found.");
                return false;
            }

            return CreateSymbolicLink(from, to, true);
        }

        public static bool CreateSymbolicLinkBetweenFiles(string from, string to)
        {
            var fileInfo = new FileInfo(from);
            if (fileInfo.Exists)
            {
                Debug.LogError(from + " is existed. please retry after removing file.");
                return false;
            }

            fileInfo = new FileInfo(to);
            if (!fileInfo.Exists)
            {
                Debug.LogError(to + " is not found.");
                return false;
            }

            return CreateSymbolicLink(from, to, false);
        }

        public static bool CreateSymbolicLink(string from, string to, bool betweenFolders)
        {
            string currentDirectory = Path.GetDirectoryName(new StackTrace(true).GetFrame(0).GetFileName());
            Process process = new Process();

            string fromRegulared = from.Replace("\\", "/");
            string toRegulared = to.Replace("\\", "/");
            process.StartInfo.WorkingDirectory = currentDirectory;
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.FileName = COMMAND_MAKE_SYMLINK;
#if UNITY_EDITOR_WIN
            process.StartInfo.Verb = "RunAs";
#endif
            if (betweenFolders)
            {
                process.StartInfo.Arguments = string.Format(ARGS_MAKE_SYMLINK_DIR, fromRegulared, toRegulared, currentDirectory);
            }
            else
            {
                process.StartInfo.Arguments = string.Format(ARGS_MAKE_SYMLINK_FILE, fromRegulared, toRegulared);
            }

#if UNITY_EDITOR_WIN
            process.StartInfo.FileName = process.StartInfo.FileName.Replace("/", "\\");
#endif
            try
            {
                process.Start();
                process.WaitForExit();
                process.Close();
            }
            catch (System.ComponentModel.Win32Exception e)
            {
                Debug.LogException(e);
                Debug.Log(e.NativeErrorCode);
            }

            return (betweenFolders) ? Directory.Exists(from) : File.Exists(from);
        }

        public static bool IsSymbolicLinkFolder(string path)
        {
            try
            {
                var info = new DirectoryInfo(path);
                return (info.Attributes & FileAttributes.ReparsePoint) != 0;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return false;
        }

        public static bool IsSymbolicLinkFile(string path)
        {
            var info = new FileInfo(path);
            return (info.Attributes & FileAttributes.ReparsePoint) != 0;
        }

        public static bool RemoveSymbolicLinkFolder(string from)
        {
            if (IsSymbolicLinkFolder(from))
            {
                try
                {
#if UNITY_EDITOR_WIN
                    string currentDirectory = Path.GetDirectoryName(new StackTrace(true).GetFrame(0).GetFileName());
                    Process process = new Process();

                    string fromRegulared = from.Replace("\\", "/");
                    process.StartInfo.FileName = COMMAND_MAKE_SYMLINK;
                    process.StartInfo.Arguments = string.Format("/c \"rmdir /Q \"{0}\"", from);
                    
                    process.Start();
                    process.WaitForExit();
                    process.Close();
#else
                    Directory.Delete(from);
#endif
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    return false;
                }
            }

            return true;
        }


        public static bool RemoveSymbolicLinkFile(string from)
        {
            if (IsSymbolicLinkFile(from))
            {
                try
                {
#if UNITY_STANDALONE_WIN
                    string currentDirectory = Path.GetDirectoryName(new StackTrace(true).GetFrame(0).GetFileName());
                    Process process = new Process();

                    string fromRegulared = from.Replace("\\", "/");
                    process.StartInfo.FileName = COMMAND_MAKE_SYMLINK;
                    process.StartInfo.Arguments = string.Format("/c \"del \"{0}\"", from);
                    
                    process.Start();
                    process.WaitForExit();
                    process.Close();
#else
                    File.Delete(from);
#endif
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    return false;
                }
            }

            return true;
        }
    }
}