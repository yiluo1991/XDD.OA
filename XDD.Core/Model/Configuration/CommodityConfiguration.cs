using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace XDD.Core.Model.Configuration
{
    public class CommodityConfiguration : EntityTypeConfiguration<Commodity>
    {
        public CommodityConfiguration()
        {
            HasKey(t => t.Id).Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.Name).HasMaxLength(256);
            Property(t => t.Content).HasMaxLength(1024 * 512);
            Property(t => t.Paths).HasMaxLength(1024);
            Property(t => t.Cover).HasMaxLength(128);
            Property(t => t.RowVersion).IsRowVersion();
            HasRequired(t => t.Member).WithMany(s => s.Commodities).HasForeignKey(s => s.MemberId).WillCascadeOnDelete(false);
            HasRequired(t => t.CommodityCategory).WithMany(s => s.Commodities).HasForeignKey(s=>s.CommodityCategoryId).WillCascadeOnDelete(false);
       
        }
    }
}
