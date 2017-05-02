<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="MyForm.aspx.vb" Inherits="AgentWS.MyForm" validateRequest="false" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .style1
        {
            width: 100%;
        }
        .style2
        {
            width: 111px;
        }
        .style3
        {
            width: 44px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table class="style1">
        <tr>
            <td class="style2">
                <asp:Label ID="lbl_Random" runat="server" Text="Hello World"></asp:Label>
            </td>
            <td class="style3">
                <asp:Label ID="lbl_RandomVal" runat="server"></asp:Label>
            </td>
            <td>
                <asp:Button ID="btnGet" runat="server" Text="Get" />
            </td>
        </tr>
        <tr>
            <td class="style2">
                &nbsp;</td>
            <td class="style3">
                &nbsp;</td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td class="style2">
                <asp:Label ID="lblUser" runat="server" Text="AgentID"></asp:Label>
            </td>
            <td class="style3">
                <asp:TextBox ID="txt_AgentID" runat="server">Emi</asp:TextBox>
            </td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td class="style2">
                <asp:Label ID="lblPwd" runat="server" Text="Password"></asp:Label>
            </td>
            <td class="style3">
                <asp:TextBox ID="txt_Pwd" runat="server">password</asp:TextBox>
            </td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td class="style2">
                <asp:Label ID="lblDev" runat="server" Text="DeviceID"></asp:Label>
            </td>
            <td class="style3">
                <asp:TextBox ID="txt_DeviceID" runat="server">5FD49A91-884F-4621-948A-D02F5B0E31C4</asp:TextBox>
            </td>
            <td>
                <asp:Button ID="btn_Check" runat="server" Text="Check" />
            </td>
        </tr>
        <tr>
            <td class="style2">
                <asp:Label ID="Label1" runat="server" Text="lbl_Result"></asp:Label>
            </td>
            <td class="style3">
                <asp:Label ID="lbl_ResultVal" runat="server"></asp:Label>
            </td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td class="style2">
                &nbsp;</td>
            <td class="style3">
                &nbsp;</td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td class="style2">
                <asp:FileUpload ID="fileUp_Encrypted" runat="server" />
                <asp:TextBox ID="txtAgentCode" runat="server"></asp:TextBox>
&nbsp;<asp:TextBox ID="txtCountFile" runat="server"></asp:TextBox>
            </td>
            <td class="style3">
                <asp:Label ID="lbl_DecryptStatus" runat="server"></asp:Label>
            </td>
            <td>
                <asp:Button ID="btn_Decrypt" runat="server" Text="Decrypt" />
            </td>
        </tr>
        <tr>
            <td class="style2">
                <asp:FileUpload ID="fileUp_Decrypted" runat="server" />
            </td>
            <td class="style3">
                <asp:Label ID="lbl_EncryptStatus" runat="server"></asp:Label>
            </td>
            <td>
                <asp:Button ID="btn_Encyrpt" runat="server" Text="Encrypt" />
            </td>
        </tr>
        <tr>
            <td class="style2">
                &nbsp;</td>
            <td class="style3">
                &nbsp;</td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
        <td>
                <asp:Label ID="lblPolNo0" runat="server" Text="Propose Number"></asp:Label>
            &nbsp;&nbsp;
                <asp:TextBox ID="txtAgenCode" runat="server"></asp:TextBox>
&nbsp;&nbsp; <asp:TextBox ID="txtProposeNo" runat="server"></asp:TextBox>
        </td>
        <td>
                <asp:Button ID="btn_Refresh" runat="server" Text="Get Policy Number" />
        </td>
        <td>
            <asp:Label ID="lblPolicyNumber" runat="server" Text="Policy Number"></asp:Label>
        </td>
        </tr>
        <tr>
            <td class="style2">
                &nbsp;</td>
            <td class="style3">
                <asp:Label ID="lbl_ResultRefresh" runat="server"></asp:Label>
            </td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td class="style2">
                &nbsp;</td>
            <td class="style3">
                &nbsp;</td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td class="style2">
                URL</td>
            <td class="style3">
                <asp:TextBox ID="txtURL" runat="server">mobile/forgotpass?AgentCode=Eugene</asp:TextBox>
            </td>
            <td>
                <asp:Button ID="btnFP" runat="server" Text="check" />
                <asp:Label ID="Label2" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="style2">
                &nbsp;</td>
            <td class="style3">
                &nbsp;</td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td class="style2">
                First Time Login</td>
            <td class="style3">
                <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:Button ID="Button1" runat="server" Text="Check" />
            </td>
        </tr>
        <tr>
            <td class="style2">
                &nbsp;</td>
            <td class="style3">
                <asp:TextBox ID="TextBox2" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:Label ID="lbl_first" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="style2">
                &nbsp;</td>
            <td class="style3">
                <asp:TextBox ID="TextBox3" runat="server"></asp:TextBox>
            </td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td class="style2">
                <asp:Button ID="Button2" runat="server" Text="Button" />
                <asp:TextBox ID="OutputTextBlock" runat="server"></asp:TextBox>
            </td>
            <td class="style3">
                <asp:TextBox ID="TextBox4" runat="server"></asp:TextBox>
            </td>
            <td>
                &nbsp;</td>
        </tr>
    </table>
</asp:Content>
