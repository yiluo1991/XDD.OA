using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDD.Core.Model
{
   public  class PermissionGroup
    {
        public int Id { get; set; }
        /// <summary>
        ///     权限组/功能菜单的友好名称。
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        ///     父级权限组Id，顶级为null。
        /// </summary>
        public int? ParentId { get; set; }
        /// <summary>
        ///     权限组/功能菜单的页面路径。
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        ///     功能菜单的HTML标识。
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        ///     头像。
        /// </summary>
        public string Headshot { get; set; }

        /// <summary>
        ///     权限组序号
        /// </summary>
        public int SN { get; set; }

    

        /// <summary>
        ///     导航属性，所属菜单。
        /// </summary>
        public virtual PermissionGroup Parent { get; set; }
     
        /// <summary>
        ///     导航属性，该权限组下的所有子权限组/功能菜单（不包含孙字辈）。
        /// </summary>
        public virtual ICollection<PermissionGroup> Children { get; set; }

    }
}
