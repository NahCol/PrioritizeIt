using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prioritize.Data
{
    [Table("d_item")]
    public partial class DItem
    {
        public int Id { get; set; }
        [StringLength(50)]
        public string Board { get; set; }
        [StringLength(50)]
        public string List { get; set; }
        [StringLength(50)]
        public string Action { get; set; }
        [StringLength(50)]
        public string CardNumber { get; set; }
        public int PriorityNumber { get; set; }
        public string Requirement { get; set; }
        public int? PriorityLevelId { get; set; }
        public int StatusId { get; set; }
        public int? CoderId { get; set; }
        public string Link { get; set; }
        public bool Active { get; set; }
        public string Description { get; set; }
        public DateTime? DueDate { get; set; }

        [ForeignKey("CoderId")]
        [InverseProperty("DItems")]
        public virtual LCoder Coder { get; set; }
        [ForeignKey("PriorityLevelId")]
        [InverseProperty("DItems")]
        public virtual LPriorityLevel PriorityLevel { get; set; }
        [ForeignKey("StatusId")]
        [InverseProperty("DItems")]
        public virtual LStatus Status { get; set; }
    }
}