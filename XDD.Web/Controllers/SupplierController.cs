using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XDD.Core.DataAccess;
using XDD.Core.Model;
using System.Text;
using XDD.Web.Infrastructure;
namespace XDD.Web.Controllers
{
    [Authorize(Roles = "管理员,票务管理")]
    public class SupplierController : Controller
    {
        XDDDbContext Context = new XDDDbContext();
        /// <summary>
        ///     门票提供商管理
        /// </summary>
        /// <returns></returns>

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Get(string keyword = null, int page = 1, int rows = 15)
        {
            IQueryable<Supplier> query = Context.Suppliers;
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(s => s.Description.Contains(keyword) || s.Member.NickName.Contains(keyword) || s.Employee.RealName.Contains(keyword));
            }
            return Json(new
            {
                total = query.Count(),
                rows = query.OrderByDescending(s => s.CreateTime).Skip((page - 1) * rows).Take(rows).Select(s => new
                {
                    s.Description,
                    s.Id,
                    s.CreateTime,
                    s.Enable,
                    EmployeeName = s.Employee.RealName,
                    Member =s.MemberId==null?null:  new { s.Member.RealName, s.Member.Id, s.Member.NickName }

                }).ToList()
            }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult Add(Supplier supplier)
        {
            supplier.EmployeeId = Convert.ToInt32(User.Identity.Name);
            supplier.CreateTime = DateTime.Now;
            Context.Suppliers.Add(supplier);
            Context.SaveChanges();
            return Json(new { ResultCode = 1, message = "添加成功" });
        }

        public JsonResult Edit(Supplier supplier)
        {

            Supplier tager = Context.Suppliers.FirstOrDefault(s => s.Id == supplier.Id);
            if (tager != null)
            {
                tager.Enable = supplier.Enable;
                tager.Description = supplier.Description;
            }
            Context.SaveChanges();
            return Json(new { ResultCode = 1, message = "修改成功" });
        }
      

        public JsonResult CreateCode(int Id)
        {
            var supplier = Context.Suppliers.FirstOrDefault(s => s.Id == Id && s.MemberId == null);
            if (supplier != null)
            {
                string str = "1234567890ABCDEFGHJKMNPRSTUVWXY";
                string y = "";
                Random ran = new Random();
                for (int x = 0; x < 6; x++)
                {
                    int r = ran.Next(0, str.Length);
                    string a = str.Substring(r, 1);
                    y = y + a;
                }
                y = y.ToUpper();
                CacheManager.SetSupplierCode(y, supplier.Id);
                return Json(new { ResultCode = 1, Message = "绑定码【" + y + "】，有效时间5分钟，请尽快告知供应商前往小程序中绑定" });
            }
            else
            {
                return Json(new { ResultCode = 0, Message = "没有找到供应商或供应商已绑定用户" });
            }
        }
    }
}