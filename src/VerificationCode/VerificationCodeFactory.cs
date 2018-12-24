using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace VerificationCode
{
    public class VerificationCodeFactory
    {
        public static VerificationCodeOption Option { get; private set; }
        public static void InitOption(VerificationCodeOption _option)
        {
            if (_option != null)
            {
                lock (typeof(VerificationCodeFactory))
                {
                    Option = _option;
                }
            }
        }

        public static IVerificationCodeProvider GetProvider(Func<VerificationCodeOption, VerificationCodeOption> getOption = null)
        {
            var op = getOption == null ? Option : getOption(Option.Clone());
            return new DefaultVerificationCodeProvider(op);
        }
    }
}
