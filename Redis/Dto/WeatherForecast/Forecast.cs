namespace Redis.Dto
{
    public class Forecast
    {
        public string date { get; set; }
        public string weekday { get; set; }
        public int max { get; set; }
        public int min { get; set; }
        public string description { get; set; }
        public string condition { get; set; }
    }
}
