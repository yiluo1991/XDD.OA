using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace XDD.Core.Model.Configuration
{
    public class BBSArticleConfiguration : EntityTypeConfiguration<BBSArticle>
    {
        public BBSArticleConfiguration()
        {
            HasKey(t => t.Id).Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.Address).HasMaxLength(256);
            HasIndex(t => t.Address).IsClustered(false).IsUnique(false);
            Property(t => t.Content).HasMaxLength(1024 * 512);
            
            Property(t => t.Paths).HasMaxLength(1024);
            Property(t => t.Subject).HasMaxLength(128);
            HasIndex(t => t.Subject).IsClustered(false).IsUnique(false);
            Property(t => t.Title).HasMaxLength(128);
            HasIndex(t => t.Title).IsClustered(false).IsUnique(false);
            Property(t => t.RowVersion).IsRowVersion();

            HasRequired(t => t.Member).WithMany(t => t.BBSArticles).HasForeignKey(s => s.MemberId).WillCascadeOnDelete(true);
            HasRequired(t => t.BBSCategory).WithMany(t => t.BBSArticles).HasForeignKey(s => s.CategoryId).WillCascadeOnDelete(true);

        }
    }
}
