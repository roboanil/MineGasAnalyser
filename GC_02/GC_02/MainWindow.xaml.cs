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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        //public string input { get; set; }

        public static string input_Path;
        public static string output_Path;

        public MainWindow()
        {
            InitializeComponent();
            this.iMainMenu.Visibility = System.Windows.Visibility.Collapsed;
            Switcher.defaultWindow = this;
            Switcher.Switch(new OptionMenu());                                                                      //Switcher.Switch(new OptionMenu());
        }
        public void Navigate(UserControl nextPage)
        {
            this.Host_Canvas.Children.Clear();
            this.Host_Canvas.Children.Add(nextPage);
        }
        private void Inp(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            
            if (result.ToString() == "OK")
            {
                AutoMode.Paths._input = dialog.SelectedPath.ToString();
                //System.Windows.Forms.MessageBox.Show(input_Path);
                
                AutoMode temp_auto = new AutoMode();
                temp_auto.inputFolder.Text = AutoMode.Paths._input;
                temp_auto.outputFolder.Text = AutoMode.Paths._output;
                this.Host_Canvas.Children.Add(temp_auto);
                

                //OptionMenu.autoEnable = true;
            }    
        }
        private void Out(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (result.ToString() == "OK")
            {
                output_Path = dialog.SelectedPath;
                
                //AutoMode temp_au = new AutoMode();
                //System.Windows.Forms.MessageBox.Show(AutoMode.Paths._output);
                if (OptionMenu.autoEnable == true)
                {
                    AutoMode.Paths._output = dialog.SelectedPath;
                    AutoMode temp_auto2 = new AutoMode();                    
                    temp_auto2.inputFolder.Text = AutoMode.Paths._input;
                    temp_auto2.outputFolder.Text = AutoMode.Paths._output;
                    this.Host_Canvas.Children.Add(temp_auto2);
                }

                //OptionMenu.autoEnable = true;
            }
        }
        private void Exi(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown(0);
        }
        private void Man(object sender, RoutedEventArgs e)
        {
            this.Title = "Manual Mode";
            OptionMenu.autoEnable = false;
            Navigate(new ManualMode());
        }
        private void Semi(object sender, RoutedEventArgs e)
        {
            this.Title = "Semi-Autonomous Mode";
            OptionMenu.autoEnable = false;
            Navigate(new SemiAutoMode());
        }
        private void Auto(object sender, RoutedEventArgs e)
        {
            this.Title = "Autonomous Mode";
            AutoMode.Paths._input = "                              ---------Please Select a Folder---------";
            AutoMode.Paths._output = "                              ---------Please Select a Folder---------";
            OptionMenu.autoEnable = true;
            Navigate(new AutoMode());
        }
        private void Hel(object sender, RoutedEventArgs e)
        {
            //Navigate(new OptionMenu());
        }

    }
}
