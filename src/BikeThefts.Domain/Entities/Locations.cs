﻿using System.Collections.Generic;

namespace BikeThefts.Domain.Entities
{
    public class Locations
    {
        public List<Filters> Operative { get; set; }
        public List<Filters> Expand { get; set; }
    }
}
