using System;
using bHapticsOSC.OpenSoundControl;

namespace bHapticsOSC.VRChat
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class VRC_InStation : Attribute, IOscAddress
    {
        private string[] AddressBook;

        public VRC_InStation()
            => AddressBook = new string[] { "InStation" };

        public string GetAddressPrefix()
            => "/avatar/parameters";
        public string[] GetAddressBook()
            => AddressBook;
    }
}
