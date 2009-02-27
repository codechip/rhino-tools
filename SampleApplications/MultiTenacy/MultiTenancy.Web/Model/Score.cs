namespace MultiTenancy.Web.Model
{
    public class Score
    {
        public virtual int Id { get; set; }
        public virtual Game Game { get; set; }
        public virtual Player Player { get; set; }
        public virtual decimal Value { get; set; }
    }
}