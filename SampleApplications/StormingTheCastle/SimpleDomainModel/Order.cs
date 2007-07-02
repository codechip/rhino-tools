using System.Collections.Generic;
namespace SimpleDomainModel
{
    [System.SerializableAttribute()]
    public class Order
    {
        private int orderID;

        private System.Decimal? freight;

        private System.DateTime? orderDate;

        private System.DateTime? requiredDate;

        private string shipAddress;

        private string shipCity;

        private string shipCountry;

        private string shipName;

        private System.DateTime? shippedDate;

        private string shipPostalCode;

        private string shipRegion;

        private Customer customer;


        private IList<OrderDetail> orderDetails = new List<OrderDetail>();


        public virtual int OrderID
        {
            get { return orderID; }
            set { orderID = value; }
        }

        public virtual System.Decimal? Freight
        {
            get { return this.freight; }
            set { this.freight = value; }
        }

        public virtual System.DateTime? OrderDate
        {
            get { return this.orderDate; }
            set { this.orderDate = value; }
        }

        public virtual System.DateTime? RequiredDate
        {
            get { return this.requiredDate; }
            set { this.requiredDate = value; }
        }

        public virtual string ShipAddress
        {
            get { return this.shipAddress; }
            set { this.shipAddress = value; }
        }

        public virtual string ShipCity
        {
            get { return this.shipCity; }
            set { this.shipCity = value; }
        }

        public virtual string ShipCountry
        {
            get { return this.shipCountry; }
            set { this.shipCountry = value; }
        }

        public virtual string ShipName
        {
            get { return this.shipName; }
            set { this.shipName = value; }
        }

        public virtual System.DateTime? ShippedDate
        {
            get { return this.shippedDate; }
            set { this.shippedDate = value; }
        }

        public virtual string ShipPostalCode
        {
            get { return this.shipPostalCode; }
            set { this.shipPostalCode = value; }
        }

        public virtual string ShipRegion
        {
            get { return this.shipRegion; }
            set { this.shipRegion = value; }
        }

        public virtual Customer Customer
        {
            get { return this.customer; }
            set { this.customer = value; }
        }

        public virtual IList<OrderDetail> OrderDetails
        {
            get { return this.orderDetails; }
            set { this.orderDetails = value; }
        }
    }
}