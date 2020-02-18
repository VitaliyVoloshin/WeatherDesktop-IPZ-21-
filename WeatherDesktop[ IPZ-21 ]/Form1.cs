using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WeatherDesktop__IPZ_21__
{
 

    public partial class MainForm : Form
    {
        Point PanelMouseDownLocation;

        public MainForm()
        {
            InitializeComponent();
            UpPanel.BackColor = Color.FromArgb(150, UpPanel.BackColor);  // Setting transparency of panel
            string lat, lon;
            (lat, lon) = MyNetwork.GetGeoLoc();
            string main, description, temp;
            (main, description, temp) = MyNetwork.GetWeather(lat, lon);
            try
            {
                temp = (Math.Round((Convert.ToDouble(temp.Replace('.', ',')) - 273.15),2)).ToString();
                label1.Text = main; label2.Text = description; label3.Text = temp+" C";
            }
            catch (Exception e) { label1.Text = e.ToString(); }
            
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            
        }

        private void UpPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void UpPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) PanelMouseDownLocation = e.Location;
        }

        private void UpPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - PanelMouseDownLocation.X;
                this.Top += e.Y - PanelMouseDownLocation.Y;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }



    static class MyNetwork
    {
        private static string API_KEY = "1216b541254a76414c87650bbd955c09";
        public static string GetIP()
        {
            using (var c = new WebClient()) {
                string x = c.DownloadString(@"https://api.myip.com");
                var o = JObject.Parse(x);
                return (string)o["ip"];
            }
        }

        public static (string, string) GetGeoLoc()
        {
            using (var c = new WebClient()) {
                var s = c.DownloadString(string.Format(@"http://www.geoplugin.net/json.gp?ip={0}", GetIP()));
                var e = JObject.Parse(s);
                return ((string)e["geoplugin_latitude"], (string)e["geoplugin_longitude"]);
            } 
        }

        public static (string, string, string) GetWeather(string lat, string lon) {
            using (var c = new WebClient())
            {
                var s = c.DownloadString(string.Format("https://api.openweathermap.org//data//2.5//weather?lat={0}&lon={1}&APPID={2}", lat, lon, API_KEY));
                var e = JObject.Parse(s);
                return ((string)(e["weather"][0]["main"]), (string)(e["weather"][0]["description"]), (string)(e["main"]["temp"]));
            }
        }

    }


}
