using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDD.Core.Model
{
    public class Member
    {
        public int Id { get; set; }

        public string AppOpenId { get; set; }
        public string WebOpenId { get; set; }
        public string UnionId { get; set; }

        /// <summary>
        ///     昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string AvatarUrl { get; set; }

        /// <summary>
        ///     真实姓名
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        ///     性别
        /// </summary>
        public Sex Sex { get; set; }

        /// <summary>
        ///     国家
        /// </summary>
        public String Country { get; set; }


        /// <summary>
        ///     省份
        /// </summary>
        public String Province { get; set; }

        /// <summary>
        ///     城市
        /// </summary>
        public String City { get; set; }

        /// <summary>
        ///     微信号绑定的手机号
        /// </summary>
        public String WeChatBindPhone { get; set; }

        /// <summary>
        ///     平台绑定的手机号
        /// </summary>
        public String PlatformBindPhone { get; set; }

        /// <summary>
        ///     账户余额
        /// </summary>
        public decimal Account { get; set; }

        /// <summary>
        ///     微信提供的秘钥，保密，不显示
        /// </summary>
        public String Session_key { get; set; }

        /// <summary>
        ///     账户状态
        /// </summary>
        public MemberStatus Status { get; set; }

        /// <summary>
        ///     队长id，二级代理有该属性
        /// </summary>
        public int? CaptainId { get;set; }

        /// <summary>
        ///     虚拟用户，后台用的假用户
        /// </summary>
        public bool IsVirtualMember { get; set; }
            
        [Timestamp]
        public byte[] RowVersion { get; set; }


        public virtual ICollection<Word> Words { get; set; }
        public virtual ICollection<WordComment> WordComments { get; set; }
        public virtual ICollection<WordLike> MyLikes { get; set; }
        public virtual ICollection<IdentityVerify> IdentityVerifies { get; set; }

        public virtual ICollection<BBSArticle> BBSArticles { get; set; }

        public virtual ICollection<BBSComment> BBSComments { get; set; }

        public virtual ICollection<Member> TeamMembers { get; set; }

        public virtual Member Captain { get; set; }

        public virtual ICollection<Withdraw> Withdraws { get; set; }

        /// <summary>
        ///     实际使用取第一条
        /// </summary>
        public virtual ICollection<Supplier> Supplier { get; set; }

        public virtual ICollection<TicketOrder> TicketOrders { get; set; }

        public virtual ICollection<AccountStatement> Statements { get; set; }

        public  virtual ICollection<AgentApply> AgentApplies { get; set; }

        public virtual ICollection<TicketOrder> AgentOrders { get; set; }

        public virtual ICollection<Message> ReciveMessages { get; set; }

        public virtual ICollection<Message> SendMessages { get; set; }

        public virtual ICollection<Commodity> Commodities { get; set; }

        public virtual ICollection<CommodityOrder> ComodityOrders { get; set; }
    }

    public enum Sex
    {
        Unknow,
        Male,
        Female
    }
    [Flags]
    public enum MemberStatus
    {
        None = 0x0,
        Identity = 0x1,
        Agant = 0x2,
        Supplier=0x4,
        Freeze = 0x8
    }
}
