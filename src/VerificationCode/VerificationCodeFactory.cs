using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace VerificationCode
{
    internal class VerificationCodeFactory
    {
        internal static VerificationCodeOption Option { get; private set; }
        internal static void InitOption(VerificationCodeOption _option)
        {
            Option = _option;
        }
        /// <summary>
        ///  Random Code
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        private static List<string> GetCode(List<string> keys, int number)
        {
            Random rand = new Random();
            var result = new List<string>();
            if (keys?.Any() == true && number > 0)
            {
                if (keys.Count < number)
                {
                    throw new Exception("验证字典数量不能少于需验证的字数");
                }
                var list = keys.ToList();
                bool randomBool = true;
                int count = keys.Count;
                while (randomBool)
                {
                    int seed = rand.Next(count);
                    var index = rand.Next(seed, count);
                    var code = list[index];
                    list.Remove(code);
                    count--;
                    result.Add(code);
                    if (result.Count >= number) { randomBool = false; }
                }
            }
            return result;
        }


        /// <summary>
        /// CreateVerificationImage
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        internal static VerificationCodeModel CreateVerificationImage()
        {
            var imageModel = new VerificationCodeModel();
            var code = GetCode(Option.Keys, Option.DeafaultCodeQty);
            Bitmap Img = null;
            Graphics g = null;
            MemoryStream ms = null;
            Image imageStream = null;
            Random random = new Random();
            try
            {
                var fileList = Directory.GetFiles(Option.ImagePath);
                int imageCount = fileList.Length;
                if (imageCount == 0) { throw new Exception("背景图片不能为空"); }
                int imageRandom = random.Next(1, (imageCount + 1));
                string randomfile = fileList[imageRandom - 1];
                imageStream = Image.FromFile(randomfile);
                Img = new Bitmap(imageStream, Option.ImageWidth, Option.ImageHeight);
                g = Graphics.FromImage(Img);
                int codelength = code.Count;
                int checkQty = random.Next(Option.DeafaultCheckQty, codelength);
                for (int i = 0; i < codelength; i++)
                {
                    int cindex = random.Next(Option.PenColor.Count);
                    int findex = random.Next(Option.FontFamilyName.Count);
                    Font f = new Font(Option.FontFamilyName[findex], Option.FontSize, FontStyle.Bold);
                    Brush b = new SolidBrush(Option.PenColor[cindex]);
                    int y = random.Next(Option.ImageHeight);
                    if (y > (Option.ImageHeight - 30))
                    {
                        y = y - 60;
                    }

                    int x = Option.ImageWidth / (i + 1);
                    if ((Option.ImageWidth - x) < 50)
                    {
                        x = Option.ImageWidth - 60;
                    }
                    string word = code[i];
                    if (imageModel.Point.Count < checkQty)
                    {
                        imageModel.Point.Add(new CodePoint()
                        {
                            Word = word,
                            X = x,
                            Y = y,
                            Sort = i
                        });
                    }
                    g.DrawString(word, f, b, x, y);
                }
                ms = new MemoryStream();
                Img.Save(ms, ImageFormat.Jpeg);
                imageModel.ImageBase64Str = "data:image/jpg;base64," + Convert.ToBase64String(ms.GetBuffer());
            }
            finally
            {
                imageStream.Dispose();
                g.Dispose();
                Img.Dispose();
                ms.Dispose();
            }
            return imageModel;
        }
    }
}
