<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" Codebehind="ChangePassword.aspx.cs" Inherits="RhinoIglooSample.Web.ChangePassword" Title="Untitled Page" %>

<asp:content id="Content1" contentplaceholderid="ContentPlaceHolder1" runat="server">
    Old: <asp:TextBox runat="server" ID="OldPass"/><br/>
    New: <asp:TextBox runat="server" ID="NewPass"/><br/>
    Confirm: <asp:TextBox runat="server" ID="NewPassConfirm"/><br/>
    <asp:Button runat="server" onclick="Submit_Click" Id="Submit" Text="Change Pass"/>
       
    <br/>   
   Success Msg: <asp:TextBox runat="server" ID="SuccessMessage"/><br/>
   Error Msg: <asp:TextBox runat="server" ID="ErrorMessage" style="background-color: red; "/><br/>
 
</asp:content>
