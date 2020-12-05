using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace NM {
    public class Neural_Network : Form {
        private int A;
        private int B;
        private int C;
        private double eta;
        private int br_epohi;
        private double[] X;
        private double[,,] data;
        private double[,] Wji;
        private double[,] Wkj;
        private double[] Y_k;
        private double curr_Y;


		public Neural_Network (int A, int B, int C, double eta, int br_epohi, double[,,] data) {
            this.A = A;
            this.B = B;
            this.C = C;
            this.eta = eta;
            this.br_epohi = br_epohi;
            this.data = data;
            Wji = new double[B, A];
            Wkj = new double[C, B];
            Y_k = new double[3];
            initW();
            initYk();
            X = new double[A];
        }

        public void initW () {
            Random r = new Random();
            for (int j = 0; j < B; j++) {
                for (int i = 0; i < A; i++) {
                    double n = r.NextDouble() / 5 - 0.1;
                    this.Wji[j, i] = n;
                }
            }
            for (int k = 0; k < C; k++) {
                for (int j = 0; j < B; j++) {
                    double n = r.NextDouble() / 5 - 0.1;
                    this.Wkj[k, j] = n;
                }
            }
        }
        public void initYk () {
            //Random r = new Random();
            //for (int k = 0; k < C; k++) {
            //    double n = r.NextDouble() * 0.8 + 0.1;
            //    Y_k[k] = n;
            //}
            Y_k[0] = 0.1;
            Y_k[1] = 0.45;
            Y_k[2] = 0.9;

        }
        public void initCurrentX (int papka,int snimka) {
            for (int k = 0; k < data.GetLength(2); k++) {
                this.X[k] = data[papka, snimka,k];
                if (X[k] == -1) MessageBox.Show("ima :(");
            }
        }  
        //public void initCurrentY (int index) {
        //    curr_Y = Y_k[index];
        //}
        public double[] netj () {
            double[] result = new double[B];
            for (int j = 0; j < B; j++) {
                for (int i = 0; i < A; i++) {
                    result[j] += X[i] * Wji[j, i];
                }
            }
            return result;
        }

        public double[] hj () {//act. hidden
            double[] result = new double[B];
            double[] net_j = netj();
            for (int j = 0; j < B; j++) {
                result[j] = 1.0 / ( 1.0 + Math.Pow(Math.E, -net_j[j]));
            }
            return result;
        }

        public double[] netk () {
            double[] result = new double[C];
            double[] h_j = hj();

            for (int k = 0; k < C; k++) {
                for (int j = 0; j < B; j++) {
                    result[k] += h_j[j] * Wkj[k, j];
                }
            }
            return result;
        }
        public double[] Ok () {
            double[] result = new double[C];
            double[] net_k = netk();
            for (int k = 0; k < C; k++) {
                result[k] = 1.0 / ( 1.0 + Math.Pow(Math.E, -net_k[k]) );
            }
            return result;

        }

        public double[] deltak () {
            double[] result = new double[C];
            double[] O_k = Ok();
            for (int k = 0; k < C; k++) {//C = 0
                result[k] = O_k[k] * ( 1.0 - O_k[k] ) * ( curr_Y - O_k[k] );
            }
            return result;
        }

        public double[,] deltaWkj () {
            double[] delta_k = deltak();
            double[] h_j = hj();
            double[,] result = new double[C, B];

            for (int k = 0; k < C; k++) {
                for (int j = 0; j < B; j++) {
                    result[k, j] = eta * delta_k[k] * h_j[j];
                }
            }
            return result;
        }

        public double[] deltaj () {
            double[] h_j = hj();
            double[] delta_k = deltak();
            double[] result = new double[B];
            double[] sum = new double[B];
            
            for (int j = 0; j < B; j++) {
                for (int k = 0; k < C; k++) {
                    sum[j] += delta_k[k] * Wkj[k, j];             
                }
            }

            for (int j = 0; j < B; j++) {
                result[j] = h_j[j] * ( 1 - h_j[j]) * sum[j];
            }
            return result;
        }
        public double[,] deltaWji () {
            double[] delta_j = deltaj();
            double[,] result = new double[B, A];

            for (int j = 0; j < B; j++) {
                for (int i = 0; i < A; i++) {
                    result[j, i] = eta * delta_j[j] * X[i];
                }
            }
            return result;
        }
       public void applyChangesWkj () {
            double[,] delta_Wki = deltaWkj();
            for (int k = 0; k < C; k++) {
                for (int j = 0; j < B; j++) {
                    Wkj[k, j] += delta_Wki[k, j];
                }
            }
        }
        public void applyChangesWki () {
            double[,] delta_Wki = deltaWji();
            for (int j = 0; j < B; j++) {
                for (int i = 0; i < A; i++) {
                    Wji[j, i] += delta_Wki[j, i];
                }
            }
        }
        public double error () {
            double result = 0.0;
            double[] O_k = Ok();
            for (int k = 0; k < C; k++) {
                result += 0.5 * (Math.Pow(( curr_Y - O_k[k] ),2));
            }
            return result;
        }

        public double[,,] traning () {
            double[,,] res = new double[br_epohi, data.GetLength(0), data.GetLength(1)];
            for (int epoha = 0; epoha < br_epohi; epoha++) {
                for (int papka = 0; papka < data.GetLength(0); papka++) {//papka
                    curr_Y = Y_k[papka];
                    for (int snimka = 0; snimka < data.GetLength(1); snimka++) {//snimka
                        initCurrentX(papka, snimka);
                        applyChangesWkj();
                        applyChangesWki();
                        res[epoha, papka, snimka] = error();
                        res[epoha, papka, snimka] = Math.Round(res[epoha, papka, snimka], 6);
                    }
                }
            }
            return res;
        }
        public double[] classifier (List<double> list) {
            double[] netj = new double[B];
            double[] h = new double[B];
            double[] netk = new double[C];
            double[] Ok = new double[C];
            double[] res = new double[3];
            for (int j = 0; j < B; j++) {
                for (int i = 0; i < A; i++) {
                    netj[j] += list[i] * Wji[j, i];
                }
            }

            for (int j = 0; j < B; j++) {
                h[j] = 1.0 / ( 1.0 + Math.Pow(Math.E, -netj[j]));
            }

            for (int k = 0; k < C; k++) {
                for (int j = 0; j < B; j++) {
                    netk[k] += h[j] * Wkj[k, j];
                }
            }

            for(int k = 0; k < C; k++) {
                Ok[k] = 1.0 / ( 1.0 + Math.Pow(Math.E, -netk[k]));
            }

            double min = 10;
            int min_index = 0;
            for (int i = 0; i < 3; i++) {
                double temp = Math.Abs(Ok[0] - Y_k[i]);
                res[i] = temp;
                Console.Out.WriteLine(Ok[0] + " " + Y_k[i]);
                Console.Out.WriteLine(temp);
                if (temp < min) {
                    min = temp;
                    min_index = i;
                }    
            }
            Console.Out.WriteLine(min_index + " " + min);
            Console.Out.WriteLine("----------------");
            return res;
            
        }

        private void InitializeComponent () {
            this.SuspendLayout();
            // 
            // Neural_Network
            // 
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Name = "Neural_Network";
            this.Load += new System.EventHandler(this.Neural_Network_Load);
            this.ResumeLayout(false);

        }

        private void Neural_Network_Load (object sender, EventArgs e) {

        }
    }
}
