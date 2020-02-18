using SharpKml.Dom;
using SharpKml.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MapMaker
{
    public static class KmlHelper
    {
        public static KmlFile Load(string filename)
        {
            TextReader reader = File.OpenText(filename);
            KmlFile file = KmlFile.Load(reader);
            return file;
        }

        public static List<Feature> LoadFeaturesFromKml(string filename)
        {
            List<Feature> ret = new List<Feature>();

            KmlFile file = Load(filename);

            if (!(file.Root is Kml kml))
            {
                Console.WriteLine("File root isn't a KML");
                return ret;
            }

            if (!(kml.Feature is Document doc))
            {
                Console.WriteLine("Main feature isn't a Document");
                return ret;
            }

            if (!doc.Features.Any())
            {
                Console.WriteLine("No features in Document");
                return ret;
            }

            foreach (Feature feature in doc.Features)
            {
                ret.AddRange(ExtractFeatures(feature));
            }

            return ret;
        }

        private static List<Feature> ExtractFeatures(Feature top)
        {
            List<Feature> ret = new List<Feature>();
            if (top is Folder folder)
            {
                //lines.Add($"{header}Folder: {top.Name}");
                if (!folder.Features.Any())
                    return ret;

                foreach (Feature child in folder.Features)
                {
                    ret.AddRange(ExtractFeatures(child));
                }
            }
            else
            {
                ret.Add(top);
            }

            return ret;
        }

        public static string ExploreKml(string filename, KmlFile file)
        {
            List<string> lines = new List<string>()
            {
                $"{filename}:"
            };

            if (!(file.Root is Kml kml))
            {
                return "File root isn't a KML";
            }

            if (!(kml.Feature is Document doc))
            {
                return "Main feature isn't a Document";
            }

            if (!doc.Features.Any())
            {
                return "No features in Document";
            }

            foreach (Feature element in doc.Features)
            {
                lines.AddRange(DisplayElement(element, 0));
            }

            lines.Add("");
            return string.Join("\r\n", lines);
        }

        public static List<string> DisplayElement(Feature element, int indent)
        {
            List<string> lines = new List<string>();

            string header = string.Empty;
            for (int i = 0; i < indent; i++)
            {
                header += "   ";
            }

            if (element is Folder folder)
            {
                lines.Add($"{header}Folder: {element.Name}");

                if (!folder.Features.Any())
                    return lines;

                foreach (Feature child in folder.Features)
                {
                    lines.AddRange(DisplayElement(child, indent + 1));
                }
            }
            else
            {
                lines.Add($"{header}Feature: {element.Name}");
            }

            return lines;
        }
    }
}
