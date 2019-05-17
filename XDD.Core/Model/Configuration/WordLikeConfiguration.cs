using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;


namespace XDD.Core.Model.Configuration
{
    public class WordLikeConfiguration : EntityTypeConfiguration<WordLike>
    {
       public WordLikeConfiguration()
       {
           HasKey(t => t.Id).Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
           HasRequired(t => t.Word).WithMany(t => t.Likes).HasForeignKey(t => t.WordId).WillCascadeOnDelete(true);
           HasRequired(t => t.Member).WithMany(t => t.MyLikes).HasForeignKey(t => t.MemberId).WillCascadeOnDelete(true);
       }
    }
}
