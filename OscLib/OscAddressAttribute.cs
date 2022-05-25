using System;

namespace OscLib
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class OscAddressAttribute : Attribute, IOscAddress
    {
        private string[] AddressBook;

        public OscAddressAttribute(params string[] addressbook)
            => AddressBook = addressbook;

        public string GetAddressPrefix()
            => null;
        public string[] GetAddressBook()
            => AddressBook;
    }
}
