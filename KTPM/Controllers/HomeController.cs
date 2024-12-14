using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KTPM.Controllers
{
    internal class HomeController : BaseController
    {
        public override object Index()
        {
            return View(Models.DVHC.HuyenDemo);
        }

        public object AddNew() => View(new Models.DVHC { cap = "huyen" });
    }
}
