using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Prioritize.Data
{
    public partial class LCoder
    {

        [NotMapped]
        public string DisplayName { get; set; }

        [NotMapped]
        public string Email { get; set; }

        [NotMapped]
        public string DisplayNameReversed
        {
            get
            {
                var NameArray = DisplayName.Split(',');
                return $"{NameArray[1]} {NameArray[0]}";
            }
        }
    }
}
