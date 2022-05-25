using System;

namespace OscLib.VRChat.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class VRC_AvatarParameter : Attribute, IOscAddress
    {
        private string[] AddressBook;

        public VRC_AvatarParameter(params string[] parameters)
            => AddressBook = parameters;

        public string GetAddressPrefix()
            => "/avatar/parameters";
        public string[] GetAddressBook()
            => AddressBook;
    }
}
