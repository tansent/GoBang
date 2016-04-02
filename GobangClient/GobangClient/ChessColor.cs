using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GobangClient
{
    static class ChessColor   // ?
    {
        public const int Black = 0;
        public const int White = 1;
        public const int None = -1;
        public const int BlackBoom = 2; //side add 2 equals boom color
        public const int WhiteBoom = 3;
        public const int BlackTrap = 4; //side add 4 equals trap color
        public const int WhiteTrap = 5;
        public const int BlackIllusion = 6; //side add 6 equals illusion color
        public const int WhiteIllusion = 7;
        public const int FrozenBlackChess = 8; //side add 8 equals illusion color
        public const int FrozenWhiteChess = 9; 
    }
}
