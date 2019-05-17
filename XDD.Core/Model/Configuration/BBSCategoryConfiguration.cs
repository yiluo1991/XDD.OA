using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace XDD.Core.Model.Configuration
{
    public class BBSCategoryConfiguration : EntityTypeConfiguration<BBSCategory>
    {
        public BBSCategoryConfiguration()
       {
           HasKey(t => t.Id).Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
           Property(t => t.Name).HasMaxLength(128);
           HasIndex(t => t.Name).IsClustered(false).IsUnique(false);
       }
    }
}
