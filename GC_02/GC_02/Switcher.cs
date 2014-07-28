using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
namespace GDA
{
    public static class Switcher
    {
        public static MainWindow defaultWindow;
        //public static string input_Path;
        //public static string output_Path;

        public static void Switch(UserControl newPage)
        {
            //input_Path = "X";
            //output_Path = "X";
            //input_Path = @"C:\Users\Anil\Desktop\InputFolder";
            //output_Path = @"C:\Users\Anil\Desktop\OutPutFolder";
            defaultWindow.Navigate(newPage);
        }
    }
}
