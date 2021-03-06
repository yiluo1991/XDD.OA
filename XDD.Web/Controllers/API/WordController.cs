using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.Security;
using XDD.Core.DataAccess;
using XDD.Core.Model;
using XDD.Web.Infrastructure;
using XDD.Web.Models.WeChat;

namespace XDD.Web.Controllers.API
{
    [RoutePrefix("api/Word")]
    public class WordController : ApiController
    {
        /// <summary>
        ///     获取热搜
        /// </summary>
        /// <returns></returns>
        [Route("GetTags"), HttpPost]
        public List<string> GetTags()
        {
            XDDDbContext ctx = new XDDDbContext();
            return ctx.WordTags.OrderBy(s => s.SN).Select(s => s.Name).ToList();
        }

        /// <summary>
        ///     获取留言列表
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
       
        [Route("GetWords"), HttpPost]
        [TokenAuthorize]
        public PageResponse GetWords(PageRequest req)
        {
            XDDDbContext ctx = new XDDDbContext();
            IQueryable<Word> q;
            int  id = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
              
       
            if (string.IsNullOrEmpty(req.keyword))
            {
                q = ctx.Words;
            }
            else
            {
                q = ctx.Words.Where(s => s.Member.NickName.Contains(req.keyword) || s.Content.Contains(req.keyword));
            }
            q = q.OrderByDescending(s => s.Top).ThenBy(s => s.SN).ThenByDescending(s => s.CreateTime);
            return new PageResponse()
            {
                Total = q.Count(),
                Rows = q.Skip((req.page - 1) * 10).Take(10).Select(s => new
                {
                    s.Member.AvatarUrl,
                    s.Member.NickName,
                    s.Id,
                    s.SN,
                    s.Content,
                    s.MemberId,
                    Like=s.Likes.Count(w=>w.MemberId==id)>0,
                    s.LikesCount,
                    s.LinksCount,
                    s.CommentsCount,
                    CreateTime = s.CreateTime,
                    Reffer = (s.RefferId == null ? null : new
                    {
                        s.Reffer.Content,
                        s.Reffer.Member.NickName,
                        s.Reffer.Member.AvatarUrl,
                        Id = s.RefferId
                    })
                }).ToList()
            };
        }

        /// <summary>
        ///     获取留言详情
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Route("GetWord"), HttpPost]
        [TokenAuthorize]
        public DetailResponse GetWord(int Id)
        {
            XDDDbContext ctx = new XDDDbContext();
            var word = ctx.Words.FirstOrDefault(s => s.Id == Id);
            int  id = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
                
            if (word == null)
            {
                return new DetailResponse() { ResultCode = 0, Message = "没有找到指定的数据" };
            }
            else
            {
                
                return new DetailResponse()
                {
                    ResultCode = 1,
                    Message = "获取成功",
                    Detail = new
                    {
                        word.Id,
                        word.Content,
                        LikesCount = word.LikesCount,
                        CommentsCount = word.CommentsCount,
                        LinksCount = word.LinksCount,
                        word.Member.NickName,
                        word.MemberId,
                        Like = word.Likes.Count(w => w.MemberId == id) > 0,
                        word.Member.AvatarUrl,
                        word.Tags,
                       
                        CreateTime = word.CreateTime,
                        Reffer = (word.RefferId == null ? null : new
                        {
                            word.Reffer.Content,
                            word.Reffer.Member.NickName,
                            word.Reffer.Member.AvatarUrl,
                            Id = word.RefferId
                        })
                    }
                };
            }
        }


        /// <summary>
        ///     获取评论
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Route("GetComments"), HttpPost]
        public PageResponse GetWordComments(PageRequest req)
        {
            XDDDbContext ctx = new XDDDbContext();

            IQueryable<WordComment> q = ctx.WordComments.Where(s => s.WordId == req.id.Value);
            return new PageResponse()
            {
                Total = q.Count(),
                Rows = q.OrderByDescending(s => s.CreateTime).Skip((req.page - 1) * 10).Take(10).Select(s => new
                {
                    s.Content,
                    s.CreateTime,
                    s.Id,
                    s.MemberId,
                    s.Member.NickName,
                    s.Member.AvatarUrl
                }).ToList()
            };

        }

        /// <summary>
        ///     删除留言
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Route("DeleteWord"), HttpPost]
        [TokenAuthorize]
        public SimpleStatusResponse DeleteWord(SimpleStatusRequest req)
        {
                int  mid = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
            XDDDbContext ctx = new XDDDbContext();
            var target= ctx.Words.FirstOrDefault(s => s.Id == req.Id && s.MemberId == mid);
            if (target!=null)
            {
                var list = ctx.Words.Where(s => s.RefferId == target.Id).ToList();
                list.ForEach(s => { s.RefferId = null; s.RefferHasDeleted = true; });
                ctx.Words.Remove(target);
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
        [Route("DeleteWordComment"), HttpPost]
        [TokenAuthorize]
        public SimpleStatusResponse DeleteWordComment(SimpleStatusRequest req)
        {
            int mid = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
            XDDDbContext ctx = new XDDDbContext();
            var target = ctx.WordComments.FirstOrDefault(s => s.Id == req.Id && (s.MemberId == mid|| s.Word.MemberId == mid));
            if (target != null)
            {
                target.Word.CommentsCount--;
                ctx.WordComments.Remove(target);
                ctx.SaveChanges();
                return new SimpleStatusResponse { Message = "删除成功", ResultCode = 1 };
            }
            else
            {
                return new SimpleStatusResponse { Message = "没有可找到可删除的留言评论", ResultCode = 0 };
            }
        }


        /// <summary>
        ///     添加留言
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Route("AddWord"), HttpPost]
        [IdentityAuthorize]
        public AddWordResponse AddWord(AddWordRequest req)
        {
            if (ModelState.IsValid)
            {

                int id = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
                if (PublisherLock.CanPublish(id))
                {
                    List<string> tags = new List<string>();
                    var matchs = Regex.Matches(req.Content, @"#[^#\|]{1,}#");
                    foreach (Match match in matchs)
                    {
                        tags.Add(match.Value);
                    }
                    XDDDbContext ctx = new XDDDbContext();
                    var word = new Word()
                    {
                        MemberId = id,
                        Content = req.Content,
                        RefferId = req.RefferId,
                        SN = 100,
                        Tags = String.Join("|", tags),
                        CreateTime = DateTime.Now
                    };
                    ctx.Words.Add(word);
                    ctx.SaveChanges();
                    PublisherLock.MemberPublish(id);
                    return new AddWordResponse() { ResultCode = 1, Message = "添加成功", Id = word.Id };
                }
                else
                {
                    return new AddWordResponse() { ResultCode = -1, Message = "发布内容需间隔" + Setting.MySetting.MemberPublisherTime + "秒" };

                }
            }
            else
            {
                return new AddWordResponse() { ResultCode = -1, Message = "参数有误" };
            }
        }

        /// <summary>
        /// 转发
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Route("TransWord"), HttpPost]
        [IdentityAuthorize]
        public AddWordResponse TransWord(AddWordRequest req)
        {
            if (ModelState.IsValid)
            {
                int id = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
                List<string> tags = new List<string>();
                var matchs = Regex.Matches(req.Content, @"#[^#\|]{1,}#");
                foreach (Match match in matchs)
                {
                    tags.Add(match.Value);
                }
                XDDDbContext ctx = new XDDDbContext();
                int? refferid = null;
                if (req.RefferId.HasValue)
                {
                    var reffer = ctx.Words.FirstOrDefault(s => s.Id == req.RefferId.Value);
                    if (reffer != null)
                    {
                        reffer.LinksCount++;
                        refferid = reffer.Id;
                        if (reffer.Reffer != null)
                        {
                            reffer.Reffer.LinksCount++;
                            refferid = reffer.Reffer.Id;
                        }
                    }
                }
                if (PublisherLock.CanPublish(id))
                {
                    var word = new Word()
                  {
                      MemberId = id,
                      Content = req.Content,
                      RefferId = refferid,
                      SN = 100,
                      Tags = String.Join("|", tags),
                      CreateTime = DateTime.Now
                  };
                    ctx.Words.Add(word);
                    ctx.SaveChanges();
                    PublisherLock.MemberPublish(id);
                    return new AddWordResponse() { ResultCode = 1, Message = "添加成功", Id = word.Id };
                }
                else
                {
                    return new AddWordResponse() { ResultCode = -1, Message = "发布内容需间隔" + Setting.MySetting.MemberPublisherTime + "秒" };

                }


            }
            else
            {
                return new AddWordResponse() { ResultCode = -1, Message = "参数有误" };
            }
        }


        /// <summary>
        ///     添加评论
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Route("AddComment"), HttpPost]
        [IdentityAuthorize]
        public AddWordCommentResponse AddComment(AddWordCommentRequest req)
        {
            if (ModelState.IsValid)
            {
                int id = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
                if (PublisherLock.CanPublish(id))
                {
                    XDDDbContext ctx = new XDDDbContext();
                    var word = ctx.Words.FirstOrDefault(s => s.Id == req.WordId);
                    if (word != null)
                    {
                        ctx.WordComments.Add(new WordComment()
                        {
                            Content = req.Content,
                            CreateTime = DateTime.Now,
                            MemberId = id,
                            WordId = req.WordId
                        });
                        word.CommentsCount++;
                        ctx.SaveChanges();
                        PublisherLock.MemberPublish(id);
                        return new AddWordCommentResponse() { ResultCode = 1, Message = "评论成功" };
                    }
                    else
                    {
                        return new AddWordCommentResponse() { ResultCode = -1, Message = "留言已删除" };
                    }
                }
                else
                {
                    return new AddWordCommentResponse() { ResultCode = -1, Message = "发布内容需间隔" + Setting.MySetting.MemberPublisherTime + "秒" };

                }
            }
            else
            {
                return new AddWordCommentResponse() { ResultCode = -1, Message = "参数有误" };
            }

        }

        /// <summary>
        /// 点赞和取消
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Route("Like"), HttpPost]
        [TokenAuthorize]
        public SimpleStatusResponse Like(SimpleStatusRequest req)
        {

            XDDDbContext ctx = new XDDDbContext();
            int id = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
            var target = ctx.WordLikes.FirstOrDefault(s => s.WordId == req.Id && s.MemberId == id);
            var word = ctx.Words.FirstOrDefault(s => s.Id == req.Id);
            if (word != null)
            {
                if (target == null)
                {
                    //点赞
                    ctx.WordLikes.Add(new WordLike() { MemberId = id, WordId = req.Id });
                    word.LikesCount++;
                    ctx.SaveChanges();
                    return new SimpleStatusResponse() { Id = req.Id, Message = "点赞", Num = word.LikesCount, ResultCode = 1 };
                }
                else
                {
                    //取消
                    var newcount = word.LikesCount - 1;
                    if (newcount < 0) newcount = 0;
                    word.LikesCount = newcount;
                    ctx.WordLikes.Remove(target);
                    ctx.SaveChanges();
                    return new SimpleStatusResponse() { Id = req.Id, Message = "取消点赞", Num = word.LikesCount, ResultCode = 1 };
                }

            }
            else
            {
                return new SimpleStatusResponse() { Id = req.Id, Message = "没有找到留言", Num = 0, ResultCode = 0 };
            }

        }
    }


}
