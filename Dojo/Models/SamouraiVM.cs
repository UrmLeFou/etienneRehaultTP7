using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dojo.Models
{
    public class SamouraiVM
    {
        public Samourai Samourai { get; set; }
        public List<Arme> Armes { get; set; }
        public int? IdArme { get; set; }
        public List<ArtMartial> ArtsMartiaux { get; set; } = new List<ArtMartial>();
        public List<int> IdArtMartiaux { get; set; } = new List<int>();
    }
}