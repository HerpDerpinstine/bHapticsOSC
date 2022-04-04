﻿using System;
using bHapticsOSC.OpenSoundControl;

namespace bHapticsOSC.VRChat
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class VRC_GestureLeft : Attribute, IOscAddress
    {
        private string[] AddressBook;

        public VRC_GestureLeft()
            => AddressBook = new string[] { "GestureLeft" };

        public string GetAddressPrefix()
            => "/avatar/parameters";
        public string[] GetAddressBook()
            => AddressBook;
    }
}
