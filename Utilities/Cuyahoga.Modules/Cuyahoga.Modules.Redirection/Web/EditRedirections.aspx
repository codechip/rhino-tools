<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditRedirections.aspx.cs" Inherits="Cuyahoga.Modules.Redirection.Web.EditRedirections" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" method="post" runat="server">
			<div id="moduleadminpane">
				<h1>Downloads management</h1>
				<p>
					<table class="tbl">
						<asp:repeater id="rptFiles" runat="server" EnableViewState="false"
						    OnItemDataBound="rptFiles_ItemDataBound">
							<headertemplate>
								<tr>
									<th>
										Title</th>
									<th>
										Url</th>
									<th>
										Published by</th>
									<th>
										Number of downloads</th>
									<th>
										Date published</th>
									<th>
									</th>
								</tr>
							</headertemplate>
							<itemtemplate>
								<tr>
									<td><%# DataBinder.Eval(Container.DataItem, "Title") %></td>
									<td><%# DataBinder.Eval(Container.DataItem, "Url") %></td>
									<td><%# DataBinder.Eval(Container.DataItem, "Publisher.FullName") %></td>
									<td><%# DataBinder.Eval(Container.DataItem, "NumberOfDownloads") %></td>
									<td><asp:literal id="litDateModified" runat="server"></asp:literal></td>
									<td>
										<asp:hyperlink id="hplEdit" runat="server">Edit</asp:hyperlink>
									</td>
								</tr>
							</itemtemplate>
						</asp:repeater>
					</table>
				</p>
				<br/>
				<input id="btnNew" type="button" value="Add File" runat="server" name="btnNew">
			</div>
		</form>
</body>
</html>
