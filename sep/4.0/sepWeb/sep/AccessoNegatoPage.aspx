<%@ Page Language="VB" AutoEventWireup="false" CodeFile="AccessoNegatoPage.aspx.vb" Inherits="AccessoNegatoPage" %>


<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
     <link type="text/css" href="Styles/Theme.css" rel="stylesheet" />
   </head>
<body>




   
    <form id="form1" runat="server">

  <center>
  <p><br/>
    <br/>
    <b><font color="#FF0000" size="4" face="Verdana, Arial, Helvetica, sans-serif">Attenzione:<br/>
    <br/>
    <font size="3"><i>ACCESSO alla funzionalit&agrave;<br/>
    <br/>
    NEGATO!</i></font></font></b> </p>
  <p>&nbsp;</p>
  <p>&nbsp;</p>
  <p>
     <asp:HyperLink NavigateUrl="~/Login.aspx" ID="IndietroHyperLink" runat="server">Indietro</asp:HyperLink>
 
  </p>
 
</center>


    </form>


  
</body>
</html>
