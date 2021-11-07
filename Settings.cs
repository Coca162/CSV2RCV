using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCVConverter
{
    public static class Settings
    {
        public static ElectionType electionType = ElectionType.Standard;
        public static bool correctRankNumbers = true;
    }

    public enum ElectionType
    {
        National,
        Standard
    }
}
