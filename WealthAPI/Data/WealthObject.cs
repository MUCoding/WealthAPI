namespace WealthAPI.Data
{
    public class WealthObject
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public WealthTypeEnum Type { get; set; }
        public string Name { get; set; }
        public decimal Value { get; set; }
    }
}
