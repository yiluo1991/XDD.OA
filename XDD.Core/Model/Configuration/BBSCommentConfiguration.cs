using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace XDD.Core.Model.Configuration
{
    public class BBSCommentConfiguration : EntityTypeConfiguration<BBSComment>
    {
        public BBSCommentConfiguration()
        {
            HasKey(t => t.Id).Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.Comment).HasMaxLength(1024 * 256);
            Property(t => t.Paths).HasMaxLength(1024);
            HasRequired(t => t.Member).WithMany(t => t.BBSComments).HasForeignKey(t => t.MemberId).WillCascadeOnDelete(false);
            HasRequired(t => t.BBSArticle).WithMany(t => t.BBSComments).HasForeignKey(t => t.BBSArticleId).WillCascadeOnDelete(true);
            HasOptional(t => t.Reffer).WithMany(t => t.Links).HasForeignKey(t => t.RefferId).WillCascadeOnDelete(false);
        }
    }
}
