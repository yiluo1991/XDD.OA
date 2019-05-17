using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using XDD.Core.DataAccess;
using XDD.Core.Model;
using XDD.Web.Infrastructure;

namespace XDD.Web.Controllers.API
{
    /// <summary>
    ///     获取首页轮播图
    /// </summary>
     [RoutePrefix("api/Banner")] 
    public class BannerController : ApiController
    {
       
         /// <summary>
         /// 获取轮播图
         /// </summary>
         /// <returns></returns>
        [Route("GetBanners"),HttpGet] 
        public List<Banner> GetBanner() {
            XDDDbContext ctx = new XDDDbContext();
            return  ctx.Banners.Where(s => s.Enable).OrderBy(s => s.SN).ToList();
        }

         /// <summary>
         /// 获取配置
         /// </summary>
         /// <returns></returns>
        [Route("GetSetting"), HttpGet]
        public SettingRoot GetSetting() {

            return Setting.MySetting;
            }
    }
}
