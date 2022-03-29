using Tomlet.Models;

namespace bOscLib.Config.Interface
{
    public abstract class ConfigCategory
    {
        public string Name;

        internal ConfigCategory(string name)
            => Name = name;

        internal abstract void Load(TomlValue tomlValue);
        internal abstract void LoadDefaults();
        internal abstract TomlValue Save();
    }
}
