using System.Collections.Generic;

namespace WeatherDesktop__IPZ_21__
{
    abstract class Implementor
    {
        public abstract string GetCache();
        public abstract (string, string, string) GetData();
        public abstract void Weather(string longtitude, string latitude);
        public abstract void Pack(ref List<Values> values);
        public abstract void Save(List<Values> values);
        public abstract void Output_cache(MainForm mainform);
        public abstract void OutputInfo(List<Values> values, MainForm mainform);
    }
}
