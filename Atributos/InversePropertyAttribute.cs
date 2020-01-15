using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ArqOrm.Model
{
    public class InversePropertyAttribute : Attribute
    {
        public InversePropertyAttribute(string property)
        {
            this.Property = property;
        }
        public string Property { get; set; }
    }
}
