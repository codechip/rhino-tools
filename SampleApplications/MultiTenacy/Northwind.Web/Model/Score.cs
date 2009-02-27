namespace Northwind.Web.Model
{
    public class Score : MultiTenancy.Web.Model.Score
    {
        public virtual int TimePlayed { get; set; }
    }
}