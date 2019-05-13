using Prioritize.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prioritize.Service
{
    public class UserService
    {
        private readonly UserRepository _userRepository;
        public UserService(UserRepository userRepository) {
            _userRepository = userRepository;
        }

        public List<LCoder> GetCoders()
        {
            return _userRepository.GetCoders().ToList();
        }

        public LCoder GetCoderByUserName(string userName)
        {
            return _userRepository.GetCoderByUserName(userName);
        }
        public bool CanAssign(string userName) {
            return GetCoders().Any(c => c.UserName == userName);
        }

        public VDEQEmp GetEmployeeByUserName(string userName)
        {
            return _userRepository.GetEmployeeByUserName(userName);
        }

    }
}
