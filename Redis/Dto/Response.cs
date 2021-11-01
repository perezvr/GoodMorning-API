using System.Collections.Generic;

namespace Redis.Dto
{
    public class Response
    {
        public List<Detail> data { get; set; }
        public string city_name { get; set; }
        public string lon { get; set; }
        public string timezone { get; set; }
        public string lat { get; set; }
        public string country_code { get; set; }
        public string state_code { get; set; }
    }
}
