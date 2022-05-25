using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Tomlet;
using Tomlet.Exceptions;
using Tomlet.Models;

namespace OscLib.Config
{
    public class ConfigFile
    {
        private static int UpdateRate = 100; // ms
        private ConfigFileWatcher Watcher;
        private TomlDocument Document = TomlDocument.CreateEmpty();
        public List<ConfigCategory> Categories = new List<ConfigCategory>();

        public Action OnFileModified;
        public void OnFileModified_SafeInvoke()
            => OnFileModified?.Invoke();

        private string FilePath;
        public string GetFilePath()
            => FilePath;

        public ConfigFile(string filepath, bool useWatcher = true)
        {
            if (string.IsNullOrEmpty(filepath))
                throw new NullReferenceException(filepath);

            FilePath = filepath;
            if (useWatcher)
                Watcher = new ConfigFileWatcher(this);

            ConfigManager.AllConfigFiles.Add(this);
        }

        public void Load()
        {
            if (string.IsNullOrEmpty(FilePath))
                return;
            if (!File.Exists(FilePath))
            {
                Save();
                return;
            }

            if (Watcher != null)
                Watcher.IgnoreEvents = true;
            Thread.Sleep(UpdateRate);

            // Exception Catching
            Document = TomlParser.ParseFile(FilePath);

            Thread.Sleep(UpdateRate);
            if (Watcher != null)
                Watcher.IgnoreEvents = false;

            if (Categories.Count > 0)
                foreach (ConfigCategory category in Categories)
                {
                    if (!(TryGetCategoryTable(category.Name) is { } table))
                    {
                        category.LoadDefaults();
                        continue;
                    }

                    category.Load(table);
                }
        }

        public void Save()
        {
            if (string.IsNullOrEmpty(FilePath))
                return;

            if (Categories.Count > 0)
                foreach (ConfigCategory category in Categories)
                    Document.PutValue(category.Name, category.Save());

            if (Watcher != null)
                Watcher.IgnoreEvents = true;
            Thread.Sleep(UpdateRate);
            File.WriteAllText(FilePath, Document.SerializedValue);
            Thread.Sleep(UpdateRate);
            if (Watcher != null)
                Watcher.IgnoreEvents = false;
        }

        private TomlTable TryGetCategoryTable(string category)
        {
            lock (Document)
            {
                try { return Document.GetSubTable(category); }
                catch (TomlTypeMismatchException) { }
                catch (TomlNoSuchValueException) { }

                return null;
            }
        }

        private static string QuoteKey(string key) =>
          key.Contains('"')
              ? $"'{key}'"
              : $"\"{key}\"";

        internal void InsertIntoDocument(string category, string key, TomlValue value, bool should_inline = false)
        {
            if (!Document.ContainsKey(category))
                Document.PutValue(category, new TomlTable());

            try
            {
                var categoryTable = Document.GetSubTable(category);
                categoryTable.ForceNoInline = !should_inline;
                categoryTable.PutValue(QuoteKey(key), value);
            }
            catch (TomlTypeMismatchException) { }
            catch (TomlNoSuchValueException) { }
        }
    }
}
