namespace stolovaya.Models.Entities
{
    public class Student
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Login { get; set; }
        public string Allergy { get; set; }
        public bool CanEatToday { get; set; }
        public bool GotBreakfast { get; set; }
        public bool GotLunch { get; set; }
        public DateTime LastDateGotBreakfast { get; set; }
        public DateTime LastDateGotLunch { get; set; }
        public string DislikeProducts { get; set; }
        public string DislikeDishes { get; set; }
    }
}
