using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerificationCode
{
    public class VerificationCodeMiddleware
    {
        private readonly RequestDelegate _next;

        public VerificationCodeMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context)
        {
            if (context.Request.Headers["VerificationCode"] == "true" && context.Request.Path.HasValue)
            {
                if (context.Request.Path.Value == "/VerificationCodeImage")
                {
                    return GetVerificationCodeImage(context);
                }
                if (context.Request.Path.Value == "/CheckVerificationCode")
                {
                    return CheckCode(context);
                }
            }

            // Call the next delegate/middleware in the pipeline
            return this._next(context);
        }

        private Task GetVerificationCodeImage(HttpContext context)
        {
            var model = VerificationCodeFactory.CreateVerificationImage();
            string msg = string.Format(VerificationCodeFactory.Option.MessageTipFormat, string.Join(" ", model.Point.Select(x => x.Word).ToList()));
            var content = JsonConvert.SerializeObject(new { Result = model.ImageBase64Str, Message = msg, Count = model.Point.Count });
            context.Session.SetString(VerificationCodeFactory.Option.SessionKey, JsonConvert.SerializeObject(model.Point));
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync(content);
        }

        private Task CheckCode(HttpContext context)
        {
            var code = context.Request.Form["code"].ToString();
            string msg = "验证失败";
            bool status = false;
            try
            {
                var pointList = JsonConvert.DeserializeObject<List<CodePoint>>(code);
                var checkCode = JsonConvert.DeserializeObject<List<CodePoint>>(context.Session.GetString(VerificationCodeFactory.Option.SessionKey) ?? String.Empty);
                if (pointList.Any() && checkCode.Any() && pointList.Count == checkCode.Count)
                {
                    bool checkResult = true;
                    var list = checkCode.OrderBy(o => o.Sort).ToList();
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (!list[i].Compare(pointList[i]))
                        {
                            checkResult = false;
                        }
                    }
                    if (checkResult)
                    {
                        status = true;
                        msg = "验证通过";
                    }
                }
            }
            catch
            {

            }
            var content = JsonConvert.SerializeObject(new { Message = msg, Status = status });
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync(content);
        }

    }
}
