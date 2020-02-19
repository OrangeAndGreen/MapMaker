using System.Collections.Generic;
using System.Drawing;

namespace MapMaker
{
    public static class Constants
    {
        public static Dictionary<BoundaryType, string> MapSources = new Dictionary<BoundaryType, string>
        {
            [BoundaryType.County] = "BoundaryKmls\\Counties.kml",
            [BoundaryType.Country] = "BoundaryKmls\\Countries.kml",
            [BoundaryType.District] = "BoundaryKmls\\Districts.kml",
            [BoundaryType.State] = "BoundaryKmls\\States.kml"
        };

        public static List<Color> BinaryColors = new List<Color>
        {
            Color.Orange,
            Color.Green
        };
    }

    public enum BoundaryType
    {
        Unknown = 0,
        County = 1,
        District = 2,
        State = 3,
        Country = 4
    }
}
