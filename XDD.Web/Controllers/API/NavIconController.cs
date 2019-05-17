using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using XDD.Core.DataAccess;
using XDD.Core.Model;

namespace XDD.Web.Controllers.API
{
      [RoutePrefix("api/NavIcon")]
    public class NavIconController : ApiController
    {

          /// <summary>
          ///   获取首页导航图标
          /// </summary>
          /// <returns></returns>
        [Route("GetNavIcons"), HttpGet]
        public List<NavIcon> GetNavIcons()
        {
            XDDDbContext ctx = new XDDDbContext();
            return ctx.NavIcons.Where(s => s.Enable).OrderBy(s => s.SN).ToList();
        }
    }
}
