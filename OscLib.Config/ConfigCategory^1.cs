using System;
using Tomlet;
using Tomlet.Exceptions;
using Tomlet.Models;

namespace OscLib.Config
{
    public class ConfigCategory<T> : ConfigCategory where T : ConfigCategoryValue
    {
        public T Value;

        public ConfigCategory(string name) : base(name) { }

        internal override void LoadDefaults() => Value = (T)Activator.CreateInstance(typeof(T));
        internal override void Load(TomlValue tomlValue)
        {
            try { Value = (T)TomletMain.To(typeof(T), tomlValue); }
            catch (TomlTypeMismatchException) { }
            catch (TomlNoSuchValueException) { }
            catch (TomlEnumParseException) { }
            Value?.Clamp();
        }

        internal override TomlValue Save()
        {
            if (Value == null)
                LoadDefaults();
            Value?.Clamp();
            return TomletMain.ValueFrom(typeof(T), Value);
        }
    }
}
