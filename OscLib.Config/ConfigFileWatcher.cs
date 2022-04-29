using System.IO;

namespace OscLib.Config
{
    internal class ConfigFileWatcher
    {
        internal bool IgnoreEvents = false;
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
            Watcher = new FileSystemWatcher(Path.GetDirectoryName(filepath));

            Watcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size;

            Watcher.Changed += OnChanged;
            Watcher.Renamed += OnRenamed;
            Watcher.Error += OnError;

            Watcher.Filter = Path.GetFileName(filepath);
            Watcher.IncludeSubdirectories = true;
            Watcher.EnableRaisingEvents = true;
        }

        internal void EndInit()
        {
            if (Watcher == null)
                return;

            Watcher.Dispose();
            Watcher = null;
        }


        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (IgnoreEvents)
                return;

            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Changed:
                    File.Load();
                    File.OnFileModified_SafeInvoke();
                    break;

                case WatcherChangeTypes.Deleted:
                    File.Save();
                    break;

                default:
                    break;
            }
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            if (IgnoreEvents)
                return;
            File.Save();
        }

        private void OnError(object sender, ErrorEventArgs e)
            => throw e.GetException();
    }
}
