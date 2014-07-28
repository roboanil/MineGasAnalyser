using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace GDA
{
    public static class Calculator
    {
        
        public static void check()
        {

        }

        
        public static double Graham(double[] gas)
        {
            return ((0.265 * gas[5] - gas[4]) <= 0) ? -1 : gas[1] / (0.265 * gas[5] - gas[4]);
        }
        public static double Trickett(double[] gas)
        {
            return ((0.265 * gas[5] - gas[4]) <= 0) ? -1 : (gas[2] + 0.75 * gas[1] - 0.25 * gas[3]) / (0.265 * gas[5] - gas[4]);
        }
        public static double Young(double[] gas)
        {
            return ((0.265 * gas[5] - gas[4]) <= 0) ? -1 : gas[2] / (0.265 * gas[5] - gas[4]);
        }
        public static double Morris(double[] gas)
        {
            return (gas[1] + gas[2] == 0) ? -1 : (gas[5] - 3.774 * gas[4]) / (gas[1] + gas[2]);
        }
        public static double H2CO(double[] gas)
        {
            return (gas[1] == 0) ? -1 : gas[3] / gas[1];
        }
        public static double COCO2(double[] gas)
        {
            return (gas[2] == 0) ? -1 : gas[1] / gas[2];
        }
    }
}
