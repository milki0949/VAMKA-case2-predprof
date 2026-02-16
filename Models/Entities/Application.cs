namespace stolovaya.Models.Entities
{
    public class Application
    {
        public int Id { get; set; }
        public List<ApplicationProductDetails> Products { get; set; } = new List<ApplicationProductDetails>();
        public int TotalPrice { get; set; }
        public bool Status { get; set; }
    }
}
