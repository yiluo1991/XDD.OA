using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XDD.Core.DataAccess;
using XDD.Core.Model;

namespace XDD.Web.Controllers
{

        [Authorize(Roles = "管理员,社交管理,二手管理")]
    public class IdentityController : Controller
    {
        
        /// <summary>
        ///     会员列表
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetUser(string keyword, int page = 1, int rows = 10)
        {
            var sexList = new List<string> { "女", "男" };
            var StatusList = new List<string> { "待审核", "允许", "拒绝" };
            XDDDbContext ctx = new XDDDbContext();
            IQueryable<IdentityVerify> query;
            if (string.IsNullOrEmpty(keyword))
            {
                query = ctx.IdentityVerifies;
            }
            else
            {
                int index = sexList.FindIndex(s => s == keyword);
                int indext = StatusList.FindIndex(s => s == keyword);
                query = ctx.IdentityVerifies.Where(s => s.RealName.Contains(keyword) || s.Member.NickName.Contains(keyword) || s.Employee.RealName.Contains(keyword) || (int)s.Sex == index || (int)s.Status == indext);
            }
            return Json(
                new
                {
                    //计算总条数
                    total = query.Count(),
                    //skip不取前几条
                    //take取出多少条数据
                    rows = query.OrderByDescending(s=>s.CreateTime).Skip((page - 1) * rows).Take(rows).ToList().Select(s => new
                    {
                      s.Id,
                      s.RealName,
                      s.Sex,
                      s.ImagePaths,
                      Member=new {s.Member.Id,s.Member.NickName,s.Member.RealName},
                      s.Status,
                      EmployeeName=s.EmployeeId==null?null:s.Employee.RealName,
                      s.CreateTime,
                      s.SchoolSN,
                      s.Institute,
                      s.Department,
                      s.Feedback
              
                    })
                }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Edit(int id, VerifyStatus stastus, string feedback)
        {
          

            XDDDbContext ctx = new XDDDbContext();
            var target = ctx.IdentityVerifies.First(s => s.Id == id);
            if (target != null) {
                target.VerifyTime = DateTime.Now;//审核时间
                target.EmployeeId = Convert.ToInt32(User.Identity.Name);//审核人
                target.Status = stastus;//是否通过
                target.Feedback = feedback;//审批的反馈信息
                if (stastus == VerifyStatus.Allow)
                {
                    target.Member.RealName = target.RealName;
                    target.Member.Sex = target.Sex;
                    if (!target.Member.Status.HasFlag(MemberStatus.Identity)) target.Member.Status |= MemberStatus.Identity;

                }
               
                ctx.SaveChanges();
                return Json(new {  ResultCode=1, message=""});
            }
            else
            {
                return Json(new { ResultCode = 0, message = "操作失败" });
            }
        

        }
    }
}