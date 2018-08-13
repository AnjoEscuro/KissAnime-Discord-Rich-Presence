using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KissAnime_Discord_Rich_Presence.Models
{
    class DataString
    {
        /*
         * 0 = Set presence
         * 1 = Clear presence
         */
        public int action;
        public string anime;
        public int number_of_episodes;
        public int current_episode;
    }
}
