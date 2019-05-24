using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMDGame
{
    class BitcoinServer
    {
        public int ID;
        public int CPULevel = 1;
        public float BtcPerSec;
        public float BtcStored;
        public DateTime lastSold;
        public DateTime lastCalculation;

        public void Init()
        {
            lastSold = DateTime.Now;
            lastCalculation = lastSold;
        }

        public float GetBitcoins()
        {
            TimeSpan timeSince = DateTime.Now - lastSold;
            BtcPerSec = CPULevel * 0.05f;
            BtcStored += timeSince.Seconds * BtcPerSec * 0.1f;
            return BtcStored;
        }

        public float BitcoinsPerMinute()
        {
            return CPULevel * 0.05f * 60;
        }

        public float TransferBitcoins()
        {
            float btcs = GetBitcoins();
            BtcStored = 0;
            lastSold = DateTime.Now;
            return btcs;
        }

        /// <summary>
        /// Sells bitcoins and returns cash value of them.
        /// </summary>
        /// <returns>Return cash</returns>
        public float SellBitcoins()
        {
            float CashAmount = GetBitcoins() * Bitcoin.value;
            BtcStored = 0;
            return CashAmount;
        }

        public float lastTranferAgo()
        {
            return (DateTime.Now - lastSold).Seconds;
        }

        public void UpgradeServer()
        {
            CPULevel++;
        }
    }
}
