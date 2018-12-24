using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace VerificationCode
{
    public static class VerificationCodeMiddlewareExtensions
    {
        public static IApplicationBuilder UseVerificationCode(this IApplicationBuilder builder, Action<VerificationCodeOption> optionSet)
        {
            if (optionSet == null)
            {
                throw new Exception("VerificationCodeOption 不能为空");
            }
            var option = new VerificationCodeOption();
            optionSet(option);
            VerificationCodeFactory.InitOption(option);
            return builder.UseMiddleware<VerificationCodeMiddleware>();
        }
    }
}
