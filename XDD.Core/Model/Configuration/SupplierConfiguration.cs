using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace XDD.Core.Model.Configuration
{
    public class SupplierConfiguration : EntityTypeConfiguration<Supplier>
    {
       public SupplierConfiguration()
       {
           HasKey(t => t.Id).Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
           Property(t => t.Description).HasMaxLength(256);
           HasOptional(t => t.Member).WithMany(s=>s.Supplier).HasForeignKey(s=>s.MemberId).WillCascadeOnDelete(false);
           HasRequired(t => t.Employee).WithMany(s => s.Suppliers).WillCascadeOnDelete(false);
       }
    }
}
