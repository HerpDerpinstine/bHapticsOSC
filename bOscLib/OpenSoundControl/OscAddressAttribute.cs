using System;
using System.Collections.Generic;

namespace bHapticsOSC.OpenSoundControl
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class OscAddressAttribute : Attribute
    {
        public string[] AddressBook;

        public OscAddressAttribute(string address) : this(new string[] { address }) { }
        public OscAddressAttribute(string[] addressbook)
            => AddressBook = addressbook;

        public OscAddressAttribute(string address, int variantcount)
        {
            List<string> addressbook = new List<string>();
            for (int i = 1; i < variantcount + 1; i++)
                addressbook.Add($"{address}_{i}");
            AddressBook = addressbook.ToArray();
        }

        public OscAddressAttribute(string address, int variantcount, string suffix)
        {
            List<string> addressbook = new List<string>();
            for (int i = 1; i < variantcount + 1; i++)
                addressbook.Add($"{address}_{i}_{suffix}");
            AddressBook = addressbook.ToArray();
        }
    }
}
