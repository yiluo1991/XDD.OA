using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace XDD.Web.Infrastructure
{
    public static class TicketWoker
    {
        public static FormsAuthenticationTicket GetTicket(string ticketString)
        {
            try
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(ticketString);
                return ticket;
            }
            catch (Exception)
            {
                return null;
            }
           
        }
    }
}