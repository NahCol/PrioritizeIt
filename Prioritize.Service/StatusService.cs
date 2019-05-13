using Prioritize.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prioritize.Service
{
    public class StatusService
    {
        StatusRepository _statusRepository;
        public StatusService(StatusRepository statusRepository)
        {
            _statusRepository = statusRepository;
        }

        public List<LStatus> GetStatuses()
        {
            return _statusRepository.GetStatuses().ToList();
        }
        public List<LStatus> GetStatusesById(List<int> Id)
        {
            return _statusRepository.GetStatusesById(Id);
        }

        public LStatus GetStatusById(int Id)
        {
            return _statusRepository.GetStatusById(Id);
        }

        public LStatus GetStatusByText(string text)
        {
            return _statusRepository.GetStatusByText(text);
        }
    }
}
