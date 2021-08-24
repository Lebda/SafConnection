using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace SafConnection
{
    public class ThirdPartySafConnector
    {
        public void ConnectSAF(FileInfo info, Action<string> callBack)
        {
            if (info?.Exists != true)
            {
                return;
            }
            Process process = new Process
            {
                StartInfo =
                {
                    FileName = info.FullName,
                    CreateNoWindow = false,
                    UseShellExecute = true,
                }
            };
            process.Start();
            Thread.Sleep(1000);
            WatchSAF(info, callBack);
        }

        private void WatchSAF(FileInfo info, Action<string> callBack)
        {
            var watcherSaf = CreateWatcher(info);
            watcherSaf.EnableRaisingEvents = true;
            var lastAccessTime = info.LastAccessTimeUtc;
            watcherSaf.Changed += (source, args) =>
            {
                try
                {
                    Log("File change ......");
                    var fileActual = new FileInfo(args.FullPath);
                    if (fileActual.Exists && lastAccessTime == fileActual.LastAccessTimeUtc)
                    {
                        return;
                    }
                    lastAccessTime = fileActual.LastAccessTimeUtc;
                    var newFileInfo = new FileInfo(Path.Combine(info.DirectoryName, "SAF_WATCH", $"{Guid.NewGuid().ToString("N")}.xlsx"));
                    if (!newFileInfo.Directory.Exists)
                    {
                        newFileInfo.Directory.Create();
                    }
                    info.CopyTo(newFileInfo.FullName);
                    Log("File copy ......");
                    while (!newFileInfo.Exists)
                    {
                        Thread.Sleep(100);
                    }
                    Thread.Sleep(100);
                    if (newFileInfo.Exists)
                    {
                        Log("Calling API ......");
                        callBack.Invoke(newFileInfo.FullName);
                    }
                }
                catch (Exception e)
                {
                    Log(e.Message);
                }
            };
        }

        private void Log(string msg)
        {
            Console.WriteLine(msg);
        }


        private static FileSystemWatcher CreateWatcher(FileInfo info)
        {
            return new FileSystemWatcher()
            {
                Path = info.DirectoryName,
                NotifyFilter = NotifyFilters.Attributes |
                               NotifyFilters.CreationTime |
                               NotifyFilters.FileName |
                               NotifyFilters.LastAccess |
                               NotifyFilters.LastWrite |
                               NotifyFilters.Size |
                               NotifyFilters.Security,
                Filter = info.Name,
            };
        }
    }
}
