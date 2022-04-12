using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AvatarEmulator.ViewModel;
using Rug.Osc;

namespace AvatarEmulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ClickAvatarChange(object sender, RoutedEventArgs e)
        {
            AvatarActor.Send(new OscMessage("/avatar/change"));
            MotorValueViewModel.Instance?.ResetCommand.Execute(null);
        }
        private void ClickInStation(object sender, RoutedEventArgs e)
        {
            AvatarActor.Send(new OscMessage("/avatar/parameters/InStation", false));
            MotorValueViewModel.Instance?.ResetCommand.Execute(null);
        }
    }
}
