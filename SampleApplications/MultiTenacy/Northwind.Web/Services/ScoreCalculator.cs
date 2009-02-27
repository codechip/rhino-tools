using System;
using MultiTenancy.Web.Model;
using MultiTenancy.Web.Services;

namespace Northwind.Web.Services
{
    public class ScoreCalculator : IScoreCalculator
    {
        public decimal Calculate(Game game, Player player)
        {
            return 8;
        }
    }
}