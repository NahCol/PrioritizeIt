using Prioritize.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prioritize.Service
{
    public class PriorityLevelService
    {
        PriorityLevelRepository _priorityLevelRepository;
        public PriorityLevelService(PriorityLevelRepository priorityLevelRepository)
        {
            _priorityLevelRepository = priorityLevelRepository;
        }

        public List<LPriorityLevel> GetLPriorityLevels()
        {
            return _priorityLevelRepository.GetPriorities().ToList();
        }
    }
}
