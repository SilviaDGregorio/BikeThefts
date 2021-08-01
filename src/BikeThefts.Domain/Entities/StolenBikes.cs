using Newtonsoft.Json;

namespace BikeThefts.Domain.Entities
{
    public class StolenBikes
    {
        public int Thefts { get; set; }
        public string Location { get; set; }
        public double Distance { get; set; }
    }
}
