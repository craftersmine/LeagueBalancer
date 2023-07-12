using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace craftersmine.LeagueBalancer
{
    public class LeagueTeam
    {
        public Summoner[] Summoners { get; private set; }

        public int LeaguePointsTotal
        {
            get
            {
                int lp = 0;
                foreach (Summoner summoner in Summoners)
                {
                    lp += summoner.LeaguePointsAmount;
                }

                return lp;
            }
        }

        public LeagueTeam(Summoner[] summoners)
        {
            Summoners = summoners;
        }
    }

    public enum LeagueTeamType
    {
        None,
        Blue,
        Red
    }
}
