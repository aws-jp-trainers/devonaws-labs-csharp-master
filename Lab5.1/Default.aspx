<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="AwsLabs._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Developing on AWS - Lab5.1</title>
    <link rel="stylesheet" href="styles/styles.css" type="text/css" media="screen"/>
    <meta http-equiv="Cache-Control" content="no-cache, no-store, must-revalidate" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="Expires" content="0" />
</head>

<body>
    
    <table width="90%">
        <tr>
            <td><h2>Configuration Parameters:</h2></td>
            <td><h2>Host Environment:</h2></td>
        </tr>
        <tr>

            <td class="topalign">
                <table>
                    <asp:Label ID="configPlaceholder" runat="server"></asp:Label>
                </table>
            </td>
            <td class="topalign">
                <table>
                    <asp:Label ID="sysenvPlaceholder" runat="server"></asp:Label>
                </table>
            </td>       

        </tr>
    </table>

    <h2>Image List:</h2>
    <asp:Label ID="imageListPlaceholder" runat="server"></asp:Label>

    <h2>Status Messages:</h2>
    <asp:Label ID="statusPlaceholder" runat="server"></asp:Label>

</body>
</html>
