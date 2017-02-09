using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity3dAzure.AppServices;

namespace Assets.Code.Models
{
    [Serializable]
    public class HighScore : DataModel
    {
        public string userId;
        public int score;
        public int timeTook;
    }
}
