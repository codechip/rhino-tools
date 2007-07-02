namespace CustomerFinder
{
    public class AuditorImpl : IAuditor
    {
        private readonly ILogger logger;

        public AuditorImpl(ILogger logger)
        {
            this.logger = logger;
        }

        public void ReadCustomer(Customer customer)
        {
            logger.Log(string.Format("Read Customer: {0}, #{1}", customer.Name, customer.Id));
        }
    }
}