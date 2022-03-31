using System;
using System.Reflection;
using Rug.Osc;

namespace bHapticsOSC.OpenSoundControl
{
    public static class OscManager
    {
        public delegate void OscAddressMethod(string address, OscMessage msg);
        private static OscAddressManager AddressBook = new OscAddressManager();
        private static OscReceiverHandler oscReceiver = new OscReceiverHandler();
        private static OscSenderHandler oscSender = new OscSenderHandler();

        static OscManager()
        {
            GrabOscAddressesFromAssembly(typeof(OscManager).Assembly);
            VRChatAvatar.SetupDevices();
        }

        public static void Connect()
        {
            oscSender.BeginInit();
            oscReceiver.BeginInit();
        }

        public static void Disconnect()
        {
            oscReceiver.EndInit();
            oscSender.EndInit();
        }

        public static void Attach(string address, OscAddressMethod oscMessageEvent)
            => AddressBook.Attach(address, (OscMessage msg) => { oscMessageEvent(address, msg); });

        public static void Send(OscPacket packet)
            => oscSender.Send(packet);

        public static OscPacketInvokeAction ShouldInvoke(OscPacket packet)
            => AddressBook.ShouldInvoke(packet);

        public static bool Invoke(OscPacket packet)
            => AddressBook.Invoke(packet);

        public static void GrabOscAddressesFromAssembly(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
                foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                {
                    // Parameter Check

                    foreach (OscAddressAttribute oscAddress in method.GetCustomAttributes<OscAddressAttribute>())
                    {
                        if (oscAddress.AddressBook == null)
                            continue;
                        if (oscAddress.AddressBook.Length <= 0)
                            continue;
                        foreach (string address in oscAddress.AddressBook)
                            Attach(address, (OscAddressMethod)method.CreateDelegate(typeof(OscAddressMethod)));
                    }
                }
        }
    }
}
