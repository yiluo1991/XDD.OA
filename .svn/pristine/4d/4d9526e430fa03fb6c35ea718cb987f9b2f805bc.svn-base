using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;


namespace XDD.Core.Model.Configuration
{
    public class WordCommentConfiguration : EntityTypeConfiguration<WordComment>
    {
       public WordCommentConfiguration()
       {
           HasKey(t => t.Id).Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
           Property(t => t.Content).HasMaxLength(512);
           HasRequired(t => t.Member).WithMany(t => t.WordComments).HasForeignKey(t => t.MemberId).WillCascadeOnDelete(true);
           HasRequired(t => t.Word).WithMany(t => t.Comments).HasForeignKey(t => t.WordId).WillCascadeOnDelete(true);
       }
    }
}
