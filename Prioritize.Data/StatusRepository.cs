using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Prioritize.Data
{
    public class StatusRepository
    {
        private PrioritizeDatabaseContext _prioritizeDatabaseContext;

        public StatusRepository(PrioritizeDatabaseContext prioritizeDatabaseContext)
        {           
            _prioritizeDatabaseContext = prioritizeDatabaseContext;
        }

        public IEnumerable<LStatus> GetStatuses()
        {
            return _prioritizeDatabaseContext.LStatuses.OrderBy(c => c.Id);
        }

        public List<LStatus> GetStatusesById(List<int> Ids)
        {
            return _prioritizeDatabaseContext.LStatuses.Select(c=> new LStatus() {
                Id = c.Id,
                Text = c.Text
            }).Where(c => Ids.Contains(c.Id)).ToList();
        }

        public LStatus GetStatusById(int Id)
        {
            return _prioritizeDatabaseContext.LStatuses.Select(c => new LStatus()
            {
                Id = c.Id,
                Text = c.Text
            }).Single(c => c.Id == Id);
        }


        public LStatus GetStatusByText(string text)
        {
            return _prioritizeDatabaseContext.LStatuses.SingleOrDefault(c => c.Text.Replace(" ","").ToLower() == text.Replace(" ", "").ToLower());
        }
    }
}
