<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="SITConnect.Profile" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Profile</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="title_label" runat="server" Text="User Profile"></asp:Label>
            <br />
            <br />
            <asp:Label ID="name_label" runat="server" Text="Name: "></asp:Label>
            <asp:Label ID="displayName_label" runat="server"></asp:Label>
            <br />
            <asp:Label ID="email_label" runat="server" Text="Email: "></asp:Label>
            <asp:Label ID="displayEmail_label" runat="server"></asp:Label>
            <br />
            <asp:Label ID="ccNo_label" runat="server" Text="Credit Card Number: "></asp:Label>
            <asp:Label ID="displayCCNo_label" runat="server"></asp:Label>
            <br />
            <br />
            <asp:Button ID="logout_button" runat="server" OnClick="logout_button_Click" Text="Logout" />
            <br />
        </div>
    </form>
</body>
</html>