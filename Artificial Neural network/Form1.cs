using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
namespace Artificial_Neural_network
{
    
    public partial class Form1 : Form
    {

        // some variables 
        Graphics grafik;
        List<point> samples = new List<point>();
        classs[] pointcolornodearray;
        int maximumloopnumber = 0;
        double learningcoefficent = 0;


        
        



        public Form1()
        {
            
         
            InitializeComponent();
            
        }

        private void trackBar1_Scroll(object sender, EventArgs e) // learning coefficent
        {
            label7.Text = " " + (double)trackBar1.Value / 1000;
        } 

        private void panel1_Paint(object sender, PaintEventArgs e) //paint canvas in the start
        {
            grafik.DrawLine(new Pen(Color.Black), new Point(0, panel1.Height / 2), new Point(panel1.Width, panel1.Height / 2));
            //y ekseni
            grafik.DrawLine(new Pen(Color.Pink), new Point(panel1.Width / 2, 0), new Point(panel1.Width / 2, panel1.Height));

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            grafik = panel1.CreateGraphics();
            label7.Text = " " + (double)trackBar1.Value / 1000;
            learningcoefficent= (double)trackBar1.Value / 1000;
            maximumloopnumber= Int32.Parse(textBox1.Text);
           
                
            
            string dosya = "save.txt";
            FileStream fs = new FileStream(dosya, FileMode.Append, FileAccess.Write);
            fs.Close();
            pointcolornodearray = new classs[50];
            for (int i = 0; i < 50; i++)
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
            double x1 = -10;
            double w1 = 0;
            double w2 = 0;
            double w0 = 0;
            double x0 = -1;

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

          

                pen = new Pen(Color.FromArgb(pointcolornodearray[int.Parse(comboBox1.Text)].r, pointcolornodearray[int.Parse(comboBox1.Text)].g, pointcolornodearray[int.Parse(comboBox1.Text)].b),2);
                sample = new point(posX, posY, Int32.Parse(comboBox1.Text));

            


            samples.Add(sample);
            MessageBox.Show("adding point location : \n x  location:  " + posX + "\n y  location:  " + posY + "   ", "");
            grafik.DrawLine(pen, new Point(e.X - 3, e.Y), new Point(e.X + 3, e.Y)); // x y noktasına 6 ya 6 birimlik  + isareti konuyor
            grafik.DrawLine(pen, new Point(e.X, e.Y - 3), new Point(e.X, e.Y + 3));
            
        } // when click draw a point and add in 

        private void button3_Click(object sender, EventArgs e) // save point location button
        {
            string dosya = "save.txt";
            FileStream fs = new FileStream(dosya, FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            
            foreach (point sample in samples)
            {
                
                string word = sample.X1.ToString("0.000000");
                sw.WriteLine(word);
                
                word = sample.X2.ToString("0.000000");
                sw.WriteLine(word);
                word = sample.sınıf.ToString("0.000000");
                sw.WriteLine(word);
                
            }

            sw.Flush();
            sw.Close();
        }

        private void button4_Click(object sender, EventArgs e) // restore last saved location button
        {
            samples.Clear(); // clear old points           
            grafik.Clear(Color.White); // paint
            // drawing axis
            grafik.DrawLine(new Pen(Color.Gainsboro), new Point(0, panel1.Height / 2), new Point(panel1.Width, panel1.Height / 2));
            grafik.DrawLine(new Pen(Color.Gainsboro), new Point(panel1.Width / 2, 0), new Point(panel1.Width / 2, panel1.Height));

            FileStream fs = new FileStream("save.txt", FileMode.Open, FileAccess.Read);
            StreamReader sw = new StreamReader(fs);
            
            while (true)
            {

                string asd = sw.ReadLine();
                if (asd == null) break;
                string s = Convert.ToDouble(asd) + "";
                

                double x = Convert.ToDouble(asd);
                double y = Convert.ToDouble(sw.ReadLine());
                double z = Convert.ToDouble(sw.ReadLine());
                int zz = (int)z;

                point b = new point(x, y, zz);
                samples.Add(b);

            }


            drawpoints();


            sw.Close();
        }

        private void button1_Click(object sender, EventArgs e) // dicscrete perceptron learning
        {
            int counter = 0;
            bool error = true;
            double MinimumSquarederror = 0;
            int numberOfclass = 0;
            List<int> uniqueValues = new List<int>();
            for (int i = 0; i < samples.Count; ++i)
            {
                    if (!uniqueValues.Contains(samples[i].sınıf))
                        uniqueValues.Add(samples[i].sınıf);
             }
            numberOfclass = uniqueValues.Count; //just look for which and how many point class used

            if (numberOfclass<2)
            {
                MessageBox.Show(@"Unfortunately the total number of  class of points fewer than 2.
                And the training failed.
                You can try again by increasing the total number of  sample set.", "Big failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (numberOfclass == 2)
            {
                Random rast = new Random();
                double w0 = rast.NextDouble();
                double w1 = rast.NextDouble();
                double w2 = rast.NextDouble();
                int o = 0;
                int y = 0;


                while (error)
                {
                    error = false;
                    MinimumSquarederror = 0;
                    foreach (point im in samples)
                    {
                        double net = w0 * im.X1 + w1 * im.X2 - w2;
                        if (net > 0)
                        {
                            o = 1;
                            y = uniqueValues[0];
                        }
                        else
                        {
                            o = -1;
                            y = uniqueValues[1];
                        }
                        
                        if (y != im.sınıf)// check the error
                        {
                            error = true;
                            MinimumSquarederror += (im.sınıf - y) * (im.sınıf - y) / 2; // E=1/2 *(d-o)*(d-o)
                            w0 = w0 + learningcoefficent * (im.sınıf - y) * im.X1 / 2; //updating weights w=w+1/2*c*(d-o)y
                            w1 = w1 + learningcoefficent * (im.sınıf - y) * im.X2 / 2; //updating weights
                            w2 = w2 + learningcoefficent * (im.sınıf - y) * -1 / 2; //updating weights
                        }

                        grafik.Clear(Color.White);
                        drawing();  // draw function on every update of weights
                        

                    }
                    counter++;
                    if (counter > maximumloopnumber)
                    {
                        MessageBox.Show(@"Unfortunately the total number of educational cycles exceeded the maximum.
And the training failed.
You can try again by increasing the maximum number of cycles or by slightly changing the sample set.", "Big failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                } // learning
            }

            else
            {
                while (error)
                {
                    error = false;
                    foreach (point im in samples)
                    {


                    }
                    if (counter > maximumloopnumber)
                    {
                        MessageBox.Show(@"Unfortunately the total number of educational cycles exceeded the maximum.
And the training failed.
You can try again by increasing the maximum number of cycles or by slightly changing the sample set.", "Big failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                } // learning

            }

            
            
        }

        private void button2_Click(object sender, EventArgs e) // continous perceptron leaning
        {

        }

        private void drawpoints() // drawing points in list
        {
            foreach (point sample in samples)
            {

                double posX = (panel1.Width / 2) + sample.X1 * 10;
                double posY = (panel1.Height / 2) - sample.X2 * 10;

                Pen pen;
               
                    pen = new Pen(Color.FromArgb(pointcolornodearray[(int)sample.sınıf].r, pointcolornodearray[(int)sample.sınıf].g, pointcolornodearray[(int)sample.sınıf].b));
                





                //drawing points
                grafik.DrawLine(pen, new Point((int)posX - 3, (int)posY), new Point((int)posX + 3, (int)posY));
                grafik.DrawLine(pen, new Point((int)posX, (int)posY - 3), new Point((int)posX, (int)posY + 3));
            }
        }

        private void button5_Click(object sender, EventArgs e) // clear button which clear
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
        int snf;

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

        public int sınıf
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
