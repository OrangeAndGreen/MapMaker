using System.Collections.Generic;

namespace MapMaker
{
    public class DataFile
    {
        public string Filename { get; set; }
        public List<KeyValuePair<string, double>> Data { get; set; }

        public DataFile(string filename)
        {
            Filename = filename;
        }

        public BoundaryType FileType
        {
            get { return BoundaryType.Unknown; }
        }

        public List<double> ValueOptions
        {
            get
            {
                return new List<double>();
            }
        }
    }
}
