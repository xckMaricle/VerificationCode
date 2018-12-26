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
        private static Dictionary<Type, Func<VerificationCodeOption, IVerificationCodeProvider>> providerList = new Dictionary<Type, Func<VerificationCodeOption, IVerificationCodeProvider>>
        {
           { typeof(DefaultVerificationCodeProvider) , (op) =>  new DefaultVerificationCodeProvider(op) },
           { typeof(TextVerificationCodeProvider) , (op) =>  new TextVerificationCodeProvider(op) }
        };
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

        public static IVerificationCodeProvider GetProvider(Action<VerificationCodeOption> getOption = null)
        {
            var op = Option.Clone();
            getOption?.Invoke(op);
            return FilterProvider(null, op);
        }

        public static IVerificationCodeProvider GetProvider(object objectType, Action<VerificationCodeOption> getOption = null)
        {
            var op = Option.Clone();
            getOption?.Invoke(op);
            return FilterProvider(objectType, op);
        }

        public static bool AddProvider<T>(Func<VerificationCodeOption, T> provider) where T : IVerificationCodeProvider
        {
            return providerList.TryAdd(typeof(T), (op) => { return provider(op); });
        }

        private static IVerificationCodeProvider FilterProvider(object objectType, VerificationCodeOption option)
        {
            int len = providerList.Count;
            var list = providerList.Values.ToList(); ;
            for (int i = len - 1; i >= 0; i--)
            {
                var provider = list[i](option);
                if (provider.CanHandle(objectType))
                {
                    return provider;
                }
            }
            return new DefaultVerificationCodeProvider(option);
        }
    }
}
