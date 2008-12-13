namespace Advance.IoC.Components.Services
{
    public interface ICrowdParticipation
    {
        void Participate();
    }

    public class DevTeachCrowdParticipation : ICrowdParticipation
    {
        public void Participate()
        {
            //throw new System.NotSupportedException("not today... ?");
        }
    }
}