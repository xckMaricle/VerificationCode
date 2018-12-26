using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VerificationCode
{
    public interface ICodeModel
    {
        string ImageBase64String { get; set; }

        bool Compare(ICodeModel codeModel);

        List<string> GetWord();
    }

    public class DefaultCodeModel : ICodeModel
    {
        public string ImageBase64String { get; set; }
        public List<CodePoint> CodeList { get; set; } = new List<CodePoint>();

        public bool Compare(ICodeModel codeModel)
        {
            var model = codeModel as DefaultCodeModel;
            if (model?.CodeList?.Any() == true && model.CodeList.Count == CodeList.Count)
            {
                int count = CodeList.Count;
                for (int i = 0; i < count; i++)
                {
                    if (CodeList[i].Compare(model.CodeList[i]) == false)
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
            return CodeList.Select(s => s.Word).ToList();
        }
    }
}
