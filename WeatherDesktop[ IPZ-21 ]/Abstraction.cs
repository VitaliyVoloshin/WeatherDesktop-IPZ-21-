using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Net;
using Newtonsoft.Json;
using System.IO;

namespace WeatherDesktop__IPZ_21__
{
    abstract class API
    {
        protected Implementor implementor;
        protected MainForm mainform;
        protected List<Values> Info = new List<Values>();

        // variables, contain info about position
        #region
        protected string latitude;
        protected string longtitude;
        protected string ip_address;
        #endregion

        public API(Implementor imp, MainForm mainform)
        {
            this.implementor = imp;
            this.mainform = mainform;
        }

        // methods
        #region
        public virtual void Device_info()
        {
            (this.ip_address, this.longtitude, this.latitude)
                 = implementor.GetData();
        }

        public virtual void Cache()
        {
            implementor.GetCache();
        }

        public virtual void Weather_Save()
        {
            implementor.Weather(this.longtitude, this.latitude);
            implementor.Pack(ref Info);
            implementor.Save(Info);
        }

        public virtual void Output_Cache()
        {
            implementor.Output_cache(mainform);
        }

        public virtual void BaseOutput()
        {
            implementor.OutputInfo(Info,mainform);
        }
        #endregion
    }

    class AccuWeather : API
    {
        public AccuWeather(Implementor imp, MainForm mainform)
            : base(imp, mainform)
        { }

        public override void Device_info()
        {
            base.Device_info();
        }

        public override void Cache()
        {
            base.Cache();
        }

        public override void Weather_Save()
        {
            base.Weather_Save();
        }

        public override void Output_Cache()
        {
            base.Output_Cache();
        }

        public override void BaseOutput()
        {
            base.BaseOutput();
        }
    }

    class OpenWeather : API
    {
        public OpenWeather(Implementor imp, MainForm mainform)
            : base(imp, mainform)
        { }

        public override void Device_info()
        {
            base.Device_info();
        }

        public override void Cache()
        {
            base.Cache();
        }

        public override void Weather_Save()
        {
            base.Weather_Save();
        }

        public override void Output_Cache()
        {
            base.Output_Cache();
        }

        public override void BaseOutput()
        {
            base.BaseOutput();
        }
    }

}
    