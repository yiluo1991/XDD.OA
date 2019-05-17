using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XDD.Core.DataAccess;
using XDD.Core.Model;
using XDD.Web.Models;
using System.Web.Configuration;
namespace XDD.Web.Controllers
{

    [Authorize(Roles = "管理员,二手管理,社交管理,票务管理")]
    public class MenuController : Controller
    {

      
        /// <summary>
        ///     开始菜单子页
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public PartialViewResult StartMenu()
        {
            List<ViewPermissionGroup> PermissionGroups;

            //由于后台Controllers部分没有加上 [Permission]  
            if (System.Web.HttpContext.Current.Session["PermissionGroups"] == null)
            {
              
                if (User.IsInRole("管理员"))
                {

                    System.Web.HttpContext.Current.Session["PermissionGroups"] = GetViewPermissionGroup();
                }
                else
                {
                    var ids = new List<string>();
                    if (User.IsInRole("社交管理"))
                    {
                        ids.AddRange(WebConfigurationManager.AppSettings["sj"].Split(','));
                    }
                     if (User.IsInRole("二手管理"))
                    {
                        ids.AddRange(WebConfigurationManager.AppSettings["es"].Split(','));
                    }
                     if (User.IsInRole("票务管理"))
                    {
                        ids.AddRange(WebConfigurationManager.AppSettings["mp"].Split(','));
                    }
                    var ints = ids.Select(s => Convert.ToInt32(s)).Distinct().ToList();
                    System.Web.HttpContext.Current.Session["PermissionGroups"] = GetViewPermissionGroup(ints);
                }
              
                
               
            }
            PermissionGroups = (System.Web.HttpContext.Current.Session["PermissionGroups"] as List<ViewPermissionGroup>).Where(p => p.Tag != "hidden").OrderBy(p => p.SN).ToList();
            return PartialView(PermissionGroups);
        }

        private object GetViewPermissionGroup(List<int> ints)
        {
            XDDDbContext ctx = new XDDDbContext();
            List<PermissionGroup> childrenPermissionGroups = ctx.PermissionGroups.Where(s=>ints.Contains(s.Id)).OrderBy(s => s.SN).ToList();
            return childrenPermissionGroups.Select(m => new ViewPermissionGroup { Id = m.Id, ParentId = m.ParentId, Url = m.Url, SN = m.SN, Headshot = m.Headshot, DisplayName = m.DisplayName, Tag = m.Tag }).ToList();//；拥有的权限组

        }

        private static List<ViewPermissionGroup> GetViewPermissionGroup()
        {
            XDDDbContext ctx = new XDDDbContext();
            List<PermissionGroup> childrenPermissionGroups = ctx.PermissionGroups.OrderBy(s=>s.SN).ToList();
            //List<PermissionGroup> newChildrenPermissionGroups = childrenPermissionGroups;
            //do
            //{
            //    newChildrenPermissionGroups = newChildrenPermissionGroups.Where(m => m.Parent != null).Select(m => m.Parent).ToList();
            //    childrenPermissionGroups.AddRange(newChildrenPermissionGroups);
            //}

            //while (newChildrenPermissionGroups.Where(m => m.Parent != null).Select(m => m.Parent).ToList().Count > 0);
            return childrenPermissionGroups.Select(m => new ViewPermissionGroup { Id = m.Id, ParentId = m.ParentId, Url = m.Url, SN = m.SN, Headshot = m.Headshot, DisplayName = m.DisplayName ,Tag=m.Tag }).ToList();//；拥有的权限组
        }

        /// <summary>
        ///     左侧子菜单子页
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public PartialViewResult SubMenu()
        {
            UrlHelper urlHelper = new UrlHelper(this.Request.RequestContext);
            var controllerName = urlHelper.RequestContext.RouteData.Values["controller"].ToString();
            var actionName = urlHelper.RequestContext.RouteData.Values["action"].ToString();
            List<ViewPermissionGroup> showPermissionGoups = new List<ViewPermissionGroup>();
            if (System.Web.HttpContext.Current.Session["PermissionGroups"] != null)
            {
                var permissonGroups = (System.Web.HttpContext.Current.Session["PermissionGroups"] as List<ViewPermissionGroup>).OrderBy(p => p.SN).ToList();
                var parent = permissonGroups.SingleOrDefault(p => p.Url.ToLower() == ("/" + controllerName + "/" + actionName).ToLower());
                var pid = parent.ParentId;
                ViewBag.subTitle = permissonGroups.SingleOrDefault(p => p.Id == pid).DisplayName;
                showPermissionGoups = permissonGroups.Where(p => p.ParentId == pid).ToList();
            }

            return PartialView(showPermissionGoups);
        }
    }
}