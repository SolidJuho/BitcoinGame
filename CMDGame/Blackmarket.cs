using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMDGame
{
    static class Blackmarket
    {
        //store prices, upgrades, does handling of payments
        public static void init()
        {
            //Load data from XML
        }

        public static bool Pay(int _price)
        {
            if(PlayerStats.PlayerCash >= _price)
            {
                PlayerStats.PlayerCash -= _price;
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public class CPU_Upgrades
    {
        public int CPULevel;
        public int price;
    }

}
