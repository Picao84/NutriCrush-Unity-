using SQLite4Unity3d;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets
{
    public class UserSettings
    {
        [PrimaryKey]
        public int Id { get; set; }

        public int SoundLevel { get; set; }

        public int MusicLevel { get; set; }

    }
}
