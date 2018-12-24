using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace VerificationCode
{
    public interface IVerificationCode
    {
        ICodeModel CreateImage(List<string> keys, VerificationCodeOption option);
    }

    public class DeafultVerificationCode : IVerificationCode
    {
        public ICodeModel CreateImage(List<string> keys, VerificationCodeOption option)
        {
            var imageModel = new DefaultCodeModel();
            if (keys?.Any() == true)
            {
                if (option == null)
                {
                    throw new Exception("请先初始化配置");
                }
                Bitmap Img = null;
                Graphics g = null;
                MemoryStream ms = null;
                Image imageStream = null;
                Random random = new Random();
                try
                {
                    var fileList = Directory.GetFiles(option.ImagePath);
                    int imageCount = fileList.Length;
                    if (imageCount == 0) { throw new Exception("背景图片不能为空"); }
                    int imageRandom = random.Next(1, imageCount);
                    string randomfile = fileList[imageRandom];
                    imageStream = Image.FromFile(randomfile);
                    Img = new Bitmap(imageStream, option.ImageWidth, option.ImageHeight);
                    g = Graphics.FromImage(Img);
                    int codelength = keys.Count;
                    int checkQty = random.Next(option.DeafaultCheckQty, codelength);
                    for (int i = 0; i < codelength; i++)
                    {
                        int cindex = random.Next(option.PenColor.Count);
                        int findex = random.Next(option.FontFamilyName.Count);
                        Font f = new Font(option.FontFamilyName[findex], option.FontSize, FontStyle.Bold);
                        Brush b = new SolidBrush(option.PenColor[cindex]);
                        int y = random.Next(option.ImageHeight);
                        if (y > (option.ImageHeight - 30))
                        {
                            y = y - 60;
                        }

                        int x = option.ImageWidth / (i + 1);
                        if ((option.ImageWidth - x) < 50)
                        {
                            x = option.ImageWidth - 60;
                        }
                        string word = keys[i];
                        if (imageModel.CodeList.Count < checkQty)
                        {
                            imageModel.CodeList.Add(new CodePoint()
                            {
                                Word = word,
                                X = x,
                                Y = y
                            });
                        }
                        g.DrawString(word, f, b, x, y);
                    }
                    ms = new MemoryStream();
                    Img.Save(ms, ImageFormat.Jpeg);
                    imageModel.ImageBase64String = "data:image/jpg;base64," + Convert.ToBase64String(ms.GetBuffer());
                }
                finally
                {
                    imageStream.Dispose();
                    g.Dispose();
                    Img.Dispose();
                    ms.Dispose();
                }
            }
            return imageModel;
        }

    }
}
