<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="SITConnect.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style1 {
            width: 100%;
        }
    </style>
    <script src="https://www.google.com/recaptcha/api.js?render=6LfgFUUaAAAAAKy_4De0VHZhyH1BYMPcyyNt1jJR"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="login_label" runat="server" Text="Login"></asp:Label>
            <br />
            <br />
            <table class="auto-style1">
                <tr>
                    <td>
                        <asp:Label ID="email_label" runat="server" Text="Email"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="email_textbox" runat="server" Width="250px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="password_label" runat="server" Text="Password"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="password_textbox" TextMode="Password" runat="server" Width="250px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="checker_label" runat="server"></asp:Label>
                    </td>
                    <td>
                        <asp:Button ID="login_button" runat="server" OnClick="login_button_Click" Text="Login" />
                        <asp:Button ID="register_button" runat="server" OnClick="register_button_Click" Text="Regiser a account" />
                    </td>
                </tr>
            </table>
        </div>

        <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response" />

    <script>
        grecaptcha.ready(function () {
            grecaptcha.execute('6LfgFUUaAAAAAKy_4De0VHZhyH1BYMPcyyNt1jJR', { action: 'Login' }).then(function (token) {
                document.getElementById("g-recaptcha-response").value = token;
            });
        });
    </script>

        <asp:Label ID="catch_label" runat="server"></asp:Label>

    </form>

    </body>
</html>