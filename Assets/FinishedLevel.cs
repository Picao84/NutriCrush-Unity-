using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets
{
    public class FinishedLevelEvent : Unity.Services.Analytics.Event
    {
        public FinishedLevelEvent() : base("finishedLevel")
        {
        }

        public int Level { set { SetParameter("Level", value); } }

        public string Grade { set { SetParameter("Grade", value); } }
    }
}
