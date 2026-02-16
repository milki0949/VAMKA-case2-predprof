namespace stolovaya.Models.Entities
{
    public class Menu
    {
        public string Day { get; set; }
        public List<MenuDish> Dishes { get; set; } = new List<MenuDish>();
    }
}
