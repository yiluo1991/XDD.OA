using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using XDD.Core.DataAccess;
using XDD.Web.Infrastructure;
using XDD.Web.Models.WeChat;

namespace XDD.Web.Controllers.API
{
    /// <summary>
    /// 会员接口
    /// </summary>
    [RoutePrefix("api/Message")]
    public class MessageController : ApiController
    {
        XDDDbContext ctx = new XDDDbContext();

        /// <summary>
        ///     获取最近联系人
        /// </summary>
        /// <returns></returns>
        [Route("GetContacts"),HttpGet]
        [TokenAuthorize]
        public List<WXContact> GetContacts()
        {
            int id = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);

            var items = ctx.Messages.Where(s => (s.FromId == id || s.ToId == id) && ctx.Messages.GroupBy(t => new { t.FromId, t.ToId }).Select(t => t.Max(o => o.SendTime)).Contains(s.SendTime)).OrderByDescending(s=>s.SendTime).ToList();
            List<int> ids = new List<int>();
            List<WXContact> contacts = new List<WXContact>();
            foreach (var item in items)
            {
                int cid = item.FromId.Value == id ? item.ToId : item.FromId.Value;
                if (!ids.Contains(cid))
                {
                    contacts.Add(new WXContact { Id = cid, LastTime = item.SendTime, LastContent = item.Content });
                    ids.Add(cid);
                }
            }
            var members = ctx.Members.Where(s => ids.Contains(s.Id)).Select(s => new {  s.NickName,s.Id,s.Status,s.AvatarUrl}).ToList();
            foreach (var item in members)
            {
               var c=  contacts.First(s => s.Id == item.Id);
               c.Status = item.Status;
               c.NickName = item.NickName;
               c.AvatarUrl = item.AvatarUrl;
            }
            return contacts;
        }

        /// <summary>
        ///     获取未读数
        /// </summary>
        /// <returns></returns>
        [Route("GetUnReadCount"), HttpGet]
        [TokenAuthorize]
        public Dictionary<int,int> GetUnReadCount() {
            int id = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
            var list=  ctx.Messages.Where(s =>  s.ToId == id && s.HasRead == false).GroupBy(s => new { s.ToId, s.FromId }).Select(s => new { Count = s.Count(), s.Key.FromId, s.Key.ToId }).ToList();
            Dictionary<int, int> dic = new Dictionary<int, int>();
            foreach (var item in list)
            {
                 int cid = item.FromId.Value == id ? item.ToId : item.FromId.Value;
                 if (dic.ContainsKey(cid))
                 {
                     dic[cid]+=item.Count;
                 }
                 else
                 {
                     dic.Add(cid, item.Count);
                 }
            }
            return dic;
        }

        /// <summary>
        ///     获取聊天记录
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Route("GetMessagesByContact"),HttpPost]
        [TokenAuthorize]
        public PageResponse GetMessagesByContact(PageRequest req)
        {
            int id = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
            var list = ctx.Messages.Where(s => (s.FromId == id || s.ToId == id) && (s.FromId == req.id || s.ToId == req.id) && (req.date.HasValue ? s.SendTime < req.date.Value : true)).OrderByDescending(s => s.SendTime).Take(10).OrderBy(s=>s.SendTime).Select(s => new {  s.SendTime,s.Id,s.HasRead,s.FromId,s.ToId,s.Content}).ToList();
            var ms=ctx.Members.Where(s=>s.Id==id||s.Id==req.id).ToList();
            var to = ms.First(s=>s.Id==req.id);
            var mine = ms.First(s => s.Id == id);
            return new PageResponse
            {
                Rows = list,
                More = new { Mine = new { mine.Id, mine.AvatarUrl, mine.NickName }, Contact = new { to.Id, to.AvatarUrl, to.NickName } }
            };
        }
    }
}
