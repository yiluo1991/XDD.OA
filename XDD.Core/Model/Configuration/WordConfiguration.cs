using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;


namespace XDD.Core.Model.Configuration
{
   public class WordConfiguration:EntityTypeConfiguration<Word>
    {
       public WordConfiguration()
       {
           HasKey(t => t.Id).Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
           Property(t => t.Content).HasMaxLength(512);
           HasIndex(t => t.Content).IsClustered(false).IsUnique(false);
           Property(t => t.Tags).HasMaxLength(128);
           HasIndex(t => t.Tags).IsClustered(false).IsUnique(false);
           HasRequired(t => t.Member).WithMany(t => t.Words).HasForeignKey(t => t.MemberId).WillCascadeOnDelete(false);
           HasOptional(t => t.Reffer).WithMany(t => t.Links).HasForeignKey(t => t.RefferId).WillCascadeOnDelete(false);
       }
    }
}
