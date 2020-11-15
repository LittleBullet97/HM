using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NM {
    public class Neural_Network {
        private int A;
        private int B;
        private int C;
        private double eta;
        private int br_epohi;
        private double[] X;
        private double[,] Wji;
        private double[,] Wkj;
        private double[] Y_k;

        public Neural_Network (int A, int B, int C, double eta, int br_epohi, double[] X) {
            this.A = A;
            this.B = B;
            this.C = C;
            this.eta = eta;
            this.br_epohi = br_epohi;
            this.X = X;
            Wji = new double[B, A];
            Wkj = new double[C, B];
            Y_k = new double[C];
            initW();
            initYk();
        }
        public Neural_Network () {
            this.C = 5;
            Y_k = new double[C];
            initYk();
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
                    this.Wji[k, j] = n;
                }
            }
        }
        public void initYk () {
            Random r = new Random();
            for (int k = 0; k < C; k++) {

                double n = r.NextDouble() * 0.8 + 0.1;
                Y_k[k] = n;
            }
            for (int k = 0; k < C; k++) {

                MessageBox.Show(Y_k[k].ToString());
            }

        }
        public double[] netj () {
            double[] result = new double[B];
            for (int j = 0; j < B; j++) {
                for (int i = 0; i < A; i++) {
                    result[j] += X[i] * Wji[j, i];
                }
            }
            return result;
        }

        public double[] hj () {
            double[] result = new double[B];
            double[] net_j = netj();
            for (int j = 0; j < B; j++) {
                result[j] = 1 / ( 1 + Math.Pow(Math.E, -net_j[j]) );
            }
            return result;
        }

        public double[] netk () {
            double[] result = new double[C];
            double[] h_j = hj();
            for (int k = 0; k < C; k++) {
                for (int j = 0; j < B; k++) {
                    result[j] += h_j[j] * Wkj[k, j];
                }
            }
            return result;
        }
        public double[] Ok () {
            double[] result = new double[C];
            double[] net_k = netk();
            for (int k = 0; k < C; k++) {
                result[k] = 1 / ( 1 + Math.Pow(Math.E, -net_k[k]) );
            }
            return result;

        }



        public double[] deltak () {
            double[] result = new double[C]; ;
            double[] O_k = Ok();

            for (int k = 0; k < C; k++) {
                result[k] = O_k[k] * ( 1 - O_k[k] ) * ( Y_k[k] - O_k[k] );
            }
            return result;
        }

        public double[,] deltaWkj () {
            double[] delta_k = deltak();
            double[] h_j = hj();
            double[,] result = new double[C, B];

            for (int k = 0; k < C; k++) {
                for (int j = 0; j < B; k++) {
                    result[k, j] = eta * delta_k[k] * h_j[j];
                }
            }
            return result;
        }

        public double[] deltaj () {
            double[] h_j = hj();
            double[] delta_k = deltak();
            double[] result = new double[B];
            double[] sum = new double[C];
            for (int k = 0; k < C; k++) {
                for (int j = 0; j < B; j++) {
                    sum[k] += delta_k[k] * Wkj[k, j];
                }
            }

            for (int j = 0; j < B; j++) {
                result[j] = h_j[j] * ( 1 - h_j[j] );
            }
            return result;
        }
        public double[,] deltaWji () {
            double[] delta_j = deltaj();
            double[,] result = new double[C, B];

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
                    Wkj[j, i] += delta_Wki[j, i];
                    //shity :'(
                }
            }
        }
    }
}
