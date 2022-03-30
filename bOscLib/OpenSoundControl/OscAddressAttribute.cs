using System;

namespace bHapticsOSC.OpenSoundControl
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class OscAddressAttribute : Attribute
    {
        public string[] AddressBook;

        public OscAddressAttribute(string address) : this(new string[] { address }) { }
        public OscAddressAttribute(string[] addressbook)
            => AddressBook = addressbook;
    }
}
