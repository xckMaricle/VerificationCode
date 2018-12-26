using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace VerificationCode
{
    public class TextVerificationCode : IVerificationCode
    {
        public ICodeModel CreateImage(List<string> keys, VerificationCodeOption option)
        {
            var imageModel = new TextCodeModel { Wrods = keys };
            if (keys?.Any() == true)
            {
                if (option == null)
                {
                    throw new Exception("请先初始化配置");
                }
                Bitmap Img = null;
                Graphics g = null;
                MemoryStream ms = null;
                Random random = new Random();
                try
                {
                    Img = new Bitmap(option.ImageWidth, option.ImageHeight);
                    g = Graphics.FromImage(Img);
                    g.Clear(Color.White);

                    int lineCount = random.Next(3, 6);
                    for (int i = 0; i < lineCount; i++)
                    {
                        var r1 = random.Next(3, 6);
                        Point[] pArr = new Point[r1];

                        var lineWd = Img.Width / r1;
                        for (int j = 0; j < r1; j++)
                        {
                            pArr[j].X = (lineWd * j) + 10;
                            pArr[j].Y = random.Next(Img.Height);
                        }
                        var cindex = random.Next(option.PenColor.Count);
                        var p = new Pen(option.PenColor[cindex], 2.5f);
                        g.DrawCurve(p, pArr, 0.5F);
                    }

                    int codelength = keys.Count;
                    var wd = option.ImageWidth / codelength;
                    for (int i = 0; i < codelength; i++)
                    {
                        int cindex = random.Next(option.PenColor.Count);
                        int findex = random.Next(option.FontFamilyName.Count);
                        Font f = new Font(option.FontFamilyName[findex], option.FontSize, FontStyle.Bold);
                        Brush b = new SolidBrush(option.PenColor[cindex]);
                        int y = random.Next(option.ImageHeight - f.Height);
                        int wx = wd * (i + 1) > (option.ImageWidth - wd) ? (option.ImageWidth - wd) : wd * (i + 1);
                        int x = random.Next(wd * i, wx);
                        string word = keys[i];
                        g.DrawString(word, f, b, x, y);
                    }
                    g.DrawRectangle(new Pen(Color.Black), 0, 0, Img.Width - 1, Img.Height - 1);
                    ms = new MemoryStream();
                    Img.Save(ms, ImageFormat.Jpeg);
                    imageModel.ImageBase64String = "data:image/jpg;base64," + Convert.ToBase64String(ms.GetBuffer());
                }
                finally
                {
                    g.Dispose();
                    Img.Dispose();
                    ms.Dispose();
                }
            }
            return imageModel;
        }
    }
}
