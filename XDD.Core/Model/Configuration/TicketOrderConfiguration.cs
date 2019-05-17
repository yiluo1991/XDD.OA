using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace XDD.Core.Model.Configuration
{
    public class TicketOrderConfiguration : EntityTypeConfiguration<TicketOrder>
    {
        public TicketOrderConfiguration()
       {
           HasKey(t => t.Id).Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
           Property(t => t.Description).HasMaxLength(128);
           Property(t => t.Detail).HasMaxLength(6000);
           Property(t => t.OrderNum).HasMaxLength(32);
           Property(t => t.RealName).HasMaxLength(64);
           Property(t => t.Mobile).HasMaxLength(24);
           Property(t => t.Request).HasMaxLength(128);
           Property(t => t.VCode).HasMaxLength(8);
           HasRequired(t => t.Member).WithMany(s => s.TicketOrders).HasForeignKey(t => t.MemberId).WillCascadeOnDelete(false);
           HasRequired(t => t.Supplier).WithMany(s => s.Orders).HasForeignKey(t => t.SupplierId).WillCascadeOnDelete(false);
           HasRequired(t => t.TicetPackage).WithMany(s => s.Orders).HasForeignKey(s => s.TicketPackageId).WillCascadeOnDelete(false);
           HasOptional(t => t.Agent).WithMany(s => s.AgentOrders).HasForeignKey(s => s.AgentId).WillCascadeOnDelete(false);
           Property(t => t.RowVersion).IsRowVersion();
           
       }
    }
}
