using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KTPM.Models
{
    internal class DVHC
    {
        public int id { get; set; }
        public string ten { get; set; }
        public string cap { get; set; }
        public int parent_id { get; set; }

        static public List<DVHC> HuyenDemo
        {
            get
            {
                return new List<DVHC> { 
                    new DVHC { id = 1, ten = "Cau Giay", cap = "huyện" },
                    new DVHC { id = 2, ten = "Tay Ho", cap = "huyện" },
                };
            }
        }
    }
}
