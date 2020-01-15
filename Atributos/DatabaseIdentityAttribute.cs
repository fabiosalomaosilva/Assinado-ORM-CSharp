using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ArqOrm.Model
{
    public class DatabaseIdentityAttribute : Attribute
    {
        public DatabaseIdentityAttribute()
        {
            this.IsActive = true;
        }

        public DatabaseIdentityAttribute(bool isActive)
        {
            this.IsActive = isActive;
        }

        public bool IsActive { get; set; }
    }
}
