using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prioritize.Data
{
    [Table("l_priority_level")]
    public partial class LPriorityLevel
    {
        public LPriorityLevel()
        {
            DItems = new HashSet<DItem>();
        }

        public int Id { get; set; }
        public int SortOrder { get; set; }
        [Required]
        [StringLength(50)]
        public string Text { get; set; }
        public bool Active { get; set; }

        [InverseProperty("PriorityLevel")]
        public virtual ICollection<DItem> DItems { get; set; }
    }
}