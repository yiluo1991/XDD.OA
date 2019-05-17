using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace XDD.Core.Model.Configuration
{
    public class AccountStatementConfiguration : EntityTypeConfiguration<AccountStatement>
    {
       public AccountStatementConfiguration()
       {
           HasKey(t => t.Id).Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
           Property(t => t.BeforeBalance).HasPrecision(28, 3);
           Property(t => t.Money).HasPrecision(28, 3);
           Property(t => t.Type).HasMaxLength(64);
           HasIndex(s => s.Type).IsClustered(false).IsUnique(false);
           HasRequired(s => s.Member).WithMany(s => s.Statements).HasForeignKey(s => s.MemberId).WillCascadeOnDelete(false);
       }
    }
}
