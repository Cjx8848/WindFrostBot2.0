using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraWiki
{
    public class CoinValue
    {
        public int Copper;

        public int Gold;

        public int Platinum;

        public int Silver;
        public bool HasValue()
        {
            if(Copper > 0 || Gold > 0 || Platinum > 0 || Silver > 0)
            {
                return true;
            }
            return false;
        }
        public CoinValue(int value)
        {
            Platinum = value / (100 * 100 * 100);
            value %= 100 * 100 * 100;
            Gold = value / (100 * 100);
            value %= 100 * 100;
            Silver = value / 100;
            Copper = value % 100;
        }
    }

    public class NpcInfo
    {
        public int Damage;
        public string Description = "";
        public int LifeMax;
        public CoinValue MonetaryValue = new(0);
        public string Name = "";
        public int NpcId;
        public List<string> Alias = new(); // Add this
    }

    public class ItemInfo
    {
        public int Damage;
        public int DamageType;
        public int Shoot;
        public float knockBack;
        public string Description = "";
        public int ItemId;
        public int MaxStack;
        public int crit;
        public int usetime;
        public CoinValue MonetaryValue = new(0);
        public string Name = "";
        public List<string> Alias = new(); // Add this
    }

    public class ProjectInfo
    {
        public int AiStyle;
        public string Name = "";
        public int ProjId;
        public bool Friendly;
        public List<string> Alias = new(); // Add this
    }

    public class BuffInfo
    {
        public int BuffId;
        public string Description = "";
        public string Name = "";
        public List<string> Alias = new(); // Add this
    }

    public class PrefixInfo
    {
        public string Name = "";
        public int PrefixId;
        public List<string> Alias = new(); // Add this
    }
}
