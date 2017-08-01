using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Artificial_Neural_network
{
    
    public partial class Form1 : Form
    {
        Graphics grafik;
        List<point> samples = new List<point>();
        double w1, w2, w0 = 0;
        double x0 = -1;
        classs[] pointcolornodearray;



        public Form1()
        {
            
         
            InitializeComponent();
            
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label7.Text = " " + (double)trackBar1.Value / 1000;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            grafik.DrawLine(new Pen(Color.Black), new Point(0, panel1.Height / 2), new Point(panel1.Width, panel1.Height / 2));
            //y ekseni
            grafik.DrawLine(new Pen(Color.Pink), new Point(panel1.Width / 2, 0), new Point(panel1.Width / 2, panel1.Height));

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            grafik = panel1.CreateGraphics();
            label7.Text = " " + (double)trackBar1.Value / 1000;

            pointcolornodearray = new classs[25];
            for (int i = 0; i < 25; i++)
            {
                pointcolornodearray[i] = new classs(i);
                comboBox1.Items.Add(i + "");
            }
        }

        private void drawing() //  first drawing axis and call drawpoints function
        {

            //eksenler
            grafik.DrawLine(new Pen(Color.Black), new Point(0, panel1.Height / 2), new Point(panel1.Width, panel1.Height / 2));
            grafik.DrawLine(new Pen(Color.Black), new Point(panel1.Width / 2, 0), new Point(panel1.Width / 2, panel1.Height));

            Pen pen = new Pen(Color.DarkGreen);
            double x1;

            x1 = -10;
            double y = -(x1 * w1 / w2) - ((x0 * w0) / w2);

            double shift = panel1.Height / 2;


            Point p1 = new Point(0, (int)(shift - y * 10));

            x1 = 10;
            double y2 = -(x1 * w1 / w2) + ((w0) / w2);

            Point p2 = new Point(panel1.Width, (int)(shift - y2 * 10));

            //drawing dicsriminating function
            if (w2 != 0)
            {
                grafik.DrawLine(pen, p1, p2);
                drawpoints();
            }
        }

        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {
            
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            point sample;
            Pen pen;



            double posX = (double)(e.X - panel1.Width / 2) / 10;
            double posY = (double)(panel1.Height / 2 - e.Y) / 10;

            if (radioButton1.Checked)
            {
                if (e.Button == MouseButtons.Left)
                {
                    pen = new Pen(Color.Blue);
                    sample = new point(posX, posY, 1);
                }
                else
                {
                    pen = new Pen(Color.Red);
                    sample = new point(posX, posY, -1);
                }
            }
            else// multikategori
            {

                pen = new Pen(Color.FromArgb(pointcolornodearray[int.Parse(comboBox1.Text)].r, pointcolornodearray[int.Parse(comboBox1.Text)].g, pointcolornodearray[int.Parse(comboBox1.Text)].b));
                sample = new point(posX, posY, Int32.Parse(comboBox1.Text));

            }


            samples.Add(sample);
            MessageBox.Show("eklenen pozisyon : \n x  pozisyonu:  " + posX + "\n y  pozisyonu:  " + posY + "   ", "");
            grafik.DrawLine(pen, new Point(e.X - 3, e.Y), new Point(e.X + 3, e.Y)); // x y noktasına 6 ya 6 birimlik  + isareti konuyor
            grafik.DrawLine(pen, new Point(e.X, e.Y - 3), new Point(e.X, e.Y + 3)); //
        }

        private void drawpoints() // drawing points in list
        {
            foreach (point sample in samples)
            {

                double posX = (panel1.Width / 2) + sample.X1 * 10;
                double posY = (panel1.Height / 2) - sample.X2 * 10;

                Pen pen;
                if (radioButton2.Checked)
                {
                    if (sample.sınıf == 1)
                    {
                        pen = new Pen(Color.Blue);
                    }
                    else
                    {
                        pen = new Pen(Color.Red);
                    }
                }
                else
                {
                    pen = new Pen(Color.FromArgb(pointcolornodearray[(int)sample.sınıf].r, pointcolornodearray[(int)sample.sınıf].g, pointcolornodearray[(int)sample.sınıf].b));
                }





                //drawing points
                grafik.DrawLine(pen, new Point((int)posX - 3, (int)posY), new Point((int)posX + 3, (int)posY));
                grafik.DrawLine(pen, new Point((int)posX, (int)posY - 3), new Point((int)posX, (int)posY + 3));
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            samples.Clear(); // örnekleri siliyor           
            grafik.Clear(Color.White); // ekranı boyuyor


            // eksenleri çiziyor
            grafik.DrawLine(new Pen(Color.Gainsboro), new Point(0, panel1.Height / 2), new Point(panel1.Width, panel1.Height / 2));
            grafik.DrawLine(new Pen(Color.Gainsboro), new Point(panel1.Width / 2, 0), new Point(panel1.Width / 2, panel1.Height));

        }
    }

    public class point
    {
        double x1;
        double x2;
        double snf;

        public point(double x1, double x2, int snf)
        {
            this.x1 = x1;
            this.x2 = x2;
            this.snf = snf;
        }

        public double X1
        {
            get { return x1; }
            set { this.x1 = value; }
        }


        public double X2
        {
            get { return x2; }
            set { this.x2 = value; }
        }

        public double sınıf
        {
            get { return snf; }
            set { this.snf = value; }
        }
    } // ornek

    public class classs
    {
        public int r, g, b;
        public int numara;

        public classs(int i)
        {
            Random rastgele = new Random(i);
            r = rastgele.Next(1, 255);
            g = rastgele.Next(1, 255);
            b = rastgele.Next(1, 255);

            numara = i;
        }
    } // sınıf
}
