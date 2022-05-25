using System;

namespace OscLib.VRChat.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class VRC_GestureRight : Attribute, IOscAddress
    {
        private string[] AddressBook;

        public VRC_GestureRight()
            => AddressBook = new string[] { "GestureRight" };

        public string GetAddressPrefix()
            => "/avatar/parameters";
        public string[] GetAddressBook()
            => AddressBook;
    }
}
