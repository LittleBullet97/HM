using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace NM {
    public partial class Form1 : Form {
        public Form1 () {
            InitializeComponent();
        }

        private void Form1_Load (object sender, EventArgs e) {
            textBox3.Text = "";
            textBox6.Text = 0.1.ToString();
            textBox7.Text = 100.ToString();
        }

        Bitmap img;
        Bitmap img_gray;
        List<List<Bitmap>> pictures = new List<List<Bitmap>>();
        string filename = @"C:\Users\Asus\Desktop\values.xml";
        private void loadPictures () {
            string[] all_folder_tranning = Directory.GetDirectories(@"C:\Users\Asus\Desktop\training");
            foreach (string folder_tranning in all_folder_tranning) {
                string[] object_tranning = Directory.GetFiles(folder_tranning);
                List<Bitmap> obj = new List<Bitmap>();
                foreach (string file in object_tranning) {
                    obj.Add(new Bitmap(file));
                }
                pictures.Add(obj);
            }
        }
        private void loadPicture_Click (object sender, EventArgs e) {
            //img = new Bitmap("C:\\Users\\Asus\\Pictures\\background\\Wallpaper-Space.jpg");
            img = new Bitmap(@"C:\Users\Asus\Desktop\img.jpg"); 
            //img = new Bitmap (@"C:\Users\Asus\Desktop\us.png");
            // img = resize(img, pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = img;
            pictureBox1.Width = img.Width;
            pictureBox1.Height = img.Height;
            pictureBox2.Width = img.Width;
            pictureBox2.Height = img.Height;
            pictureBox3.Width = img.Width;
            pictureBox3.Height = img.Height;
            button2.Enabled = true;
        }

        private void transformToGray_Click (object sender, EventArgs e) {
            img_gray = convertToGray(img);
            pictureBox2.Image = img_gray;
            singlePicture.Enabled = true;
        }
        private Bitmap resize (Bitmap img, int x, int y) {
            Bitmap result = new Bitmap(x, y);
            using (Graphics g = Graphics.FromImage(result)) {
                g.DrawImage(img, 0, 0, x, y);
            }
            return result;
        }
        private Bitmap convertToGray (Bitmap img) {
            Bitmap result = new Bitmap(img.Width, img.Height);
            for (int x = 0; x < img.Width; x++) {
                for (int y = 0; y < img.Height; y++) {
                    Color c = img.GetPixel(x, y);
                    int n = (int) Math.Sqrt(( Math.Pow(c.R, 2) + Math.Pow(c.G, 2) + Math.Pow(c.B, 2) ) / 3);
                    result.SetPixel(x, y, Color.FromArgb(c.A, n, n, n));
                }
            }
            return result;
        }

        private List<int> AverageGray (Bitmap img, int count_rectX, int count_rectY,bool single_pic) {
            // list for every rect
            List<int>[] lists = new List<int>[count_rectX * count_rectY];

            List<int> result = new List<int>();

            //init list
            for (int i = 0; i < lists.Length; i++) {
                lists[i] = new List<int>();
            }

            int x1, y1;
            int rectX = img.Width / count_rectX;
            int rectY = img.Height / count_rectY;
            
            if (single_pic) {
                img = resize(img, rectX * count_rectX, rectY * count_rectY);
                pictureBox3.Width = rectX * count_rectX;
                pictureBox3.Height = rectY * count_rectY;
            }

            for (int x = 0; x < rectX * count_rectX; x++) {
                for (int y = 0; y < rectY * count_rectY; y++) {
                    Color c = img.GetPixel(x, y);
                    x1 = x / rectX;
                    y1 = y / rectY;
                    int index = x1 + y1 * count_rectX;
                    if (x >= x1 * rectX && x < x1 * rectX + rectX && y >= y1 * rectY && y < y1 * rectY + rectY) {
                        lists[index].Add(c.R);
                    }

                }
            }

            for (int y = 0; y < count_rectY; y++) {
                for (int x = 0; x < count_rectX; x++) {
                    int loc = x + y * count_rectX;
                    double temp = Math.Round(lists[loc].Average());
                   if(single_pic) richTextBox1.AppendText(temp.ToString() + "   ");
                    result.Add((int) temp);
                }
                if (single_pic) richTextBox1.AppendText("\n");

            }
            if (single_pic) richTextBox1.AppendText("--------------------------\n");
            return result;

        }

        private void pixelization (List<int> value, Bitmap img, int count_rectX, int count_rectY) {// is used only with single pic, show the pic
            Bitmap result = new Bitmap(img.Width, img.Height);
            int x1, y1;
            int rectX = img.Width / count_rectX;
            int rectY = img.Height / count_rectY;
            for (int x = 0; x < rectX * count_rectX; x++) {
                for (int y = 0; y < rectY * count_rectY; y++) {
                    x1 = x / rectX;
                    y1 = y / rectY;
                    int index = x1 + y1 * count_rectX;
                    if (x >= x1 * rectX && x < x1 * rectX + rectX && y >= y1 * rectY && y < y1 * rectY + rectY) {
                        result.SetPixel(x, y, Color.FromArgb(img.GetPixel(x, y).A, value[index], value[index], value[index]));
                    }
                }
            }
            
            pictureBox3.Image = result;
        }

        private List<double> scaleAndShifting (List<int> avr_gray_value) {   
            int old_min_value = avr_gray_value.Min();
            int old_max_value = avr_gray_value.Max();

            double scale = ( 0.9 - 0.1 ) / ( old_max_value - old_min_value );
            List<double> scaled_gray = new List<double>();

            for (int i = 0; i < avr_gray_value.Count; i++) {
                scaled_gray.Add(avr_gray_value[i] * scale);
            }

            double new_min_value = scaled_gray.Min();
            double new_max_value = scaled_gray.Max();

            double shift = 0.1 - new_min_value;


            for (int i = 0; i < scaled_gray.Count; i++) {
                scaled_gray[i] = avr_gray_value[i] * scale + shift;
            }

            return scaled_gray;
        }

        private void fixXY (bool single_pic) { 

            int x = ( textBox1.Text == "" || textBox1.Text == "0" ) ? x = 3 : x = Int32.Parse(textBox1.Text);

            int y = ( textBox2.Text == "" || textBox2.Text == "0" ) ? y = 3 : y = Int32.Parse(textBox2.Text);
            if (single_pic) {
                x = x > img.Width ? x = img.Width : ( x < 1 ? x = 1 : x );
                y = y > img.Height ? y = img.Height : ( y < 1 ? y = 1 : y );
            }
            textBox1.Text = x.ToString();
            textBox2.Text = y.ToString();

        }

        private void singlePicture_Click (object sender, EventArgs e) {
            richTextBox1.Clear();
            fixXY(true);
            int x = Int32.Parse(textBox1.Text);
            int y = Int32.Parse(textBox2.Text);
            List<int> avr_gray_value = new List<int>();
            avr_gray_value = AverageGray(img_gray, x, y,true);
            pixelization(avr_gray_value, img_gray, x, y);
        }
      
 

        private void textBox1_TextChanged (object sender, EventArgs e) {
            sumA();
        }

        private void textBox2_TextChanged (object sender, EventArgs e) {
            sumA();
        }
        private void sumA () {
            String a = textBox1.Text;
            String b = textBox2.Text;
            if (a == "") {
                a = "1";
            }
            if (b == "") {
                b = "1";
            }
            int temp = Int32.Parse(a) * Int32.Parse(b);
            textBox3.Text = temp.ToString();
       
        }

        private void exeAndSaveAllPictures_Click (object sender, EventArgs e) {
            loadPictures();
            //all pictures convert to gray
            for (int folder = 0; folder < pictures.Count; folder++) {
                for (int pic = 0; pic < pictures[folder].Count; pic++) {
                    pictures[folder][pic] = convertToGray(pictures[folder][pic]);
                }

            }
            fixXY(false);
            int x = Int32.Parse(textBox1.Text);
            int y = Int32.Parse(textBox2.Text);
            int max_folder_pic = pictures.Max(t => t.Count);
            int charAfterPoint = 2;

            List<int> average_gray = new List<int>();
            List<int>[,] all_gray_values = new List<int>[pictures.Count, max_folder_pic];//3,42
            List<double>[,] all_shifted_and_scaled_values = new List<double>[pictures.Count, max_folder_pic];//3,42


            //init all_shifted_and+scaled_values
            for (int folder = 0; folder < pictures.Count; folder++) {
                for (int pic = 0; pic < pictures[folder].Count; pic++) {
                    all_gray_values[folder, pic] = new List<int>();
                    all_shifted_and_scaled_values[folder, pic] = new List<double>();
                }
            }
            // adding all gray values for all pictures
            for (int folder = 0; folder < pictures.Count; folder++) {
                for (int pic = 0; pic < pictures[folder].Count; pic++) {
                    average_gray = AverageGray(pictures[folder][pic], x, y,false);
                    for (int k = 0; k < average_gray.Count; k++) {//tuk
                        all_gray_values[folder, pic].Add(average_gray[k]);
                    }
                }
            }
            // adding all scale and shifting values for all pictures
            List<double> scale_and_shifting_values = new List<double>();
            for (int folder = 0; folder < pictures.Count; folder++) {
                for (int pic = 0; pic < pictures[folder].Count; pic++) {
                    scale_and_shifting_values = scaleAndShifting(all_gray_values[folder, pic]);
                    for (int val = 0; val < scale_and_shifting_values.Count; val++) {
                        all_shifted_and_scaled_values[folder, pic].Add(scale_and_shifting_values[val]);
                    }
                }
            }
            richTextBox1.Clear();
            //spit out the richtext box + format double
            for (int folder = 0; folder < pictures.Count; folder++) {
                for (int pic = 0; pic < pictures[folder].Count; pic++) {
                    for (int val = 0; val < scale_and_shifting_values.Count; val++) {
                        all_shifted_and_scaled_values[folder, pic][val] = Math.Round(all_shifted_and_scaled_values[folder, pic][val], charAfterPoint);
                        richTextBox1.AppendText(all_shifted_and_scaled_values[folder, pic][val].ToString() + " ");
                    }
                    richTextBox1.AppendText("\n");

                }
                richTextBox1.AppendText("------------------------\n");
            }
            saveXML(all_shifted_and_scaled_values);
        }
        void saveXML(List<double>[,] values ) {
            XmlTextWriter xmlWriter = new XmlTextWriter(filename, System.Text.Encoding.UTF8);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteComment("Data of pictures");
            xmlWriter.WriteStartElement("values");
            for (int folder = 0; folder < pictures.Count; folder++) {
                xmlWriter.WriteStartElement("folder" + (folder + 1).ToString());
                for (int pic = 0; pic < pictures[folder].Count; pic++) {
                    xmlWriter.WriteStartElement("picture" + (pic + 1).ToString());
                    for (int val = 0; val < values[folder, pic].Count; val++) {
                        xmlWriter.WriteElementString("value", values[folder, pic][val].ToString());
                    }
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndDocument();
            xmlWriter.Flush();
            xmlWriter.Close();
        }
        double[,,] readXML () {
            XmlTextReader xtr = new XmlTextReader(filename);
            int folder = 0;
            List<int> pic = new List<int>();
            int val = 0;
            int br_photos = 0;

            while (xtr.Read()) {
                if (xtr.NodeType == XmlNodeType.Element && xtr.Name.Contains("folder")) {
                    if(folder > 0) pic.Add(br_photos);
                    br_photos = 0;
                    folder++;
                }
                if (xtr.NodeType == XmlNodeType.Element && xtr.Name.Contains("picture")) {
                    val = 0;
                    br_photos++;
                }
                if (xtr.NodeType == XmlNodeType.Element && xtr.Name == "value") {
                    val++;
                }
            }
            pic.Add(br_photos);
            xtr.Close();
            int maxpic = pic.Max();
            MessageBox.Show(maxpic.ToString());
            double[,,] data = new double[folder, maxpic, val];
            for (int i = 0; i < folder; i++) {
                for (int j = 0; j < pic[i]; j++) {
                    for (int k = 0; k < val; k++) {
                        data[i, j, k] = -1;
                    }
                }
            }
            xtr = new XmlTextReader(filename);
            int f = 0, p = 0, v = 0, index = 0;
            while (xtr.Read()) {
                if (xtr.NodeType == XmlNodeType.Text) {
                    data[f, p, v] = Convert.ToDouble(xtr.Value);
                    v++;
                    if (v == val) {
                        v = 0;
                        p++;
                    }
                    if (p == pic[index]) {
                        index++;
                        p = 0;
                        f++;
                    }


                }

            }
            for (int i = 0; i < folder; i++) {
                for (int j = 0; j < pic[i]; j++) {
                    for (int k = 0; k < val; k++) {
                        richTextBox1.AppendText(data[i, j, k].ToString() + " ");
                    }
                    richTextBox1.AppendText("\n");
                }
                richTextBox1.AppendText("************\n");
            }
            return data;


        }
        private void loadValues_Click (object sender, EventArgs e) {

            readXML();

        }
    }
}
