using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace XDD.Core.Model.Configuration
{
    public class TicketConfiguration : EntityTypeConfiguration<Ticket>
    {
        public TicketConfiguration()
       {
           HasKey(t => t.Id).Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        
           Property(t => t.Name).HasMaxLength(128);
           Property(t => t.Address).HasMaxLength(256);
           Property(t => t.Content).HasMaxLength(1024 * 512); 
           Property(t => t.Lat).HasPrecision(10, 7);
           Property(t => t.Lng).HasPrecision(10, 7);
           Property(t => t.Pic).HasMaxLength(128);
           Property(t => t.ShopName).HasMaxLength(64);
           Property(t => t.Tags).HasMaxLength(64);
           Property(t => t.MoreUrl).HasMaxLength(256);

           HasRequired(t => t.TicketCategory).WithMany(t => t.Tickets).HasForeignKey(t => t.TicketCategoryId).WillCascadeOnDelete(false);
           HasRequired(t => t.Employee).WithMany(t => t.Tickets).HasForeignKey(t => t.EmployeeId).WillCascadeOnDelete(false);
           
       }
    }
}
