using System.ComponentModel;
using AvatarEmulator.UI;
using Rug.Osc;

namespace AvatarEmulator.ViewModel
{
    public class MotorValue: INotifyPropertyChanged
    {
        public RelayCommand ToggleCommand { get; set; }
        public int Index { get; set; } = 0;
        public int Value { get; set; } = 0;
        public bool On { get; set; } = false;

        public string Position = "Arm_Left";

        public MotorValue()
        {
            ToggleCommand = new RelayCommand(ToggleValue);
        }

        private void ToggleValue(object obj)
        {
            if (Value > 0)
            {
                Value = 0;
                On = false;
            }
            else
            {
                Value = 100;
                On = true;
            }

            SendOSC();
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void Reset()
        {
            if (Value != 0)
            {
                Value = 0;
                On = false;
                SendOSC();
            }
        }

        private void SendOSC()
        {
                AvatarActor.Send(new OscMessage($"/avatar/parameters/bHaptics_{Position}_{Index+1}_bool", On));
        }
    }
}