using System;
using System.Collections.Generic;
using System.Text;

namespace VerificationCode
{
    public class VerificationCodeModel
    {
        public string ImageBase64Str { get; set; } = "";

        public List<CodePoint> Point { get; set; } = new List<CodePoint>();

    }
}
