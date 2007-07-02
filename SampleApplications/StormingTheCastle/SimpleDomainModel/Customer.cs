using System.Collections.Generic;

namespace SimpleDomainModel
{
    [System.SerializableAttribute()]
    public class Customer
    {
        private string customerID;

        private string address;

        private string city;

        private string companyName;

        private string contactName;

        private string contactTitle;

        private string country;

        private string fax;

        private string phone;

        private string postalCode;

        private string region;

        private IList<Order> orders = new List<Order>();

        public virtual string CustomerID
        {
            get { return this.customerID; }
            set { this.customerID = value; }
        }

        public virtual string Address
        {
            get { return this.address; }
            set { this.address = value; }
        }

        public virtual string City
        {
            get { return this.city; }
            set { this.city = value; }
        }

        public virtual string CompanyName
        {
            get { return this.companyName; }
            set { this.companyName = value; }
        }

        public virtual string ContactName
        {
            get { return this.contactName; }
            set { this.contactName = value; }
        }

        public virtual string ContactTitle
        {
            get { return this.contactTitle; }
            set { this.contactTitle = value; }
        }

        public virtual string Country
        {
            get { return this.country; }
            set { this.country = value; }
        }

        public virtual string Fax
        {
            get { return this.fax; }
            set { this.fax = value; }
        }

        public virtual string Phone
        {
            get { return this.phone; }
            set { this.phone = value; }
        }

        public virtual string PostalCode
        {
            get { return this.postalCode; }
            set { this.postalCode = value; }
        }

        public virtual string Region
        {
            get { return this.region; }
            set { this.region = value; }
        }

        public virtual IList<Order> Orders
        {
            get { return this.orders; }
            set { this.orders = value; }
        }
    }
}