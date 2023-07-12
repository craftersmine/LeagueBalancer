using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using craftersmine.League.CommunityDragon;
using craftersmine.Riot.Api.League.Mastery;

namespace craftersmine.LeagueBalancer
{
    public class LeagueChampion
    {
        private BitmapImage? _icon;

        public LeagueChampionMastery? Mastery { get; internal set; }
        public Champion Champion { get; internal set; }

        public ImageSource Icon
        {
            get
            {
                if (_icon is null)
                    _icon = new BitmapImage(new Uri(this.Champion.PortraitIcon.GetAssetUri()));

                return _icon;
            }
        }

        public int MasteryPoints
        {
            get
            {
                if (Mastery is not null)
                    return Mastery.MasteryPoints;
                return 0;
            }
        }

        public LeagueChampion(Champion champion, LeagueChampionMastery? mastery)
        {
            Mastery = mastery;
            Champion = champion;
        }

        public override string ToString()
        {
            return Champion.Name;
        }
    }
}
