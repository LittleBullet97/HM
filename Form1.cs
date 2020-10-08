using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace NM {
    public partial class Form1 : Form {
        public Form1 () {
            InitializeComponent();
        }
        Bitmap img;
        Bitmap img_gray;
        private void button1_Click (object sender, EventArgs e) {

            //img = new Bitmap("C:\\Users\\Asus\\Pictures\\background\\Wallpaper-Space.jpg");
            img = new Bitmap(@"C:\Users\Asus\Desktop\img.jpg");
            //img = new Bitmap (@"C:\Users\Asus\Desktop\us.png");
            pictureBox1.Image = img;
            button2.Enabled = true;

        }

        private void button2_Click (object sender, EventArgs e) {
            img_gray = convertToGray(img);
            pictureBox2.Image = img_gray;
            button3.Enabled = true;
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

        private List<int> AverageGray (Bitmap img, int count_rectX, int count_rectY) {

            List<int>[] lists = new List<int>[count_rectX * count_rectY];
            List<int> result = new List<int>();


            for (int i = 0; i < lists.Length; i++) {
                lists[i] = new List<int>();
            }
            int x1, y1;
            int rectX = img.Width / count_rectX;
            int rectY = img.Height / count_rectY;

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
                    richTextBox1.AppendText(temp.ToString() + "   ");
                    result.Add((int) temp);
                }
                richTextBox1.AppendText("\n");

            }
            return result;

        }

        private void pixelization (List<int> value, Bitmap img, int count_rectX, int count_rectY) {
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
        private void scaleAndShifting (List<int> avr_gray_value) {
           

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
            richTextBox1.AppendText(" ---------------\n");
            for (int i = 0; i < scaled_gray.Count; i++) {
                richTextBox1.AppendText(scaled_gray[i].ToString() + "\n");
            }
        }


        private void button3_Click (object sender, EventArgs e) {
            richTextBox1.Clear();
            int x = Int32.Parse(textBox1.Text);
            int y = Int32.Parse(textBox2.Text);
            if (x >= img.Width) {
                x = img.Width;
            }
            if (y >= img.Height) {
                y = img.Height;
            }
            List<int> avr_gray_value = new List<int>();
            avr_gray_value = AverageGray(img_gray, x, y);
            pixelization(avr_gray_value, img_gray, x, y);

            scaleAndShifting(avr_gray_value);


        }

        private void Form1_Load (object sender, EventArgs e) {
            
        }
    }
}
