using System;
using SharpKml.Engine;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using SharpKml.Base;
using SharpKml.Dom;

namespace MapMaker
{
    public static class MapBuilder
    {
        static Random random = new Random((int)DateTime.Now.Ticks);

        public static void ApplyRandomColoring(string kmlFilename, List<Color> colors)
        {
            KmlFile file = KmlHelper.Load(kmlFilename);

            if (!(file.Root is Kml kml))
            {
                Console.WriteLine("File root isn't a KML");
                return;
            }

            if (!(kml.Feature is Document doc))
            {
                Console.WriteLine("Main feature isn't a Document");
                return;
            }

            //Create styles for each of the desired colors
            for (int i = 0; i < colors.Count; i++)
            {
                Color color = colors[i];
                doc.AddStyle(new Style
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

            foreach (Feature feature in doc.Features)
            {
                ApplyFeatureColoring(feature, colors);
            }

            using (var stream = System.IO.File.OpenWrite("TestWrite.kml"))
            {
                file.Save(stream);
            }
        }

        private static void ApplyFeatureColoring(Feature top, List<Color> colors)
        {
            
            if (top is Folder folder)
            {
                if (!folder.Features.Any())
                    return;

                foreach (Feature child in folder.Features)
                {
                    ApplyFeatureColoring(child, colors);
                }
            }
            else
            {
                int index = random.Next(0, colors.Count);
                top.StyleUrl = new Uri($"Style{index}", UriKind.Relative);
            }
        }
    }
}
