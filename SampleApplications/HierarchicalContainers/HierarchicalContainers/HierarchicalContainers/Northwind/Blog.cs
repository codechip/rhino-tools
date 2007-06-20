using System;

namespace HierarchicalContainers.Northwind
{
    public class NorthwindBlog : Blog
    {
        private string northwindCustomerId;

        public string NorthwindCustomerId
        {
            get { return northwindCustomerId; }
            set { northwindCustomerId = value; }
        }
    }
}