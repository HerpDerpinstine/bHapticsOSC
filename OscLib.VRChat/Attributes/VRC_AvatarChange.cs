using System;

namespace OscLib.VRChat.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class VRC_AvatarChange : Attribute, IOscAddress
    {
        private string[] AddressBook;

        public VRC_AvatarChange()
            => AddressBook = new string[] { "change" };

        public string GetAddressPrefix()
            => "/avatar";
        public string[] GetAddressBook()
            => AddressBook;
    }
}
