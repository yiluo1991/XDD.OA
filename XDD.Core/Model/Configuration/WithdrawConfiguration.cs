using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace XDD.Core.Model.Configuration
{
    public class WithdrawConfiguration : EntityTypeConfiguration<Withdraw>
    {
        public WithdrawConfiguration()
       {
           HasKey(t => t.Id).Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
         
           Property(t => t.Money).HasPrecision(28, 2);
           Property(t => t.OrderSN).HasMaxLength(64);
           HasIndex(s => s.OrderSN).IsClustered(false).IsUnique(false);
           Property(t => t.BankCard).HasMaxLength(32);
           Property(t => t.BankName).HasMaxLength(64);
           Property(t => t.BankOfDeposit).HasMaxLength(64);
           Property(t => t.Remark).HasMaxLength(512);
           HasRequired(s => s.Member).WithMany(s => s.Withdraws).HasForeignKey(s => s.MemberId).WillCascadeOnDelete(false);
           HasOptional(s => s.Employee).WithMany(s => s.Withdraws).HasForeignKey(s => s.EmployeeId).WillCascadeOnDelete(false);
           Property(t => t.RowVersion).IsRowVersion();
       }
    }
}
