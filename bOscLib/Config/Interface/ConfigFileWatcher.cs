using System;
using System.IO;

namespace bHapticsOSC.Config.Interface
{
    internal class ConfigFileWatcher
    {
        private ConfigFile File;
        private FileSystemWatcher Watcher;

        internal ConfigFileWatcher(ConfigFile config)
        {
            File = config;
            BeginInit();
        }

        internal void BeginInit()
        {
            if (Watcher != null)
                return;

            string filepath = File.GetFilePath();
            Watcher = new FileSystemWatcher(Path.GetDirectoryName(filepath), Path.GetFileName(filepath))
            {
                NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.Size,
                EnableRaisingEvents = true
            };

            Watcher.Changed += new FileSystemEventHandler(OnFileWatcherTriggered);
            Watcher.BeginInit();
        }

        internal void EndInit()
        {
            if (Watcher == null)
                return;

            Watcher.EndInit();
            Watcher.Dispose();
            Watcher = null;
        }

        private void OnFileWatcherTriggered(object source, FileSystemEventArgs e)
        {
            File.Load();
        }
    }
}
