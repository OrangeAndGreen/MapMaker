using System;
using SharpKml.Engine;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using SharpKml.Base;
using SharpKml.Dom;

namespace MapMaker
{
    public static class MapBuilder
    {
        static readonly Random random = new Random((int)DateTime.Now.Ticks);

        public static void ApplyRandomColoring(string kmlFilename, List<Color> colors)
        {
            KmlFile file = KmlHelper.Load(kmlFilename);

            List<Style> styles = GenerateStyles(colors);

            List<Placemark> placemarks = KmlHelper.LoadPlacemarksFromKml(file);
            foreach (Placemark feature in placemarks)
            {
                ApplyRandomFeatureColoring(feature, colors);
            }

            string outputFilename = kmlFilename.Insert(kmlFilename.LastIndexOf('.'), "_random");

            KmlHelper.WriteNewKml(outputFilename, placemarks, styles);
        }

        private static void ApplyRandomFeatureColoring(Feature top, List<Color> colors)
        {
            if (top is Folder folder)
            {
                if (!folder.Features.Any())
                    return;

                foreach (Feature child in folder.Features)
                {
                    ApplyRandomFeatureColoring(child, colors);
                }
            }
            else
            {
                int index = random.Next(0, colors.Count);
                top.ClearStyles();
                top.StyleUrl = new Uri($"Style{index}", UriKind.Relative);
            }
        }

        private static List<Style> GenerateStyles(List<Color> colors)
        {
            List<Style> ret = new List<Style>();
            //Create styles for each of the desired colors
            for (int i = 0; i < colors.Count; i++)
            {
                Color color = colors[i];
                ret.Add(new Style
                {
                    Id = $"Style{i}",
                    Line = new LineStyle
                    {
                        Color = new Color32(205, color.B, color.G, color.R),
                        Width = 1
                    },
                    Polygon = new PolygonStyle
                    {
                        Color = new Color32(128, color.B, color.G, color.R)
                    }
                });
            }

            return ret;
        }
    }
}
