using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;


namespace WeatherDesktop__IPZ_21__
{
    class ImplementOpenW : Implementor, IVariables
    {
        // services which provides info about device's location
        #region
        private static string ip_site = @"https://api.myip.com";
        private static string geopos_site = @"http://www.geoplugin.net/json.gp?ip={0}";
        #endregion

        // service variables
        #region
        private string cache_name = "cache_ow.json";
        private readonly static string API_KEY_OW_1 = "32bf033f83b7d3bbb497ecbb47bc9e67";
        private string pattern_url = @"https://api.openweathermap.org//data//2.5//weather?lat={0}&lon={1}&APPID={2}";
        #endregion

        // Properties
        #region
        public string Temp { get; set; }
        public string Image_icon { get; set; }
        public string Description { get; set; }
        public string Wind_speed { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Feels_like { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        #endregion

        JObject Information;

        public override (string, string, string) GetData()
        {
            using (var c = new WebClient())
            {
                string ip_info = c.DownloadString(ip_site);
                string ip_address = JObject.Parse(ip_info)["ip"].ToString();

                var container = c.DownloadString(string.Format(geopos_site, ip_address));
                var geoinfo = JObject.Parse(container);
                string latitude = geoinfo["geoplugin_latitude"].ToString();
                string longitude = geoinfo["geoplugin_longitude"].ToString();

                return (ip_info, longitude, latitude);
            }
        }

        public override string GetCache()
        {
            return cache_name;
        }

        public override void Weather(string longtitude, string latitude)
        {
            using (var c = new WebClient())
            {
                var s = c.DownloadString(string.Format(this.pattern_url, latitude, longtitude, API_KEY_OW_1));
                Information = JObject.Parse(s);
            }
        }

        public override void Pack(ref List<Values> Info)
        {
            Info.Add(new Values("image_icon") { Value = Image_icon = Information["weather"][0]["icon"].ToString() });
            Info.Add(new Values("description") { Value = Description = Information["weather"][0]["description"].ToString() });
            Info.Add(new Values("temp")
            {
                Value = Temp = (Math.Round((Convert.ToDouble(Information["main"]["temp"].ToString().Replace('.', ',')) - 273.15), 2)).ToString()
            });
            Info.Add(new Values("feels_like")
            {
                Value = Feels_like = (Math.Round((Convert.ToDouble(Information["main"]["feels_like"].ToString().Replace('.', ',')) - 273.15), 2)).ToString()
            });
            Info.Add(new Values("wind_speed") { Value = Wind_speed = Information["wind"]["speed"].ToString() });
            Info.Add(new Values("country") { Value = Country = Information["sys"]["country"].ToString() });
            Info.Add(new Values("city") { Value = City = Information["name"].ToString() });
            Info.Add(new Values("time") { Value = Time = DateTime.Now.ToString().Split(new char[] { ' ' })[1] });
            Info.Add(new Values("date") { Value = Date = DateTime.Now.Date.ToShortDateString() });
        }

        public override void Save(List<Values> inf_l)
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

        public override void Output_cache(MainForm mainform)
        {
            string option = "Warning";
            string path = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            string[] files = Directory.GetFiles(path, cache_name, SearchOption.AllDirectories);
            if (files.Count() > 0)
            {
                string file_path = String.Join("", files);
                string json = File.ReadAllText(file_path);
                if (json == String.Empty)
                {
                    mainform.Warning();
                    return;
                }
                string json_local = JToken.Parse(json).Children<JProperty>().
                FirstOrDefault(x => x.Name == "saved_info").Value.ToString();
                string image_icon = String.Empty;
                var sb = new StringBuilder();
                string status = String.Empty;

                JArray array = JArray.Parse(json_local);
                foreach (JObject content in array.Children<JObject>())
                {
                    foreach (JProperty prop in content.Properties())
                    {
                        if (prop.Name == "image_icon")
                        {
                            image_icon = prop.Value.ToString();
                            mainform.WeatherIcon("_" + image_icon);
                        }
                        else
                        {
                            sb.Append(prop.Name + ": " + prop.Value + "\n");
                            if (prop.Name == "time")
                                status = prop.Value.ToString();
                        }
                    }
                }
                mainform.ChangeWeatherData(sb.ToString(), "Updated " + DateTime.Now.ToString("HH:mm:ss"));
            }
            else
            {
                if (option == "Warning") { mainform.Warning(); }
                else { throw new Exception(); }
            }
        }

        public override void OutputInfo(List<Values> inf_list, MainForm mainform)
        {
            var sb = new StringBuilder();
            string status = String.Empty;
            for (int i = 1; i <= inf_list.Count() - 1; i++)
            {
                sb.Append(inf_list[i].Desc + ":  " + inf_list[i].Value + "\n");
                if (inf_list[i].Desc == "time")
                    status = inf_list[i].Value;
            }
            mainform.ChangeWeatherData(sb.ToString(), "Updated " + DateTime.Now.ToString("HH:mm:ss"));
            mainform.WeatherIcon("_" + inf_list[0].Value);
        }
    }
}
