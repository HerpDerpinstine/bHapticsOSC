using System;
using System.IO;
using System.Threading;
using bHapticsLib;
using OscLib;
using OscLib.Config;

namespace bHapticsOSC
{
    internal static class Program
    {
        internal static DevicesConfig Devices;
        internal static VRChatConfig VRChat;
        internal static VRChatSupport VRCSupport = new VRChatSupport();

        static Program()
        {
            string basefolder = Path.GetDirectoryName(typeof(Program).Assembly.Location);
            Devices = ConfigManager.CreateConfig<DevicesConfig>(basefolder, nameof(Devices));
            VRChat = ConfigManager.CreateConfig<VRChatConfig>(basefolder, nameof(VRChat));
        }

        internal static int Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += ProcessExit;

            bool isFirst;
            Mutex mutex = new Mutex(true, BuildInfo.Name, out isFirst);
            if (!isFirst)
                return 0;

            WelcomeMessage();
            
            try
            {
                OscManager.Load();
                ConfigManager.LoadAll();

                Action originalConnectionAct = OscManager.Connection.OnFileModified;
                OscManager.Connection.OnFileModified = () =>
                {
                    Console.WriteLine();
                    Console.WriteLine("Connection.cfg Reloaded!");
                    Console.WriteLine();
                    PrintConnection();
                    originalConnectionAct();
                };

                Action originalPassthroughAct = OscManager.Passthrough.OnFileModified;
                OscManager.Passthrough.OnFileModified = () =>
                {
                    Console.WriteLine();
                    Console.WriteLine("Passthrough.cfg Reloaded!");
                    Console.WriteLine();
                    PrintPassthrough();
                    originalPassthroughAct();
                };

                Devices.OnFileModified += () =>
                {
                    Console.WriteLine();
                    Console.WriteLine("Devices.cfg Reloaded!");
                    Console.WriteLine();
                    PrintDevices(true);
                };

                VRChat.OnFileModified += () =>
                {
                    Console.WriteLine();
                    Console.WriteLine("VRChat.cfg Reloaded!");
                    Console.WriteLine();
                    PrintVRChat();
                };

                PrintConnection();
                PrintDevices(false);

                PrintVRChat();
                //PrintUdonAudioLink();

                bHapticsManager.Load(BuildInfo.Name, BuildInfo.Name);

                OscManager.AttachOscAttributesFromAssembly(typeof(Program).Assembly);
                OscManager.Connect();
                VRCSupport.BeginInit();

                Console.WriteLine();
                Console.WriteLine("Awaiting Packets...");
                Console.WriteLine();
                Console.WriteLine("Please leave this application open to handle OSC Communication.");
                Console.WriteLine("Press ESC to Exit.");
                Console.WriteLine();

                while (Console.ReadKey(true).Key != ConsoleKey.Escape)
                    Thread.Sleep(1);
            }
            catch (Exception ex) { ErrorMessageBox(ex.ToString()); }

            Environment.Exit(0);
            return 0;
        }

        private static void ErrorMessageBox(string msg)
        {
            // MessageBox.Show(msg, "bHapticsOSC ~ ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Console.Error.WriteLine(msg);
        }


        private static void ProcessExit(object sender, EventArgs e)
        {
            try
            {
                VRCSupport.EndInit();
                OscManager.Disconnect();
                //bHapticsManager.Quit();
                ConfigManager.SaveAll();
            }
            catch (Exception ex) { ErrorMessageBox(ex.ToString()); }
        }

        private static void WelcomeMessage()
        {
            Console.WriteLine(Console.Title = $"{BuildInfo.Name} v{BuildInfo.Version}");
            Console.WriteLine($"Created by Herp Derpinstine");
            Console.WriteLine();
            Console.WriteLine();
        }

        private static void PrintVRChat()
        {
            Console.WriteLine($"===== VRChat =====");
            Console.WriteLine();
            Console.WriteLine($"[AFK] = {VRChat.vrchat.Value.AFK}");
            Console.WriteLine($"[InStation] = {VRChat.vrchat.Value.InStation}");
            Console.WriteLine($"[Seated] = {VRChat.vrchat.Value.Seated}");
            Console.WriteLine();
            Console.WriteLine();
        }

        private static void PrintConnection()
        {
            Console.WriteLine($"===== OSC Receiver =====");
            Console.WriteLine();
            Console.WriteLine($"[Port] = {OscManager.Connection.receiver.Value.Port}");
            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine($"===== OSC Sender =====");
            Console.WriteLine();
            Console.WriteLine($"[Enabled] = {OscManager.Connection.sender.Value.Enabled}");
            Console.WriteLine($"[IP] = {OscManager.Connection.sender.Value.IP}");
            Console.WriteLine($"[Port] = {OscManager.Connection.sender.Value.Port}");
            Console.WriteLine();
            Console.WriteLine();
        }

        private static void PrintPassthrough()
        {
            Console.WriteLine($"===== Passthrough Receiver =====");
            Console.WriteLine();
            Console.WriteLine($"[Enabled] = {OscManager.Passthrough.receiver.Value.Enabled}");
            Console.WriteLine($"[Port] = {OscManager.Passthrough.receiver.Value.Port}");
            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine($"===== Passthrough Sender =====");
            Console.WriteLine();
            Console.WriteLine($"[Enabled] = {OscManager.Passthrough.sender.Value.Enabled}");
            Console.WriteLine($"[IP] = {OscManager.Passthrough.sender.Value.IP}");
            Console.WriteLine($"[Port] = {OscManager.Passthrough.sender.Value.Port}");
            Console.WriteLine();
            Console.WriteLine();
        }

        /*
        private static void PrintUdonAudioLink()
        {
            Console.WriteLine($"===== Udon AudioLink =====");
            Console.WriteLine();
            Console.WriteLine($"[OverrideTouch] = {ConfigManager.VRChat.udonAudioLink.Value.OverrideTouch}");
            Console.WriteLine();
            PrintDevices(true);
        }
        */

        private static void PrintDevices(bool isAudioLink)
        {
            if (!isAudioLink)
            {
                Console.WriteLine($"===== Devices =====");
                Console.WriteLine();
            }

            PrintDevice("Head", PositionType.Head, isAudioLink);

            PrintDevice("Vest", PositionType.Vest, isAudioLink);

            PrintDevice("Arm Left", PositionType.ForearmL, isAudioLink);
            PrintDevice("Arm Right", PositionType.ForearmR, isAudioLink);

            PrintDevice("Hand Left", PositionType.HandL, isAudioLink);
            PrintDevice("Hand Right", PositionType.HandR, isAudioLink);

            PrintDevice("Foot Left", PositionType.FootL, isAudioLink);
            PrintDevice("Foot Right", PositionType.FootR, isAudioLink);

            Console.WriteLine();
        }

        private static void PrintDevice(string name, PositionType positionType, bool isAudioLink)
        {
            //Console.WriteLine($"[{name}  |  Enabled] = {(isAudioLink ? ConfigManager.VRChat.PositionTypeToUALEnabled(positionType) : ConfigManager.Devices.PositionTypeToEnabled(positionType))}");
            //Console.WriteLine($"[{name}  |  Intensity] = {(isAudioLink ? ConfigManager.VRChat.PositionTypeToUALIntensity(positionType) : ConfigManager.Devices.PositionTypeToIntensity(positionType))}");

            Console.WriteLine($"[{name}  |  Enabled] = {Devices.PositionTypeToEnabled(positionType)}");
            Console.WriteLine($"[{name}  |  Intensity] = {Devices.PositionTypeToIntensity(positionType)}");

            Console.WriteLine();
        }
    }
}
