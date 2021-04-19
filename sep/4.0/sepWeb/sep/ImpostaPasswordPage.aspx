<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ImpostaPasswordPage.aspx.vb" Inherits="ImpostaPasswordPage" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Imposta Password</title>
    <link type="text/css" href="Styles/Theme.css" rel="stylesheet" />
</head>

<body>

    <form id="form1" runat="server">

    <telerik:RadScriptManager ID="ScriptManager" runat="server" />

    <center>

        <table width="480px" cellpadding="5" cellspacing="5" border="0">

            <tr>
                <td>

                    <table class="ContainerWrapper" border="0" cellpadding="2" cellspacing="0" width="100%">

                        <%--HEADER--%>
                        <tr>
                            <td style="background-color: #BFDBFF; padding: 4px; border-bottom: 1px solid  #9ABBE8;
                                border-top: 1px solid  #9ABBE8; height: 25px">
                                &nbsp;<asp:Label ID="TitleLabel" runat="server" Style="color: #00156E" Font-Bold="True"
                                    Text=" Imposta Password" />
                            </td>
                        </tr>

                        <%--BODY--%>
                        <tr>
                            <td class="ContainerMargin">
                                <table class="Container" cellpadding="0" cellspacing="4" width="100%" border="0">
                                    <tr>
                                        <td>
                                            <div style="overflow: auto; height: 100%; width: 100%; background-color: #FFFFFF;
                                                border: 0px solid #5D8CC9;">
                                                <table style="width: 100%">
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="NuovaPasswordLabel" runat="server" CssClass="Etichetta" Text="Nuova password" />
                                                        </td>
                                                        <td>
                                                            <telerik:RadTextBox ID="NuovaPasswordTextBox" runat="server" Skin="Office2007" Width="185px"
                                                                TextMode="Password" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="ConfermaPasswordLabel" runat="server" CssClass="Etichetta" Text="Conferma password" />
                                                        </td>
                                                        <td>
                                                            <telerik:RadTextBox ID="ConfermaPasswordTextBox" runat="server" Skin="Office2007"
                                                                Width="185px" TextMode="Password" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>

                        <%--FOOTER--%>
                        <tr>
                            <td align="center" style="background-color: #BFDBFF; padding: 4px; border-bottom: 0px solid  #9ABBE8;
                                border-top: 1px solid  #9ABBE8; height: 25px">
                                <telerik:RadButton ID="SalvaButton" runat="server" Text="Salva" Width="80px" Skin="Office2007"
                                    ToolTip="Salva impostazioni utente">
                                    <Icon PrimaryIconUrl="images/Save16.png" PrimaryIconLeft="5px" />
                                </telerik:RadButton>
                            </td>
                        </tr>

                    </table>

                </td>
            </tr>

        </table>

    </center>

    </form>

</body>

</html>
