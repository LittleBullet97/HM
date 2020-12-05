using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
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
    public partial class Graph : Form {
        double[,,] data;
        double[] sums;
        public Graph (double[,,] d) {
            InitializeComponent();
            data = d;
        }
        private void calc () {
            sums = new double[data.GetLength(0)];

            for (int i = 0; i < data.GetLength(0); i++) {
                double sum = 0;
                for (int j = 0; j < data.GetLength(1); j++) {
                    for (int k = 0; k < data.GetLength(2); k++) {
                        sum += data[i, j, k];
                    }
                }
                sums[i] = sum;
            }
        }

        private void Graph_Load (object sender, EventArgs e) {
            calc();
            cartesianChart1.AxisX.Add(new LiveCharts.Wpf.Axis {
                Title = "Epohi"
            });
            SeriesCollection series = new SeriesCollection();
            
            series.Add(new LineSeries() { 
                Values = new ChartValues<double>(sums)}
            
            );
            cartesianChart1.Series = series;           
        }

        private void cartesianChart1_ChildChanged (object sender, System.Windows.Forms.Integration.ChildChangedEventArgs e) {

        }
    }
}
