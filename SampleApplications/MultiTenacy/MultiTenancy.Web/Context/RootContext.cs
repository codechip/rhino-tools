namespace MultiTenancy.Web.Context
{
    public class RootContext
    {
        public string GetConnectionStringFor(string tenantId)
        {
            return "Data Source=localhost;Initial Catalog=" + 
                tenantId +
                ";Integrated Security=True";
        }

    }
}