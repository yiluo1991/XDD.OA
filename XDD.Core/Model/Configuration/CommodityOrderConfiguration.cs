using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace XDD.Core.Model.Configuration
{
    public class CommodityOrderConfiguration : EntityTypeConfiguration<CommodityOrder>
    {
        public CommodityOrderConfiguration()
        {
            HasKey(t => t.Id).Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.RowVersion).IsRowVersion();
            HasRequired(t => t.Member).WithMany(s => s.ComodityOrders).HasForeignKey(s => s.MemberId).WillCascadeOnDelete(false);
            HasRequired(t => t.Commodity).WithMany(s=>s.CommodityOrders).HasForeignKey(s=>s.CommodityId).WillCascadeOnDelete(false);
        }
    }
}
