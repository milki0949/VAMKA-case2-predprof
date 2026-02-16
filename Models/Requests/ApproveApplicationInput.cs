using stolovaya.Models.Entities;

namespace stolovaya.Models.Requests
{
    public class ApproveApplicationInput
    {
        public int ApplicationId { get; set; }
        public List<ApplicationProductDetails> Products { get; set; }
        public int TotalSpent { get; set; }
    }
}
