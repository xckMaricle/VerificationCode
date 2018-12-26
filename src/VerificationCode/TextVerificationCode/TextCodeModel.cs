using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VerificationCode
{
    public class TextCodeModel : ICodeModel
    {
        public string ImageBase64String { get; set; }

        public List<string> Wrods { get; set; }

        public bool Compare(ICodeModel codeModel)
        {
            var model = codeModel as TextCodeModel;
            if (model?.Wrods?.Any() == true && model.Wrods.Count == Wrods.Count)
            {
                int count = Wrods.Count;
                for (int i = 0; i < count; i++)
                {
                    if (!string.Equals(Wrods[i], model.Wrods[i], StringComparison.OrdinalIgnoreCase))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public List<string> GetWord()
        {
            return Wrods;
        }
    }
}
