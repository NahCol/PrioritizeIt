using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Prioritize.Data
{
    [Table("v_deq_emp")]
    public class VDEQEmp
    {
        [Column("EXT_EMAIL")]
        public string Email { get; set; }
        public string GoBy { get; set; }
        [Column("USR_ADSUSER")]
        public string UserName { get; set; }

        [NotMapped]
        public string GoByReversed {
            get
            {
                var NameArray = GoBy.Split(',');
                return $"{NameArray[1]} {NameArray[0]}";
            }
        }

    }
}
