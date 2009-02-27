<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true"
    CodeBehind="Index.aspx.cs" Inherits="MultiTenancy.Web.Views.Home.Index, MultiTenancy.Web" %>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        The mandatory disclaimer</h2>
    <p>
        Please note that this is a really simple site meant to demo only multi tenancy.
        Other aspects of good software development are not covered.
    </p>
    <table>
        <thead>
            <tr>
                <th>
                </th>
                <% foreach (var player in ViewData.Model.Players)
                   { %>
                <th>
                    <%=Html.Encode(player.Name) %>
                </th>
                <% } %>
            </tr>
        </thead>
        <tbody>
            <% foreach (var game in ViewData.Model.Games)
               { %>
            <tr>
                <td>
                    <%=Html.Encode(game.Name) %>
                </td>
                <% foreach (var player in ViewData.Model.Players)
                   { %>
                <td>
                    <%=ViewData.Model.Calculator.Calculate(game,player) %>
                </td>
                <% }%>
            </tr>
            <% } %>
        </tbody>
    </table>
</asp:Content>
