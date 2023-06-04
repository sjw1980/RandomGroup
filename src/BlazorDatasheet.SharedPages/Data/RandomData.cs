using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorDatasheet.SharedPages.Data
{
    public class Limit
    {
        public int MinPerson { get; set; } = 0;
        public int MaxPerson { get; set; } = 0;
        public int SameCount { get; set; } = 0;
        public double Ratio { get; set; } = 0;
        public double StandardDeviation { get; set; } = 0;

        public Limit(int min, int max, double ratio, double standardDeviation, int count)
        {
            MinPerson = min;
            MaxPerson = max;
            Ratio = ratio;
            StandardDeviation = standardDeviation;
            SameCount = count;
        }
    }

    public class Group
    {
        public int Index { get; set; } = 0;
        public string Alias { get; set; } = "";
        public bool Lack { get; set; } = false;
        public int TargetMemberLength { get; set; } = 0;
        public int CurrentMemberCount { get; set; } = 0;
        public List<string> Members { get; set; } = new List<string>();
        public Dictionary<string, int> Totals { get; set; } = new Dictionary<string, int>();
    }
}
