using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Resources;
using Newtonsoft.Json;
using System.IO;

namespace WeatherDesktop__IPZ_21__
{

    abstract class Variables
    {
        // variables, contain info about position
        #region
        protected string latitude;
        protected string longtitude;
        protected string ip_address;
        #endregion

        // weather information variables (include cache), this info would be displayed in UI
        #region
        protected string temp; // C
        protected string image_icon;
        protected string description;
        protected string feels_like; // C
        protected string city;
        protected string country;
        protected string wind_speed; // km per hour 
        protected string time; // hh:mm:ss
        protected string date; // dd-mm-yyyy
        #endregion

        protected MainForm mainform;
    }

    // contains a value and it's description
    public class Values
    {
        public string Desc { get; set; }
        public string Value { get; set; }
        public Values()
        {

        }
        public Values(string desc)
        {
            this.Desc = desc;
        }
    }

    class Methods
    {
        // services which provides info about device's location
        #region
        public static string ip_site = @"https://api.myip.com";
        public static string geopos_site = @"http://www.geoplugin.net/json.gp?ip={0}";
        #endregion

        /// <summary>
        /// returns the ip address as string of using device
        /// </summary>
        public string GetIP()
        {
            using (var c = new WebClient())
            {
                string x = c.DownloadString(ip_site);
                return JObject.Parse(x)["ip"].ToString();
            }
        }

        /// <summary>
        /// returns the latitude and longitude as string based on ip address
        /// </summary>
        public void GetPosition(string ip_address, ref string latitude, ref string longitude)
        {
            try
            {
                using (var c = new WebClient())
                {
                    var s = c.DownloadString(string.Format(geopos_site, ip_address));
                    var container = JObject.Parse(s);
                    if (container["geoplugin_status"].ToString() == "404")
                        throw new Exception("Incorrect IP-address");
                    else
                    {
                        latitude = container["geoplugin_latitude"].ToString();
                        longitude = container["geoplugin_longitude"].ToString();
                    }
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine($"Error: {exc.Message}");
                Console.ReadKey();
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// creating a "cache" file
        /// </summary>
        public void Save_Info_JSON(List<Values> inf_l, string cache_name)
        {
            JObject cache = new JObject(
                new JProperty("saved_info",
                    new JArray(
                        from inf in inf_l
                        select new JObject(
                            new JProperty(inf.Desc, inf.Value)))));

            // find the file and rewrite it, or create new
            #region
            string path = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            string[] files = Directory.GetFiles(path, cache_name, SearchOption.AllDirectories);
            if (files.Count() > 0)
            {
                string file_path = String.Join("", files);
                File.WriteAllText(file_path, JsonConvert.SerializeObject(cache));
            }
            else
            {
                File.WriteAllText(Directory.GetParent(Environment.CurrentDirectory).FullName + "\\" + cache_name, JsonConvert.SerializeObject(cache));
            }
            #endregion
        }

        /// <summary>
        /// output latest successful info from some service from the project's directory
        /// </summary>
        public void Output_Cache(string cache_name)
        {
            string path = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            string[] files = Directory.GetFiles(path, cache_name, SearchOption.AllDirectories);
            if (files.Count() > 0)
            {
                string file_path = String.Join("", files);
                string json = File.ReadAllText(file_path);
                Output(json);
            }
            else
            {
                Console.WriteLine("No cache file yet!!!");
            }
        }

        /// <summary>
        /// output infomation on the screen
        /// </summary>
        public void Output(List<Values> inf_list, MainForm mainform)
        {
            var sb = new StringBuilder();
            for (int i = 1; i <= inf_list.Count() - 1; i++)
            {
                sb.Append(inf_list[i].Desc + ":  " + inf_list[i].Value + "\n");
            }
            mainform.ChangeWeatherData(sb.ToString(), "Updated "+DateTime.Now.ToString("HH:mm:ss"));
        }

        /// <summary>
        /// output information on the screen from cache.This method is invoked from Output_Cache_OW
        /// </summary>
        public void Output(string json)
        {
            string json_local = JToken.Parse(json).Children<JProperty>().
                FirstOrDefault(x => x.Name == "saved_info").Value.ToString();
            string image_icon = String.Empty;

            JArray array = JArray.Parse(json_local);
            foreach (JObject content in array.Children<JObject>())
            {
                foreach (JProperty prop in content.Properties())
                {
                    if (prop.Name == "image_icon")
                        image_icon = prop.Value.ToString();
                    else
                        Console.WriteLine(prop.Value);
                }
            }
        }
    }

    class AccuWeather : Variables
    {

        private string location_id;
        private string cache_name = "cache_accu.json";

        // services
        #region
        private string location_url =
            @"http://dataservice.accuweather.com/locations/v1/cities/geoposition/search?apikey={0}&q={1}%2C{2}&details={3}";
        private string weather_url =
            @"http://dataservice.accuweather.com/currentconditions/v1/{0}?apikey={1}&details={2}";
        #endregion

        //  readonly variables
        #region
        private readonly static string API_KEY_Accu = "sl0cGJumpYfxzuYJvDrOfMv9UVPHduih";
        private readonly List<Values> Info = new List<Values>();
        #endregion

        // all main methods invokes here
        public AccuWeather(MainForm mainform)
        {
            this.ip_address = new Methods().GetIP();
            new Methods().GetPosition(this.ip_address, ref this.latitude, ref this.longtitude);
            this.location_id = GetLocation_id();
            Weather();
            new Methods().Output(Info, mainform);
            this.mainform = mainform;
            //new Methods().Output_Cache(cache_name);
        }

        /// <summary>
        /// Returns the unique location id of the region
        /// </summary>
        private string GetLocation_id()
        {
            using (var c = new WebClient())
            {
                var s = c.DownloadString(
                    string.Format(this.location_url, API_KEY_Accu, this.latitude, this.longtitude, true.ToString()));
                var e = JObject.Parse(s);
                this.country = e["Country"]["LocalizedName"].ToString();
                this.city = e["LocalizedName"].ToString();
                return e["Key"].ToString();
            }
        }

        /// <summary>
        /// get weather info for region
        /// </summary>
        private void Weather()
        {
            using (var c = new WebClient())
            {
                var s = c.DownloadString(
                    string.Format(this.weather_url, this.location_id, API_KEY_Accu, true.ToString()));
                var e = JArray.Parse(s);
                PackageInfo(e);
                new Methods().Save_Info_JSON(Info, this.cache_name);
            }
        }

        /// <summary>
        /// pack defined values from JArray into List
        /// </summary>
        private void PackageInfo(JArray jArray)
        {
            Info.Add(new Values("image_icon") { Value = image_icon = jArray[0]["WeatherIcon"].ToString() });
            Info.Add(new Values("description") { Value = description = jArray[0]["WeatherText"].ToString() });
            Info.Add(new Values("temp") { Value = temp = jArray[0]["Temperature"]["Metric"]["Value"].ToString() });
            Info.Add(new Values("feels_like") { Value = feels_like = jArray[0]["RealFeelTemperature"]["Metric"]["Value"].ToString() });
            Info.Add(new Values("wind_speed") { Value = wind_speed = jArray[0]["Wind"]["Speed"]["Metric"]["Value"].ToString() });
            Info.Add(new Values("country") { Value = this.country });
            Info.Add(new Values("city") { Value = this.city });
            Info.Add(new Values("time") { Value = time = DateTime.Now.ToString().Split(new char[] { ' ' })[1] });
            Info.Add(new Values("date") { Value = date = DateTime.Now.Date.ToShortDateString() });

        }
    }

    class OpenWeather : Variables
    {
        private string pattern_url = @"https://api.openweathermap.org//data//2.5//weather?lat={0}&lon={1}&APPID={2}";
        private string cache_name = "cache_ow.json";

        //  readonly variables
        #region
        private readonly static string API_KEY_OW_1 = "32bf033f83b7d3bbb497ecbb47bc9e67";
        private readonly List<Values> Info = new List<Values>();
        #endregion

        // all main methods invokes here
        public OpenWeather(MainForm mainform)
        {
            this.ip_address = new Methods().GetIP();
            new Methods().GetPosition(ip_address, ref this.latitude, ref this.longtitude);
            Weather();
            new Methods().Output(Info, mainform);
            this.mainform = mainform;
            //new Methods().Output_Cache(cache_name);
        }

        /// <summary>
        /// get weather info for region
        /// </summary>
        private void Weather()
        {
            using (var c = new WebClient())
            {
                var s = c.DownloadString(string.Format(this.pattern_url, this.latitude, this.longtitude, API_KEY_OW_1));
                var e = JObject.Parse(s);
                Package_Info(e);
                new Methods().Save_Info_JSON(Info, this.cache_name);
            }
        }

        /// <summary>
        /// pack defined values from JObject into List
        /// </summary>
        private void Package_Info(JObject jObject)
        {
            Info.Add(new Values("image_icon") { Value = image_icon = jObject["weather"][0]["icon"].ToString() });
            Info.Add(new Values("description") { Value = description = jObject["weather"][0]["description"].ToString() });
            Info.Add(new Values("temp")
            {
                Value = temp = (Math.Round((Convert.ToDouble(jObject["main"]["temp"].ToString().Replace('.', ',')) - 273.15), 2)).ToString()
            });
            Info.Add(new Values("feels_like")
            {
                Value = feels_like = (Math.Round((Convert.ToDouble(jObject["main"]["feels_like"].ToString().Replace('.', ',')) - 273.15), 2)).ToString()
            });
            Info.Add(new Values("wind_speed") { Value = wind_speed = jObject["wind"]["speed"].ToString() });
            Info.Add(new Values("country") { Value = country = jObject["sys"]["country"].ToString() });
            Info.Add(new Values("city") { Value = city = jObject["name"].ToString() });
            Info.Add(new Values("time") { Value = time = DateTime.Now.ToString().Split(new char[] { ' ' })[1] });
            Info.Add(new Values("date") { Value = date = DateTime.Now.Date.ToShortDateString() });
        }

    }


    /*
    class ProgramY
    {
        // entry point of the program
        static void Main(string[] args)
        {
            if (Console.ReadLine() == "1")
                new AccuWeather();
            else
                new OpenWeather();
            Console.ReadKey();
        }
    }
    */


}