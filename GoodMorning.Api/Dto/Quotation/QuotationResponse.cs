namespace Redis.Dto.Quotation
{
    public class QuotationResponse
    {
        public string by { get; set; }
        public bool valid_key { get; set; }
        public bool from_cache { get; set; }
        public double execution_time { get; set; }
        public Result results { get; set; }
    }
}
