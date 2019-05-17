using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using XDD.Core.DataAccess;
using XDD.Core.Model;

namespace XDD.Web.Controllers
{

    public class EmployeeController : Controller
    {
        [Authorize(Roles = "管理员")]
        /// <summary>
        ///     后台账号管理
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "管理员")]
        public JsonResult Get(string keyword = "")
        {
            XDDDbContext ctx = new XDDDbContext();
            List<Employee> list = new List<Employee>();
            IQueryable<Employee> query;
            if (string.IsNullOrEmpty(keyword))
            {
                query = ctx.Employees;
                //没有传递keyword或者keyword为""
            }
            else
            {
                //有传递keyword并且keyword的值不为空字符串
                query = ctx.Employees.Where(s => s.LoginName.Contains(keyword) || s.Email.Contains(keyword) || s.RealName.Contains(keyword) || s.RoleName.Contains(keyword));
            }//查询
            return Json(
            new
            {
                total = query.Count(),
                rows = query.OrderBy(s => s.Id).Select(s => new
                {
                    s.Id,
                    s.LoginName,
                    s.Email,
                    s.RealName,
                    s.RoleName,
                    s.CreateTime,
                    s.CreatorId,
                  CreatorLoginName=  s.Creator.LoginName,
                  CreatorRealName=  s.Creator.RealName,
                    s.Enable,
                    //获取数据
                })

            }, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "管理员")]
        public JsonResult Add(Employee user)
        {
           XDDDbContext ctx = new XDDDbContext();
            
            user.CreateTime = DateTime.Now;
            user.CreatorId = Convert.ToInt32(User.Identity.Name);
            user.Password = Crypto.HashPassword("123456");
            ctx.Employees.Add(user);
            
            ctx.SaveChanges();
            return Json(new { ResultCode=1,message="添加成功"});
        }
        [Authorize(Roles = "管理员")]
        public JsonResult Edit(Employee user)
        {


            XDDDbContext ctx = new XDDDbContext();
            var targetUser = ctx.Employees.FirstOrDefault(s => s.Id == user.Id);
            if (targetUser != null)
            {
                targetUser.CreateTime = DateTime.Now;//时间
                targetUser.CreatorId = Convert.ToInt32(User.Identity.Name);
                targetUser.LoginName = user.LoginName;
                targetUser.Email = user.Email;
                targetUser.RealName = user.RealName;
                targetUser.RoleName = user.RoleName;
                targetUser.Enable = user.Enable;
            }

                ctx.SaveChanges();
                return Json(new { ResultCode = 1, message = "添加成功" });
        }
        [Authorize(Roles = "管理员")]
        public JsonResult pass(Employee user)
        {


            XDDDbContext ctx = new XDDDbContext();
            var targetUser = ctx.Employees.FirstOrDefault(s => s.Id == user.Id);
            if (targetUser != null)
            {
                targetUser.Password = Crypto.HashPassword("123456");
            }

            ctx.SaveChanges();
            return Json(new { ResultCode = 1, message = "修改成功" });
        }
        [Authorize(Roles = "管理员,二手管理,社交管理,票务管理")]
        public ActionResult Logout()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            //Request.Cookies["name"].Value = "";
            for (int i = 0; i < Response.Cookies.Count; i++)
            {
                Response.Cookies[i].Expires = DateTime.Now;//cookie将马上过期
            }
            String UrlReferrer = System.Web.HttpContext.Current.Request.UrlReferrer.ToString();
            return Redirect("/Enter/index");
        }
        [Authorize(Roles = "管理员,二手管理,社交管理,票务管理")]
        [HttpPost]
        public ActionResult ChangeMinePassword(String oldpassword, String newpassword)
        {
            XDDDbContext ctx = new XDDDbContext();
            int currentID = GetUserID();
            var emp = ctx.Employees.FirstOrDefault(s => s.Id == currentID);

            if (!Crypto.VerifyHashedPassword(emp.Password, oldpassword))
                return Json(new { resultCode = 0 });
            else
            {
                emp.Password = Crypto.HashPassword(newpassword);
                ctx.SaveChanges();
                return Json(new { ResultCode = 1 });
            }
        }
        [Authorize(Roles = "管理员,二手管理,社交管理,票务管理")]
        public int GetUserID()
        {
            var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            var ticket = FormsAuthentication.Decrypt(cookie.Value);
            int ID = Convert.ToInt32(ticket.Name.Replace(" ",""));
            return ID;
        }
        [HttpPost]
        [Authorize(Roles = "管理员,二手管理,社交管理,票务管理")]
        public JsonResult GetUserName()
        {
            XDDDbContext ctx = new XDDDbContext();
            int currentID = GetUserID();
            var emp = ctx.Employees.FirstOrDefault(s => s.Id == currentID);

            if (!String.IsNullOrEmpty(emp.RealName))
            {
                return Json(new { resultCode = 1, name = emp.RealName });
            }
            else if(!String.IsNullOrEmpty(emp.LoginName))
            {
                return Json(new { resultCode = 1, name = emp.LoginName });
            }
            else return Json(new { resultCode = 0 });
        }
    }
}