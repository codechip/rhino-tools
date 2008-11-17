<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Redirections.ascx.cs"
    Inherits="Cuyahoga.Modules.Redirection.Web.Redirections" %>
<asp:Repeater ID="rptFiles" runat="server" EnableViewState="False" 
    OnItemDataBound="rptFiles_ItemDataBound">
    <ItemTemplate>
        <li>
            <h4>
                <asp:HyperLink ID="hplFileImg" runat="server"></asp:HyperLink>
                <asp:HyperLink ID="hplFile" runat="server">
					<%# DataBinder.Eval(Container.DataItem, "Title") %>
                </asp:HyperLink>
            </h4>
            <asp:Panel ID="pnlFileDetails" CssClass="articlesub" runat="server">
                <asp:Label ID="lblDateModified" runat="server" >
                </asp:Label>
                <asp:Label ID="lblPublisher" runat="server" >
                </asp:Label>
                <asp:Label ID="lblNumberOfDownloads" runat="server" >
                </asp:Label>
            </asp:Panel>
        </li>
    </ItemTemplate>
</asp:Repeater>
