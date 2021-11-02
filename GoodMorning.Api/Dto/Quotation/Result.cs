using System.Collections.Generic;

namespace Redis.Dto.Quotation
{
    public class Result
    {
        public Quotation currencies { get; set; }
        public Stock stocks { get; set; }
        public List<string> available_sources { get; set; }
        public List<object> taxes { get; set; }
    }
}
