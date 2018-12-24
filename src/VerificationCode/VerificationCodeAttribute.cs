using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerificationCode
{
    public class VerificationCodeAttribute : Attribute, IAsyncActionFilter
    {
        private string Field { get; set; }
        public VerificationCodeAttribute(string CodeField)
        {
            Field = CodeField;
        }
        public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            try
            {
                var result = new List<CodePoint>();
                var valueProvider = CompositeValueProvider.CreateAsync(((ControllerBase)context.Controller).ControllerContext).Result;
                var code = valueProvider.GetValue(Field).ToString();
                if (string.IsNullOrEmpty(code))
                {
                    context.ModelState.TryAddModelError(Field, "验证码为空");
                }
                else
                {
                    var provider = VerificationCodeFactory.GetProvider();
                    var pointList = provider.Decode(code);
                    var checkCode = provider.Decode(context.HttpContext.Session.GetString(VerificationCodeFactory.Option.SessionKey) ?? String.Empty);
                    if (checkCode?.Compare(pointList) != true)
                    {
                        context.ModelState.TryAddModelError(Field, "验证失败");
                    }
                }
            }
            catch
            {

            }

            return next();
        }
    }
}
