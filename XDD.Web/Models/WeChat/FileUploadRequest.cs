using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XDD.Web.Models.WeChat
{
    public class FileUploadRequest
    {
        public HttpPostedFileBase File { get; set; }
    }
}