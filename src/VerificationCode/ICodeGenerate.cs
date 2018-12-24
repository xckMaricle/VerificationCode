using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VerificationCode
{
    public interface ICodeGenerate
    {
        List<string> GetCode(int number);
    }

    public class DefaultCodeGenerate : ICodeGenerate
    {
        private VerificationCodeOption option;
        public DefaultCodeGenerate(VerificationCodeOption _option)
        {
            option = _option;
        }
        public List<string> GetCode(int number)
        {
            List<string> keys = option?.Keys;
            Random rand = new Random();
            var result = new List<string>();
            if (keys?.Any() == true && number > 0)
            {
                if (keys.Count < number)
                {
                    throw new Exception("验证字典数量不能少于需验证的字数");
                }
                var indexList = new List<int>();
                int randomCount = 0;
                int count = keys.Count;
                while (randomCount < number)
                {
                    int seed = rand.Next(count);
                    var index = rand.Next(seed, count);
                    if (!indexList.Contains(index))
                    {
                        var code = keys[index];
                        result.Add(code);
                        indexList.Add(index);
                        randomCount++;
                    }
                }
            }
            return result;
        }
    }
}
