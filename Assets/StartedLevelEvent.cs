using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets
{
    public class StartedLevelEvent : Unity.Services.Analytics.Event
    {
        public StartedLevelEvent() : base("startedLevel")
        {
        }

        public int Level { set { SetParameter("Level", value); } }
    }
}
