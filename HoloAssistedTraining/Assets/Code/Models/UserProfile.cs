using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity3dAzure.AppServices;

namespace Assets.Code.Models
{
    [Serializable]
    public class UserProfile : DataModel
    {
        public string first;
        public string last;

        public string GetFullName()
        {
            return string.Format("{0} {1}", first, last);
        }
    }
}
