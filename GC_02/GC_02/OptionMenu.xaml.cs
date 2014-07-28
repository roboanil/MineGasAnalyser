using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GDA
{
    /// <summary>
    /// Interaction logic for OptionMenu.xaml
    /// </summary>
    public partial class OptionMenu : UserControl,ISwitchable
    {
        public static bool autoEnable = false;
        public OptionMenu()
        {
            InitializeComponent();
        }

        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }

        private void AutoClick(object sender, RoutedEventArgs e)
        {            
            autoEnable = true;
            Switcher.defaultWindow.Title = "Autonomous Mode";
            Switcher.Switch(new AutoMode());            
        }

        private void SemiAuto(object sender, RoutedEventArgs e)
        {
            autoEnable = false;
            Switcher.defaultWindow.Title = "Semi-Autonomous Mode";
            Switcher.Switch(new SemiAutoMode());            
        }

        private void Manual_Click(object sender, RoutedEventArgs e)
        {
            autoEnable = false;
            Switcher.defaultWindow.Title = "Manual Mode";
            Switcher.Switch(new ManualMode());            
        }
    }
}
