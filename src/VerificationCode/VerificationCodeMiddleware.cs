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
            if (context.Request.Path.HasValue)
            {
                if (context.Request.Path.Value == VerificationCodeFactory.Option.CodeImageUrl)
                {
                    return GetVerificationCodeImage(context);
                }
                if (context.Request.Path.Value == VerificationCodeFactory.Option.CheckCodeUrl)
                {
                    return CheckCode(context);
                }
            }

            // Call the next delegate/middleware in the pipeline
            return this._next(context);
        }

        private Task GetVerificationCodeImage(HttpContext context)
        {
            var provider = VerificationCodeFactory.GetProvider();
            var model = provider.CreateImage();
            string msg = string.Format(VerificationCodeFactory.Option.MessageTipFormat, string.Join(" ", model.GetWord()));
            var content = JsonConvert.SerializeObject(new { Result = model.ImageBase64String, Message = msg, Count = model.GetWord().Count });
            context.Session.SetString(VerificationCodeFactory.Option.SessionKey, provider.Encode(model));
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
                var provider = VerificationCodeFactory.GetProvider();
                var pointList = provider.Decode(code);
                var checkCode = provider.Decode(context.Session.GetString(VerificationCodeFactory.Option.SessionKey) ?? String.Empty);
                if (pointList != null && checkCode != null)
                {
                    bool checkResult = pointList.Compare(checkCode);
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
