namespace Redis.Dto.Quotation
{
    public class Currency
    {
        public string name { get; set; }
        public double buy { get; set; }
        public double? sell { get; set; }
        public double variation { get; set; }
    }
}
