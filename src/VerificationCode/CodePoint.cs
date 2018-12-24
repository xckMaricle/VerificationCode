using System;
using System.Collections.Generic;
using System.Text;

namespace VerificationCode
{
    public class CodePoint
    {
        public int X { get; set; }

        public int Y { get; set; }

        public string Word { get; set; }

        public bool Compare(CodePoint point)
        {
            int _x = Math.Abs(this.X - point.X);
            int _y = Math.Abs(this.Y - point.Y);
            if (_x > 25 || _y > 25)
            {
                return false;
            }
            return true;
        }

    }
}
