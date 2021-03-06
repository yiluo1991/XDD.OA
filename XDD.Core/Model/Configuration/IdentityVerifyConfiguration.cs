using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace XDD.Core.Model.Configuration
{
   public class IdentityVerifyConfiguration:EntityTypeConfiguration<IdentityVerify>
    {
       public IdentityVerifyConfiguration()
       {
           HasKey(t => t.Id).Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
           Property(s => s.Feedback).HasMaxLength(256);
           Property(s => s.Department).HasMaxLength(128);
           Property(s => s.Institute).HasMaxLength(128);
           Property(s => s.SchoolSN).HasMaxLength(32);
           Property(s => s.ImagePaths).HasMaxLength(1024);
           Property(s => s.RealName).HasMaxLength(128);
           HasRequired(t => t.Member).WithMany(t => t.IdentityVerifies).HasForeignKey(t => t.MemberId).WillCascadeOnDelete(true);
           HasOptional(t => t.Employee).WithMany(t => t.IdentityVerifies).HasForeignKey(t => t.EmployeeId).WillCascadeOnDelete(false);
       }
    }
}
