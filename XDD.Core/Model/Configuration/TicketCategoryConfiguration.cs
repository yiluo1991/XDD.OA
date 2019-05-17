using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace XDD.Core.Model.Configuration
{
    public class TicketCategoryConfiguration : EntityTypeConfiguration<TicketCategory>
    {
        public TicketCategoryConfiguration()
       {
           HasKey(t => t.Id).Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
           Property(t => t.Icon).HasMaxLength(128);
           Property(t => t.Name).HasMaxLength(32);
           
       }
    }
}
