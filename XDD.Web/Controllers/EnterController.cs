using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using XDD.Core.DataAccess;
using XDD.Web.Infrastructure;

namespace XDD.Web.Controllers
{
    public class EnterController : Controller
    {
        // GET: Enter
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login(string username, string password)
        {
            XDDDbContext ctx = new XDDDbContext();
            var emp = ctx.Employees.FirstOrDefault(s => s.LoginName == username);
            if (null != emp)
            {
                if (LoginLock.CanLogin(username))
                {
                    if (emp.Enable)
                    {
                        if (Crypto.VerifyHashedPassword(emp.Password, password))
                        {
                            Session.Clear();
                            LoginLock.MemberLoginSuccess(username);
                            var exp = DateTime.Now.Add(FormsAuthentication.Timeout);
                            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(2, emp.Id.ToString(), DateTime.Now, exp, true, emp.RoleName);
                            string cookieValue = FormsAuthentication.Encrypt(ticket);
                            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, cookieValue);
                            cookie.HttpOnly = true;
                            cookie.Secure = FormsAuthentication.RequireSSL;
                            cookie.Domain = FormsAuthentication.CookieDomain;
                            cookie.Path = FormsAuthentication.FormsCookiePath;
                            Response.Cookies.Add(cookie);
                            if (Request.QueryString["returnurl"] != null)
                            {
                                return Redirect(Request.QueryString["returnurl"]);
                            }
                            else
                            {
                                return Redirect("~/home");
                            }
                        }
                        else
                        {
                            LoginLock.MemberPasswordError(username);
                        }
                    }
                }
                else
                {
                    ViewBag.U = username;
                    ViewBag.P = password;
                    ViewBag.Error = "连续登录失败次数过多，请等待"+(Setting.MySetting.LoginLimitTime/60)+"分钟后重试";
                    return View("index");
                }
            }

            ViewBag.U = username;
            ViewBag.P = password;
            ViewBag.Error = "用户名或密码有误";
            return View("index");
        }
    }
}