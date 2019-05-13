using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prioritize.Data
{
    [Table("l_coder")]
    public partial class LCoder
    {
        public LCoder()
        {
            DItems = new HashSet<DItem>();
        }

        public int Id { get; set; }


        [Required]
        [StringLength(50)]
        public string UserName { get; set; }

        [InverseProperty("Coder")]
        public virtual ICollection<DItem> DItems { get; set; }
    }
}