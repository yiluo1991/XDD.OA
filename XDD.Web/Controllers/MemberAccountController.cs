using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XDD.Core.DataAccess;
using XDD.Core.Model;

namespace XDD.Web.Controllers
{

        [Authorize(Roles = "管理员")]
    public class MemberAccountController : Controller
    {
        /// <summary>
        /// 会员账户
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "管理员")]
        public ActionResult Index()
        {
            return View();
        }
        XDDDbContext Context = new XDDDbContext();
        /// <summary>
        /// 会员列表
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public JsonResult GetMembers(string keyword, int page = 1, int rows = 10) {

            IQueryable<Member> query = Context.Members;
            if (!string.IsNullOrEmpty(keyword))
            {
                int keywordId;
                bool isId = int.TryParse(keyword, out keywordId);
                query = query.Where(s =>(isId?s.Id==keywordId:false)|| s.NickName.Contains(keyword) || s.RealName.Contains(keyword) || s.PlatformBindPhone.Contains(keyword) || s.WeChatBindPhone.Contains(keyword));
                return Json( new{
                    total =query.Count(),
                    rows = query.OrderBy(s => s.Id).Skip((page - 1) * rows).Take(rows).Select(s => new {
                        s.Id,
                        s.AppOpenId,
                        s.WebOpenId,
                        s.UnionId,
                        s.NickName,
                        s.Sex,
                        s.RealName,
                        s.Country,
                        s.City,
                        s.Province,
                        s.WeChatBindPhone,
                        s.PlatformBindPhone,
                        s.Account,
                        s.Status,
                    }).ToList()
                }, JsonRequestBehavior.AllowGet);
           
            }
            return Json(new
            {
                total = query.Count(),
                rows = query.OrderBy(s => s.Id).Skip((page - 1) * rows).Take(rows).Select(s => new {
                    s.Id,
                    s.AppOpenId,
                    s.WebOpenId,
                    s.UnionId,
                    s.NickName,
                    s.Sex,
                    s.RealName,
                    s.Country,
                    s.City,
                    s.Province,
                    s.WeChatBindPhone,
                    s.PlatformBindPhone,
                    s.Account,
                    s.Status,
                }).ToList()
            }, JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// 流水
        /// </summary>
        /// <param name="id"></param>
        /// <param name="keyword"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public JsonResult GetStatements(int id, string keyword, DateTime? start, DateTime? end, int page = 1, int rows = 10)
        {

            var query = Context.AccountStatements.Where(s => s.MemberId == id);
            if (end != null)
            {
                end = end.Value.AddDays(1);
                query = query.Where(s => s.CreateTime < end.Value);
            }
            if (start != null)
            {
                query = query.Where(s => s.CreateTime > start.Value);
            }
            if (!string.IsNullOrEmpty(keyword))
            {

               query= query.Where(s => s.Type.Contains(keyword));


            }
            return Json(new
            {
                total = query.Count(),
                rows = query.OrderByDescending(s => s.CreateTime).ThenByDescending(s => s.Id).Skip((page - 1) * rows).Take(rows).Select(s => new
                {
                    s.Id,
                    s.CreateTime,
                    s.Type,
                    s.Money,
                    s.BeforeBalance
                }).ToList()
            });

        }
    }
}