using System;
using System.IO;
using System.Linq;
using System.Reflection;
using OscLib.Config;
using Rug.Osc;

namespace OscLib
{
    public static class OscManager
    {
        public static ConnectionConfig Connection;
        private static OscAddressManager AddressBook = new OscAddressManager();
        private static OscReceiverHandler oscReceiver = new OscReceiverHandler();
        private static OscSenderHandler oscSender = new OscSenderHandler();
        internal static OscPacketQueue oscPacketQueue = new OscPacketQueue();

        public static void Load()
        {
            Assembly baseAssembly = typeof(OscManager).Assembly;
            Connection = ConfigManager.CreateConfig<ConnectionConfig>(Path.GetDirectoryName(baseAssembly.Location), nameof(Connection));
        }

        public static void Connect()
        {
            oscSender.BeginInit();
            oscReceiver.BeginInit();
            oscPacketQueue.BeginInit();
        }

        public static void Disconnect()
        {
            oscReceiver.EndInit();
            oscSender.EndInit();
            oscPacketQueue.EndInit();
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
                    int parameterCount = parameters.Length;
                    foreach (IOscAddress oscAddress in method.GetCustomAttributes().Where(x => x.GetType().GetInterface("IOscAddress") != null))
                    {
                        string prefix = oscAddress.GetAddressPrefix();
                        string[] addressBook = oscAddress.GetAddressBook();

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
                                if ((msg == null) || (parameterCount <= 0))
                                {
                                    method.Invoke(null, new object[0]);
                                    return;
                                }

                                int msgCount = msg.Count;
                                if (msgCount <= 0)
                                {
                                    method.Invoke(null, new object[parameterCount]);
                                    return;
                                }

                                if (msgCount != parameterCount)
                                {
                                    Console.WriteLine($"Count Mismatch for {newAddress}  |  Expected {parameterCount}, Got {msgCount}");
                                    return;
                                }

                                for (int i = 0; i < msgCount; i++)
                                {
                                    Type msgType = msg[i].GetType();
                                    Type parameterType = parameters[i].ParameterType;
                                    if (msgType != parameterType)
                                    {
                                        Console.WriteLine($"Type Mismatch for {newAddress} at Parameter {i}  |  Expected {parameterType}, Got {msgType}");
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
