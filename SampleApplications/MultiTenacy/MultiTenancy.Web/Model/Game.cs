namespace MultiTenancy.Web.Model
{
    public class Game
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }

        public virtual void Play(Player player)
        {
            
        }
    }
}