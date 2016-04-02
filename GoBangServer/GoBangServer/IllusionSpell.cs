using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GoBangServer
{
    class IllusionSpell
    {
        public IllusionSpell(int side)
        {
            r = new Random();
            this.side = side;
            illusionSpellTimer = 0;
        }

        public void setIllusionSpellTimer() //
        {
            this.illusionSpellTimer = 2 * (r.Next(3)) + 2;
            
        }

        public void IllusionSpellTimeCollapse()
        {
            illusionSpellTimer--;
        }

        public int illusionSpellTimer { get; set; }
        public int preIllusionState { get; set; }
        public int side { get; set; }
        public Random r;
        public int illusionSpellX { get; set; }
        public int illusionSpellY { get; set; }
        public bool illusionSpellEnable { get; set; }
    }
}
