<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="LoginSite.aspx.vb" Inherits="AgentWS.LoginSite" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="~/Styles/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        body {
          padding-top: 40px;
          padding-bottom: 40px;
          background-color: #eee;
        }

        .form-signin {
          max-width: 330px;
          padding: 15px;
          margin: 0 auto;
        }
        .form-signin .form-signin-heading,
        .form-signin .checkbox {
          margin-bottom: 10px;
        }
        .form-signin .checkbox {
          font-weight: normal;
        }
        .form-signin .form-control {
          position: relative;
          height: auto;
          -webkit-box-sizing: border-box;
             -moz-box-sizing: border-box;
                  box-sizing: border-box;
          padding: 10px;
          font-size: 16px;
        }
        .form-signin .form-control:focus {
          z-index: 2;
        }
        .form-signin input[type="email"] {
          margin-bottom: -1px;
          border-bottom-right-radius: 0;
          border-bottom-left-radius: 0;
        }
        .form-signin input[type="password"] {
          margin-bottom: 10px;
          border-top-left-radius: 0;
          border-top-right-radius: 0;
        }
    </style>
</head>
<body>
    <div class="container">
        <form id="form1" class="form-signin" runat="server">
            <h2 class="form-signin-heading">TMConnect</h2>
            <asp:TextBox ID="txt_user" runat="server" class="form-control" placeholder="username" Width="300px"></asp:TextBox>
            <br />
            <asp:TextBox ID="txt_password" runat="server" class="form-control" placeholder="password" TextMode="Password" Width="300px" ></asp:TextBox>
            <asp:Label ID="Label1" runat="server" Text=""></asp:Label>
            <br />
            <asp:Button ID="btn_login" runat="server" Text="Sign in" class="btn btn-lg btn-primary btn-block" />
            <br />
            <br />
            <div style="text-align:center;">
                <asp:Label ID="lbl_Versionno" runat="server" Text=""></asp:Label>
            </div>
        </form>
    </div>
</body>
</html>
