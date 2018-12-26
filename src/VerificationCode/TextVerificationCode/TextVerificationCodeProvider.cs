using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace VerificationCode
{
    public class TextVerificationCodeProvider : IVerificationCodeProvider
    {
        private VerificationCodeOption option;
        public TextVerificationCodeProvider(VerificationCodeOption _option)
        {
            option = _option;
            CodeGenerate = new DefaultCodeGenerate(_option);
            VerificationCode = new TextVerificationCode();
        }
        public ICodeGenerate CodeGenerate { get; private set; }

        public IVerificationCode VerificationCode { get; private set; }

        public bool CanHandle(object objectType)
        {
            return objectType?.ToString().ToLower() == "text";
        }

        public ICodeModel CreateImage()
        {
            var keys = CodeGenerate.GetCode(option.DeafaultCodeQty);
            return VerificationCode.CreateImage(keys, option);
        }

        public string Encode(ICodeModel code)
        {
            var model = code as TextCodeModel;
            return model == null ? string.Empty : JsonConvert.SerializeObject(new { model.Wrods });
        }

        public ICodeModel Decode(string code)
        {
            if (string.IsNullOrEmpty(code)) { return null; }
            return JsonConvert.DeserializeObject<TextCodeModel>(code);
        }
    }
}
