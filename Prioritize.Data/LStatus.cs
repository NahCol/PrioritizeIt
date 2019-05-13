using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prioritize.Data
{
    [Table("l_status")]
    public partial class LStatus
    {
        public LStatus()
        {
            DItems = new HashSet<DItem>();
        }

        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Text { get; set; }

        [InverseProperty("Status")]
        public virtual ICollection<DItem> DItems { get; set; }
    }
}