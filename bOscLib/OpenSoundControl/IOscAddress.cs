using System;

namespace bHapticsOSC.OpenSoundControl
{
    public interface IOscAddress
    {
        public abstract string GetAddressPrefix();
        public abstract string[] GetAddressBook();
    }
}
