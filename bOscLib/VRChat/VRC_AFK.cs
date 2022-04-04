using System;
using bHapticsOSC.OpenSoundControl;

namespace bHapticsOSC.VRChat
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class VRC_AFK : Attribute, IOscAddress
    {
        private string[] AddressBook;

        public VRC_AFK()
            => AddressBook = new string[] { "AFK" };

        public string GetAddressPrefix()
            => "/avatar/parameters";
        public string[] GetAddressBook()
            => AddressBook;
    }
}
