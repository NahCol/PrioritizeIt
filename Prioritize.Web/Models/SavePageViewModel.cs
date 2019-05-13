using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Prioritize.Web.Models
{
    public class SavePageViewModel
    {
        public string View { get; set; }
        public List<int> Status { get; set; }
        public IEnumerable<ItemViewModel> Items {get;set;}


    }
}
