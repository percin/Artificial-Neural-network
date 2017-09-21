using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Artificial_Neural_network
{

    public partial class Form1 : Form
    {

        // some variables 
        Graphics grafik;
        Graphics grafik2;
        List<point> samples = new List<point>();
        List<double> weights = new List<double>();
        classs[] ccolor;
        int maximumloopnumber = 200;
        double learningcoefficent = 1;
        private List<Button> dynamicbuttons = new List<Button>();
        private List<int> weightstring = new List<int>();
        List<int> lines = new List<int>();


        public Form1()
        {
            InitializeComponent();

        }

        private void trackBar1_Scroll(object sender, EventArgs e) // learning coefficent
        {
            learningcoefficent = (double)trackBar1.Value / 10;
            label7.Text = " " + learningcoefficent;
        }

        private void panel1_Paint(object sender, PaintEventArgs e) //paint canvas in the start
        {
           
            grafik.DrawLine(new Pen(Color.Black), new Point(0, panel1.Height / 2), new Point(panel1.Width, panel1.Height / 2));
            grafik.DrawLine(new Pen(Color.Pink, 2), new Point(panel1.Width / 2, 0), new Point(panel1.Width / 2, panel1.Height));

        }

        private void Form1_Load(object sender, EventArgs e)
        {

            ResizeRedraw = true;
            grafik = panel1.CreateGraphics();
            grafik2 = panel2.CreateGraphics();
            grafik.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            label7.Text = " " + (double)trackBar1.Value / 10;
            learningcoefficent = (double)trackBar1.Value / 10;
            maximumloopnumber = Int32.Parse(textBox1.Text);

            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;

            Random rast = new Random();
            weights.Add(rast.NextDouble());
            weights.Add(rast.NextDouble());
            weights.Add(rast.NextDouble());

            Weighttable(sender, e);




            string dosya = "save.txt";
            FileStream fs = new FileStream(dosya, FileMode.Append, FileAccess.Write);
            fs.Close();
            Random randa = new Random();
            //
            ccolor = new classs[2];
            for (int i = 0; i < 2; i++)
            {
                ccolor[i] = new classs(i);
                ccolor[i].numara = i;
                ccolor[i].r = randa.Next(256);
                ccolor[i].g = randa.Next(256);
                ccolor[i].b = randa.Next(256);

                comboBox1.Items.Add(i + "");
            }

           


        }

        private void drawing(bool updatepointErrors=false) //  first drawing axis,functions and call drawpoints function
        {
            List<double> lipstick = new List<double>();
            grafik.Clear(Color.White);
            //dimensions
            grafik.DrawLine(new Pen(Color.Black), new Point(0, panel1.Height / 2), new Point(panel1.Width, panel1.Height / 2));
            grafik.DrawLine(new Pen(Color.Black), new Point(panel1.Width / 2, 0), new Point(panel1.Width / 2, panel1.Height));

            Pen pen = new Pen(Color.DarkGreen,3);

            for (int i = 0; i < weights.Count; i += 3)
            {
                double w = panel1.Width / 2;
                double h = panel1.Height / 2;
                Graphics variable = grafik;


                if (radioButton2.Checked && i == 6)
                {
                    w = panel2.Width / 2;
                    h = panel2.Height / 2;
                    variable = grafik2;
                }

                if (weights[1 + i] == 0)
                    weights[1 + i] = 0.000001;
                if (weights[i] == 0)
                    weights[i] = 0.000001;



                double a = (weights[2 + i] - weights[0 + i] * h) / weights[1 + i];
                if (h > a && -h < a)
                {
                    lipstick.Add(w);
                    lipstick.Add(a);
                }

                a = (weights[2 + i] - weights[0 + i] * -h) / weights[1 + i];
                if (h > a && -h < a)
                {
                    lipstick.Add(-w);
                    lipstick.Add(a);
                }

                a = (weights[2 + i] - weights[1 + i] * w) / weights[0 + i];
                if (w > a && -w < a)
                {
                    lipstick.Add(a);
                    lipstick.Add(h);
                }

                a = (weights[2 + i] - weights[1 + i] * -w) / weights[0 + i];
                if (w > a && -w < a)
                {
                    lipstick.Add(a);
                    lipstick.Add(-h);
                }

                pen.Color = Color.FromArgb(ccolor[i/3].r, ccolor[i / 3].g, ccolor[i / 3].b) ; 

                try
                {
                    PointF p1 = new PointF((float)(lipstick[0] + w), (float)(-lipstick[1] + h));
                    PointF p2 = new PointF((float)(lipstick[2] + w), (float)(-lipstick[3] + h));
                    variable.DrawLine(pen, p1, p2);
                }
                catch (Exception hata)
                {
                    MessageBox.Show("çizim fonksiyonunda hata oldu :"+hata.Message);
                }
                

                //MessageBox.Show("p1 x:"+p1.X+"p1 y:"+p1.Y+"p2 x:"+p2.X+"p2 y:"+p2.Y);
                

                lipstick.Clear();

            } //draw all lines
            drawpoints(updatepointErrors);


        }

        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            point sample;
            Pen pen;



            double posX = (double)(e.X - panel1.Width / 2);
            double posY = (double)(panel1.Height / 2 - e.Y);



            pen = new Pen(Color.FromArgb(ccolor[int.Parse(comboBox1.Text)].r, ccolor[int.Parse(comboBox1.Text)].g, ccolor[int.Parse(comboBox1.Text)].b), 2);
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
            int k = 0;
            int o = 0;

            // single layer and two category
            if (radioButton1.Checked && radioButton3.Checked)
            {



                while (error)
                {
                    error = false;
                    MinimumSquarederror = 0;
                    foreach (point im in samples)
                    {
                        im.error = false;
                        double net = weights[0] * im.X0 + weights[1] * im.X1 - weights[2];
                        if (net > 0)
                        {
                            o = 1;
                        }
                        else
                        {
                            o = -1;
                        }


                        if (im.sınıf == 1)
                            k = 1;
                        else
                            k = -1;



                        if (o != k)// check the error
                        {
                            im.error = true;
                            error = true;
                            MinimumSquarederror += ((k - o) * (k - o)) / 2; // E=1/2 *(d-o)*(d-o)
                            weights[0] = weights[0] + learningcoefficent * (k - o) * im.X0 / 2; //updating weights w=w+1/2*c*(d-o)y
                            weights[1] = weights[1] + learningcoefficent * (k - o) * im.X1 / 2; //updating weights
                            weights[2] = weights[2] + learningcoefficent * (k - o) * -1 / 2; //updating weights
                        }

                    }

                    // draw function on every update of weights
                    counter++;
                    drawing();
                    Weighttable(sender, e);
                    if (counter > maximumloopnumber)
                    {
                        MessageBox.Show(@"Unfortunately the total number of educational cycles exceeded the maximum.
And the training failed.
You can try again by increasing the maximum number of cycles or by slightly changing the sample set. \n Total cycle number= " + counter, "Big failure with over try", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        MessageBox.Show("" + MinimumSquarederror + "" + error + "" + learningcoefficent);
                        drawing(true);
                        gui();
                        return;
                    }
                    else progressBar1.Value = (int)((((double)counter) / ((double)maximumloopnumber)) * 100);
                } // learning
                
                MessageBox.Show(" operation is succesfull \nTotal learning cycle:" + counter + "\nTotal Error:" + MinimumSquarederror);
            }


            // single layer and multi category
            else if (radioButton1.Checked && radioButton4.Checked)
            {


                while (error) //keep learning until there is no more error
                {
                    error = false;
                    MinimumSquarederror = 0;

                    foreach (point im in samples) //control every point
                    {
                        im.error = false;
                        for (int i = 0; i < ccolor.Length; i++) // control with every category
                        {
                            double net = weights[0 + i * 3] * im.X0 + weights[1 + i * 3] * im.X1 - weights[2 + i * 3];
                            if (net > 0)
                            {
                                o = 1;
                            }
                            else
                            {
                                o = -1;
                            }


                            if (im.sınıf == i)
                                k = 1;
                            else
                                k = -1;



                            if (o != k)// check the error
                            {
                                im.error = true;
                                error = true;
                                MinimumSquarederror += ((k - o) * (k - o)) / 2; // E=1/2 *(d-o)*(d-o)
                                weights[0 + i * 3] = weights[0 + i * 3] + learningcoefficent * (k - o) * im.X0 / 2; //updating weights w=w+1/2*c*(d-o)y
                                weights[1 + i * 3] = weights[1 + i * 3] + learningcoefficent * (k - o) * im.X1 / 2; //updating weights
                                weights[2 + i * 3] = weights[2 + i * 3] + learningcoefficent * (k - o) * -1 / 2; //updating weights
                            }

                        }




                    }

                    // draw function on every update of weights
                    counter++;
                    drawing();
                    Weighttable(sender, e);
                    
                    if (counter > maximumloopnumber)
                    {
                        MessageBox.Show(@"Unfortunately the total number of educational cycles exceeded the maximum.
                        And the training failed.
                        You can try again by increasing the maximum number of cycles or by slightly changing 
                        the sample set. \n Total cycle number= " + counter, "Big failure with over try", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        MessageBox.Show("" + MinimumSquarederror + "" + error + "" + learningcoefficent);
                        drawing(true);
                        return;
                    }
                    else progressBar1.Value = (int)((((double)counter) / ((double)maximumloopnumber)) * 100);
                } // learning
                MessageBox.Show(" operation is succesfull \nTotal learning cycle:" + counter + "\nTotal Error:" + MinimumSquarederror);
                

            }


            // multi layer exor
            else
            {


                int i;





                while (error)
                {
                    error = false;


                    for (i = 0; i <= samples.Count - 1; i++) // her örnek icin bir step 
                    {
                        double x0 = samples[i].X0;
                        double x1 = samples[i].X1;
                        double y1, y2, y3;

                        double net1 = (weights[0] * x0) + (weights[1] * x1) - 1 * (weights[2]);
                        if (0 < net1)
                            y1 = 1;
                        else
                            y1 = -1;

                        double net2 = (weights[3] * x0) + (weights[4] * x1) - 1 * (weights[5]);
                        if (0 < net2)
                            y2 = 1;
                        else
                            y2 = -1;

                        double net3 = (weights[6] * y1) + (weights[7] * y2) - 1 * (weights[8]);
                        if (0 < net3)
                            y3 = 1;
                        else
                            y3 = -1;

                        if (samples[i].sınıf == 1)
                            k = 1;
                        else
                            k = -1;

                        MinimumSquarederror += (k - y3) * (k - y3) / 2;

                        double s3 = (k - y3);
                        double s1 = weights[6] * s3;
                        double s2 = weights[7] * s3;

                        weights[6] += learningcoefficent * y1 * (k - y3) * 0.5;
                        weights[7] += learningcoefficent * y2 * (k - y3) * 0.5;
                        weights[8] += learningcoefficent * -1 * (k - y3) * 0.5;

                        weights[3] += learningcoefficent * x0 * weights[7] * (k - y3) * 0.5;
                        weights[4] += learningcoefficent * x1 * weights[7] * (k - y3) * 0.5;
                        weights[5] += learningcoefficent * -1 * weights[7] * (k - y3) * 0.5;

                        weights[0] += learningcoefficent * x0 * weights[6] * (k - y3) * 0.5;
                        weights[1] += learningcoefficent * x1 * weights[6] * (k - y3) * 0.5;
                        weights[2] += learningcoefficent * -1 * weights[6] * (k - y3) * 0.5;







                        //MessageBox.Show("hata0"+ s+"\nhata1"+hata1+"\nhata2"+hata2, "");
                    }



                    Weighttable(sender, e);
                    drawing();
                    counter++;


                }

                string success;
                if (counter == maximumloopnumber)
                    success = "maksimum iterasyon aşıldığı için başarısız";
                else success = "başarılı";
                string sonuc = "Başarı Durumu: " + success + "\nHata değeri: " + MinimumSquarederror + "\nCycle (iterasyon) sayısı : " + counter;
                MessageBox.Show(sonuc, "Öğrenme Sonucu:");


            }
            gui();
        }

        private void button2_Click(object sender, EventArgs e) // continous perceptron leaning
        {
            gui();
        }

        private void drawpoints(bool updatepointError=false) // drawing points in list
        {

            if (updatepointError)
            {
                int o, k;
                foreach (point im in samples)
                {
                    im.error = false;
                    double net = weights[0] * im.X0 + weights[1] * im.X1 - weights[2];
                    if (net > 0)
                        o = 1;
                    else
                        o = -1;
                    if (im.sınıf == 1)
                        k = 1;
                    else
                        k = -1;
                    
                    if (o != k)// check the error
                        im.error = true;
                    }

            }

            foreach (point sample in samples)
            {

                double posX = (panel1.Width / 2) + sample.X0;
                double posY = (panel1.Height / 2) - sample.X1;

                Pen pen;

                pen = new Pen(Color.FromArgb(ccolor[(int)sample.sınıf].r, ccolor[(int)sample.sınıf].g, ccolor[(int)sample.sınıf].b), 2);


                //drawing points
                grafik.DrawLine(pen, new Point((int)posX - 3, (int)posY), new Point((int)posX + 3, (int)posY));
                grafik.DrawLine(pen, new Point((int)posX, (int)posY - 3), new Point((int)posX, (int)posY + 3));

                pen = new Pen(Color.Red);
                if (sample.error == true)
                {
                    Rectangle r1 = new Rectangle((int)posX - 5, (int)posY - 5, 10, 10);
                    grafik.DrawEllipse(pen, r1);
                }
            }
        }

        public void button5_Click(object sender, EventArgs e) // clear button which clear
        {

            progressBar1.Value = 0;
            lines.Clear();
            weightstring.Clear();
            foreach (Button b in dynamicbuttons)
                this.Controls.Remove(b);
            Invalidate();


            samples.Clear(); // erase points           
            grafik.Clear(Color.White); // cleaning


            // drawing dimensions
            grafik.DrawLine(new Pen(Color.Black), new Point(0, panel1.Height / 2), new Point(panel1.Width, panel1.Height / 2));
            grafik.DrawLine(new Pen(Color.Pink, 2), new Point(panel1.Width / 2, 0), new Point(panel1.Width / 2, panel1.Height));

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            maximumloopnumber = Int32.Parse(textBox1.Text);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            button5_Click(sender, e);
            comboBox1.Items.Clear();
            weights.Clear();
            Random rast = new Random();
            ccolor = new classs[Int32.Parse(textBox2.Text)];
                for (int i = 0; i < Int32.Parse(textBox2.Text); i++)
                {
                    ccolor[i] = new classs(i);
                    comboBox1.Items.Add(i + "");
                    ccolor[i].numara = i;
                    ccolor[i].r = rast.Next(256);
                    ccolor[i].g = rast.Next(256);
                    ccolor[i].b = rast.Next(256);

                    weights.Add(rast.NextDouble());
                    weights.Add(rast.NextDouble());
                    weights.Add(rast.NextDouble());

                }
            Weighttable(sender, e);
            gui();
             
            
            ////////////////////////////////////
            
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e) //multicategory checked ?
        {
            button5_Click(sender, e);
            comboBox1.Items.Clear();
            weights.Clear();
            Random rast = new Random();
            if (radioButton4.Checked) //if multicategory is choosen
            {


                label8.Visible = true;
                textBox2.Visible = true;
                ccolor = new classs[Int32.Parse(textBox2.Text)];
                for (int i = 0; i < Int32.Parse(textBox2.Text); i++)
                {
                    ccolor[i] = new classs(i);
                    comboBox1.Items.Add(i + "");
                    ccolor[i].numara = i;
                    ccolor[i].r = rast.Next(256);
                    ccolor[i].g = rast.Next(256);
                    ccolor[i].b = rast.Next(256);

                    weights.Add(rast.NextDouble());
                    weights.Add(rast.NextDouble());
                    weights.Add(rast.NextDouble());

                }
                Weighttable(sender, e);


            }
            else //if two category choosen
            {


                weights.Add(rast.NextDouble());
                weights.Add(rast.NextDouble());
                weights.Add(rast.NextDouble());

                ccolor = new classs[2];
                for (int i = 0; i < 2; i++)
                {
                    ccolor[i] = new classs(i);
                    ccolor[i].numara = i;
                    ccolor[i].r = rast.Next(256);
                    ccolor[i].g = rast.Next(256);
                    ccolor[i].b = rast.Next(256);

                    comboBox1.Items.Add(i + "");
                }

                label8.Visible = false;
                textBox2.Visible = false;
                Weighttable(sender, e);
            }

            gui();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


        private static String InsertTableInRichTextBox(DataTable dtbl, int width)
        {
            //Since too much string appending go for string builder
            StringBuilder sringTableRtf = new StringBuilder();

            //beginning of rich text format,dont customize this begining line
            sringTableRtf.Append(@"{\rtf1 ");

            //create 5 rows with 3 cells each
            int cellWidth;

            //Start the Row
            sringTableRtf.Append(@"\trowd");

            //Populate the Table header from DataTable column headings.
            for (int j = 0; j < dtbl.Columns.Count; j++)
            {
                //A cell with width 1000.
                sringTableRtf.Append(@"\cellx" + ((j + 1) * width).ToString());

                if (j == 0)
                    sringTableRtf.Append(@"\intbl  " + dtbl.Columns[j].ColumnName);
                else
                    sringTableRtf.Append(@"\cell   " + dtbl.Columns[j].ColumnName);
            }

            //Add the table header row
            sringTableRtf.Append(@"\intbl \cell \row");

            //Loop to populate the table cell data from DataTable
            for (int i = 0; i < dtbl.Rows.Count; i++)
            {
                //Start the Row
                sringTableRtf.Append(@"\trowd");

                for (int j = 0; j < dtbl.Columns.Count; j++)
                {
                    cellWidth = (j + 1) * width;

                    //A cell with width 1000.
                    sringTableRtf.Append(@"\cellx" + cellWidth.ToString());

                    if (j == 0)
                        sringTableRtf.Append(@"\intbl  " + dtbl.Rows[i][j].ToString());
                    else
                        sringTableRtf.Append(@"\cell   " + dtbl.Rows[i][j].ToString());
                }

                //Insert data row
                sringTableRtf.Append(@"\intbl \cell \row");
            }

            sringTableRtf.Append(@"\pard");
            sringTableRtf.Append(@"}");

            //convert the string builder to string
            return sringTableRtf.ToString();
        }
        private void Weighttable(object sender, EventArgs e)
        {
            //Create a DataTable with four columns.
            DataTable dtbl = new DataTable();
            dtbl.Columns.Add("Node No", typeof(int));
            dtbl.Columns.Add("weight0", typeof(double));
            dtbl.Columns.Add("weight1", typeof(double));
            dtbl.Columns.Add("weight2", typeof(double));

            for (int i = 0; i < weights.Count; i += 3)
            {
                dtbl.Rows.Add(i / 3, (Math.Truncate(weights[i] * 1000)) / 1000, (Math.Truncate(weights[i + 1] * 1000)) / 1000, (Math.Truncate(weights[i + 2] * 1000)) / 1000);
            }


            this.richTextBox1.Rtf = InsertTableInRichTextBox(dtbl, 700);
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e) //radiobuton2 is multilayer buton
        {
            button5_Click(sender, e); // clear button which clear

            comboBox1.Items.Clear();
            weights.Clear();
            Random rast = new Random();


            if (radioButton2.Checked) //if multilayer choosen
            {
                panel3.Visible = false;

                for (int i = 0; i < 9; i++)
                    weights.Add(rast.NextDouble());


                ccolor = new classs[2];
                for (int i = 0; i < 2; i++)
                {
                    ccolor[i] = new classs(i);
                    ccolor[i].numara = i;
                    ccolor[i].r = rast.Next(256);
                    ccolor[i].g = rast.Next(256);
                    ccolor[i].b = rast.Next(256);

                    comboBox1.Items.Add(i + "");
                }

                Weighttable(sender, e);
                label8.Visible = false;
                textBox2.Visible = false;
            }
            else //single layer choosen
            {
                panel3.Visible = true;
                if (radioButton4.Checked) //if multicategory is choosen
                {

                    label8.Visible = true;
                    textBox2.Visible = true;
                    ccolor = new classs[Int32.Parse(textBox2.Text)];
                    for (int i = 0; i < Int32.Parse(textBox2.Text); i++)
                    {
                        ccolor[i] = new classs(i);
                        comboBox1.Items.Add(i + "");
                        ccolor[i].numara = i;
                        ccolor[i].r = rast.Next(256);
                        ccolor[i].g = rast.Next(256);
                        ccolor[i].b = rast.Next(256);

                        weights.Add(rast.NextDouble());
                        weights.Add(rast.NextDouble());
                        weights.Add(rast.NextDouble());

                    }
                    Weighttable(sender, e);


                }
                else //if two category choosen
                {

                    weights.Add(rast.NextDouble());
                    weights.Add(rast.NextDouble());
                    weights.Add(rast.NextDouble());

                    ccolor = new classs[2];
                    for (int i = 0; i < 2; i++)
                    {
                        ccolor[i] = new classs(i);
                        ccolor[i].numara = i;
                        ccolor[i].r = rast.Next(256);
                        ccolor[i].g = rast.Next(256);
                        ccolor[i].b = rast.Next(256);

                        comboBox1.Items.Add(i + "");
                    }
                    Weighttable(sender, e);
                    label8.Visible = false;
                    textBox2.Visible = false;

                }
                gui();
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e) //draw dimensions at start
        {
           
            grafik2.DrawLine(new Pen(Color.Black), new Point(0, panel2.Height / 2), new Point(panel2.Width, panel2.Height / 2));
            grafik2.DrawLine(new Pen(Color.Pink), new Point(panel2.Width / 2, 0), new Point(panel2.Width / 2, panel2.Height));

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        public void drawbutton(double x,double y,double xsize=20,double ysize=20)
        {
            int x1 = (int)x + 869;
            int y1 = (int)y + 248;
            int xsize1 = (int)xsize;
            int ysize1 = (int)ysize;

            ShapedButton elips_Buton = new ShapedButton();

            elips_Buton.Size = new Size(xsize1,ysize1);

            elips_Buton.Location = new Point(x1,y1 );

            elips_Buton.FlatAppearance.BorderSize = 0;

            elips_Buton.BackColor = Color.Blue;

            elips_Buton.FlatStyle = FlatStyle.Flat;
            
            dynamicbuttons.Add(elips_Buton);
            this.Controls.Add(elips_Buton);
        }

        //public void Drawweightlabel(double weight,int locationx1,int locationy1,int locationx2,int locationy2,int sizex=64,int sizey=64)
        //{
        //    // mylabel a = new mylabel();
        //    // a.Location = new Point((2*locationx1+locationx2)/3+869-(sizex/2), (2*locationy1 + locationy2) / 3+248-(sizey/2));
        //    // a.Location = new Point(locationx1 + 869-62, locationy1  + 248-62);
        //    // a.Size = new Size(sizex, sizey);

        //    // double tangle = ((double)(locationy1 - locationy2) / (double)(locationx1 - locationx2));
        //    // int angle = (int)((180 / Math.PI)* Math.Atan(tangle));
        //    // a.RotateAngle = angle;
        //    // weight = Math.Truncate(weight*1000)/1000;  
        //    // a.NewText = "dsafdsafdsafdsa" + weight;
        //    // a.BackColor = Color.Transparent;
        //    // a.ForeColor = Color.Tomato;
        //    // //a.AutoSize = true;

        //    // this.Controls.Add(a);

            
           
        //}

        public void drawline(Graphics g, int x, int y, int x2, int y2,  int width = 2)
        {
            

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Pen myPen = new Pen(Color.Red);
            myPen.Width = width;
            g.DrawLine(myPen, x + 869, y + 248, x2 + 869, y2 + 248);

        }
        
        public void drawweightstring(PaintEventArgs e, int x, int y, int x2, int y2, double weight)
        {

            double tangle = ((double)(y - y2) / (double)(x - x2));
            int angle = (int)((180 / Math.PI) * Math.Atan(tangle));
            int RotateAngle = angle;
            

            // Create font and brush.
            Font drawFont = new Font("Arial", 10);
            SolidBrush drawBrush = new SolidBrush(Color.Tomato);

            // Create point for upper-left corner of drawing.
            PointF drawPoint = new Point((2 * x + x2) / 3 + 869 , (2 * y + y2) / 3 + 248 );
            //PointF drawPoint = new Point(400, 400);
            e.Graphics.TranslateTransform((2 * x + x2) / 3 + 869, (2 * y + y2) / 3 + 248);
            e.Graphics.RotateTransform(RotateAngle);
            weight = Math.Truncate(weight * 1000) / 1000000;
            e.Graphics.DrawString(""+weight, drawFont, drawBrush, 0,0);
            
            e.Graphics.ResetTransform();
        }

        public void gui()
        {
            
            int k = 13;
            int e = 9;
            lines.Clear();
            weightstring.Clear();
            foreach (Button b in dynamicbuttons)
                this.Controls.Remove(b);
            Invalidate();

            
            drawbutton(100, 100, 30, 30);
            dynamicbuttons[0].Text = "X";
            drawbutton(100, 200, 30, 30);
            dynamicbuttons[1].Text = "Y";
            drawbutton(100, 300, 30, 30);
            dynamicbuttons[2].Text = "-1";
            int number=weights.Count / 3;
            switch (number)
            {
                case 1:
                    {
                        
                        drawbutton(250,200);
                        addline(100+k,100 + k, 250+e,200+e);
                        addweightstring(100 + k, 100 + k, 250 + e, 200 + e, weights[0]);
                        addline(100 + k, 200 + k, 250+e, 200+e);
                        addweightstring(100 + k, 200 + k, 250 + e, 200 + e, weights[1]);
                        addline(100 + k, 300 + k, 250+e, 200+e);
                        addweightstring(100 + k, 300 + k, 250 + e, 200 + e, weights[2]);
                        addline(250 + e, 200 + e, 350 + e, 200 + e);
                    }
                    break;
                case 2:
                    {
                        drawbutton(250,150);
                        addline(100 + k, 100 + k, 250 + e, 150 + e);
                        addweightstring(100 + k, 100 + k, 250 + e, 150 + e, weights[0]);
                        addline(100 + k, 200 + k, 250 + e, 150 + e);
                        addweightstring(100 + k, 200 + k, 250 + e, 150 + e, weights[1]);
                        addline(100 + k, 300 + k, 250 + e, 150 + e);
                        addweightstring(100 + k, 300 + k, 250 + e, 150 + e, weights[2]);
                        addline(250 + e, 150 + e, 350 + e, 150 + e);

                        drawbutton(250,250);
                        addline(100 + k, 100 + k, 250 + e, 250 + e);
                        addweightstring(100 + k, 100 + k, 250 + e, 250 + e, weights[3]);
                        addline(100 + k, 200 + k, 250 + e, 250 + e);
                        addweightstring(100 + k, 200 + k, 250 + e, 250 + e, weights[4]);
                        addline(100 + k, 300 + k, 250 + e, 250 + e);
                        addweightstring(100 + k, 300 + k, 250 + e, 250 + e, weights[5]);
                        addline(250 + e, 250 + e, 350 + e, 250 + e);
                    }
                    break;
                default:
                    {
                        int length = 440 /( number-1);
                        for (int i = 0; i < number; i++)
                        {
                            drawbutton(250, i * length);
                            addline(100 + k, 100 + k, 250 + e, i * length + e);
                            addweightstring(100 + k, 100 + k, 250 + e, i * length + e, weights[i*3]);
                            addline(100 + k, 200 + k, 250 + e, i * length + e);
                            addweightstring(100 + k, 200 + k, 250 + e, i * length + e, weights[1+i*3]);
                            addline(100 + k, 300 + k, 250 + e, i * length + e);
                            addweightstring(100 + k, 300 + k, 250 + e, i * length + e, weights[2+i*3]);
                            addline(250+k, i * length+k, 350 + k, i * length + k);
                        }
                            
                    }
                    break;

            }

            

        }

        

        


        public void addline(int x1,int y1,int x2,int y2)
        {
            lines.Add(x1);
            lines.Add(y1);
            lines.Add(x2);
            lines.Add(y2);
        }
        public void addweightstring(int x1, int y1, int x2, int y2, double weight)
        {
            weightstring.Add(x1);
            weightstring.Add(y1);
            weightstring.Add(x2);
            weightstring.Add(y2);
            weightstring.Add((int)(weight*1000));
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            
            
            for (int i = 0; i < lines.Count; i += 4)
            {
                try
                {
                    drawline(e.Graphics, lines[0 + i ], lines[1 + i ], lines[2 + i ], lines[3 + i ]);
                }
                catch (Exception hata)
                {
                    MessageBox.Show("hata var form1 paint drawline");
                }
            }

            for (int i = 0; i < weightstring.Count; i += 5)
            {
                try
                {
                    drawweightstring(e, weightstring[0 + i], weightstring[1 + i], weightstring[2 + i], weightstring[3 + i], i);
                }
                catch (Exception hata)
                {
                    MessageBox.Show("hata var form1 paint drawstring");
                }
            }
            ////////////////////////

            StringFormat format = new StringFormat();
            //format.Alignment = StringAlignment.Center;

            SizeF txt = e.Graphics.MeasureString(Text, this.Font);
            SizeF sz = e.Graphics.VisibleClipBounds.Size;

            //////90 degrees
            //e.Graphics.TranslateTransform(sz.Width, 0);
            ////e.Graphics.RotateTransform(180);
            //e.Graphics.RotateTransform(90);
            //e.Graphics.DrawString(Text, this.Font, Brushes.Black, new RectangleF(0, 0, sz.Height, sz.Width), format);
            ////e.Graphics.ResetTransform();

            ////180 degrees
            //e.Graphics.TranslateTransform(sz.Width, sz.Height);
            //e.Graphics.RotateTransform(180);
            //e.Graphics.DrawString(Text, this.Font, Brushes.Black, new RectangleF(0, 0, sz.Width, sz.Height), format);
            //e.Graphics.ResetTransform();

            ////270 degrees
            //e.Graphics.TranslateTransform(0, sz.Height);
            //e.Graphics.RotateTransform(270);
            //e.Graphics.DrawString(Text, this.Font, Brushes.Black, new RectangleF(0, 0, sz.Height, sz.Width), format);
            //e.Graphics.ResetTransform();

            //0 = 360 degrees
            e.Graphics.TranslateTransform(100, 0);
            e.Graphics.RotateTransform(45);
            e.Graphics.TranslateTransform(0, 0);
            e.Graphics.DrawString(Text, this.Font, Brushes.Black, new RectangleF(0, 0, sz.Width, sz.Height), format);
            e.Graphics.ResetTransform();


        }

        private void button2_Paint(object sender, PaintEventArgs e)
        {

        }
    }

    public class point
    {
        double x0;
        double x1;
        double snf;
        public bool error;

        public point(double x0, double x1, double snf)
        {
            this.x0 = x0;
            this.x1 = x1;
            this.snf = snf;
            this.error = false;
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

        public double sınıf
        {
            get { return snf; }
            set { this.snf = value; }
        }

    } // ornek

    public class classs
    {
        public int r, g, b;
        public double numara;

        public classs(int i)
        {
            Random rastgele = new Random(i);
            r = rastgele.Next(1, 255);
            g = rastgele.Next(1, 255);
            b = rastgele.Next(1, 255);

            numara = i;
        }
    } // sınıf

    public class ShapedButton : Button

    {

        protected override void OnResize(EventArgs e)

        {

            base.OnResize(e);

            GraphicsPath gp = new GraphicsPath();

            // this.Size ın en boyu aynı oldugundan elips bir şekil oluşuyor

            gp.AddEllipse(new Rectangle(Point.Empty, Size));

            // oluşan yeni elips şekli oluşturulan butona atanıyor

            this.Region = new Region(gp);

            // Butonun yeni şekli elips oluyor

        }

    }

    public class mylabel: Label    
    {
        public int RotateAngle { get; set; }  // to rotate your text
        public string NewText { get; set; }   // to draw text
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            Brush b = new SolidBrush(this.ForeColor);
            e.Graphics.TranslateTransform(this.Width / 2, this.Height / 2);
            e.Graphics.RotateTransform(this.RotateAngle);
            e.Graphics.DrawString(this.NewText, this.Font, b, 0f, 0f);

            // Create string to draw.
            String drawString = "Sample Text";

            // Create font and brush.
            Font drawFont = new Font("Arial", 16);
            SolidBrush drawBrush = new SolidBrush(Color.Black);

            // Create point for upper-left corner of drawing.
            PointF drawPoint = new PointF(1000.0F, 450.0F);

            // Draw string to screen.
            e.Graphics.DrawString(drawString, drawFont, drawBrush, drawPoint);


        }
    }

    
}