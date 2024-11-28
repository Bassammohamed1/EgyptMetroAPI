namespace MetroAPI.Models
{
    public class Line
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int LineNo { get; set; }
        public List<Station>? Stations { get; set; }

    }
}
