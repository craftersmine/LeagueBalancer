using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using craftersmine.League.CommunityDragon;

namespace craftersmine.LeagueBalancer
{
    public class AppCache
    {
        private static AppCache instance;

        public SummonerIconsCollection? Icons { get; set; }
        public ChampionsCollection? Champions { get; set; }

        public static AppCache Instance
        {
            get
            {
                if (instance is null)
                    instance = new AppCache();

                return instance;
            }
        }
    }
}
