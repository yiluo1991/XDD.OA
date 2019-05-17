using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDD.Web.Infrastructure
{
    public class TagInfo
    {
        public string tagname;
        public string[] attrs;
        public bool hasAttr(string attr)
        {
            attr = attr.ToLower();
            foreach (string str in attrs)
            {
                if (str == attr)
                {
                    return true;
                }
            }
            return false;

        }


    }
}
