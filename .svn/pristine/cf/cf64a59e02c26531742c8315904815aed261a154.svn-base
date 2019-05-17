using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace XDD.Core.Model.Configuration
{
    public class TicketPackageConfiguration : EntityTypeConfiguration<TicketPackage>
    {
        public TicketPackageConfiguration()
       {
           HasKey(t => t.Id).Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        
           Property(t => t.Name).HasMaxLength(128);
           Property(t => t.RowVersion).IsRowVersion();

           HasRequired(t => t.LastUpdator).WithMany(t => t.TicketPackages).HasForeignKey(t => t.LastUpdateId).WillCascadeOnDelete(false);
           HasRequired(t => t.Ticket).WithMany(t => t.TicketPackages).HasForeignKey(t => t.TicketId).WillCascadeOnDelete(false);
           HasRequired(t => t.Supplier).WithMany(t => t.TicketPackages).HasForeignKey(t => t.SupplierId).WillCascadeOnDelete(false);
           
       }
    }
}
