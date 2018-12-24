using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace VerificationCode
{
    public interface IVerificationCodeProvider
    {
        ICodeGenerate CodeGenerate { get; }
        IVerificationCode VerificationCode { get; }

        ICodeModel CreateImage();

        string Encode(ICodeModel code);

        ICodeModel Decode(string code);
    }

    public class DefaultVerificationCodeProvider : IVerificationCodeProvider
    {
        public ICodeGenerate CodeGenerate { get; private set; }

        public IVerificationCode VerificationCode { get; private set; }

        private VerificationCodeOption option;
        public DefaultVerificationCodeProvider(VerificationCodeOption _option)
        {
            option = _option;
            CodeGenerate = new DefaultCodeGenerate(_option);
            VerificationCode = new DeafultVerificationCode();
        }

        public ICodeModel CreateImage()
        {
            var keys = CodeGenerate.GetCode(option.DeafaultCodeQty);
            return VerificationCode.CreateImage(keys, option);
        }

        public string Encode(ICodeModel code)
        {
            var model = code as DefaultCodeModel;
            return model == null ? string.Empty : JsonConvert.SerializeObject(new { model.CodeList });
        }

        public ICodeModel Decode(string code)
        {
            if (string.IsNullOrEmpty(code)) { return null; }
            return JsonConvert.DeserializeObject<DefaultCodeModel>(code);
        }
    }
}
