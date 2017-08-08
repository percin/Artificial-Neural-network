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
        List<double> weights = new List<double>();
        classs[] pointcolornodearray;
        int maximumloopnumber = 200;
        double learningcoefficent = 1;


        
        



        public Form1()
        {
            
         
            InitializeComponent();
            
        }

        private void trackBar1_Scroll(object sender, EventArgs e) // learning coefficent
        {
            learningcoefficent =(double)trackBar1.Value / 10;
            label7.Text=" "+ learningcoefficent;
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
            label7.Text = " " + (double)trackBar1.Value / 10;
            learningcoefficent= (double)trackBar1.Value / 10;
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

        private void drawing() //  first drawing axis,functions and call drawpoints function
        {

            grafik.Clear(Color.White);
            //eksenler
            grafik.DrawLine(new Pen(Color.Black), new Point(0, panel1.Height / 2), new Point(panel1.Width, panel1.Height / 2));
            grafik.DrawLine(new Pen(Color.Black), new Point(panel1.Width / 2, 0), new Point(panel1.Width / 2, panel1.Height));

            Pen pen = new Pen(Color.DarkGreen);

            for (int i=0;i<weights.Count;i+=3)
            {
                // w0*x +w1*y -w2=0 is our basic discriminant function.  in our   basic artificial neuran version there is 3 input.
                // x0 =x , x1=y,  x2=-1
                // w0*x0 +w1*x1 -1*w2=0 
                // x0 means x coordinate and x1 means y coordinate. And x2 is just constant -1 for Enhanced input
                // in this drawing function ı just found two point p1 and p2. than ı draw line p1 to p2.
                // this p points belongs to borderline of canvas. So The x coordinate of p1 equals the zero. 
                // and The x coordinate of p2 equals the edge of the canvas.
                
                if (weights[1+i] == 0)
                    weights[1+i]+=0.01;

                double y = (weights[2 + i] - weights[0 + i] * -(panel1.Width) / 2) / weights[1 + i];
                PointF p1 = new PointF(0, (float) (panel1.Height/2-y));
                
                double y2 = (weights[2+i]- weights[0+i]*panel1.Width/2)/ weights[1+i];
                PointF p2 = new PointF(panel1.Width, (float)(panel1.Height / 2 - y2));
                //drawing dicsriminating function

                //MessageBox.Show(" y:" +y+"   y2:"+y2 );
                grafik.DrawLine(pen, p1, p2);
                    
                
                
            }
            drawpoints();


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
            FileStream fs = new FileStream(dosya, FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            
            foreach (point sample in samples)
            {
                
                string word = sample.X0.ToString("0.000000");
                sw.WriteLine(word);
                
                word = sample.X1.ToString("0.000000");
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
                weights.Add( rast.NextDouble());
                weights.Add( rast.NextDouble());
                weights.Add( rast.NextDouble());
                drawing();
                MessageBox.Show("w0="+weights[0]+"  w1="+weights[1]+"   w2="+weights[2]);
                int o = 0;
                int y = 0;


                while (error)
                {
                    error = false;
                    MinimumSquarederror = 0;
                    foreach (point im in samples)
                    {
                        double net = weights[0] * im.X0 + weights[1] * im.X1 - weights[2];
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
                            weights[0] = weights[0] + learningcoefficent * (2) * im.X0 / 2; //updating weights w=w+1/2*c*(d-o)y
                            weights[1] = weights[1] + learningcoefficent * (2) * im.X1 / 2; //updating weights
                            weights[2] = weights[2] + learningcoefficent * (2) * -1 / 2; //updating weights
                        }


                        drawing();

                    }
                    
                    // draw function on every update of weights
                    counter++;
                    if (counter > maximumloopnumber)
                    {
                        MessageBox.Show(@"Unfortunately the total number of educational cycles exceeded the maximum.
And the training failed.
You can try again by increasing the maximum number of cycles or by slightly changing the sample set. \n Total cycle number= " + counter, "Big failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        drawing();
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

                double posX = (panel1.Width / 2) + sample.X0 * 10;
                double posY = (panel1.Height / 2) - sample.X1 * 10;

                Pen pen;
               
                    pen = new Pen(Color.FromArgb(pointcolornodearray[(int)sample.sınıf].r, pointcolornodearray[(int)sample.sınıf].g, pointcolornodearray[(int)sample.sınıf].b));
                
                
                //drawing points
                grafik.DrawLine(pen, new Point((int)posX - 3, (int)posY), new Point((int)posX + 3, (int)posY));
                grafik.DrawLine(pen, new Point((int)posX, (int)posY - 3), new Point((int)posX, (int)posY + 3));
            }
        }

        public void button5_Click(object sender, EventArgs e) // clear button which clear
        {
            samples.Clear(); // örnekleri siliyor           
            grafik.Clear(Color.White); // ekranı boyuyor


            // eksenleri çiziyor
            grafik.DrawLine(new Pen(Color.Gainsboro), new Point(0, panel1.Height / 2), new Point(panel1.Width, panel1.Height / 2));
            grafik.DrawLine(new Pen(Color.Gainsboro), new Point(panel1.Width / 2, 0), new Point(panel1.Width / 2, panel1.Height));

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            maximumloopnumber = Int32.Parse(textBox1.Text);
        }
    }

    public class point
    {
        double x0;
        double x1;
        int snf;

        public point(double x0, double x1, int snf)
        {
            this.x0 = x0;
            this.x1 = x1;
            this.snf = snf;
        }

        public double X0
        {
            get { return x0; }
            set { this.x0 = value; }
        }


        public double X1
        {
            get { return x1; }
            set { this.x1 = value; }
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
