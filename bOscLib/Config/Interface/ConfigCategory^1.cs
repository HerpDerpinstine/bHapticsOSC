using System;
using Tomlet;
using Tomlet.Exceptions;
using Tomlet.Models;

namespace bOscLib.Config.Interface
{
    public class ConfigCategory<T> : ConfigCategory
    {
        public T Value;

        internal ConfigCategory(string name) : base(name) { }

        internal override void LoadDefaults() => Value = (T)Activator.CreateInstance(typeof(T));
        internal override void Load(TomlValue tomlValue)
        {
            try { Value = (T)TomletMain.To(typeof(T), tomlValue); }
            catch (TomlTypeMismatchException) { }
            catch (TomlNoSuchValueException) { }
            catch (TomlEnumParseException) { }
        }

        internal override TomlValue Save()
        {
            if (Value == null)
                LoadDefaults();

            return TomletMain.ValueFrom(typeof(T), Value);
        }
    }
}
