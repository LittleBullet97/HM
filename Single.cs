using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NM {
    public partial class Single : Form {

        public Single () {
            InitializeComponent();
        }
        Bitmap img;
        Bitmap img_gray;
        private void button1_Click (object sender, EventArgs e) {
            img = new Bitmap(@"D:\Desktop\img.jpg");
            pictureBox1.Image = img;
            pictureBox1.Width = img.Width;
            pictureBox1.Height = img.Height;
            pictureBox2.Width = img.Width;
            pictureBox2.Height = img.Height;
            pictureBox3.Width = img.Width;
            pictureBox3.Height = img.Height;
            button2.Enabled = true;
        }

        private void button2_Click (object sender, EventArgs e) {
            img_gray = convertToGray(img);
            pictureBox2.Image = img_gray;
            button3.Enabled = true;
        }

        private void button3_Click (object sender, EventArgs e) {
            richTextBox1.Clear();
            fixXY();
            int x = Int32.Parse(textBox1.Text);
            int y = Int32.Parse(textBox2.Text);
            List<int> avr_gray_value = new List<int>();
            avr_gray_value = AverageGray(img_gray, x, y);
            pixelization(avr_gray_value, img_gray, x, y);

        }

        public static Bitmap convertToGray (Bitmap img) {
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
        private void fixXY () {
            int x = ( textBox1.Text == "" || textBox1.Text == "0" ) ? x = 3 : x = Int32.Parse(textBox1.Text);
            int y = ( textBox2.Text == "" || textBox2.Text == "0" ) ? y = 3 : y = Int32.Parse(textBox2.Text);
            x = x > img.Width ? x = img.Width : ( x < 1 ? x = 1 : x );
            y = y > img.Height ? y = img.Height : ( y < 1 ? y = 1 : y );
            textBox1.Text = x.ToString();
            textBox2.Text = y.ToString();
        }
        private Bitmap resize (Bitmap img, int x, int y) {
            Bitmap result = new Bitmap(x, y);
            using (Graphics g = Graphics.FromImage(result)) {
                g.DrawImage(img, 0, 0, x, y);
            }
            return result;
        }
        private List<int> AverageGray (Bitmap img, int count_rectX, int count_rectY) {
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

            img = resize(img, rectX * count_rectX, rectY * count_rectY);
            pictureBox3.Width = rectX * count_rectX;
            pictureBox3.Height = rectY * count_rectY;
           

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
             richTextBox1.AppendText("--------------------------\n");
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

        private void button4_Click (object sender, EventArgs e) {
            Multy ml = new Multy();
            this.Hide();
            ml.Show();
        }

        private void button5_Click (object sender, EventArgs e) {
            int[,] test = new int[2, 3];
            
            for (int i = 0; i < 2; i++) {
                for (int j = 0; j < 3; j++) {
                    test[i, j] = (j + j * i)+1;
                }
            }
            double sum = 0;
            for (int i = 0; i < 2; i++) {
                for (int j = 0; j < 3; j++) {
                    sum += test[i, j];
                }
            }
            double res = sum / 6;
            MessageBox.Show(res.ToString());
        }
    }
}
