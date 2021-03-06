using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using XDD.Core.DataAccess;
using XDD.Core.Model;
using XDD.Web.Infrastructure;

namespace XDD.Web.Controllers
{
    [Authorize(Roles = "管理员,票务管理")]
    public class TicketController : Controller
    {

        XDDDbContext Context = new XDDDbContext();

        public ActionResult Index()
        {
            return View();
        }


        public JsonResult GetTickets(string keyword, int page = 1, int rows = 15)
        {
            IQueryable<Ticket> query = Context.Tickets;
            int keywordId;
            bool isId = int.TryParse(keyword, out keywordId);
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(s => (isId?s.Id==keywordId:false) || s.Address.Contains(keyword) || s.Content.Contains(keyword) || s.Name.Contains(keyword) || s.ShopName.Contains(keyword) || s.TicketCategory.Name.Contains(keyword) || s.Employee.LoginName.Contains(keyword) || s.Employee.RealName.Contains(keyword) || (s.TicketPackages.Count(t => t.Supplier.Description.Contains(keyword) || t.Supplier.Member.RealName.Contains(keyword) || t.Supplier.Member.PlatformBindPhone.Contains(keyword) || t.Supplier.Member.NickName.Contains(keyword)||s.Tags.Contains(keyword)) > 0));
            }
            return Json(new
            {
                total = query.Count(),
                rows = query.OrderBy(s => s.TicketCategory.SN).ThenBy(s => s.SN).ThenByDescending(s => s.CreateTime).Skip((page - 1) * rows).Take(rows).Select(s => new
                {
                    s.Address,
                    s.Content,
                    s.CreateTime,
                    s.Employee.LoginName,
                    s.Employee.RealName,
                    s.Enable,
                    s.Id,
                    s.Lat,
                    s.Lng,
                    s.Name,
                    s.Tags,
                    s.OnSale,
                    s.ServiceType,
                    s.SaleNum,
                    s.EarlyDay,
                    s.OrginPrice,
                    s.Pic,
                    s.MoreUrl,
                    s.Price,
                    s.ShopName,
                    s.SN,
                    TicketCategoryName = s.TicketCategory.Name,
                    s.TicketCategoryId,
                    PackageCount = s.TicketPackages.Count()
                }).ToList()
            });
        }

        [ValidateInput(false)]
        public JsonResult AddTicket(Ticket ticket)
        {

            XDDDbContext ctx = new XDDDbContext();

           

            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                string uploadPath = Server.MapPath("~/Images/Ticket/");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }
                string name = DateTime.Now.ToString("b-yyyyMMddHHmmssfff");
                if (ImageWorker.IsValidImage(file))
                {

                    ImageFormat imageFormat;
                    switch (Path.GetExtension(file.FileName).ToLower())
                    {
                        case ".jpeg":
                            imageFormat = ImageFormat.Jpeg;
                            break;
                        case ".png":
                            imageFormat = ImageFormat.Png;
                            break;
                        case ".gif":
                            imageFormat = ImageFormat.Gif;
                            break;
                        case ".jpg":
                            imageFormat = ImageFormat.Jpeg;
                            break;
                        default:
                            return Json(new { ResultCode = 0, message = "只接受jpg/png/gif图片" });
                    }

                    //保存
                    ImageWorker.MakeThumbnail(file, string.Format("{0}{1}{2}", uploadPath, name, Path.GetExtension(file.FileName).ToLower()), 900, 9999, "W", imageFormat);
                    ImageWorker.MakeThumbnail(file, string.Format("{0}{1}{2}", uploadPath, name.Replace("b-", "s-"), Path.GetExtension(file.FileName).ToLower()), 360, 9999, "W", imageFormat);
                    string domain = WebConfigurationManager.AppSettings["domain"];
                    ticket.Pic = domain + "/Images/Ticket/" + name + Path.GetExtension(file.FileName).ToLower();
                    ticket.CreateTime = DateTime.Now;
                    ticket.EmployeeId = Convert.ToInt32(User.Identity.Name);

                    Context.Tickets.Add(ticket);
                    Context.SaveChanges();
                    return Json(new
                    {
                        ResultCode = 1,
                        message = "添加成功"
                    });
                }
                else
                {
                    return Json(new { ResultCode = 0, message = "参数有误" });
                }

            }
            else
            {
                return Json(new { ResultCode = 0, message = "只接受jpg/png/gif图片" });
            }
        }
        [ValidateInput(false)]
        public JsonResult EditTicket(Ticket ticket)
        {
            var target = Context.Tickets.FirstOrDefault(s => s.Id == ticket.Id);
            if (target != null)
            {
                target.Address = ticket.Address;
                target.Content = HtmlSaferAnalyser.ToSafeHtml(ticket.Content, false);
                target.EmployeeId = Convert.ToInt32(User.Identity.Name);
                target.Enable = ticket.Enable;
                target.Lat = ticket.Lat;
                target.Lng = ticket.Lng;
                target.Name = ticket.Name;
                target.SaleNum = ticket.SaleNum;
                target.OnSale = ticket.OnSale;
                target.Price = ticket.Price;
                target.ShopName = ticket.ShopName;
                target.OrginPrice = ticket.OrginPrice;
                target.MoreUrl = ticket.MoreUrl;
                target.Tags = ticket.Tags;
                target.EarlyDay = ticket.EarlyDay;
                target.SN = ticket.SN;
                target.TicketCategoryId = ticket.TicketCategoryId;
                target.ServiceType = ticket.ServiceType;
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];
                    string uploadPath = Server.MapPath("~/Images/Ticket/");
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }
                    string name = DateTime.Now.ToString("b-yyyyMMddHHmmssfff");
                    if (ImageWorker.IsValidImage(file))
                    {
                        ImageFormat imageFormat;
                        switch (Path.GetExtension(file.FileName).ToLower())
                        {
                            case ".jpeg":
                                imageFormat = ImageFormat.Jpeg;
                                break;
                            case ".png":
                                imageFormat = ImageFormat.Png;
                                break;
                            case ".gif":
                                imageFormat = ImageFormat.Gif;
                                break;
                            case ".jpg":
                                imageFormat = ImageFormat.Jpeg;
                                break;
                            default:
                                return Json(new { ResultCode = 0, message = "只接受jpg/png/gif图片" });
                        }
                        //保存
                        ImageWorker.MakeThumbnail(file, string.Format("{0}{1}{2}", uploadPath, name, Path.GetExtension(file.FileName).ToLower()), 900, 9999, "W", imageFormat);
                        ImageWorker.MakeThumbnail(file, string.Format("{0}{1}{2}", uploadPath, name.Replace("b-", "s-"), Path.GetExtension(file.FileName).ToLower()), 360, 9999, "W", imageFormat);
                        string domain = WebConfigurationManager.AppSettings["domain"];
                        target.Pic = domain + "/Images/Ticket/" + name + Path.GetExtension(file.FileName).ToLower();
                    }
                    else
                    {
                        return Json(new { ResultCode = 0, message = "图片格式有误" });
                    }
                }
                Context.SaveChanges();
                return Json(new { ResultCode = 1, message = "修改成功" });

            }
            else
            {
                return Json(new { ResultCode = 0, message = "没有找到门票信息" });
            }

        }

        public JsonResult ChangeTicketStatus(int id, int type, bool status)
        {
            var target = Context.Tickets.FirstOrDefault(s => s.Id == id);
            if (target != null)
            {
                if (type == 1)
                {
                    target.OnSale = status;
                }
                else
                {
                    target.Enable = status;
                }
                Context.SaveChanges();
                return Json(new { ResultCode = 1, message = "修改成功" });
            }
            else
            {
                return Json(new { ResultCode = 0, message = "没有找到门票信息" });
            }
        }

        public JsonResult GetCategoryComboData()
        {
            return Json(Context.TicketCategories.Where(s => s.Enable).OrderBy(s => s.SN).Select(s => new
            {
                v = s.Id,
                t = s.Name
            }).ToList());
        }

        public JsonResult UploadImg()
        {
            List<String> list = new List<string>();
            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];
                var p = "/Images/Upload/" + DateTime.Now.ToString("yyyyMMdd") + "/";
                string uploadPath = Server.MapPath(p);
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }
                string name = DateTime.Now.ToString("HHmmssfff" + Guid.NewGuid().ToString().Split('-')[0]);
                if (ImageWorker.IsValidImage(file))
                {

                    ImageFormat imageFormat;
                    switch (Path.GetExtension(file.FileName).ToLower())
                    {
                        case ".jpeg":
                            imageFormat = ImageFormat.Jpeg;
                            break;
                        case ".png":
                            imageFormat = ImageFormat.Png;
                            break;
                        case ".gif":
                            imageFormat = ImageFormat.Gif;
                            break;
                        case ".jpg":
                            imageFormat = ImageFormat.Jpeg;
                            break;
                        default:

                            continue;
                    }

                    //保存
                    ImageWorker.MakeThumbnail(file, string.Format("{0}{1}{2}", uploadPath, name, Path.GetExtension(file.FileName).ToLower()), 900, 9999, "W", imageFormat);
                    string domain = WebConfigurationManager.AppSettings["domain"];
                    list.Add(domain + p + name + Path.GetExtension(file.FileName).ToLower());
                }

            }
            return Json(new
            {
                errno = 0,
                data = list
            });

        }


        public JsonResult GetTicketPackages(int id)
        {
            return Json(Context.TicketPackages.Where(s => s.TicketId == id).Select(s => new
              {
                  s.DeliveryPrice,
                  s.Enable,
                  s.OnSale,
                  s.Id,
                  s.L1AgentCharges,
                  s.L1AgentChargesPercent,
                  s.L1AgentMoreCharges,
                  s.L1AgentMoreChargesPercent,
                  s.L2AgentCharges,
                  s.L2AgentChargesPercent,
                  s.LastDay,
                  s.LastUpdateId,
                  s.Name,
                  s.Price,
                  s.SN,
                  s.Stock,
                  s.SupplierId,
                  s.TicketId,
                  s.UpdateTime,
                  LastUpdatorLoginName = s.LastUpdator.LoginName,
                  LastUpdatorName = s.LastUpdator.RealName,
                  SupplierRealName = s.Supplier.Member.RealName,
                  SupplierDescription = s.Supplier.Description
              }).OrderBy(s => s.SN).ToList());
        }

        public JsonResult EditPackage(TicketPackage package)
        {
            var target = Context.TicketPackages.FirstOrDefault(s => s.Id == package.Id);
            if (target != null)
            {
                target.DeliveryPrice = package.DeliveryPrice;
                target.Enable = package.Enable;
                target.L1AgentCharges = package.L1AgentCharges;
                target.L1AgentChargesPercent = package.L1AgentChargesPercent;
                target.L1AgentMoreCharges = package.L1AgentMoreCharges;
                target.L1AgentMoreChargesPercent = package.L1AgentMoreChargesPercent;
                target.L2AgentCharges = package.L2AgentCharges;
                target.L2AgentChargesPercent = package.L2AgentChargesPercent;
                target.LastDay = package.LastDay;
                target.LastUpdateId = Convert.ToInt32(User.Identity.Name);
                target.Name = package.Name;
                target.OnSale = package.OnSale;
                target.Price = package.Price;
                target.SN = package.SN;
                target.Stock = package.Stock;
                target.SupplierId = package.SupplierId;
                target.TicketId = package.TicketId;
                target.UpdateTime = DateTime.Now;
                try
                {
                    Context.SaveChanges();
                    return Json(new { ResultCode = 1, message = "修改成功" });
                }
                catch (Exception)
                {

                    return Json(new { ResultCode = 0, message = "事务冲突，请重试" });
                }
            }
            else
            {
                return Json(new { ResultCode = 0, message = "没有找到要修改的信息" });
            }
        }


        public JsonResult AddPackage(TicketPackage package)
        {
            Context.TicketPackages.Add(new TicketPackage
            {

                DeliveryPrice = package.DeliveryPrice,
                Enable = package.Enable,
                L1AgentCharges = package.L1AgentCharges,
                L1AgentChargesPercent = package.L1AgentChargesPercent,
                L1AgentMoreCharges = package.L1AgentMoreCharges,
                L1AgentMoreChargesPercent = package.L1AgentMoreChargesPercent,
                L2AgentCharges = package.L2AgentCharges,
                L2AgentChargesPercent = package.L2AgentChargesPercent,
                LastDay = package.LastDay,
                Name = package.Name,
                LastUpdateId = Convert.ToInt32(User.Identity.Name),
                OnSale = package.OnSale,
                Price = package.Price,
                SN = package.SN,
                Stock = package.Stock,
                SupplierId = package.SupplierId,
                TicketId = package.TicketId,
                UpdateTime = DateTime.Now
            });
            Context.SaveChanges();
            return Json(new { ResultCode = 1, message = "添加成功" });
        }

        public JsonResult GetSupplierComboGridData(string keyword)
        {
            IQueryable<Supplier> query = Context.Suppliers.Where(s => s.MemberId != null);
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(s => s.Description.Contains(keyword) || s.Member.RealName.Contains(keyword) || s.Member.PlatformBindPhone.Contains(keyword));
            }
            return Json(query.Select(s => new
            {
                s.Id,
                s.Description,
                s.Enable,
                s.CreateTime,
                s.MemberId,
                s.Member.RealName,
                s.Member.NickName,
                s.Member.PlatformBindPhone
            }).OrderByDescending(s => s.CreateTime).ToList());
        }


    }
}