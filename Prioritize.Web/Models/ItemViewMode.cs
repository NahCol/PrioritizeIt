using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Prioritize.Web.Models
{
    public class ItemViewModel
    {
        public bool Active { get; set; }
        public string CardNumber { get; set; }
        public int Id { get; set; }
        public int PriorityNumber { get; set; }
        public int? CoderId { get; set; }
        public string Link { get; set; }
        public int StatusId { get; set; }
        public string Description { get; set; }
        public string Action { get; set; }
        public int? PriorityLevelId { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
