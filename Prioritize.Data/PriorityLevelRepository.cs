using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Prioritize.Data
{
    public class PriorityLevelRepository
    {
        private PrioritizeDatabaseContext _prioritizeDatabaseContext;

        public PriorityLevelRepository(PrioritizeDatabaseContext prioritizeDatabaseContext)
        {           
            _prioritizeDatabaseContext = prioritizeDatabaseContext;
        }

        public IEnumerable<LPriorityLevel> GetPriorities()
        {
            return _prioritizeDatabaseContext.LPriorityLevels.OrderBy(c => c.SortOrder);
        }
    }
}
