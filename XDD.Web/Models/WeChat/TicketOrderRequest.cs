using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XDD.Web.Models.WeChat
{
    public class TicketOrderRequest
    {
        public int Id { get; set; }

        public int Quantity { get; set; }

        public string Name { get; set; }

        public string Mobile { get; set; }

        public int? RefferId { get; set; }

        public DateTime UseDate { get; set; }
        public string Address { get; set; }
        public string PackageCode { get; set; }
    }
}