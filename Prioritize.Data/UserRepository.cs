using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Prioritize.Data
{
    public class UserRepository
    {
        private readonly PrioritizeDatabaseContext _prioritizeDatabaseContext;
        public UserRepository(PrioritizeDatabaseContext prioritizeDatabaseContext)
        {
            _prioritizeDatabaseContext = prioritizeDatabaseContext;
        }
        public LCoder GetCoderByUserName(string userName)
        {
            return _prioritizeDatabaseContext.LCoders.Where(c=> c.UserName == userName ).Join(_prioritizeDatabaseContext.DEQEmps,
                            Coder => Coder.UserName,
                            DEQEmp => DEQEmp.UserName, (Coder, DEQEmp) => new LCoder()
                {
                        Id = Coder.Id,
                        UserName = Coder.UserName,
                        Email = DEQEmp.Email,
                        DisplayName = DEQEmp.GoBy,
                        DItems = Coder.DItems
                }).SingleOrDefault();
        }
        public IEnumerable<LCoder> GetCoders()
        {
            return _prioritizeDatabaseContext.LCoders
                .Join(_prioritizeDatabaseContext.DEQEmps,
                Coder => Coder.UserName,
                DEQEmp => DEQEmp.UserName, (Coder, DEQEmp) => new LCoder()
                {
                    Id = Coder.Id,
                    UserName = Coder.UserName,
                    Email = DEQEmp.Email,
                    DisplayName = DEQEmp.GoBy
                });
        }

        public VDEQEmp GetEmployeeByUserName(string userName)
        {
            return _prioritizeDatabaseContext.DEQEmps.SingleOrDefault(c => c.UserName == userName);
        }



    }
}
