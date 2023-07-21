using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace rpa_robot.Classes
{
    internal class Watcher
    {
        public delegate void FileCreatedEventHandler(object sender, EventArgs e);
        public event FileCreatedEventHandler FileCreated;

        public FileSystemWatcher watcher = new FileSystemWatcher();
        public void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            Log.Information($"FileCreated: {e.FullPath}");

            // Wait for the file to be completely downloaded
            bool isFileDownloaded = false;
            int timeoutSeconds = 30;
            int elapsedSeconds = 0;
            string filePath = e.FullPath;

            while (!isFileDownloaded && elapsedSeconds < timeoutSeconds)
            {
                if (File.Exists(filePath))
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    if (fileInfo.Length > 0)
                    {
                        isFileDownloaded = true;
                    }
                }

                Thread.Sleep(1000);
                elapsedSeconds++;
            }
            if (isFileDownloaded)
            {
                // File is completely downloaded
                Log.Information($"File downloaded: {e.FullPath}");
                Log.Information($"FileCreated: {e.FullPath}");
                Helper.DeleteeWorkFlow(Globals.destinationFilePath);
                //Helper.WorkFlowCreated = true;
                OnFileCreated();
            }
            else
            {
                // File download timed out or encountered an error
                Log.Warning($"File download failed: {e.FullPath}");
                // Handle the failure or cleanup operations
            }

        }
        protected virtual void OnFileCreated()
        {
            if (FileCreated != null)
            { FileCreated(this, EventArgs.Empty); }
        }
    }

}
