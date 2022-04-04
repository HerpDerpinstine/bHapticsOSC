using System;
using System.Linq;
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
            AttachOscAttributesFromAssembly(typeof(OscManager).Assembly);
            VRChatSupport.SetupDevices();
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

        public static void Send(OscPacket packet)
            => oscSender.Send(packet);

        public static void Attach(string address, OscMessageEvent oscMessageEvent)
            => AddressBook.Attach(address, oscMessageEvent);

        public static void Detach(string address, OscMessageEvent oscMessageEvent)
            => AddressBook.Detach(address, oscMessageEvent);

        public static OscPacketInvokeAction ShouldInvoke(OscPacket packet)
            => AddressBook.ShouldInvoke(packet);

        public static bool Invoke(OscPacket packet)
            => AddressBook.Invoke(packet);

        public static void AttachOscAttributesFromAssembly(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
                foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                {
                    ParameterInfo[] parameters = method.GetParameters();
                    foreach (IOscAddress oscAddress in method.GetCustomAttributes().Where(x => x.GetType().GetInterface("IOscAddress") != null))
                    {
                        string prefix = oscAddress.GetAddressPrefix();
                        string[] addressBook = oscAddress.GetAddressBook();

                        // To-Do: Parameter Check

                        if ((addressBook == null) || (addressBook.Length <= 0))
                            continue;

                        foreach (string address in addressBook)
                        {
                            if (string.IsNullOrEmpty(address))
                                continue;

                            string newAddress = address;
                            if (!string.IsNullOrEmpty(prefix))
                                newAddress = $"{prefix}/{address}";

                            Attach(newAddress, (OscMessage msg) =>
                            {
                                if (parameters.Length > 0)
                                {
                                    if (msg.Count != parameters.Length)
                                        return;

                                    if (msg.Count > 0)
                                        for (int i = 0; i < msg.Count; i++)
                                            if (msg[i].GetType() != parameters[i].ParameterType)
                                            {
                                                // To-Do: Log Information
                                                Console.WriteLine($"Parameter Mismatch for {newAddress}");
                                                return;
                                            }
                                }
                                method.Invoke(null, msg.ToArray());
                            });
                        }
                    }
                }
        }
    }
}
