namespace Eastsea.Web.Model
{
    public class Score : MultiTenancy.Web.Model.Score
    {
        public virtual string Currency { get; set; }
    }
}