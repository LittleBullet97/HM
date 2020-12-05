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
    public partial class Multy : Form {
        public Multy () {
            InitializeComponent();
        }

        private void Form1_Load (object sender, EventArgs e) {
            textBox3.Text = "";
            textBox6.Text = 0.1.ToString();
            textBox7.Text = 100.ToString();
        }

        List<List<Bitmap>> pictures = new List<List<Bitmap>>();//3//35~40
        string filename = @"D:\Desktop\values.xml";
        private void loadPictures () {
            string[] all_folder_tranning = Directory.GetDirectories(@"D:\Desktop\training");
            foreach (string folder_tranning in all_folder_tranning) {
                string[] object_tranning = Directory.GetFiles(folder_tranning);
                List<Bitmap> obj = new List<Bitmap>();
                foreach (string file in object_tranning) {
                    obj.Add(new Bitmap(file));
                }
                pictures.Add(obj);
            }
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
            }
            return result;
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

        private void fixXY () { 
            int x = ( textBox1.Text == "" || textBox1.Text == "0" ) ? x = 3 : x = Int32.Parse(textBox1.Text);
            int y = ( textBox2.Text == "" || textBox2.Text == "0" ) ? y = 3 : y = Int32.Parse(textBox2.Text);
            textBox1.Text = x.ToString();
            textBox2.Text = y.ToString();
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
            textBox5.Text = temp.ToString();
       
        }

        private void exeAndSaveAllPictures_Click (object sender, EventArgs e) {
            loadPictures();
            //all pictures convert to gray
            for (int folder = 0; folder < pictures.Count; folder++) {
                for (int pic = 0; pic < pictures[folder].Count; pic++) {
                    pictures[folder][pic] = convertToGray(pictures[folder][pic]);
                }

            }
            fixXY();
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
                    average_gray = AverageGray(pictures[folder][pic], x, y);
                    for (int k = 0; k < average_gray.Count; k++) {
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

        Neural_Network nn;
        public void Training_Click (object sender, EventArgs e) {
            int A, B, C;
            double eta;
            int br_epohi;
            double[,,] data = readXML();

            int br_primeri = data.GetLength(1);
            A = textBox3.Text == "" ? A = 9: A = Int32.Parse(textBox3.Text);
            B = (int)Math.Round(Math.Sqrt(br_primeri) + 2);
           
            eta = textBox6.Text == "" ? eta = 0.1 : eta = Convert.ToDouble(textBox6.Text);
            br_epohi = textBox7.Text == "" ? br_epohi = 1000 : br_epohi = Int32.Parse(textBox7.Text);  

            nn = new Neural_Network(A, B, 1, eta, 1000, data);
            richTextBox1.Clear();
            double[,,] res = nn.traning();

            //print
            for (int i = 0; i < br_epohi; i++) {
                for (int j = 0; j < data.GetLength(0); j++) {
                    for (int k = 0; k < data.GetLength(1); k++) {
                        richTextBox1.AppendText(res[i, j, k].ToString() + " ");
                    }
                    richTextBox1.AppendText("\n----\n");
                }
                richTextBox1.AppendText(i + "**************\n");
            }


            Graph gr = new Graph(res);
            gr.Show();


        }

        private void button1_Click (object sender, EventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();
            Bitmap upload_img;
            if (ofd.ShowDialog() == DialogResult.OK) {
                upload_img = new Bitmap(ofd.FileName);
                upload_img = convertToGray(upload_img);
                fixXY();
                int x = Int32.Parse(textBox1.Text);
                int y = Int32.Parse(textBox2.Text);
                List<double> list_upload_img = new List<double>();
                AverageGray(upload_img, x, y);
                list_upload_img = scaleAndShifting(AverageGray(upload_img, x, y));
                richTextBox1.Clear();
                foreach(double d in list_upload_img){
                    richTextBox1.AppendText(d.ToString() + "\n");
                }
                double[] temp = nn.classifier(list_upload_img);
                int[] res = toProcent(temp);

                // pie chart
                //utre si igraq
                chart1.Series.Clear();
                chart1.Series.Add("Series1");
                chart1.Series["Series1"].Points.AddXY("ръководство", res[0]);
                chart1.Series["Series1"].Points.AddXY("чиния", res[1]);
                chart1.Series["Series1"].Points.AddXY("ст.книжка", res[2]);
          
                //pictureBox1.Image = upload_img;
            }

        }
        private int[] toProcent (double[] data) {
            int[] result = new int[data.Length];
            double sum = 0.0;
            for (int i = 0; i < data.Length; i++) {
                sum += data[i];   
            }
            for (int i = 0; i < data.Length; i++) {
                result[i] = (int)Math.Round(data[i] / sum);
            }
            return result;

        }
    }
}
