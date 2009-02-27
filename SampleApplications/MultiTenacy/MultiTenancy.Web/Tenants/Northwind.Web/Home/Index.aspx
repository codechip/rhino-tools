<%@ Page Language="C#" 
    MasterPageFile="~/Views/Shared/Site.Master" 
    AutoEventWireup="true"
    CodeBehind="Index.aspx.cs" 
    Inherits="MultiTenancy.Web.Tenants.Northwind.Web.Home.Index" %>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>A note from northwind</h2>
    <p>
        See how I changed the text?
    </p>
    <div id="players">
        <h2>
            Players</h2>
        <ul>
            <% foreach (var player in ViewData.Model.Players)
               { %>
            <li>
                <%=Html.Encode(player.Name) %></li>
            <% } %>
        </ul>
    </div>
    <div id="games">
        <h2>Games</h2>
        <ul>
            <% foreach (var game in ViewData.Model.Games)
               { %>
            <li>
                <%=Html.Encode(game.Name) %></li>
            <% } %>
        </ul>
    </div>
</asp:Content>
