namespace Redis.Dto.WeatherForecast
{
    public class ForecastResponse
    {
        public string by { get; set; }
        public bool valid_key { get; set; }
        public Detail results { get; set; }
        public double execution_time { get; set; }
        public bool from_cache { get; set; }
    }
}
