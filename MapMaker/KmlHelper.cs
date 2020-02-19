using SharpKml.Dom;
using SharpKml.Engine;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
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

        public static List<Placemark> LoadPlacemarksFromKml(string filename)
        {
            KmlFile file = Load(filename);
            
            return LoadPlacemarksFromKml(file);
        }

        public static List<Placemark> LoadPlacemarksFromKml(KmlFile file)
        {
            List<Placemark> ret = new List<Placemark>();

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
                ret.AddRange(ExtractPlacemarks(feature));
            }

            return ret;
        }

        private static List<Placemark> ExtractPlacemarks(Feature top)
        {
            List<Placemark> ret = new List<Placemark>();
            switch (top)
            {
                case Folder folder when !folder.Features.Any():
                    return ret;
                case Folder folder:
                {
                    foreach (Feature child in folder.Features)
                    {
                        ret.AddRange(ExtractPlacemarks(child));
                    }

                    break;
                }
                case Placemark p:
                    ret.Add(p);
                    break;
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

        public static string GetFeatureName(Feature feature)
        {
            string name = feature.Name;
            while (true)
            {
                int openIndex = name.IndexOf('<');
                int closeIndex = name.IndexOf('>');

                if (openIndex < 0 || closeIndex < openIndex)
                {
                    break;
                }

                name = name.Remove(openIndex, closeIndex - openIndex + 1);
            }

            return name;
        }

        public static void SaveKml(KmlFile file, string filename)
        {
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }

            using (var stream = File.OpenWrite(filename))
            {
                file.Save(stream);
            }
        }

        public static void WriteNewKml(string filename, List<Placemark> features, List<Style> styles)
        {
            Document doc = new Document();

            foreach (Style style in styles)
            {
                doc.AddStyle(style);
            }

            features.Sort((p1, p2) => p1.Name.CompareTo(p2.Name));

            foreach (Placemark feature in features)
            {
                MultipleGeometry newMulti = new MultipleGeometry();
                switch (feature.Geometry)
                {
                    case MultipleGeometry multi:
                    {
                        foreach (Geometry geom in multi.Geometry)
                        {
                            if (geom is Polygon p)
                            {
                                newMulti.AddGeometry(new Polygon { OuterBoundary = p.OuterBoundary });
                            }
                        }

                        break;
                    }
                    case Polygon p:
                    {
                        newMulti.AddGeometry(new Polygon { OuterBoundary = p.OuterBoundary });
                        break;
                    }
                    default:
                    {
                        Console.WriteLine("Encountered unexpected geometry");
                        break;
                    }
                }

                Placemark newPlacemark = new Placemark
                {
                    Name = GetFeatureName(feature),
                    Geometry = newMulti,
                    StyleUrl = feature.StyleUrl
                };

                doc.AddFeature(newPlacemark);
            }

            KmlFile file = KmlFile.Create(new Kml { Feature = doc }, false);

            SaveKml(file, filename);
        }
    }
}
