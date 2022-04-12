using System.Collections.ObjectModel;
using System.ComponentModel;
using AvatarEmulator.UI;

namespace AvatarEmulator.ViewModel
{
    public class MotorValueViewModel: INotifyPropertyChanged
    {
        public int VestMotorCount { get; } = 20;
        public int HeadMotorCount { get; } = 6;
        public int ArmMotorCount { get; } = 6;
        public int HandMotorCount { get; } = 3;
        
        public ObservableCollection<MotorValue> VestFrontMotors { get; set; }
        public ObservableCollection<MotorValue> VestBackMotors { get; set; }
        public ObservableCollection<MotorValue> HeadMotors { get; set; }
        public ObservableCollection<MotorValue> LeftArmMotors { get; set; }
        public ObservableCollection<MotorValue> RightArmMotors { get; set; }
        public ObservableCollection<MotorValue> LeftHandMotors { get; set; }
        public ObservableCollection<MotorValue> RightHandMotors { get; set; }
        public ObservableCollection<MotorValue> LeftFootMotors { get; set; }
        public ObservableCollection<MotorValue> RightFootMotors { get; set; }
        
        public RelayCommand ResetCommand { get; set; }

        public static MotorValueViewModel Instance;

        public MotorValueViewModel()
        {
            Instance = this;
            VestFrontMotors = Initialize(VestMotorCount, "Vest_Front");
            VestBackMotors = Initialize(VestMotorCount, "Vest_Back");
            HeadMotors = Initialize(HeadMotorCount, "Head");
            LeftArmMotors = Initialize(ArmMotorCount, "Arm_Left");
            RightArmMotors = Initialize(ArmMotorCount, "Arm_Right");
            LeftHandMotors = Initialize(HandMotorCount, "Hand_Left");
            RightHandMotors = Initialize(HandMotorCount, "Hand_Right");
            LeftFootMotors = Initialize(HandMotorCount, "Foot_Left");
            RightFootMotors = Initialize(HandMotorCount, "Foot_Right");
            
            ResetCommand = new RelayCommand(o =>
            {
                Reset(VestFrontMotors);
                Reset(VestBackMotors);
                Reset(HeadMotors);
                Reset(LeftArmMotors);
                Reset(RightArmMotors);
                Reset(LeftHandMotors);
                Reset(RightHandMotors);
                Reset(LeftFootMotors);
                Reset(RightFootMotors);
            });
        }


        private void Reset(ObservableCollection<MotorValue> motors)
        {
            foreach (var vestBackMotor in motors)
            {
                vestBackMotor.Reset();
            }
        }

        private ObservableCollection<MotorValue> Initialize(int count, string pos)
        {
            var motors =  new ObservableCollection<MotorValue>();

            for (int i = 0; i < count; i++)
            {
                motors.Add(new MotorValue()
                {
                    Index = i,
                    Value = 0,
                    Position = pos,
                });   
            }

            return motors;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}