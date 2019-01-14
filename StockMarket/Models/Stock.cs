namespace StockMarket.Models
{
    public class Stock
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public double Change { get; set; }
        public double WeeklyChange { get; set; }
        public double MonthlyChange { get; set; }
        public double YearlyChange { get; set; }

    }
}