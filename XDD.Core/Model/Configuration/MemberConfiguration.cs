using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;


namespace XDD.Core.Model.Configuration
{
   public class MemberConfiguration:EntityTypeConfiguration<Member>
    {
       public MemberConfiguration()
       {
           HasKey(t => t.Id).Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
           Property(t => t.AppOpenId).HasMaxLength(32);
           HasIndex(t => t.AppOpenId).IsClustered(false).IsUnique(false);
           Property(t => t.WebOpenId).HasMaxLength(32);
           HasIndex(t => t.WebOpenId).IsClustered(false).IsUnique(false);
           Property(t => t.UnionId).HasMaxLength(32);
           HasIndex(t => t.UnionId).IsClustered(false).IsUnique(false);
           Property(t => t.NickName).HasMaxLength(64);
           HasIndex(t => t.NickName).IsClustered(false).IsUnique(false);
           Property(t => t.RealName).HasMaxLength(64);
           HasIndex(t => t.RealName).IsClustered(false).IsUnique(false);
           Property(t => t.PlatformBindPhone).HasMaxLength(24);
           HasIndex(t => t.PlatformBindPhone).IsClustered(false).IsUnique(false);
           Property(t => t.WeChatBindPhone).HasMaxLength(24);
           HasIndex(t => t.WeChatBindPhone).IsClustered(false).IsUnique(false);
           Property(t => t.Account).HasPrecision(28, 2);
           HasOptional(t => t.Captain).WithMany(t => t.TeamMembers).HasForeignKey(s => s.CaptainId).WillCascadeOnDelete(false);
           Property(p => p.RowVersion).IsRowVersion();

       }
    }
}
