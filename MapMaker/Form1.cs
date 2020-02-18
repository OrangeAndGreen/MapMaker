using SharpKml.Engine;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SharpKml.Dom;

namespace MapMaker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            mainText.Text = "Loading";

            //Dictionary<string, string> filenames = new Dictionary<string, string>
            //{
            //    //["Counties"] = "BoundaryKmls\\Counties.kml",
            //    //["Countries"] = "BoundaryKmls\\Countries.kml",
            //    //["Districts"] = "BoundaryKmls\\Districts.kml",
            //    ["States"] = "BoundaryKmls\\States.kml"
            //};

            //Dictionary<string, List<Feature>> features = new Dictionary<string, List<Feature>>();
            //foreach (string area in filenames.Keys)
            //{
            //    features[area] = KmlHelper.LoadFeaturesFromKml(filenames[area]);
            //    Console.WriteLine($"{area}: {features[area].Count} features");
            //}

            //foreach (Feature feature in features["States"])
            //{
            //    Console.WriteLine($"{feature.Name}");
            //}

            List<Color> colors = new List<Color>
            {
                Color.Orange,
                Color.Green
            };

            MapBuilder.ApplyRandomColoring("BoundaryKmls\\States.kml", colors);
            
            mainText.Text = "Finished loading";
        }
    }
}
