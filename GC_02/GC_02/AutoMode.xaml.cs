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
using System.IO;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Drawing.Layout;
using System.Diagnostics;

namespace GDA
{
    /// <summary>
    /// Interaction logic for AutoMode.xaml
    /// </summary>
    public partial class AutoMode : UserControl
    {
        // Input Variables: Air Flow (m3/s) & Percentage Concentrations of (CO, CO2, H2, N2, O2, CH4)
        double CO, CO2, CH4, H2, O2, N2;
        
        double GR, Ratio1, MR, JTR, YR, Ratio2;
        double Lx, Ly, Hx, Hy, Nx, Ny, Fx, Fy;
        double Cx, Cy;
        double OxyDef;

        double B = 200;  // Coward's Triangle Base length
        double H = 135;  //  Coward's Triangle Height

        string zone;     // Used to convey 
        XBrush brush;    // the zone for dv the figures. 

        //public static bool autoEnable = true;
        public static class Paths
        {
            static string _Input = "                              ---------Please Select a Folder---------";
            static string _Output = "                              ---------Please Select a Folder---------";
            public static string _input { get { return _Input; } set { _Input = value; } }
            public static string _output { get { return _Output; } set { _Output = value; } }
        }
                
        public AutoMode()
        {
            
            InitializeComponent();
            //OptionMenu.autoEnable = true;
            
            Switcher.defaultWindow.iMainMenu.Visibility = System.Windows.Visibility.Visible;
            
            Switcher.defaultWindow.ip.IsEnabled = true;
            Switcher.defaultWindow.op.IsEnabled = true;

            Switcher.defaultWindow.au.IsEnabled = false;
            Switcher.defaultWindow.sa.IsEnabled = true;
            Switcher.defaultWindow.ma.IsEnabled = true;
            
        }

        public class Files
        {
            public string input { get; set; }

            public string output { get; set; }

            public string status { get; set; }
        }             

        private void Ref(object sender, RoutedEventArgs e)
        {
            
            inputlistBox.Items.Clear();
            //Switcher.defaultWindow.Navigate(new AutoMode());
            //DirectoryInfo dir = new DirectoryInfo(Switcher.input_Path);

            DirectoryInfo dir = new DirectoryInfo(AutoMode.Paths._input);

            foreach (FileInfo file in dir.GetFiles("*.txt"))
            {
                inputlistBox.Items.Add(file.Name.ToString());
                inputlistBox.Items.Add(new Separator());
            }
            
        }

        private void convert(object sender, RoutedEventArgs e)
        {
            outputlistBox.Items.Clear();
         
            //DirectoryInfo dir = new DirectoryInfo(Switcher.input_Path);

            DirectoryInfo dir = new DirectoryInfo(AutoMode.Paths._input);
         

            foreach (FileInfo file in dir.GetFiles("*.txt"))
            {

                //outputlistBox.Items.Add("Processing......");
                /////////////////////////////////////////////////////
                PdfDocument document = new PdfDocument();
                document.Info.Title = "Gas Analyses";
                document.Info.Author = "ExpertAlly";
                

                PdfPage page = document.AddPage();
                page.Width = 595;
                page.Height = 842;
                XGraphics gfx = XGraphics.FromPdfPage(page);
                double width = page.Width;     //595 units 
                double height = page.Height;    //842 units --> 1 inch = 72 units

                ReadInputFromFile(file.FullName);
                IndiceCalculation();
                CowardsVerticeCalculation();
                ElliottsDiagramCalculation();

                DrawPdfImage(gfx, width, height);
                DrawIndices(gfx, width, height);
                DrawFigures(gfx, width, height);

                //Naming and Saving
                int n = file.FullName.LastIndexOf("\\");
                int l = file.FullName.Length - n - 5;
                string filename = file.FullName.Substring(n+1, l) + ".pdf"; // e.g. Extracting "Test" from "......../Test.txt" 

                // ******************************Works only if the folder is present*******************************
                //document.Save(Switcher.output_Path + "//"+filename);

                document.Save(MainWindow.output_Path + "//" + filename);

                //Process.Start(Switcher.output_Path  + filename);


                outputlistBox.Items.Add(filename.ToString());
                outputlistBox.Items.Add(new Separator());
            }
            
            
        }

        void ReadInputFromFile(string path)
        {
            string[] values = File.ReadAllText(path).Split();
            CH4 = double.Parse(values[0]);
            CO = double.Parse(values[1]);
            CO2 = double.Parse(values[2]);
            H2 = double.Parse(values[3]);
            O2 = double.Parse(values[4]);
            N2 = double.Parse(values[5]);
            
            OxyDef = 0.265 * N2 - O2;
        }

        void IndiceCalculation()
        {
            // -1 is assigned if the index is Not Applicable.

            /* Graham's Ratio */
            GR = (OxyDef <= 0) ? -1 : CO / OxyDef;

            /* Jones - Trickett Ratio */
            JTR = (OxyDef <= 0) ? -1 : (CO2 + 0.75 * CO - 0.25 * H2) / OxyDef;

            /* Young's Ratio */
            YR = (OxyDef <= 0) ? -1 : CO2 / OxyDef;

            /* CO/CO2 Ratio */
            Ratio1 = (CO2 == 0) ? -1 : CO / CO2;

            /* H2/CO Ratio */
            Ratio2 = (CO == 0) ? -1 : H2 / CO;

            /* Morris Ratio */
            MR = (CO + CO2 == 0)? -1 : (N2 - 3.774*O2)/(CO + CO2);
        }

        void CowardsVerticeCalculation()
        {
            // NEW FORMULAE REQUIRED. THESE ARE JUST TEST (APPROXIMATE) FORMULAE!!

            Cx = 0.01 * B * (CH4 + CO + H2);
            Cy = H - 0.02 * B * O2;

            Lx = Cx / (CH4 / 5.0 + CO / 12.5 + H2 / 4.0);
            Hx = Cx / (CH4 / 14.0 + CO / 74.2 + H2 / 74.2);
            Nx = Cx / (CH4 / 5.9 + CO / 13.8 + H2 / 4.3);

            Ly = H - (H * (1 - Lx / B));
            Hy = H - (H * (1 - Hx / B));
            Ny = H - ((H / B) * (B - Nx - Nx * (6.07 * CH4 + 4.13 * CO + 16.59 * H2) / (CH4 + CO + H2)));

            Fx = H * Nx / Ny;
            Fy = H;

            //Offsets
            double X = 84, Y = 550;
            Cx += X; Lx += X; Hx += X; Nx += X; Fx += X;
            Cy += Y; Ly += Y; Hy += Y; Ny += Y; Fy += Y;
        }

        void ElliottsDiagramCalculation()
        {
            //Decide Zone for both figures from this calculation
            zone = "Non-Explosive Zone";
            brush = XBrushes.Green;
        }

        void DrawPdfImage(XGraphics gfx, double W, double H)
        {
            string pdfSamplePath = @"C:\Users\Anil\Documents\Visual Studio 2013\Projects\GC_02\GC_02\Gas_CA.pdf";

            XImage image = XImage.FromFile(pdfSamplePath);

            gfx.DrawImage(image, 0, 0, W, H);
        }

        void DrawIndices(XGraphics gfx, double W, double H)
        {
            XFont T11R = new XFont("Times New Roman", 11, XFontStyle.Regular);
            XFont T11B = new XFont("Times New Roman", 11, XFontStyle.Bold);
            XTextFormatter tf = new XTextFormatter(gfx);
            tf.Alignment = XParagraphAlignment.Center;

            //Gas Composition
            //CH4
            XRect rect = new XRect(216, 351, 45, 11);
            string text = CH4.ToString("F4");
            tf.DrawString(text, T11R, XBrushes.Black, rect);

            //CO
            rect = new XRect(324, 351, 45, 11);
            text = CO.ToString("F4");
            tf.DrawString(text, T11R, XBrushes.Black, rect);

            //CO2
            rect = new XRect(432, 351, 45, 11);
            text = CO2.ToString("F4");
            tf.DrawString(text, T11R, XBrushes.Black, rect);

            //O2
            rect = new XRect(216, 374, 45, 11);
            text = O2.ToString("F4");
            tf.DrawString(text, T11R, XBrushes.Black, rect);

            //H2
            rect = new XRect(324, 374, 45, 11);
            text = H2.ToString("F4");
            tf.DrawString(text, T11R, XBrushes.Black, rect);

            //N2
            rect = new XRect(432, 374, 45, 11);
            text = N2.ToString("F4");
            tf.DrawString(text, T11R, XBrushes.Black, rect);

            //Indices
            tf.Alignment = XParagraphAlignment.Left;

            //Graham's Ratio
            rect = new XRect(162, 461, 126, 11);
            text = (GR == -1) ? "Not Applicable" : GR.ToString("F2");
            tf.DrawString(text, T11B, XBrushes.Green, rect);

            //Jones-Trickett Ratio
            rect = new XRect(162, 484, 126, 11);
            text = (JTR == -1) ? "Not Applicable" : JTR.ToString("F2");
            tf.DrawString(text, T11B, XBrushes.Green, rect);

            //Young's Ratio
            rect = new XRect(162, 507, 126, 11);
            text = (YR == -1) ? "Not Applicable" : YR.ToString("F2");
            tf.DrawString(text, T11B, XBrushes.Green, rect);

            // CO/CO2 Ratio
            rect = new XRect(378, 461, 126, 11);
            text = (Ratio1 == -1) ? "Not Applicable" : Ratio1.ToString("F2");
            tf.DrawString(text, T11B, XBrushes.Green, rect);

            // H2/CO Ratio
            rect = new XRect(378, 484, 126, 11);
            text = (Ratio2 == -1) ? "Not Applicable" : Ratio2.ToString("F2");
            tf.DrawString(text, T11B, XBrushes.Green, rect);

            // Morris ratio
            rect = new XRect(378, 507, 126, 11);
            text = MR.ToString("F2");
            tf.DrawString(text, T11B, XBrushes.Green, rect);

            //Coward's Triangle
            rect = new XRect(170, 529, 126, 11);
            tf.DrawString(zone, T11B, brush, rect);

            //Ellicott Diagram
            rect = new XRect(383, 529, 126, 11);
            tf.DrawString(zone, T11B, brush, rect);
        }

        void DrawFigures(XGraphics gfx, double W, double H)
        {
            
            /* COWARD'S TRIANGLE */

            XPen pen = new XPen(XColors.Black, 1);
            pen.LineJoin = XLineJoin.Round;
            XFont T10R = new XFont("Times New Roman", 10, XFontStyle.Regular);
            XTextFormatter tf = new XTextFormatter(gfx);
            tf.Alignment = XParagraphAlignment.Center;

            //Triangles
            XPoint[] polygon = new XPoint[] { new XPoint(84, 550), new XPoint(84, 685), new XPoint(284, 685) };
            gfx.DrawPolygon(pen, XBrushes.Green, polygon, XFillMode.Alternate);

            polygon = new XPoint[] { new XPoint(Hx, Hy), new XPoint(Nx, Ny), new XPoint(Fx, Fy), new XPoint(284, 685) };
            gfx.DrawPolygon(pen, XBrushes.Yellow, polygon, XFillMode.Alternate);

            polygon = new XPoint[] { new XPoint(84, 550), new XPoint(Nx, Ny), new XPoint(Lx, Ly) };
            gfx.DrawPolygon(pen, XBrushes.DarkOrange, polygon, XFillMode.Alternate);

            polygon = new XPoint[] { new XPoint(Hx, Hy), new XPoint(Nx, Ny), new XPoint(Lx, Ly) };
            gfx.DrawPolygon(pen, XBrushes.Red, polygon, XFillMode.Alternate);

            //Point
            gfx.DrawEllipse(pen, XBrushes.White, Cx, Cy, 6, 6);

            //Texts
            XRect rect = new XRect(72, 550, 10, 10);
            tf.DrawString("21", T10R, XBrushes.Red, rect);

            rect = new XRect(72, 687, 10, 10);
            tf.DrawString("0", T10R, XBrushes.Red, rect);

            rect = new XRect(150, 687, 50, 10);
            tf.DrawString("Methane %", T10R, XBrushes.Red, rect);

            rect = new XRect(270, 687, 15, 10);
            tf.DrawString("100", T10R, XBrushes.Red, rect);

            /* ELLICOTT DIAGRAM */

            rect = new XRect(297, 550, 112, 75);
            gfx.DrawRectangle(XPens.Black, XBrushes.DarkOrange, rect);

            rect = new XRect(409, 550, 112, 75);
            gfx.DrawRectangle(XPens.Black, XBrushes.Red, rect);

            rect = new XRect(297, 625, 112, 75);
            gfx.DrawRectangle(XPens.Black, XBrushes.Green, rect);

            rect = new XRect(409, 625, 112, 75);
            gfx.DrawRectangle(XPens.Black, XBrushes.Yellow, rect);

            //Arrows & Texts
            pen.LineCap = XLineCap.Round;
            gfx.DrawLine(pen, 409, 625, 381, 606);
            gfx.DrawLine(pen, 382, 610, 381, 606);
            gfx.DrawLine(pen, 385, 606, 381, 606);
            gfx.DrawString("Lean", T10R, XBrushes.Black, 372, 600);

            gfx.DrawLine(pen, 409, 625, 437, 644);
            gfx.DrawLine(pen, 436, 640, 437, 644);
            gfx.DrawLine(pen, 433, 644, 437, 644);
            gfx.DrawString("Rich", T10R, XBrushes.Black, 430, 655);

            gfx.DrawLine(pen, 409, 625, 381, 644);
            gfx.DrawLine(pen, 382, 640, 381, 644);
            gfx.DrawLine(pen, 385, 644, 381, 644);
            gfx.DrawString("Inert", T10R, XBrushes.Black, 372, 655);

            //Point
        }
        
        
    }
}
