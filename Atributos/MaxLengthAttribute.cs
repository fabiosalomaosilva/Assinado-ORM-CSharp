using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ArqOrm.Model
{
    public class MaxLengthAttribute : ValidationAttribute
    {
        public MaxLengthAttribute()
        {
            this.Length = 255;
        }

        public MaxLengthAttribute(int length)
        {
            this.Length = length;
        }

        public int Length { get; set; }

        public override string FormatErrorMessage(string name)
        {
            return base.FormatErrorMessage(name);
        }
    }
}
