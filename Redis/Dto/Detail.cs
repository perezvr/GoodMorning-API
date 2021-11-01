using System;

namespace Redis.Dto
{
    public class Detail
    {
        public string wind_cdir { get; set; }
        public int rh { get; set; }
        public string pod { get; set; }
        public DateTime timestamp_utc { get; set; }
        public string pres { get; set; }
        public string solar_rad { get; set; }
        public string ozone { get; set; }
        public Weather weather { get; set; }
        public string wind_gust_spd { get; set; }
        public DateTime timestamp_local { get; set; }
        public int snow_depth { get; set; }
        public int clouds { get; set; }
        public int ts { get; set; }
        public string wind_spd { get; set; }
        public int pop { get; set; }
        public string wind_cdir_full { get; set; }
        public string slp { get; set; }
        public string dni { get; set; }
        public string dewpt { get; set; }
        public int snow { get; set; }
        public string uv { get; set; }
        public int wind_dir { get; set; }
        public int clouds_hi { get; set; }
        public string precip { get; set; }
        public string vis { get; set; }
        public string dhi { get; set; }
        public string app_temp { get; set; }
        public string datetime { get; set; }
        public string temp { get; set; }
        public string ghi { get; set; }
        public int clouds_mid { get; set; }
        public int clouds_low { get; set; }
    }

}
