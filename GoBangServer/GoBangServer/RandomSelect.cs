using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace GoBangServer
{
    class RandomSelect
    {
        List<int> list = new List<int>();

        public RandomSelect(List<int> lists)
        {
            this.list = lists;
        }

        public int getRandomSelectNumber()
        { 
            Random r = new Random();
            int index = r.Next(list.Count);       //
            Thread.Sleep(50); //let it cease for a while so the result will not concatanate
            return list[index];
        }

    }
}
