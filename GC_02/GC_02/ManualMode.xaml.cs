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
using System.Diagnostics;
using System.ComponentModel;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Drawing.Layout;


namespace GDA
{
    /// <summary>
    /// Interaction logic for ManualMode.xaml
    /// </summary>
    public partial class ManualMode : UserControl
    {
        PdfDocument document = new PdfDocument();

        public double[] gas;

        public ManualMode()
        {
            gas = new double[] { 99, 99, 99, 99, 99, 99 };
            InitializeComponent();
            Switcher.defaultWindow.iMainMenu.Visibility = System.Windows.Visibility.Visible;
            
            Switcher.defaultWindow.ip.IsEnabled = false;
            Switcher.defaultWindow.op.IsEnabled = true;

            Switcher.defaultWindow.ma.IsEnabled = false;
            Switcher.defaultWindow.au.IsEnabled = true;
            Switcher.defaultWindow.sa.IsEnabled = true;

            Polygon p1 = new Polygon();
            p1.Stroke = Brushes.Transparent;
            p1.Fill = Brushes.Orange;
            p1.StrokeThickness = 1;
            p1.Points = new PointCollection() { new Point(0, 0), new Point(200, 0), new Point(200, 110), new Point(0, 110) };

            Polygon p2 = new Polygon();
            p2.Stroke = Brushes.Transparent;
            p2.Fill = Brushes.Red;
            p2.StrokeThickness = 1;
            p2.Points = new PointCollection() { new Point(200, 0), new Point(400, 0), new Point(400, 110), new Point(200, 110) };

            Polygon p3 = new Polygon();
            p3.Stroke = Brushes.Transparent;
            p3.Fill = Brushes.Green;
            p3.StrokeThickness = 1;
            p3.Points = new PointCollection() { new Point(0, 110), new Point(200, 110), new Point(200, 220), new Point(0, 220) };

            Polygon p4 = new Polygon();
            p4.Stroke = Brushes.Transparent;
            p4.Fill = Brushes.Yellow;
            p4.StrokeThickness = 1;
            p4.Points = new PointCollection() { new Point(200, 110), new Point(400, 110), new Point(400, 220), new Point(200, 220) };

            this.ellicott.Children.Add(p1);
            this.ellicott.Children.Add(p2);
            this.ellicott.Children.Add(p3);
            this.ellicott.Children.Add(p4);

        }

        private void Ref(object sender, RoutedEventArgs e)
        {
            Switcher.defaultWindow.Navigate(new ManualMode());
        }

        private void calculate_button(object sender, RoutedEventArgs e)
        {
            calculator_02.CH4 = double.Parse(VMet.Text);
            calculator_02.CO = double.Parse(VMono.Text);
            calculator_02.CO2 = double.Parse(VDia.Text);
            calculator_02.O2 = double.Parse(VOxy.Text);
            calculator_02.H2 = double.Parse(VHyd.Text);
            calculator_02.N2 = double.Parse(VNit.Text);

            // CH4 , CO, CO2, H2, O2, N2
            gas[0] = double.Parse(VMet.Text);
            gas[1] = double.Parse(VMono.Text);
            gas[2] = double.Parse(VDia.Text);
            gas[3] = double.Parse(VHyd.Text);
            gas[4] = double.Parse(VOxy.Text);
            gas[5] = double.Parse(VNit.Text);

            document.Info.Title = "Gas Analyses";
            document.Info.Author = "ExpertAlly";



            PdfPage page = document.AddPage();
            page.Width = 595;
            page.Height = 842;
            XGraphics gfx = XGraphics.FromPdfPage(page);
            double width = page.Width;     //595 units 
            double height = page.Height;    //842 units --> 1 inch = 72 units

            //calculator_02 ReadInputFromFile(file.FullName);

            calculator_02.IndiceCalculation();
            calculator_02.CowardsVerticeCalculation();
            calculator_02.ElliottsDiagramCalculation();

            calculator_02.DrawPdfImage(gfx, width, height);
            calculator_02.DrawIndices(gfx, width, height);
            calculator_02.DrawFigures(gfx, width, height);

            gr.Content = calculator_02.GR.ToString("F4");
            jtr.Content = calculator_02.JTR.ToString("F4");
            yr.Content = calculator_02.YR.ToString("F4");
            rt1.Content = calculator_02.Ratio1.ToString("F4"); 
            rt2.Content = calculator_02.Ratio2.ToString("F4");
            mr.Content = calculator_02.MR.ToString("F4");

            

            double Cx, Cy, Lx, Ly, Nx, Ny, Hx, Hy, Fx, Fy;
            double B = 400, H = 220;

            Cx = 0.01 * B * (gas[0] + gas[1] + gas[3]);
            Cy = H - 0.02 * B * gas[4];

            Lx = Cx / (gas[0] / 5.0 + gas[1] / 12.5 + gas[3] / 4.0);
            Hx = Cx / (gas[0] / 14.0 + gas[1] / 74.2 + gas[3] / 74.2);
            Nx = Cx / (gas[0] / 5.9 + gas[1] / 13.8 + gas[3] / 4.3);

            Ly = H - (H * (1 - Lx / B));
            Hy = H - (H * (1 - Hx / B));
            Ny = H - ((H / B) * (B - Nx - Nx * (6.07 * gas[0] + 4.13 * gas[1] + 16.59 * gas[3]) / (gas[0] + gas[1] + gas[3])));

            Fx = H * Nx / Ny;
            Fy = H;

            ///////////////////////////////////////////////////////////////////
            coward.Children.Clear();
            ///////////////////////////////////////////////////////////////////

            Polygon p1 = new Polygon();
            p1.Stroke = Brushes.Transparent;
            p1.Fill = Brushes.Yellow;
            p1.StrokeThickness = 1;
            p1.Points = new PointCollection() { new Point(0, 0), new Point(0, 220), new Point(400, 220) };

            Polygon p2 = new Polygon();
            p2.Stroke = Brushes.Transparent;
            p2.Fill = Brushes.Green;
            p2.StrokeThickness = 1;
            p2.Points = new PointCollection() { new Point(0, 0), new Point(0, 220), new Point(Fx, Fy) };

            Polygon p3 = new Polygon();
            p3.Stroke = Brushes.Transparent;
            p3.Fill = Brushes.Orange;
            p3.StrokeThickness = 1;
            p3.Points = new PointCollection() { new Point(0, 0), new Point(Nx, Ny), new Point(Lx, Ly) };

            Polygon p4 = new Polygon();
            p4.Stroke = Brushes.Transparent;
            p4.Fill = Brushes.Red;
            p4.StrokeThickness = 1;
            p4.Points = new PointCollection() { new Point(Lx, Ly), new Point(Nx, Ny), new Point(Hx, Hy) };

            coward.Children.Add(p1);
            coward.Children.Add(p2);
            coward.Children.Add(p3);
            coward.Children.Add(p4);
        }

        private void save_button(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            //saveFileDialog1.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif";
            //saveFileDialog1.InitialDirectory = MainWindow.output_Path;
            //System.Windows.Forms.MessageBox.Show(MainWindow.output_Path);
            //saveFileDialog1.RestoreDirectory = true;
            saveFileDialog1.Filter = "Gas Files (*.pdf)|*.pdf";
            saveFileDialog1.Title = "Save an Output File";
            saveFileDialog1.FileName = "";
            saveFileDialog1.ShowDialog();

            //int n = textBox.Text.LastIndexOf("\\");
            //int l = textBox.Text.Length - n - 5;
            //string filename = textBox.Text.Substring(n + 1, l) + ".pdf";
            if (saveFileDialog1.FileName != "")
            {
                document.Save(saveFileDialog1.FileName);
                Process.Start(saveFileDialog1.FileName);
            } 
            //document.Save(saveFileDialog1.FileName);
            //System.Windows.Forms.MessageBox.Show(saveFileDialog1.FileName);
            
        }
    }
}
