using System;
using System.Threading;
using bHapticsOSC.Config;
using bHapticsOSC.Utils;
using bHapticsOSC.OpenSoundControl;

namespace bHapticsOSC
{
    internal static class Program
    {
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
                ConfigManager.LoadAll();

                ConfigManager.Connection.OnFileModified = () =>
                {
                    Console.WriteLine();
                    Console.WriteLine("Connection.cfg Reloaded!");
                    Console.WriteLine();
                    PrintConnection();
                    OscManager.Connect();
                };

                ConfigManager.Devices.OnFileModified += () =>
                {
                    Console.WriteLine();
                    Console.WriteLine("Devices.cfg Reloaded!");
                    Console.WriteLine();
                    PrintDevices(true);
                };

                ConfigManager.VRChat.OnFileModified += () =>
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

                bHaptics.Load();
                OscManager.Connect();

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
                OscManager.Disconnect();
                bHaptics.Quit();
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
            Console.WriteLine($"[AFK] = {ConfigManager.VRChat.vrchat.Value.AFK}");
            Console.WriteLine($"[InStation] = {ConfigManager.VRChat.vrchat.Value.InStation}");
            Console.WriteLine($"[Seated] = {ConfigManager.VRChat.vrchat.Value.Seated}");
            Console.WriteLine();
            Console.WriteLine();
        }

        private static void PrintConnection()
        {
            Console.WriteLine($"===== OSC Receiver =====");
            Console.WriteLine();
            Console.WriteLine($"[Port] = {ConfigManager.Connection.receiver.Value.Port}");
            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine($"===== OSC Sender =====");
            Console.WriteLine();
            Console.WriteLine($"[Enabled] = {ConfigManager.Connection.sender.Value.Enabled}");
            Console.WriteLine($"[IP] = {ConfigManager.Connection.sender.Value.IP}");
            Console.WriteLine($"[Port] = {ConfigManager.Connection.sender.Value.Port}");
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

            PrintDevice("Head", bHaptics.PositionType.Head, isAudioLink);

            PrintDevice("Vest", bHaptics.PositionType.Vest, isAudioLink);

            PrintDevice("Arm Left", bHaptics.PositionType.ForearmL, isAudioLink);
            PrintDevice("Arm Right", bHaptics.PositionType.ForearmR, isAudioLink);

            PrintDevice("Hand Left", bHaptics.PositionType.HandL, isAudioLink);
            PrintDevice("Hand Right", bHaptics.PositionType.HandR, isAudioLink);

            PrintDevice("Foot Left", bHaptics.PositionType.FootL, isAudioLink);
            PrintDevice("Foot Right", bHaptics.PositionType.FootR, isAudioLink);

            Console.WriteLine();
        }

        private static void PrintDevice(string name, bHaptics.PositionType positionType, bool isAudioLink)
        {
            //Console.WriteLine($"[{name}  |  Enabled] = {(isAudioLink ? ConfigManager.VRChat.PositionTypeToUALEnabled(positionType) : ConfigManager.Devices.PositionTypeToEnabled(positionType))}");
            //Console.WriteLine($"[{name}  |  Intensity] = {(isAudioLink ? ConfigManager.VRChat.PositionTypeToUALIntensity(positionType) : ConfigManager.Devices.PositionTypeToIntensity(positionType))}");

            Console.WriteLine($"[{name}  |  Enabled] = {ConfigManager.Devices.PositionTypeToEnabled(positionType)}");
            Console.WriteLine($"[{name}  |  Intensity] = {ConfigManager.Devices.PositionTypeToIntensity(positionType)}");

            Console.WriteLine();
        }
    }
}
