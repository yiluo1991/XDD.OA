using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using XDD.Core.DataAccess;
using XDD.Core.Model;

namespace XDD.Web.Controllers
{
    public class AController : Controller
    {
        public static object GetPropertyValue(object obj, string property)
        {
            System.Reflection.PropertyInfo propertyInfo = obj.GetType().GetProperty(property);
            return propertyInfo.GetValue(obj, null);
        }


        public static object GetValue<T>(Expression<Func<T>> func)
        {
            object value = new object();
            var body = func.Body;

            if (body.NodeType == ExpressionType.Constant)
            {
                value = ((ConstantExpression)body).Value;
            }
            else
            {
                var memberExpression = (MemberExpression)body;

                var @object =
                  ((ConstantExpression)(memberExpression.Expression)).Value; //这个是重点

                if (memberExpression.Member.MemberType == MemberTypes.Field)
                {
                    value = ((FieldInfo)memberExpression.Member).GetValue(@object);
                }
                else if (memberExpression.Member.MemberType == MemberTypes.Property)
                {
                    value = ((PropertyInfo)memberExpression.Member).GetValue(@object);
                }
            }
            return value;
        }


        // GET: A
        public ActionResult Index()
        {
            var parameter = Expression.Parameter(typeof(Banner), "s");
            XDDDbContext ctx = new XDDDbContext();

            //创建一个访问属性的表达式
            Expression left = Expression.Property(parameter, typeof(Banner).GetProperty("Id"));
           

            //var orderByExp = Expression.Lambda(propertyAccess, parameter);

            System.Reflection.PropertyInfo propertyInfo = typeof(Banner).GetProperty("Id");
            return Json(ctx.Banners.Select(s => GetValue(z)).ToList(), JsonRequestBehavior.AllowGet);
        }

        private object sss(Banner arg)
        {
            return new { Id = GetPropertyValue(arg, "Id") };
        }
    }
}