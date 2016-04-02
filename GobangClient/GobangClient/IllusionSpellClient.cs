using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GobangClient
{
    class IllusionSpellClient
    {
        public IllusionSpellClient(int side)
        {
            this.side = side;
            this.IllusionSpellTimer = 0;
        }

        public int IllusionSpellX{ get; set; }
        public int IllusionSpellY { get; set; }
        public int IllusionSpellTimer { get; set; }
        public int side { get; set; }
    }
}
