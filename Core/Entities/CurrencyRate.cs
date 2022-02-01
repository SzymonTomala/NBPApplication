using System;

namespace Core.Entities
{
    public class CurrencyRate
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public decimal Value { get; set; }
        public DateTime Date { get; set; }
    }
}
