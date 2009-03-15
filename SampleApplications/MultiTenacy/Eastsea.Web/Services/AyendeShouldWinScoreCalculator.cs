using System;
using MultiTenancy.Web.Model;
using MultiTenancy.Web.Services;

namespace Eastsea.Web.Services
{
    public class AyendeShouldWinScoreCalculator : IScoreCalculator
    {
        public decimal Calculate(Game game, Player player)
        {
            if (player.Name == "Ayende")
                return 103012312;
            return 5;
        }
    }
}