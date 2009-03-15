using System;
using MultiTenancy.Web.Model;

namespace Southsand.Web.Model
{
    public class SouthGame : Game
    {
        public virtual int NumberOfPeopleWithBBQ { get; set; }

        public override void Play(Player player)
        {
            if(NumberOfPeopleWithBBQ==0)
                throw new InvalidOperationException("no BBW no game");
            base.Play(player);
        }
    }
}