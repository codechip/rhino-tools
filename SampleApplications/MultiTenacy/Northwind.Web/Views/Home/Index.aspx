<%@ Page Language="C#" 
    MasterPageFile="~/Views/Shared/Site.Master" 
    AutoEventWireup="true"
    CodeBehind="Index.aspx.cs" 
    Inherits="MultiTenancy.Web.Views.Home.Index, MultiTenancy.Web" %>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        The mandatory disclaimer</h2>
    <p>
        Please note that this is a really simple site meant to demo only multi tenancy.
        Other aspects of good software development are not covered.
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
