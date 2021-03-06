using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDD.Core.Model;
using XDD.Core.Model.Configuration;

namespace XDD.Core.DataAccess
{
    public class XDDDbContext : DbContext
    {
        public XDDDbContext()
            : base("XDDDbContext")
        {

        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Configurations.Add(new AccountStatementConfiguration());
            modelBuilder.Configurations.Add(new AgentApplyConfiguration());
            modelBuilder.Configurations.Add(new BannerConfiguration());
            modelBuilder.Configurations.Add(new BBSArticleConfiguration());
            modelBuilder.Configurations.Add(new BBSCategoryConfiguration());
            modelBuilder.Configurations.Add(new BBSCommentConfiguration());
            modelBuilder.Configurations.Add(new CommodityConfiguration());
            modelBuilder.Configurations.Add(new CommodityCategoryConfiguration());
            modelBuilder.Configurations.Add(new CommodityOrderConfiguration());
            modelBuilder.Configurations.Add(new EmployeeConfiguration());
            modelBuilder.Configurations.Add(new IdentityVerifyConfiguration());
            modelBuilder.Configurations.Add(new MemberConfiguration());
            modelBuilder.Configurations.Add(new MessageConfiguration());
            modelBuilder.Configurations.Add(new NavIconConfiguration());
            modelBuilder.Configurations.Add(new PermissionGroupConfiguration());
            modelBuilder.Configurations.Add(new SupplierConfiguration());
            modelBuilder.Configurations.Add(new TicketCategoryConfiguration());
            modelBuilder.Configurations.Add(new TicketConfiguration());
            modelBuilder.Configurations.Add(new TicketOrderConfiguration());
            modelBuilder.Configurations.Add(new TicketPackageConfiguration());
            modelBuilder.Configurations.Add(new WithdrawConfiguration());
            modelBuilder.Configurations.Add(new WordCommentConfiguration());
            modelBuilder.Configurations.Add(new WordConfiguration());
            modelBuilder.Configurations.Add(new WordLikeConfiguration());
            modelBuilder.Configurations.Add(new WordTagConfiguration());
        }
        public DbSet<AccountStatement> AccountStatements { get; set; }
        public DbSet<AgentApply> AgentApplys { get; set; }
        public DbSet<Banner> Banners { get; set; }
        public DbSet<BBSArticle> BBSArticles { get; set; }
        public DbSet<BBSCategory> BBSCategories { get; set; }
        public DbSet<BBSComment> BBSComments { get; set; }
        public DbSet<Commodity> Commodities { get; set; }
        public DbSet<CommodityCategory> CommodityCategories { get; set; }

        public DbSet<CommodityOrder> CommodityOrders { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<IdentityVerify> IdentityVerifies { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<NavIcon> NavIcons { get; set; }
        public DbSet<PermissionGroup> PermissionGroups { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketCategory> TicketCategories { get; set; }
        public DbSet<TicketOrder> TicketOrders { get; set; }
        public DbSet<TicketPackage> TicketPackages { get; set; }
        public DbSet<Withdraw> Withdraws { get; set; }
        public DbSet<WordComment> WordComments { get; set; }
        public DbSet<Word> Words { get; set; }
        public DbSet<WordLike> WordLikes { get; set; }
        public DbSet<WordTag> WordTags { get; set; }
    }
}
