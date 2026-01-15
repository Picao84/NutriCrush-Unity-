using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets
{
    public class GameOverEvent : Unity.Services.Analytics.Event
    {
        public GameOverEvent() : base("gameOver")
        {
        }

        public int Level { set { SetParameter("Level", value); } }

        public string Reason { set { SetParameter("reason", value); } }
    }
}
