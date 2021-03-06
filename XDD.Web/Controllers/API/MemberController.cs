using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using XDD.Web.Infrastructure;
using XDD.Web.Models.WeChat;
using System.Web.Script.Serialization;
using XDD.Core.DataAccess;
using System.Web.Security;
using XDD.Core.Model;
using System.Web;
using System.IO;
using System.Drawing.Imaging;
using System.Web.Configuration;
using System.Text.RegularExpressions;


using XDD.SMS;
namespace XDD.Web.Controllers.API
{
    /// <summary>
    /// 会员接口
    /// </summary>
    [RoutePrefix("api/Member")]
    public class MemberController : ApiController
    {
        /// <summary>
        ///     检测是否安全版本，返回1为安全版本
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        [Route("APIVersion"), HttpGet]
        public int APIVersion(string version)
        {
            var limitstring = Setting.MySetting.VersionLimie;
            var limits = limitstring.Split('.');
            var vs = version.Split('.');
            int result = 1;
            try
            {
                for (int i = 0; i < limits.Length; i++)
                {
                    var l = Convert.ToInt32(limits[i]);
                    var v = Convert.ToInt32(vs[i]);
                    if (v > l)
                    {
                        result = 0;
                        break;
                    }
                    else if (v < l)
                    {
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            catch (Exception)
            {
                return 0;
            }
            return result;
        }

        /// <summary>
        ///     预登陆
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [Route("PreLogin"), HttpGet]

        public int PreLogin(string code)
        {

            WebClient client = new WebClient();
            string res = client.DownloadString("https://api.weixin.qq.com/sns/jscode2session?appid=" + WeChat.AppId + "&secret=" + WeChat.AppSecret + "&js_code=" + code + "&grant_type=authorization_code");
            JavaScriptSerializer jss = new JavaScriptSerializer();
            WXSessionResponse r = jss.Deserialize<WXSessionResponse>(res);
            if (r.openid != null)
            {
                XDDDbContext ctx = new XDDDbContext();
                var target = ctx.Members.FirstOrDefault(s => s.AppOpenId == r.openid || (s.UnionId != null && s.UnionId == r.unionid));
                if (target == null)
                {
                    //首次预登录
                    target = new Core.Model.Member() { AppOpenId = r.openid, UnionId = r.unionid, Session_key = r.session_key };
                    ctx.Members.Add(target);
                }
                else
                {
                    //已登录，更新session_key
                    target.UnionId = r.unionid;
                    target.Session_key = r.session_key;
                }
                ctx.SaveChanges();
                return target.Id;
            }
            return 0;
        }


        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Route("Login"), HttpPost]
        public AppLoginResponse Login(AppLoginRequest req)
        {
            XDDDbContext ctx = new XDDDbContext();
            var target = ctx.Members.FirstOrDefault(s => s.Id == req.id);
            try
            {
                string userinfostr = AES_Decrypt.AES_decrypt(req.encryptedData, target.Session_key, req.iv);
                JavaScriptSerializer jss = new JavaScriptSerializer();
                WXUserInfo r = jss.Deserialize<WXUserInfo>(userinfostr);
                if (target.AppOpenId == r.openId)
                {
                    if (target.AvatarUrl == null || target.AvatarUrl.StartsWith("https://wx.qlogo.cn/")) target.AvatarUrl = r.avatarUrl;
                    if (target.City == null) target.City = r.city;
                    if (target.UnionId == null && !string.IsNullOrEmpty(r.unionId)) target.UnionId = r.unionId;
                    if (target.Country == null) target.Country = r.country;
                    if (target.Province == null) target.Province = r.province;
                    if (target.Sex == Sex.Unknow) target.Sex = (Sex)r.gender;
                    if (target.NickName == null) target.NickName = r.nickName;
                    ctx.SaveChanges();
                    FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(target.Id.ToString(), true, 60 * 24);
                    string ticketencrypt = FormsAuthentication.Encrypt(ticket);
                    return new AppLoginResponse { ResultCode = 1, Message = ticketencrypt };
                }
                else
                {
                    return new AppLoginResponse() { Message = "身份验证失败", ResultCode = 0 };
                }
            }
            catch (Exception)
            {
                return new AppLoginResponse() { Message = "登录失败", ResultCode = -1 };
            }

        }
        /// <summary>
        /// 检查token是否有效
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Route("CheckToken"), HttpPost]
        public bool CheckToken(SimpleTokenRequest req)
        {
            var ticket = TicketWoker.GetTicket(req.Token);
            if (ticket == null)
            {
                return false;
            }
            else
            {
                return !ticket.Expired;
            }
        }
        /// <summary>
        /// 获取Id
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Route("GetId"), HttpPost]
        public string GetId(SimpleTokenRequest req)
        {
            var ticket = TicketWoker.GetTicket(req.Token);
            if (ticket != null)
            {
                return ticket.Name;
            }
            else
            {
                return "0";
            }
        }

        /// <summary>
        ///     检查是否实名
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Route("CheckIdentity"), HttpPost]
        public bool CheckIdentity(SimpleTokenRequest req)
        {
            var ticket = TicketWoker.GetTicket(req.Token);
            if (ticket != null)
            {
                int id = Convert.ToInt32(ticket.Name);
                XDDDbContext ctx = new XDDDbContext();
                var target = ctx.Members.FirstOrDefault(s => s.Id == id && s.Status.HasFlag(MemberStatus.Identity) && !s.Status.HasFlag(MemberStatus.Freeze));
                if (target != null)
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="req"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        [TokenAuthorize]
        [Route("GetUserInfo"), HttpGet]
        public DetailResponse GetUserInfo()
        {
            int id = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
            var target = (new XDDDbContext()).Members.FirstOrDefault(s => s.Id == id);
            if (target != null)
            {
                return new DetailResponse { Detail = new { target.Id, target.AvatarUrl, target.NickName, target.RealName, target.Account, target.City, target.Country, target.Province, target.Sex, target.Status, target.WeChatBindPhone, target.PlatformBindPhone, target.CaptainId, IsSupplier = target.Supplier.Count > 0 ? true : false }, ResultCode = 1, Message = "获取成功" };
            }
            else
            {
                return new DetailResponse()
                {
                    Detail = null,
                    Message = "没有找到",
                    ResultCode = 0
                };
            }
        }

        /// <summary>
        ///     编辑用户基础信息
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [TokenAuthorize]
        [Route("EditInfo"), HttpPost]
        public SimpleStatusResponse EditInfo(EditUserInfoRequest req)
        {
            int id = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
            var ctx = new XDDDbContext();
            var target = ctx.Members.FirstOrDefault(s => s.Id == id);
            if (target != null)
            {
                target.AvatarUrl = req.AvatarUrl;
                target.City = req.City;
                target.Province = req.Province;
                target.NickName = req.Name;
                ctx.SaveChanges();
                return new SimpleStatusResponse()
                {
                    ResultCode = 1,
                    Message = "修改成功"
                };
            }
            else
            {
                return new SimpleStatusResponse()
                {

                    Message = "没有找到",
                    ResultCode = 0
                };
            }
        }


        /// <summary>
        ///     上传头像图片
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [TokenAuthorize]
        [Route("UploadHead"), HttpPost]
        public DetailResponse UploadHead(FileUploadRequest req)
        {

            if (System.Web.HttpContext.Current.Request.Files[0] != null)
            {

                HttpPostedFile postfile = System.Web.HttpContext.Current.Request.Files[0];
                HttpPostedFileBase file = new HttpPostedFileWrapper(postfile) as HttpPostedFileBase;
                var path = "/Images/WX/Head/" + DateTime.Now.ToString("yyyyMMdd" + "/");
                string uploadPath = HttpContext.Current.Server.MapPath("~" + path);
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }
                string name = DateTime.Now.ToString("HHmmssfff");
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
                            return new DetailResponse { ResultCode = 0, Message = "只接受jpg/png/gif图片" };
                    }

                    //保存
                    ImageWorker.MakeThumbnail(file, string.Format("{0}{1}{2}", uploadPath, name, Path.GetExtension(file.FileName).ToLower()), 130, 9999, "W", imageFormat);
                    string domain = WebConfigurationManager.AppSettings["domain"];

                    return new DetailResponse { ResultCode = 1, Message = "上传成功", Detail = domain + string.Format("{0}{1}{2}", path, name, Path.GetExtension(file.FileName).ToLower()) };
                }
                else
                {
                    return new DetailResponse { ResultCode = 0, Message = "只接受图片文件" };
                }

            }
            else
            {
                return new DetailResponse { ResultCode = 0, Message = "没有图片文件" };
            }
        }


        /// <summary>
        ///     上传验证图片
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [TokenAuthorize]
        [Route("UploadImage"), HttpPost]
        public DetailResponse UploadImage(FileUploadRequest req)
        {

            if (System.Web.HttpContext.Current.Request.Files[0] != null)
            {

                HttpPostedFile postfile = System.Web.HttpContext.Current.Request.Files[0];
                HttpPostedFileBase file = new HttpPostedFileWrapper(postfile) as HttpPostedFileBase;
                var path = "/Images/WX/Verify/" + DateTime.Now.ToString("yyyyMMdd" + "/");
                string uploadPath = HttpContext.Current.Server.MapPath("~" + path);
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }
                string name = DateTime.Now.ToString("HHmmssfff");
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
                            return new DetailResponse { ResultCode = 0, Message = "只接受jpg/png/gif图片" };
                    }

                    //保存
                    ImageWorker.MakeThumbnail(file, string.Format("{0}{1}{2}", uploadPath, name, Path.GetExtension(file.FileName).ToLower()), 1280, 9999, "W", imageFormat);
                    string domain = WebConfigurationManager.AppSettings["domain"];

                    return new DetailResponse { ResultCode = 1, Message = "上传成功", Detail = domain + string.Format("{0}{1}{2}", path, name, Path.GetExtension(file.FileName).ToLower()) };
                }
                else
                {
                    return new DetailResponse { ResultCode = 0, Message = "只接受图片文件" };
                }

            }
            else
            {
                return new DetailResponse { ResultCode = 0, Message = "没有图片文件" };
            }
        }



        /// <summary>
        ///     发送验证码
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        [Route("SendCode"), HttpPost]
        [TokenAuthorize]
        public DetailResponse SendCode(string mobile)
        {
            if (Regex.IsMatch(mobile, @"^1\d{10}$"))
            {
                int id = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
                //string str = "QWERTYUPASDFGHJKLZXCVBNM1234567890";
                string str = "1234567890";
                string y = "";
                Random ran = new Random();
                for (int x = 0; x < 6; x++)
                {
                    int r = ran.Next(0, str.Length);
                    string a = str.Substring(r, 1);
                    y = y + a;
                }
                var code = new Models.MobileCode { SendTime = DateTime.Now, Code = y, Mobile = mobile };
                if (CacheManager.SetMobileCode(id, code))
                {
                    return new DetailResponse { Message = SMSManager.SendVerifyCode(code.Mobile, code.Code), ResultCode = 1 };
                }
                else
                {
                    return new DetailResponse { Message = "两次发送短信需间隔120秒", ResultCode = 1 };
                }

            }
            else
            {
                return new DetailResponse() { Message = "手机号码有误", ResultCode = 0 };
            }
        }

        /// <summary>
        ///     添加实名验证申请
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [TokenAuthorize]
        [Route("AddIdentityVerify"), HttpPost]
        public DetailResponse AddIdentityVerify(AddIdentityVerifyRequest req)
        {
            int id = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
            XDDDbContext ctx = new XDDDbContext();
            var member = ctx.Members.FirstOrDefault(s => s.Id == id && !s.Status.HasFlag(MemberStatus.Freeze));
            if (member == null)
            {
                return new DetailResponse { ResultCode = -1, Message = "用户不可用" };
            }
            else
            {

                if (member.Status.HasFlag(MemberStatus.Identity))
                {
                    return new DetailResponse { ResultCode = 0, Message = "用户已实名，无需重复认证" };
                }
                else if (member.IdentityVerifies.Count(s => s.Status == VerifyStatus.None) > 0)
                {
                    return new DetailResponse { ResultCode = 0, Message = "有未审核的实名申请，请等待平台审核" };
                }
                else
                {
                    //开始验证
                    if (string.IsNullOrEmpty(member.PlatformBindPhone))
                    {
                        //需验证手机
                        if (!CacheManager.CheckMobileCode(id, req.Mobile, req.Code))
                        {
                            //手机验证有误
                            return new DetailResponse { ResultCode = -1, Message = "手机验证码有误，请重试或重新获取" };
                        }
                        else
                        {
                            CacheManager.ClearMobileCode(id);
                            if (ctx.Members.Count(s => s.PlatformBindPhone == req.Mobile) > 0)
                            {
                                return new DetailResponse { ResultCode = -1, Message = "手机号已被绑定，请更换手机号" };
                            }
                            else
                            {
                                member.PlatformBindPhone = req.Mobile;
                            }

                        }
                    }
                    var verify = new IdentityVerify()
                    {
                        CreateTime = DateTime.Now,
                        ImagePaths = req.ImagePaths,
                        SchoolSN = req.SchoolSN,
                        Institute = req.Institute,
                        Department = req.Department,
                        Sex = req.Sex,
                        MemberId = id,
                        Status = VerifyStatus.None,
                        RealName = req.RealName,
                    };
                    ctx.IdentityVerifies.Add(verify);
                    ctx.SaveChanges();
                    return new DetailResponse { ResultCode = 1, Message = "申请已提交，请等待平台审核" };
                }

            }
        }


        /// <summary>
        ///     获取最后一条实名验证状态
        /// </summary>
        /// <returns></returns>
        [TokenAuthorize]
        [Route("GetIdentityVerifyStatus"), HttpPost]
        public DetailResponse GetIdentityVerifyStatus()
        {
            int id = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
            XDDDbContext ctx = new XDDDbContext();
            var member = ctx.Members.FirstOrDefault(s => s.Id == id && !s.Status.HasFlag(MemberStatus.Freeze));
            if (member == null)
            {
                return new DetailResponse { ResultCode = -1, Message = "用户不可用" };
            }
            else
            {
                var target = member.IdentityVerifies.OrderByDescending(s => s.CreateTime).Select(s => new { s.Status, s.Feedback }).FirstOrDefault();
                return new DetailResponse { ResultCode = 1, Message = "获取成功", Detail = new { RealName = member.RealName, Sex = member.Sex, Mobile = member.PlatformBindPhone, MemberStatus = member.Status, Target = target == null ? null : target } };
            }
        }

        /// <summary>
        ///   获取最后一条代理验证状态
        /// </summary>
        /// <returns></returns>
        [TokenAuthorize]
        [Route("GetAgentVerifyStatus"), HttpPost]
        public DetailResponse GetAgentVerifyStatus()
        {
            int id = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
            XDDDbContext ctx = new XDDDbContext();
            var member = ctx.Members.FirstOrDefault(s => s.Id == id && !s.Status.HasFlag(MemberStatus.Freeze));
            if (member == null)
            {
                return new DetailResponse { ResultCode = -1, Message = "用户不可用" };
            }
            else
            {
                var target = member.AgentApplies.OrderByDescending(s => s.CreateTime).Select(s => new { s.Status, s.Feedback }).FirstOrDefault();
                return new DetailResponse { ResultCode = 1, Message = "获取成功", Detail = new { RealName = member.RealName, Sex = member.Sex, Mobile = member.PlatformBindPhone, MemberStatus = member.Status, Target = target == null ? null : target } };
            }
        }



        /// <summary>
        ///     添加代理验证申请
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [TokenAuthorize]
        [Route("AddAgentVerify"), HttpPost]
        public DetailResponse AddAgentVerify(AddAgentVerifyRequest req)
        {
            int id = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
            int? captainId = null;

            XDDDbContext ctx = new XDDDbContext();
            if (!string.IsNullOrEmpty(req.RefferCode))
            {
                var cid = (Convert.ToInt32(req.RefferCode.Replace("C", "")) - 10000);
                var catpain = ctx.Members.FirstOrDefault(s => s.Id == cid && s.CaptainId == null && s.Status.HasFlag(MemberStatus.Agant));
                if (catpain != null)
                {
                    captainId = cid;
                }
            }
            var member = ctx.Members.FirstOrDefault(s => s.Id == id && !s.Status.HasFlag(MemberStatus.Freeze));
            if (member == null)
            {
                return new DetailResponse { ResultCode = -1, Message = "用户不可用" };
            }
            else
            {
                if (member.Status.HasFlag(MemberStatus.Agant))
                {
                    return new DetailResponse { ResultCode = 0, Message = "用户已是代理，无需重复认证" };
                }
                else if (member.AgentApplies.Count(s => s.Status == VerifyStatus.None) > 0)
                {
                    return new DetailResponse { ResultCode = 0, Message = "有未审核的代理申请，请等待平台审核" };
                }
                else
                {
                    //开始验证
                    if (string.IsNullOrEmpty(member.PlatformBindPhone))
                    {
                        //需验证手机
                        if (!CacheManager.CheckMobileCode(id, req.Mobile, req.Code))
                        {
                            //手机验证有误
                            return new DetailResponse { ResultCode = -1, Message = "手机验证码有误，请重试或重新获取" };
                        }
                        else
                        {
                            CacheManager.ClearMobileCode(id);
                            if (ctx.Members.Count(s => s.PlatformBindPhone == req.Mobile) > 0)
                            {
                                return new DetailResponse { ResultCode = -1, Message = "手机号已被绑定，请更换手机号" };
                            }
                            else
                            {
                                member.PlatformBindPhone = req.Mobile;
                            }

                        }
                    }
                    if (ctx.AgentApplys.Count(s => s.IdCard == req.IDCard && s.Status == VerifyStatus.Allow) > 0)
                    {
                        return new DetailResponse { ResultCode = -1, Message = "身份证已被使用，如有问题请联系客服" };
                    }
                    else
                    {
                        var verify = new AgentApply()
                        {
                            CreateTime = DateTime.Now,
                            Paths = req.ImagePaths,
                            Sex = req.Sex,
                            MemberId = id,
                            Status = VerifyStatus.None,
                            RealName = req.RealName,
                            IdCard = req.IDCard,
                            CaptainId = captainId
                        };
                        ctx.AgentApplys.Add(verify);
                        ctx.SaveChanges();
                        return new DetailResponse { ResultCode = 1, Message = "申请已提交，请等待平台审核" };
                    }
                }
            }
        }



    }
}
