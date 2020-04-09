

namespace WeatherDesktop__IPZ_21__
{
    public interface IVariables
    {
        // weather information (include cache), this info would be displayed in UI
        #region
        string Temp { get; set; } // C
        string Image_icon { get; set; }
        string Description { get; set; }
        string Feels_like { get; set; } // C
        string City { get; set; }
        string Country { get; set; }
        string Wind_speed { get; set; } // km per hour 
        string Time { get; set; } // hh:mm:ss
        string Date { get; set; } // dd-mm-yyyy
        #endregion
    }
}
