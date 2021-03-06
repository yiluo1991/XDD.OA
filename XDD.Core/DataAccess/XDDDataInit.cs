using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using XDD.Core.Model;

namespace XDD.Core.DataAccess
{
    public class XDDDataInit : CreateDatabaseIfNotExists<XDDDbContext>
    {
        protected override void Seed(XDDDbContext context)
        {


            base.Seed(context);
            context.Banners.Add(new Banner { Enable = true, Src = "http://xdd.lihangsoft.com/Images/Banner/20180801134110890.jpg", Url = "", Name = "测试数据", SN = 1 });
            context.Banners.Add(new Banner { Enable = true, Src = "http://xdd.lihangsoft.com/Images/Banner/20180801112022497.jpg", Url = "", Name = "测试数据", SN = 2 });
            context.NavIcons.Add(new NavIcon { Src = "http://xdd.lihangsoft.com/Images/Banner/20180801124101631.png", Name = "测试", Url = "#", SN = 100, Enable = true });
            context.NavIcons.Add(new NavIcon { Src = "http://xdd.lihangsoft.com/Images/Banner/20180801124111021.png", Name = "测试", Url = "#", SN = 100, Enable = true });
            context.NavIcons.Add(new NavIcon { Src = "http://xdd.lihangsoft.com/Images/Banner/20180801124119339.png", Name = "测试", Url = "#", SN = 100, Enable = true });
            context.NavIcons.Add(new NavIcon { Src = "http://xdd.lihangsoft.com/Images/Banner/20180801125319719.png", Name = "测试", Url = "#", SN = 100, Enable = true });
            context.NavIcons.Add(new NavIcon { Src = "http://xdd.lihangsoft.com/Images/Banner/20180801125337036.png", Name = "测试", Url = "#", SN = 100, Enable = true });
            context.NavIcons.Add(new NavIcon { Src = "http://xdd.lihangsoft.com/Images/Banner/20180801125352853.png", Name = "测试", Url = "#", SN = 100, Enable = true });
            context.NavIcons.Add(new NavIcon { Src = "http://xdd.lihangsoft.com/Images/NavIcon/20180801134520656.jpg", Name = "测试", Url = "#", SN = 100, Enable = true });
            context.NavIcons.Add(new NavIcon { Src = "http://xdd.lihangsoft.com/Images/Banner/20180801133909669.jpg", Name = "测试", Url = "#", SN = 100, Enable = true });
            var tagNams = new String[] { "天气", "厦门", "每日短语", "生活", "美食推荐", "表白", "游戏", "吐槽" };
            foreach (var item in tagNams)
            {
                context.WordTags.Add(new WordTag() { Name = item, SN = 100 });
            }
            ///管理员
            context.Employees.Add(new Employee()
            {
                CreateTime = DateTime.Now,
                Enable = true,
                LoginName = "admin",
                Password = Crypto.HashPassword("123456"),
                RoleName = "管理员"
            });
            //虚拟账号
            //context.Members.Add(new Member { 
            // Account=0,
            // IsVirtualMember=true,
            //  AppOpenId="000000",
            //    City="厦门",
            //     Country="中国",
            //      Province="福建",
            //       NickName="小叮当官方"
            
            //});



            #region 菜单
            ///菜单

            context.PermissionGroups.Add(new PermissionGroup()
            {
                DisplayName = "留言板管理",
                SN = 2,
                Tag = "#",
                Url = "#",
                Headshot = "order",
                Children = new List<PermissionGroup> {
                    new  PermissionGroup{
                     DisplayName="热门话题管理",
                     SN=2,
                     Url="/WordTag/Index"
                    },
                    new PermissionGroup{
                        DisplayName="留言管理",
                        SN=1,
                        Url="/Word/Index"
                    }
                }
            });
            context.PermissionGroups.Add(new PermissionGroup()
            {
                DisplayName = "基础设置",
                SN = 1,
                Tag = "#",
                Url = "#",
                Headshot = "credit",
                Children = new List<PermissionGroup> {
                     new PermissionGroup{
                        DisplayName="功能设置",
                        SN=1,
                        Url="/Basic/Index"
                    },
                  new PermissionGroup{
                        DisplayName="首页导航图标管理",
                        SN=3,
                        Url="/NavIcon/Index"
                    },
                     new PermissionGroup{
                        DisplayName="轮播图管理",
                        SN=2,
                        Url="/Banner/Index"
                    },
                     new PermissionGroup{
                        DisplayName="后台账号管理",
                        SN=4,
                        Url="/Employee/Index"
                    }
                }
            });

            context.PermissionGroups.Add(new PermissionGroup()
            {
                DisplayName = "交流社区管理",
                SN = 3,
                Tag = "#",
                Url = "#",
                Headshot = "log",
                Children = new List<PermissionGroup> { 
                    new PermissionGroup{
                        DisplayName="频道分类管理",
                        SN=1,
                        Url="/BBSCategory/Index"
                    },
                    new PermissionGroup{
                        DisplayName="频道文章管理",
                        SN=2,
                        Url="/BBSArticle/Index"
                    }

                }
            });


            context.PermissionGroups.Add(new PermissionGroup()
            {
                DisplayName = "会员管理",
                SN = 4,
                Tag = "#",
                Url = "#",
                Headshot = "member",
                Children = new List<PermissionGroup> {
                 new PermissionGroup{
                        DisplayName="会员列表",
                        SN=1,
                        Url="/Member/Index"
                    },
                     new PermissionGroup{
                        DisplayName="实名认证审核",
                        SN=2,
                        Url="/Identity/Index"
                    },
                     new PermissionGroup{
                        DisplayName="消息列表",
                        SN=2,
                        Url="/Message/Index"
                    }
                
                }
            });


            context.PermissionGroups.Add(new PermissionGroup()
            {
                DisplayName = "资金中心",
                SN = 5,
                Tag = "#",
                Url = "#",
                Headshot = "finance",
                Children = new List<PermissionGroup> {
                 new PermissionGroup{
                        DisplayName="会员账户列表",
                        SN=2,
                        Url="/MemberAccount/Index"
                    }, new PermissionGroup{
                        DisplayName="平台流水",
                        SN=2,
                        Url="/PlatformStatement/Index"
                    }, new PermissionGroup{
                        DisplayName="提现管理",
                        SN=2,
                        Url="/Withdraw/Index"
                    }
                }
            });


            context.PermissionGroups.Add(new PermissionGroup()
            {
                DisplayName = "代理中心",
                SN = 6,
                Tag = "#",
                Url = "#",
                Headshot = "platform",
                Children = new List<PermissionGroup> { 
                 new PermissionGroup{
                        DisplayName="代理申请审核",
                        SN=2,
                        Url="/AgentApply/Index"
                    },
                 new PermissionGroup{
                        DisplayName="代理列表",
                        SN=2,
                        Url="/Agent/Index"
                    },
                }
                
            });
            context.PermissionGroups.Add(new PermissionGroup()
            {
                DisplayName = "票务中心",
                SN = 7,
                Tag = "#",
                Url = "#",
                Headshot = "product",
                Children = new List<PermissionGroup> { 
                    new PermissionGroup{
                        DisplayName="门票分类管理",
                        SN=2,
                        Url="/TicketCategory/Index"
                    },
                     new PermissionGroup{
                        DisplayName="门票提供商管理",
                        SN=2,
                        Url="/Supplier/Index"
                    },
                     new PermissionGroup{
                        DisplayName="购票订单",
                        SN=2,
                        Url="/TicketOrder/Index"
                    },
                      new PermissionGroup{
                        DisplayName="门票管理",
                        SN=2,
                        Url="/Ticket/Index"
                    }
                }
            });
            context.PermissionGroups.Add(new PermissionGroup()
            {
                DisplayName = "二手市场管理",
                SN = 8,
                Tag = "#",
                Url = "#",
                Headshot = "bid",
                Children = new List<PermissionGroup> { }
            });
            #endregion
            var catNames = new String[] { "电影", "吃饭", "二次元", "萌宠", "摄影约拍", "学车", "学习", "运动", "早安晚安", "桌游" };
            var catIcons = new String[] { 
                "http://www.xyxiaodingdang.com/Images/BBSCategory/20180815175133550.jpg",
                "http://www.xyxiaodingdang.com/Images/BBSCategory/20180815174956128.jpg",
                "http://www.xyxiaodingdang.com/Images/BBSCategory/20180815175004685.jpg",
                "http://www.xyxiaodingdang.com/Images/BBSCategory/20180815175017468.jpg",
                "http://www.xyxiaodingdang.com/Images/BBSCategory/20180815175031167.jpg",
                "http://www.xyxiaodingdang.com/Images/BBSCategory/20180815175040266.jpg",
                "http://www.xyxiaodingdang.com/Images/BBSCategory/20180815175047427.jpg",
                "http://www.xyxiaodingdang.com/Images/BBSCategory/20180815175055011.jpg",
                "http://www.xyxiaodingdang.com/Images/BBSCategory/20180815175102810.jpg",
                "http://www.xyxiaodingdang.com/Images/BBSCategory/20180815175112860.jpg"
            };
            for (int i = 0; i < catNames.Length; i++)
            {
                context.BBSCategories.Add(new BBSCategory { Name = catNames[i], SN = i+1,Icon=catIcons[i], Enable = true });
            }
               
            context.SaveChanges();
        }
    }
}
