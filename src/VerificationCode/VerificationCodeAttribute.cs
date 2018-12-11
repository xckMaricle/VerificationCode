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
                var pointList = GetRequestCode(context);
                if (pointList?.Any() != true)
                {
                    context.ModelState.TryAddModelError(Field, "验证码为空");
                }
                else
                {
                    var checkCode = JsonConvert.DeserializeObject<List<CodePoint>>(context.HttpContext.Session.GetString(VerificationCodeFactory.Option.SessionKey));
                    if (pointList.Any() && checkCode.Any() && pointList.Count == checkCode.Count)
                    {
                        var list = checkCode.OrderBy(o => o.Sort).ToList();
                        for (int i = 0; i < list.Count; i++)
                        {
                            if (!list[i].Compare(pointList[i]))
                            {
                                context.ModelState.TryAddModelError(Field, "验证失败");
                                break;
                            }
                        }
                    }
                }
            }
            catch
            {

            }

            return next();
        }

        private List<CodePoint> GetRequestCode(ActionExecutingContext context)
        {
            var result = new List<CodePoint>();
            var valueProvider = CompositeValueProvider.CreateAsync(((ControllerBase)context.Controller).ControllerContext).Result;
            if (valueProvider.ContainsPrefix(Field))
            {
                var codeValues = valueProvider.GetKeysFromPrefix(Field);
                foreach (var item in codeValues)
                {
                    var xStr = valueProvider.GetValue(item.Value + ".X");
                    var yStr = valueProvider.GetValue(item.Value + ".Y");
                    int x = 0; int y = 0;
                    if (int.TryParse(xStr.FirstValue, out x) && int.TryParse(yStr.FirstValue, out y))
                    {
                        result.Add(new CodePoint { X = x, Y = y });
                    }
                }
            }
            return result;
        }
    }
}
