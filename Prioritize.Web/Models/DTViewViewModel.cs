using Microsoft.AspNetCore.Mvc.Rendering;
using Prioritize.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Prioritize.Web.Models
{
    public class DTViewViewModel
    {
        public List<SelectListItem> Coders { get; set; }
        public List<SelectListItem> PriorityList { get; set; }
        public List<SelectListItem> Statuses { get; set; }
        public List<DItem> items { get; set; }
        public bool CanAssign { get; set; }
        public string View { get; set; }
        public string ViewText { get; set; }
        public List<int> Status { get; set; }
        public List<string> StatusText { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }

    }
}
