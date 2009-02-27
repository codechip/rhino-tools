using System;
using MultiTenancy.Web.Model;

namespace MultiTenancy.Web.Services
{
    public interface IScoreCalculator
    {
        decimal Calculate(Game game, Player player);
    }
}