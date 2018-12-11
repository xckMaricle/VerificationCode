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
            if (option.Keys?.Distinct()?.Any() != true)
            {
                throw new Exception("验证字典不能为空");
            }
            option.Keys = option.Keys.Distinct().ToList();
            if (string.IsNullOrEmpty(option.ImagePath))
            {
                throw new Exception("背景图片路径不能为空");
            }
            if (!Directory.Exists(option.ImagePath))
            {
                throw new Exception("背景图片路径无效");
            }
            if (Directory.GetFiles(option.ImagePath)?.Any() != true)
            {
                throw new Exception("背景图片不能为空");
            }
            if (option.DeafaultCheckQty >= option.DeafaultCodeQty)
            {
                throw new Exception("验证字数不能大于要验证的总字数");
            }
            VerificationCodeFactory.InitOption(option);
            return builder.UseMiddleware<VerificationCodeMiddleware>();
        }
    }
}
