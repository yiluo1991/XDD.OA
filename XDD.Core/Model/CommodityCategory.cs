using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDD.Core.Model
{
    public class CommodityCategory
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Icon { get; set; }

        public bool Enable { get; set; }

        public int SN { get; set; }

        public virtual ICollection<Commodity> Commodities { get; set; }
    }
}
