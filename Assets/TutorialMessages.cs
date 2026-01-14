using SQLite4Unity3d;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets
{
    public class TutorialMessages
    {
        [PrimaryKey]
        public int Id { get; set; }

        public string Title { get; set; }

        public int Showed {  get; set; }
    }
}
