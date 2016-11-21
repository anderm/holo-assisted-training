using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code.Models
{
    [Serializable]
    public class User
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public uint LastScore { get; set; }
        public uint LastAttemptTime { get; set; }
        public Guid UserId { get; set; }
    }
}
