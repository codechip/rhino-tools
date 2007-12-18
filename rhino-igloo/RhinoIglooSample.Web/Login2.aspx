<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Login2.aspx.cs" Inherits="RhinoIglooSample.Web.Login2" Title="Untitled Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
   Name: <asp:TextBox runat="server" ID="Name"/>
        <br />
        Password: <asp:TextBox runat="server" ID="Password"/><br />
        <br/>
        <asp:Button runat="server" onclick="Submit_Click" Id="Submit" Text="Login"/>
        <br/>
        <asp:TextBox runat="server" ID="Result"/>
</asp:Content>
