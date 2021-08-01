using BikeThefts.Domain.Entities;
using System.Collections.Generic;

namespace BikeThefts.Domain.Settings
{
    public class Locations
    {
        public List<Filters> Operative { get; set; }
        public List<Filters> Expand { get; set; }
    }
}
