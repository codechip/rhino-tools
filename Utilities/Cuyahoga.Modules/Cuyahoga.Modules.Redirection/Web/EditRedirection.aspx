<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditRedirection.aspx.cs" Inherits="Cuyahoga.Modules.Redirection.Web.EditRedirection" %>
<%@ Register TagPrefix="cc1" Namespace="Cuyahoga.ServerControls" Assembly="Cuyahoga.ServerControls" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" method="post" runat="server">
			<div id="moduleadminpane">
				<h1>Edit File</h1>
				<div class="group">
					<h4>File properties</h4>
					<table>
						<tr>
							<td style="WIDTH: 100px">Url</td>
							<td><asp:panel id="pnlFileName" runat="server" visible="True">
									<asp:requiredfieldvalidator id="rfvFile" runat="server" errormessage="Url is required" display="Dynamic" cssclass="validator"
										controltovalidate="txtUrl" enableclientscript="False"></asp:requiredfieldvalidator>
								</asp:panel>
								<asp:textbox id="txtUrl" runat="server" width="300px"></asp:textbox></td>
						</tr>
						<tr>
							<td style="WIDTH: 100px">Title</td>
							<td>
							<asp:requiredfieldvalidator id="Requiredfieldvalidator1" runat="server" errormessage="Title is required" display="Dynamic" cssclass="validator"
										controltovalidate="txtTitle" enableclientscript="False"></asp:requiredfieldvalidator>
								<asp:textbox id="txtTitle" runat="server" width="300px"></asp:textbox></td>
						</tr>
						<tr>
							<td style="WIDTH: 100px">Date published</td>
							<td>
								<cc1:calendar id="calDatePublished" runat="server" displaytime="True"></cc1:calendar>
								<asp:requiredfieldvalidator id="rfvDatePublished" runat="server" errormessage="Date published is required" display="Dynamic"
									cssclass="validator" controltovalidate="calDatePublished" enableclientscript="False"></asp:requiredfieldvalidator></td>
						</tr>
					</table>
				</div>
				<p><asp:button id="btnSave" OnClick="btnSave_Click" runat="server" text="Save"></asp:button><asp:button id="btnDelete" runat="server" OnClick="btnDelete_Click" visible="False" causesvalidation="False" text="Delete"></asp:button><asp:button id="btnCancel" runat="server" causesvalidation="False" text="Cancel" OnClick="btnCancel_Click"></asp:button></p>
			</div>
		</form>
</body>
</html>
