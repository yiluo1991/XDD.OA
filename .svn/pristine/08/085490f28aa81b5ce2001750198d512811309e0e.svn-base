using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace XDD.Core.Model.Configuration
{
    public class AgentApplyConfiguration : EntityTypeConfiguration<AgentApply>
    {
       public AgentApplyConfiguration()
       {
           HasKey(t => t.Id).Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
           Property(t => t.IdCard).HasMaxLength(18);
           Property(s => s.Feedback).HasMaxLength(256);
           Property(s => s.Paths).HasMaxLength(1024);
           Property(s => s.RealName).HasMaxLength(128);
           HasRequired(t => t.Member).WithMany(t => t.AgentApplies).HasForeignKey(t => t.MemberId).WillCascadeOnDelete(true);
           HasOptional(t => t.Employee).WithMany(t => t.AgentApplies).HasForeignKey(t => t.EmployeeId).WillCascadeOnDelete(false);
       }
    }
}
