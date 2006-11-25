using CookComputing.XmlRpc;

public interface ITracXmlRpc : IXmlRpcProxy
{
	[XmlRpcMethod("ticket.create")]
	int TicketCreate(string summary, string description, TicketAttributes ticket);

	[XmlRpcMethod("ticket.milestone.getAll")]
	string[] GetMilestones();
	//ticket.putAttachment

	[XmlRpcMethod("ticket.component.getAll")]
	string[] GetComponents();

	[XmlRpcMethod("ticket.priority.getAll")]
	string[] GetPriorities();

	[XmlRpcMethod("ticket.severity.getAll")]
	string[] GetSeverities();

	[XmlRpcMethod("ticket.version.getAll")]
	string[] GetVersions();

	[XmlRpcMethod("ticket.putAttachment")]
	string PutAttachment(int ticketId, string filename, string description, byte[] base64Data, bool replace);
}

public struct TicketAttributes
{
	public string milestone;
	public string keywords;
	public string component;
	public string severity;
	public string priority;
	public string cc;
	public string version;
	public string comment;
}