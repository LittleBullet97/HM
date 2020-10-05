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
            img = new Bitmap (@"C:\Users\Asus\Desktop\gray.png");
            //img = new Bitmap (@"C:\Users\Asus\Desktop\img.jpg");
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

        private void pixialization (Bitmap img,int count_rectX,int count_rectY) {

            List<int>[] lists = new List<int>[count_rectX*count_rectY];
            List<int> result = new List<int>();

            for (int i = 0; i < lists.Length; i++) {
                lists[i] = new List<int>();
            }
            int x1, y1;
            int rectX = img.Width / count_rectX;//100
            int rectY = img.Height / count_rectY;//75
            for (int x = 0; x < img.Width; x++) {
                for (int y = 0; y < img.Height; y++) {
                    Color c = img.GetPixel(x, y);
                    
                    x1 = x / rectX;
                    y1 = y / rectY;
                    int index = x1 + y1 * count_rectX;
                    if (x > x1 * rectX && x <= x1 * rectX + rectX && y > y1 * rectY && y <= y1 * rectY + rectY) {
                        lists[index].Add(c.R);
                    }
                        
                }
            }
          

            for (int y = 0;  y < count_rectY; y++) {
                for (int x = 0; x < count_rectX; x++) {
                    int loc = x + y * count_rectX;          
                    richTextBox1.AppendText(Math.Round(lists[loc].Average()).ToString() + "   ");
                    
                }
                richTextBox1.AppendText("\n");

            }
          
          
        }

        private void button3_Click (object sender, EventArgs e) {
            int x = Int32.Parse(textBox1.Text);
            int y = Int32.Parse(textBox2.Text);
            pixialization(img_gray,x,y);
        }

        private void Form1_Load (object sender, EventArgs e) {
            
        }
    }
}
