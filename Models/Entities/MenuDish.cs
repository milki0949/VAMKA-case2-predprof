namespace stolovaya.Models.Entities
{
    public class MenuDish
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Calories { get; set; }
        public List<int> ProductIds { get; set; } = new List<int>();
        public List<string> ProductNames { get; set; } = new List<string>();
    }
}
