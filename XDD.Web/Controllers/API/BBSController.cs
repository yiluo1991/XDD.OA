using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using XDD.Core.DataAccess;
using XDD.Core.Model;
using XDD.Web.Infrastructure;
using XDD.Web.Models.WeChat;

namespace XDD.Web.Controllers.API
{
    [RoutePrefix("api/BBS")]
    public class BBSController : ApiController
    {
        XDDDbContext Context = new XDDDbContext();

        /// <summary>
        ///     获取文章分类列表
        /// </summary>
        /// <returns>文章分类列表</returns>
        [Route("GetCategories")]
        public object GetCategories()
        {
            return Context.BBSCategories.Where(s => s.Enable).OrderBy(s => s.SN).Select(s => new { Enable = s.Enable, Icon = s.Icon, Id = s.Id, Name = s.Name, Option = s.Option, Required = s.Required, SN = s.SN, TimeAreaOneEnd = s.TimeAreaOneEnd, TimeAreaOneStart = s.TimeAreaOneStart, TimeAreaTwoEnd = s.TimeAreaTwoEnd, TimeAreaTwoStart = s.TimeAreaTwoStart }).ToList();
        }


        /// <summary>
        ///     分页获取文章列表
        /// </summary>
        /// <param name="req">分页请求</param>
        /// <returns>文章列表</returns>
        [Route("GetArticles"), HttpPost]
        public PageResponse GetArticles(PageRequest req)
        {
            IQueryable<BBSArticle> query = Context.BBSArticles;
            if (req.id.HasValue)
            {
                query = query.Where(s => s.CategoryId == req.id.Value && !s.Member.Status.HasFlag(MemberStatus.Freeze) && s.BBSCategory.Enable);
            }
            if (!string.IsNullOrEmpty(req.keyword))
            {
                query = query.Where(s => (s.Content.Contains(req.keyword) || s.Member.NickName.Contains(req.keyword) || s.Title.Contains(req.keyword) || s.Subject.Contains(req.keyword) || s.Address.Contains(req.keyword)) && !s.Member.Status.HasFlag(MemberStatus.Freeze) && s.BBSCategory.Enable);
            }
            return new PageResponse()
            {
                Total = query.Count(),
                Rows = query.OrderBy(s => s.SN).ThenByDescending(s => s.CreateTime).Skip((req.page - 1) * 10).Take(10).Select(s => new
                {
                    s.Title,
                    s.CommentCount,
                    s.ReadCount,
                    s.CreateTime,
                    s.Content,
                    s.SN,
                    s.Subject,
                    s.CategoryId,
                    s.MemberId,
                    CategoryName = s.BBSCategory.Name,
                    s.Paths,
                    s.Member.AvatarUrl,
                    s.Member.NickName,
                    s.Id,
                    s.IsBackgroundArticle
                }).ToList()
            };
        }
        [Route("GetHomeArticles"), HttpPost]
        public PageResponse GetHomeArticles()
        {
            IQueryable<BBSArticle> query = Context.BBSArticles;
           
                query = query.Where(s => !s.Member.Status.HasFlag(MemberStatus.Freeze) && s.BBSCategory.Enable&&s.HomeShow);
            
            return new PageResponse()
            {
                Total = query.Count(),
                Rows = query.OrderBy(s => s.SN).ThenByDescending(s => s.CreateTime).Select(s => new
                {
                    s.Title,
                    s.CommentCount,
                    s.ReadCount,
                    s.CreateTime,
                    s.Content,
                    s.SN,
                    s.Subject,
                    s.CategoryId,
                    s.MemberId,
                    CategoryName = s.BBSCategory.Name,
                    s.Paths,
                    s.Member.AvatarUrl,
                    s.Member.NickName,
                    s.Id,
                    s.IsBackgroundArticle
                }).ToList()
            };
        }

        /// <summary>
        ///     获取文章
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Route("GetArticle"), HttpPost]
        public DetailResponse GetArticle(SimpleStatusRequest req)
        {
            var s = Context.BBSArticles.Where(q => q.Id == req.Id && !q.Member.Status.HasFlag(MemberStatus.Freeze)&&q.BBSCategory.Enable).FirstOrDefault();
            if (s!=null)
            {
                s.ReadCount++;
                Context.SaveChanges();
                return new DetailResponse
                {

                    Detail =  new
                    {
                        s.Title,
                        s.CommentCount,
                        s.ReadCount,
                        s.CreateTime,
                        s.Content,
                        s.SN,
                        s.Subject,
                        s.CategoryId,
                        CategoryName = s.BBSCategory.Name,
                        s.Paths,
                        s.Member.AvatarUrl,
                        s.Member.NickName,
                        s.Id,
                        s.IsBackgroundArticle,
                        s.Address,
                        s.AllowComment,
                        s.DateTime,
                        s.MemberId,
                        s.Payment,
                        s.PeopleEnd,
                        s.PeopleStart,
                        s.ShowComment,
                    },
                    Message = "获取成功",
                    ResultCode = 1
                };
            }
            else
            {
                return new DetailResponse()
                {
                    ResultCode = 0,
                    Message = "没有找到相应资源",
                    Detail = null
                };
            }
        }


        /// <summary>
        ///     获取评论
        /// </summary>
        /// <param name="req">分页请求</param>
        /// <returns>分页评论</returns>
        [Route("GetComments"), HttpPost]
        public PageResponse GetComments(PageRequest req)
        {
            IQueryable<BBSComment> query = Context.BBSComments.Where(s => s.BBSArticleId == req.id.Value && !s.Member.Status.HasFlag(MemberStatus.Freeze));
            return new PageResponse()
            {
                Total = query.Count(),
                Rows = query.OrderBy(s => s.SN).Skip((req.page - 1) * 10).Take(10).Select(s => new
                {
                    s.Id,
                    s.SN,
                    s.CreateTime,
                    s.Comment,
                    s.Member.NickName,
                    MemberId = s.Member.Id,
                    s.Member.AvatarUrl,
                    s.Paths,
                    s.RefferHasDeleted,
                    Reffer = s.RefferId == null ? null : s.Member.Status.HasFlag(MemberStatus.Freeze) ? null : new
                    {
                        s.Reffer.Id,
                        s.Reffer.SN,
                        s.Reffer.Member.AvatarUrl,
                        MemberId = s.Member.Id,
                        s.Reffer.Comment,
                        s.Reffer.Member.NickName
                    }
                }).ToList()
            };
        }

        /// <summary>
        ///     获取评论总数
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Route("GetCommentCount"), HttpPost]
        public SimpleStatusResponse GetCommentCount(SimpleStatusRequest req)
        {
            IQueryable<BBSComment> query = Context.BBSComments.Where(s => s.BBSArticleId == req.Id && !s.Member.Status.HasFlag(MemberStatus.Freeze));
            return new SimpleStatusResponse() { Num = query.Count(), ResultCode = 1 };
        }

        /// <summary>
        ///     获取最后一条评论所在页号
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Route("GetLastCommentPage"), HttpPost]
        public SimpleStatusResponse GetLastCommentPage(SimpleStatusRequest req)
        {
            IQueryable<BBSComment> query = Context.BBSComments.Where(s => s.BBSArticleId == req.Id && !s.Member.Status.HasFlag(MemberStatus.Freeze));
            return new SimpleStatusResponse() { Num = ((query.Count() - 1) / 10) + 1, ResultCode = 1 };
        }

        /// <summary>
        ///     上传图片
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [IdentityAuthorize]
        [Route("UploadImage"), HttpPost]
        public DetailResponse UploadImage(FileUploadRequest req)
        {

            if (System.Web.HttpContext.Current.Request.Files[0] != null)
            {

                HttpPostedFile postfile = System.Web.HttpContext.Current.Request.Files[0];
                HttpPostedFileBase file = new HttpPostedFileWrapper(postfile) as HttpPostedFileBase;
                var path = "/Images/WX/" + DateTime.Now.ToString("yyyyMMdd" + "/");
                string uploadPath = HttpContext.Current.Server.MapPath("~" + path);
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }
                string name = DateTime.Now.ToString("b-HHmmssfff");
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
                    ImageWorker.MakeThumbnail(file, string.Format("{0}{1}{2}", uploadPath, name, Path.GetExtension(file.FileName).ToLower()), 1080, 9999, "W", imageFormat);
                    ImageWorker.MakeThumbnail(file, string.Format("{0}{1}{2}", uploadPath, name.Replace("b-", "s-"), Path.GetExtension(file.FileName).ToLower()), 320, 9999, "W", imageFormat);
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
        ///     添加评论
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [IdentityAuthorize]
        [Route("AddComment"), HttpPost]
        public AddBBSCommentResponse AddComment(AddBBSCommentRequest req)
        {
            int memberId = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
            BBSComment comment = new BBSComment() { Comment = req.Comment, BBSArticleId = req.ArticleId, CreateTime = DateTime.Now, MemberId = memberId, Paths = req.Paths };
            if (ModelState.IsValid)
            {
                if (PublisherLock.CanPublish(memberId))
                {
                    var article = Context.BBSArticles.FirstOrDefault(s => s.Id == req.ArticleId);
                    if (article != null)
                    {
                        if (article.AllowComment)
                        {
                            if (req.RefferId.HasValue)
                            {
                                var reffer = Context.BBSComments.First(s => s.Id == req.RefferId.Value);
                                if (reffer != null && reffer.BBSArticleId == article.Id)
                                {
                                    comment.RefferId = reffer.Id;
                                }
                                else
                                {
                                    comment.RefferHasDeleted = true;
                                }
                            }
                            //执行
                            article.Counter++;
                            article.CommentCount++;
                            comment.SN = article.Counter;
                            Context.BBSComments.Add(comment);
                            try
                            {
                                Context.SaveChanges();
                                PublisherLock.MemberPublish(memberId);
                                return new AddBBSCommentResponse { Message = "添加成功", ResultCode = 1, TogoPage = ((Context.BBSComments.Where(s => s.BBSArticleId == req.ArticleId && !s.Member.Status.HasFlag(MemberStatus.Freeze)).Count() - 1) / 10) + 1 };
                            }
                            catch (Exception)
                            {
                                return new AddBBSCommentResponse { Message = "服务器忙，请重试", ResultCode = 0 };
                            }
                        }
                        else
                        {
                            return new AddBBSCommentResponse { Message = "该文章不允许评论", ResultCode = 0 };
                        }
                    }
                    else
                    {
                        return new AddBBSCommentResponse { Message = "没有找到文章", ResultCode = 0 };
                    }
                }
                else
                {
                    return new AddBBSCommentResponse() { ResultCode = -1, Message = "发布内容需间隔" + Setting.MySetting.MemberPublisherTime + "秒" };

                }
            }
            else
            {
                return new AddBBSCommentResponse { Message = "参数有误", ResultCode = -1 };
            }
        }

        /// <summary>
        ///     添加文章
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Route("AddArticle"), HttpPost]
        [IdentityAuthorize]

        public SimpleStatusResponse AddArticle(AddBBSArticleRequest req)
        {
            var memberId = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
            var category = Context.BBSCategories.FirstOrDefault(s => s.Id == req.CategoryId);
            if (PublisherLock.CanPublish(memberId))
            {


                if (category != null && category.Enable)
                {
                    if (category.Required.HasFlag(CheckableItems.PeopleLimit))
                    {
                        if (req.PeopleEnd == null) req.PeopleEnd = 0;
                        if (req.PeopleStart == null) req.PeopleStart = 0;
                    }
                    BBSArticle article = new BBSArticle()
                    {
                        Address = req.Address,
                        AllowComment = true,
                        CategoryId = req.CategoryId,
                        CreateTime = DateTime.Now,
                        Content = req.Content,
                        DateTime = req.DateTime,
                        IsBackgroundArticle = false,
                        MemberId = memberId,
                        Paths = req.Paths,
                        Payment = req.Payment,
                        PeopleEnd = req.PeopleEnd,
                        PeopleStart = req.PeopleStart,
                        ShowComment = true,
                        SN = 100,
                        Subject = req.Subject,
                        Title = req.Title
                    };
                    Context.BBSArticles.Add(article);
                    try
                    {
                        Context.SaveChanges();
                        PublisherLock.MemberPublish(memberId);
                        return new SimpleStatusResponse { Message = "添加成功", Id = article.Id, ResultCode = 1 };
                    }
                    catch (Exception)
                    {
                        return new SimpleStatusResponse { Message = "参数有误", ResultCode = -1 };

                    }



                }
                else
                {
                    return new SimpleStatusResponse { Message = "频道不可用", ResultCode = 0 };
                }
            }
            else
            {
                return new SimpleStatusResponse() { ResultCode = -1, Message = "发布内容需间隔" + Setting.MySetting.MemberPublisherTime + "秒" };

            }
        }


        /// <summary>
        ///     删除留言
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Route("DeleteArticle"), HttpPost]
        [TokenAuthorize]
        public SimpleStatusResponse DeleteArticle(SimpleStatusRequest req)
        {
            int mid = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
            XDDDbContext ctx = new XDDDbContext();
            var target = ctx.BBSArticles.FirstOrDefault(s => s.Id == req.Id && s.MemberId == mid);
            if (target != null)
            {
                ctx.BBSArticles.Remove(target);
                ctx.SaveChanges();
                return new SimpleStatusResponse { Message = "删除成功", ResultCode = 1 };
            }
            else
            {
                return new SimpleStatusResponse { Message = "没有可找到可删除的文章", ResultCode = 0 };
            }
        }


        /// <summary>
        ///     删除留言评论
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Route("DeleteComment"), HttpPost]
        [TokenAuthorize]
        public SimpleStatusResponse DeleteComment(SimpleStatusRequest req)
        {
            int mid = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
            XDDDbContext ctx = new XDDDbContext();
            var target = ctx.BBSComments.FirstOrDefault(s => s.Id == req.Id && (s.MemberId == mid || s.BBSArticle.MemberId == mid));
            if (target != null)
            {
                var list = ctx.BBSComments.Where(s => s.RefferId == target.Id).ToList();
                list.ForEach(s => { s.RefferId = null; s.RefferHasDeleted = true; });
                target.BBSArticle.CommentCount--;
                ctx.BBSComments.Remove(target);
                ctx.SaveChanges();
                return new SimpleStatusResponse { Message = "删除成功", ResultCode = 1 };
            }
            else
            {
                return new SimpleStatusResponse { Message = "没有可找到可删除的留言评论", ResultCode = 0 };
            }
        }



    }
}
