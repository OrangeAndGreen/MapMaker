using SharpKml.Dom;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace MapMaker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            progressBar.Maximum = 100;

            statusText.Text = "Ready";
        }

        private void UpdateStatus(string status, int? progress)
        {
            if (statusStrip1.InvokeRequired)
            {
                statusStrip1.Invoke((MethodInvoker)delegate {
                    statusText.Text = status ?? string.Empty;
                    progressBar.Value = progress ?? 0;
                });
            }
            else
            {
                statusText.Text = status ?? string.Empty;
                progressBar.Value = progress ?? 0;
            }
        }

        private void fileButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                //openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                //openFileDialog.FilterIndex = 2;
                //openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    fileText.Text = openFileDialog.FileName;
                }
            }
        }

        private void buildButton_Click(object sender, EventArgs e)
        {
            UpdateStatus("Working", 0);

            BackgroundWorker bw = new BackgroundWorker
            {
                WorkerReportsProgress = true
            };

            bw.DoWork += delegate (object o, DoWorkEventArgs args)
            {
                Dictionary<BoundaryType, List<Placemark>> features = new Dictionary<BoundaryType, List<Placemark>>();
                int fileCounter = 0;
                foreach (BoundaryType boundaryType in Constants.MapSources.Keys)
                {
                    if (o is BackgroundWorker worker)
                    {
                        worker.ReportProgress(100 * (fileCounter + 1) / Constants.MapSources.Keys.Count, $"Processing {boundaryType}");
                    }

                    fileCounter++;

                    MapBuilder.ApplyRandomColoring(Constants.MapSources[boundaryType], Constants.BinaryColors);

                    features[boundaryType] = KmlHelper.LoadPlacemarksFromKml(Constants.MapSources[boundaryType]);
                    Console.WriteLine($"{boundaryType}: {features[boundaryType].Count} features");
                    
                    //Code to display all feature names
                    //foreach (Feature feature in features[area])
                    //{
                    //    Console.WriteLine(KmlHelper.GetFeatureName(feature));
                    //}
                    //Console.WriteLine();
                }
            };

            bw.ProgressChanged += delegate (object o, ProgressChangedEventArgs args)
            {
                UpdateStatus((string)args.UserState, args.ProgressPercentage);
            };

            bw.RunWorkerCompleted += delegate
            {
                UpdateStatus("Finished", 0);
            };

            bw.RunWorkerAsync();
        }
    }
}
