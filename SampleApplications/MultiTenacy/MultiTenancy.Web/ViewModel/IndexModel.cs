using System;
using System.Collections.Generic;
using MultiTenancy.Web.Model;
using MultiTenancy.Web.Services;

namespace MultiTenancy.Web.ViewModel
{
    public class IndexModel
    {
        public IEnumerable<Game> Games { get; set;}
        public IEnumerable<Player> Players { get; set; }
        public IScoreCalculator Calculator { get; set; }
    }
}