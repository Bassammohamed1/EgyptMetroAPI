namespace MetroAPI.Models.DTOS
{
    public class StationDTO
    {
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int StationNO { get; set; }
        public bool isShared { get; set; }
        public int? SharedWith { get; set; }
        public int LineId { get; set; }
    }
}
