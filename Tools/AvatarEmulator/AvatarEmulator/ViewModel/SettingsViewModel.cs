using System;
using System.ComponentModel;
using AvatarEmulator.Properties;
using AvatarEmulator.UI;
using Rug.Osc;

namespace AvatarEmulator.ViewModel
{
    public class SettingsViewModel: INotifyPropertyChanged
    {
        public string Ip { get; set; }
        public OscSocketState State { get; set; }
        public RelayCommand StartCommand { get; set; }
        public RelayCommand StopCommand { get; set; }

        public SettingsViewModel()
        {
            Ip = Settings.Default.IP;

            AvatarActor.ConnectionChanged += () =>
            {
                State = AvatarActor.State;
                Console.WriteLine($"ConnectionChanged {AvatarActor.State}");
            };
            StartCommand = new RelayCommand(o =>
            {
                MotorValueViewModel.Instance?.ResetCommand.Execute(null);
                AvatarActor.Connect(Ip);
                Settings.Default.IP = Ip;
                Settings.Default.Save();
            });

            StopCommand = new RelayCommand(o =>
            {
                MotorValueViewModel.Instance?.ResetCommand.Execute(null);
                AvatarActor.EndInit();
            });

            State = AvatarActor.State;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}