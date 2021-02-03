<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Registration.aspx.cs" Inherits="SITConnect.Registration" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Registration</title>
    <script type="text/javascript">
        function validate() {
            var str = document.getElementById('<%=password_textbox.ClientID %>').value;

            if (str.length < 8) {
                document.getElementById("passwordChecker_label").innerHTML = "Your password must be more than 8 characters";
                document.getElementById("passwordChecker_label").style.color = "Red";
                return "Too short";
            }
            else if (str.search(/[0-9]/) == -1) {
                document.getElementById("passwordChecker_label").innerHTML = "Your password must have at least 1 number";
                document.getElementById("passwordChecker_label").style.color = "Red";
                return "No number";
            }
            else if (str.search(/[A-Z]/) == -1) {
                document.getElementById("passwordChecker_label").innerHTML = "Your password must have at least 1 uppercase letter";
                document.getElementById("passwordChecker_label").style.color = "Red";
                return "No uppercase";
            }
            else if (str.search(/[a-z]/) == -1) {
                document.getElementById("passwordChecker_label").innerHTML = "Your password must have at least 1 lowercase letter";
                document.getElementById("passwordChecker_label").style.color = "Red";
                return "No lowercase";
            }
            else if (str.search(/[^a-zA-Z0-9]/) == -1) {
                document.getElementById("passwordChecker_label").innerHTML = "Your password must have at least 1 special character";
                document.getElementById("passwordChecker_label").style.color = "Red";
                return "No special character";
            }

            document.getElementById("passwordChecker_label").innerHTML = "";

        }
        
    </script>

    <script src="https://www.google.com/recaptcha/api.js?render=6LfgFUUaAAAAAKy_4De0VHZhyH1BYMPcyyNt1jJR"></script>

    <style type="text/css">
        .auto-style6 {
            width: 951px;
        }
        </style>
</head>
<body>
    <form id="form1" runat="server">
        <div style="margin-left: auto; margin-right: auto; text-align: center;">
            <asp:Label ID="title" runat="server" Text="SITConnect" Font-Size="18pt" ></asp:Label>
        </div>
        <p>
            <asp:Label ID="register_title" runat="server" Text="Registration Form"></asp:Label>
        </p>
        <table class="nav-justified">
        <tr>
            <td style="width: 140px">
                <asp:Label ID="fName_label" runat="server" Text="First Name"></asp:Label>
            </td>
            <td class="auto-style6">
                <asp:TextBox ID="fName_textbox" runat="server" Width="300px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td style="width: 139px">
                <asp:Label ID="lName_label" runat="server" Text="Last Name"></asp:Label>
            </td>
            <td class="auto-style6">
                <asp:TextBox ID="lName_textbox" runat="server" Width="300px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td style="width: 139px">
                <asp:Label ID="email_label" runat="server" Text="Email"></asp:Label>
            </td>
            <td class="auto-style6">
                <asp:TextBox ID="email_textbox" runat="server" Width="300px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td style="width: 139px">
                <asp:Label ID="ccNo_label" runat="server" Text="Credit Card Number"></asp:Label>
            </td>
            <td class="auto-style6">
                <asp:TextBox ID="ccNo_textbox" runat="server" Width="300px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td style="width: 139px">
                <asp:Label ID="cvv_label" runat="server" Text="CVV"></asp:Label>
            </td>
            <td class="auto-style6">
                <asp:TextBox ID="cvv_textbox" runat="server" Width="300px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td style="width: 139px">
                <asp:Label ID="ccED_label" runat="server" Text="CC Expiry Date"></asp:Label>
            </td>
            <td class="auto-style6">
                <asp:TextBox ID="ccED_textbox" runat="server" Width="300px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td style="width: 139px">
                <asp:Label ID="dob_label" runat="server" Text="Date of Birth"></asp:Label>
            </td>
            <td class="auto-style6">
                <asp:TextBox ID="dob_textbox" TextMode="Date" runat="server" Width="300px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td style="width: 139px">
                <asp:Label ID="password_label" runat="server" Text="Password"></asp:Label>
            </td>
            <td class="auto-style6">
                <asp:TextBox ID="password_textbox" TextMode="Password"  onkeyup="javascript:validate()" runat="server" Width="300px"></asp:TextBox>
&nbsp;<asp:Label ID="passwordChecker_label" runat="server" Text=""></asp:Label>
            </td>
        </tr>
    </table>
        <asp:Label ID="checker_label" runat="server"></asp:Label>
        <br />
        <asp:Button ID="register_button" runat="server" OnClick="register_button_Click1" Text="Register" />
        <br />
        <br />
        <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response" />
        <br />
        <br />
        <asp:Label ID="display_label" runat="server" Text=""></asp:Label>
        <br />
        <asp:Button ID="route_button" runat="server" OnClick="route_button_Click1" Text="To Login Page" />
        <br />
    </form>

    <script>
        grecaptcha.ready(function () {
            grecaptcha.execute('6LfgFUUaAAAAAKy_4De0VHZhyH1BYMPcyyNt1jJR', { action: 'Login' }).then(function (token) {
                document.getElementById("g-recaptcha-response").value = token;
            });
        });
    </script>


</body>
</html>