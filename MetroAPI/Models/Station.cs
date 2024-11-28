using System.ComponentModel.DataAnnotations.Schema;

namespace MetroAPI.Models
{
    public class Station
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int StationNO { get; set; }
        public bool isShared { get; set; }
        public int? SharedWith { get; set; }
        public int LineId { get; set; }
        [ForeignKey(nameof(LineId))]
        public Line? Line { get; set; }
    }
}
