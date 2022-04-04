﻿using System;
using bHapticsOSC.OpenSoundControl;

namespace bHapticsOSC.VRChat
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class VRC_Seated : Attribute, IOscAddress
    {
        private string[] AddressBook;

        public VRC_Seated()
            => AddressBook = new string[] { "Seated" };

        public string GetAddressPrefix()
            => "/avatar/parameters";
        public string[] GetAddressBook()
            => AddressBook;
    }
}
