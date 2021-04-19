<%@ Page Language="VB" AutoEventWireup="false" CodeFile="TimeOutPage.aspx.vb" Inherits="TimeOutPage" %>



<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
   

    <center>
   

        <table  cellpadding="2"  style="width: 400px; background-color: #BFDBFF; border: 1 solid #00156E">
                <tr>
                    <td style="height: 20px">
                    <img src="images/info.png" alt="" />
                    
                    </td>
                </tr>
                <tr>
                    <td>
                    <table style="background-color: #FFFFFF;width:100%" >
                    <tr>

                    <td align="center">

                   <p>
                  <br/>
                  <br/>
                  <b><font color="#FF0000" size="4" face="Verdana, Arial, Helvetica, sans-serif">Sessione Scaduta!<br/>
                   <br/>
                   <font color="#000000" size="3"><i>Effettuare nuovamente il login
                
    </i></font></font></b> </p>
 

 <hr />
  <p>
     <asp:HyperLink NavigateUrl="~/Login.aspx" ID="IndietroHyperLink" runat="server">Esegui Login</asp:HyperLink>
 
  </p>

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
