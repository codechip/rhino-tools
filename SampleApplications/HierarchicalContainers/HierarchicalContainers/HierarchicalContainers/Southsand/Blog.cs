using System;

namespace HierarchicalContainers.Southsand
{
    public class SouthsandBlog : Blog
    {
        private Guid southSandCustomerId;

        public Guid SouthSandCustomerId
        {
            get { return southSandCustomerId; }
            set { southSandCustomerId = value; }
        }
    }
}