using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace XDD.Core.Model.Configuration
{
    public class MessageConfiguration : EntityTypeConfiguration<Message>
    {
        public MessageConfiguration()
       {
           HasKey(t => t.Id).Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
           Property(t => t.Content).HasMaxLength(512);
           HasRequired(t => t.To).WithMany(t => t.ReciveMessages).HasForeignKey(t => t.ToId).WillCascadeOnDelete(false);
           HasOptional(t => t.From).WithMany(t => t.SendMessages).HasForeignKey(t => t.FromId).WillCascadeOnDelete(false);
       }
    }
}
