using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using XDD.Core.DataAccess;
using XDD.Core.Model;


namespace XDD.Web.Infrastructure
{

    public class TokenAuthorizeAttribute : System.Web.Http.AuthorizeAttribute
    {


        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {

            try
            {
                // base.OnAuthorization(actionContext);
                var token = actionContext.Request.Headers.GetValues("token").FirstOrDefault();
                if (token != null)
                {
                    var ticket = TicketWoker.GetTicket(token);
                    if (ticket != null && !ticket.Expired)
                    {

                    }
                    else
                    {

                        var response = actionContext.Response = actionContext.Response ?? new HttpResponseMessage();
                        response.StatusCode = HttpStatusCode.Forbidden;
                        response.Content = new StringContent("{\"ResultCode\":-1,\"Message\":\"无权访问\"}", Encoding.UTF8, "application/json");
                    }
                }
                else
                {
                    var response = actionContext.Response = actionContext.Response ?? new HttpResponseMessage();
                    response.StatusCode = HttpStatusCode.Forbidden;
                    response.Content = new StringContent("{\"ResultCode\":-1,\"Message\":\"无权访问\"}", Encoding.UTF8, "application/json");
                }

            }
            catch (Exception)
            {
                var response = actionContext.Response = actionContext.Response ?? new HttpResponseMessage();
                response.Content = new StringContent("{\"ResultCode\":-1,\"Message\":\"无权访问\"}", Encoding.UTF8, "application/json");
                return;
            }
        }


    }

    public class IdentityAuthorizeAttribute : System.Web.Http.AuthorizeAttribute
    {


        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            // base.OnAuthorization(actionContext);

            try
            {
                var token = actionContext.Request.Headers.GetValues("token").FirstOrDefault();
                if (token != null)
                {
                    var ticket = TicketWoker.GetTicket(token);
                    if (ticket != null && !ticket.Expired)
                    {
                        XDDDbContext ctx = new XDDDbContext();
                        var id = Convert.ToInt32(ticket.Name);
                        var member = ctx.Members.FirstOrDefault(s => s.Id == id);
                        if (member != null)
                        {
                            if (member.Status.HasFlag(MemberStatus.Identity))
                            {
                                if (!member.Status.HasFlag(MemberStatus.Freeze))
                                {
                                    return;
                                }
                                else
                                {
                                    var response = actionContext.Response = actionContext.Response ?? new HttpResponseMessage();
                                    response.StatusCode = HttpStatusCode.Forbidden;
                                    response.Content = new StringContent("{\"ResultCode\":-1,\"Message\":\"账户已被禁用\"}", Encoding.UTF8, "application/json");
                                }
                            }
                            else
                            {
                                var response = actionContext.Response = actionContext.Response ?? new HttpResponseMessage();
                                response.StatusCode = HttpStatusCode.Forbidden;
                                response.Content = new StringContent("{\"ResultCode\":-1,\"Message\":\"未实名的用户无法使用该功能\"}", Encoding.UTF8, "application/json");
                            }
                        }
                        else
                        {
                            var response = actionContext.Response = actionContext.Response ?? new HttpResponseMessage();
                            response.StatusCode = HttpStatusCode.Forbidden;
                            response.Content = new StringContent("{\"ResultCode\":-1,\"Message\":\"用户不存在\"}", Encoding.UTF8, "application/json");
                        }

                    }
                    else
                    {
                        var response = actionContext.Response = actionContext.Response ?? new HttpResponseMessage();
                        response.StatusCode = HttpStatusCode.Forbidden;
                        response.Content = new StringContent("{\"ResultCode\":-1,\"Message\":\"无权访问\"}", Encoding.UTF8, "application/json");
                    }
                }
                else
                {
                    var response = actionContext.Response = actionContext.Response ?? new HttpResponseMessage();
                    response.StatusCode = HttpStatusCode.Forbidden;
                    response.Content = new StringContent("{\"ResultCode\":-1,\"Message\":\"无权访问\"}", Encoding.UTF8, "application/json");
                }
            }
            catch (Exception)
            {
                var response = actionContext.Response = actionContext.Response ?? new HttpResponseMessage();
                response.Content = new StringContent("{\"ResultCode\":-1,\"Message\":\"无权访问\"}", Encoding.UTF8, "application/json");
                return;
            }




        }



    }
}