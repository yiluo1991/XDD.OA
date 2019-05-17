using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace XDD.Core.Model.Configuration
{
    public class EmployeeConfiguration : EntityTypeConfiguration<Employee>
    {
        public EmployeeConfiguration()
        {
            HasKey(t => t.Id).Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.LoginName).IsRequired().HasMaxLength(128);
            HasIndex(t => t.LoginName).IsClustered(false).IsUnique(true);
            Property(t => t.Password).IsRequired().HasMaxLength(128);
            HasOptional(t => t.Creator).WithMany().HasForeignKey(s => s.CreatorId).WillCascadeOnDelete(false);
        }
    }
}
