<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Login.aspx.vb" Inherits="Login" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">



<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
     <link type="text/css" href="Styles/Theme.css" rel="stylesheet" />


    <style type="text/css">
        .rcRefreshImage {
            margin-left: 180px;
          
            font-size: 10px;
            float: left;
            color: White;
            display: block;
            line-height: 1.5em;
            background-image: url('images/reload_refresh.png');
            background-repeat: no-repeat;
            height: 50px;
            padding-left: 30px;
            margin-top: -45px;
            width: 50px;
            /*padding-left: 25px; 
            padding-right: 25px; 
            display: block;
            visibility:hidden;
            
            background-image:url('images/reload_refresh.png');
            background-color:transparent;*/
        }
        .auto-style1 {
            height: 21px;
        }
    </style>



</head>


<body onload="if(document.getElementById('Login1_UserName')){document.getElementById('Login1_UserName').focus();}">




    <asp:PlaceHolder ID="scriptHolder" runat="server"></asp:PlaceHolder>
    <form id="form1" runat="server">

  <telerik:RadScriptManager ID="ScriptManager" runat="server" />





  
    <center>
        <div>
            <br />

           


         <asp:Panel ID="AggiornamentoPanel" runat="server" Height="260px" Width="570px" >
           <center>
              <table width="320" border="0" cellspacing="0" cellpadding="0">
                 <tr><td align="right"><img src="images/logoAmministrazione/logoCliente_login.gif" width="276" height="51" alt="Logo del cliente"/></td></tr>
              </table>
	          <h1 style="color:red"> S.E.P. in AGGIORNAMENTO!</h1>
              <p style="font-family:Arial Black;font-style:italic"><br/><br/>Il Sistema è momentaneamente sospeso per Aggiornamenti.</p>
              <p style="font-family:Arial Black;font-style:italic">Siete pregati di ricollegarvi più tardi, GRAZIE!</p>
              <p style="font-family:Arial Black; font-size:medium;color:red">Assistenza Parsec 3.26 S.r.l. Tel: 0832 22.84.77</p>
           </center>
       </asp:Panel>

            <asp:Panel ID="LoginPanel" runat="server">
                <img alt="" src="images/LogoAmministrazione/logoCliente_login.gif" />

                
               <asp:Login 
            ID="Login1" runat="server" FailureText="Accesso Fallito. Riprovare." >
            <LayoutTemplate>
        <table id="Tabella_01" width="356" style="height:343" border="0" cellpadding="0" cellspacing="0">
	<tr>
		<td colspan="7">
			<img src="images/immaginiLogin/sep_Login2_01.gif"  width="355" height="142" alt="" /></td>
		<td>
			<img src="images/immaginiLogin/spaziatore.gif"  width="1" height="142" alt="" /></td>
	</tr>
	<tr>
		<td rowspan="8">
			<img src="images/immaginiLogin/sep_Login2_02.gif"  width="41" height="213" alt="" /></td>
		<td colspan="2">

        
			<asp:TextBox ID="UserName" runat="server" Width="268px"  
                CssClass="rlbText" Height="18px" />

             <%--<telerik:RadTextBox ID="UserName"  runat="server" Skin="Office2007"  Width="268px" />--%>
                                        </td>
		<td colspan="2" rowspan="4">
			<img src="images/immaginiLogin/sep_Login2_04.gif" width="7" height="120" alt="" /></td>
		<td>
			<asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                                            ControlToValidate="UserName" ErrorMessage="Il nome utente è obbligatorio." 
                                            ToolTip="Il nome utente è obbligatorio." 
                ValidationGroup="Login1" Font-Size="XX-Small">*</asp:RequiredFieldValidator></td>
		<td rowspan="8">
			<img src="images/immaginiLogin/sep_Login2_06.gif"   width="27" height="213" alt="" /></td>
		<td>
			<img src="images/immaginiLogin/spaziatore.gif" width="1" height="18" alt="" /></td>
	</tr>
	<tr>
		<td colspan="2">
			<img src="images/immaginiLogin/sep_Login2_07.gif" width="272"  height="55" alt="" /></td>
		<td>
			<img src="images/immaginiLogin/sep_Login2_08.gif" width="8"  height="55" alt="" /></td>
		<td>
			<img src="images/immaginiLogin/spaziatore.gif" width="1" height="55" alt="" /></td>
	</tr>
	<tr>
		<td colspan="2">
			<asp:TextBox ID="Password" runat="server" TextMode="Password" Width="268px" 
                CssClass="riTextBox riEnabled" Height="18px" />

            
  <%-- <telerik:RadTextBox ID="Password" TextMode="Password" runat="server" Skin="Office2007"  Width="268px" />--%>
             
              
                </td>
		<td>
			<asp:RequiredFieldValidator ID="PasswordRequired" runat="server" 
                                            ControlToValidate="Password" ErrorMessage="La password è obbligatoria." 
                                            ToolTip="La password è obbligatoria." 
                ValidationGroup="Login1" Font-Size="XX-Small">*</asp:RequiredFieldValidator></td>
		<td>
			<img src="images/immaginiLogin/spaziatore.gif"   width="1" height="18" alt="" /></td>
	</tr>


         

          
	<tr>
		<td colspan="2" style=" text-align:left">
       
            <asp:CheckBox runat="server" ID="UsaUtenteWindows" Text="&nbsp;" />
             <asp:Label runat="server" ID="UsaUtenteWindowsLabel" Text="Utente Windows" style=" font-family:arial; font-size:12px" />
			<img src="images/immaginiLogin/sep_Login2_11.gif"  width="272" height="1" alt="" /></td>
		<td rowspan="5">
			<img src="images/immaginiLogin/sep_Login2_12.gif"   width="8" height="109" alt="" /></td>
		<td>
			<img src="images/immaginiLogin/spaziatore.gif"  width="1" height="29" alt="" /></td>
	</tr>
	<tr>
		<td rowspan="2">
			<img src="images/immaginiLogin/sep_Login2_13.gif"  width="189" height="57" alt="" /></td>
		<td colspan="2">
			<asp:ImageButton ID="LoginButton" runat="server" 
                ImageUrl="images/immaginiLogin/sep_Login2_14.gif" CommandName="Login" 
                ValidationGroup="Login1" />
        </td>
		<td rowspan="4">
			<img src="images/immaginiLogin/sep_Login2_15.gif"   width="3" height="80" alt="" /></td>
		<td>
			<img src="images/immaginiLogin/spaziatore.gif"  width="1" height="27" alt="" /></td>
	</tr>
	<tr>
		<td colspan="2" rowspan="3">
			<img src="images/immaginiLogin/sep_Login2_16.gif"   width="87" height="53" alt="" /></td>
		<td>
			<img src="images/immaginiLogin/spaziatore.gif"  width="1" height="30" alt="" /></td>
	</tr>
	<tr>
		<td style="font-size: 10px; color: #FF0000; font-family: Arial; background-color: #F5F6F7;">
            <asp:Literal ID="FailureText" runat="server" EnableViewState="False"></asp:Literal>
        </td>
       
		<td>
			<img src="images/immaginiLogin/spaziatore.gif"  width="1" height="12" alt="" /></td>
	</tr>
	<tr>
		<td>
			<img src="images/immaginiLogin/sep_Login2_18.gif"  width="189" height="11" alt="" /></td>
		<td>
			<img src="images/immaginiLogin/spaziatore.gif"  width="1" height="11" alt="" /></td>
	</tr>
	<tr>
		<td>
			<img src="images/immaginiLogin/spaziatore.gif" width="41" height="1" alt="" /></td>
		<td>
			<img src="images/immaginiLogin/spaziatore.gif"  width="189" height="1" alt="" /></td>
		<td>
			<img src="images/immaginiLogin/spaziatore.gif"  width="83" height="1" alt="" /></td>
		<td>
			<img src="images/immaginiLogin/spaziatore.gif"   width="4" height="1" alt="" /></td>
		<td>
			<img src="images/immaginiLogin/spaziatore.gif"   width="3" height="1" alt="" /></td>
		<td>
			<img src="images/immaginiLogin/spaziatore.gif"   width="8" height="1" alt="" /></td>
		<td>
			<img src="images/immaginiLogin/spaziatore.gif" width="27" height="1" alt="" /></td>
		<td></td>
	</tr>
</table>

<table>
<tr>
<td style="font-size: 10px; color: #FF0000; font-family: Arial; background-color: #F5F6F7; height:15px">
 <asp:Literal ID="WinFailureText" runat="server" EnableViewState="False"></asp:Literal>
</td>
</tr>
</table>

               


    </LayoutTemplate>
            
        </asp:Login>
          
                <table style="width: 356px" border="0" cellpadding="0" cellspacing="0">

                    <tr>

                        <td>

                            <div id="divcaptcha" runat="server" visible="false" style="height: auto">
                                <telerik:RadCaptcha ID="RadCaptcha1" runat="server" ErrorMessage="Captcha non valido"
                                    CaptchaImage-TextLength="5" CaptchaImage-TextColor="Red" IgnoreCase="true" EnableRefreshImage="true"
                                    CaptchaImage-RenderImageOnly="true" ValidatedTextBoxID="CaptchaTextBox" ValidationGroup="V">
                                </telerik:RadCaptcha>
                                <telerik:RadTextBox ID="CaptchaTextBox" runat="server">
                                </telerik:RadTextBox>

                                <input type="button" id="RefreshImageButton" onclick="RefreshImage(); return false;"
                                    value="Nuova Immagine" />

                            </div>

                        </td>

                    </tr>
                    <tr>
                        <td>
                            <asp:Label Text="Non ci sono avvisi!" runat="server" ID="AvvisoInfoLabel" Font-Bold="True"
                                Font-Size="X-Large" ForeColor="Red" Width="350px" />
                        </td>
                    </tr>
                </table>

           </asp:Panel>

            <asp:Panel ID="AvvisiPanel" runat="server">
               <asp:Label Text="Si prega di leggere i seguenti avvisi; premere OK per entrare in SEP!" 
                    runat="server" ID="AvvisiLabel" Font-Bold="True" Font-Size="Large" 
                    ForeColor="Red" Width="711px" /><br />
               <br />
                <table style="width:700px; height: 300px" cellpadding="5" cellspacing="5" border="0">
                    <tr>
                        <td valign="top">
                            <table class="ContainerWrapper" border="0" cellpadding="2" cellspacing="0" style="width: 100%;
                                height: 100%">
                                <tr class="ContainerHeader">
                                    <td align="left">
                                        <asp:Label ID="TitoloLabel" runat="server" CssClass="Etichetta" Text="Elenco Avvisi" />
                                    </td>
                                </tr>
                                <tr>
                                    <td valign="top" class="ContainerMargin">
                                        <table class="Container" cellpadding="0" cellspacing="4" style="width: 100%; height: 100%"
                                            border="0">
                                            <tr>
                                                <td valign="top">
                                                 <div style="height:300px; width:720px">
                                               
                                                    <div style="overflow: auto; height: 100%">
                                                        <telerik:RadGrid ID="AvvisiGridView" runat="server" AutoGenerateColumns="False"
                                                            CellSpacing="0" GridLines="None" Skin="Office2007" Width="700px" 
                                                            AllowSorting="True">
                                                            <MasterTableView DataKeyNames="Id">
                                                                <CommandItemSettings ExportToPdfText="Export to PDF" />
                                                                <RowIndicatorColumn FilterControlAltText="Filter RowIndicator column">
                                                                </RowIndicatorColumn>
                                                                <ExpandCollapseColumn FilterControlAltText="Filter ExpandColumn column">
                                                                </ExpandCollapseColumn>
                                                                <Columns>
                                                                    <telerik:GridImageColumn FilterControlAltText="Filter Flag column" 
                                                                        ImageHeight="" ImageWidth="" UniqueName="Flag">
                                                                        <HeaderStyle Width="20px" />
                                                                        <ItemStyle Width="20px" />
                                                                    </telerik:GridImageColumn>
                                                                    <telerik:GridBoundColumn DataField="Id" DataType="System.Int32" 
                                                                        FilterControlAltText="Filter Id column" HeaderText="Id" ReadOnly="True" 
                                                                        SortExpression="Id" UniqueName="Id" Visible="False">
                                                                        <HeaderStyle Width="70px" />
                                                                        <ItemStyle HorizontalAlign="Left" Width="70px" />
                                                                    </telerik:GridBoundColumn>
                                                                    <telerik:GridBoundColumn DataField="Contenuto" 
                                                                        FilterControlAltText="Filter Contenuto column" HeaderText="Contenuto" 
                                                                        SortExpression="Contenuto" UniqueName="Contenuto">
                                                                        <ItemStyle HorizontalAlign="Left" />
                                                                    </telerik:GridBoundColumn>
                                                                    <telerik:GridBoundColumn DataField="Data" DataFormatString="{0:dd/MM/yyyy}" 
                                                                        FilterControlAltText="Filter Data column" HeaderText="Data" 
                                                                        SortExpression="Data" UniqueName="Data">
                                                                        <HeaderStyle Width="90px" />
                                                                        <ItemStyle HorizontalAlign="Left" Width="90px" />
                                                                    </telerik:GridBoundColumn>
                                                                    <telerik:GridButtonColumn ButtonType="LinkButton" CommandName="Preview" 
                                                                        FilterControlAltText="Filter Preview column" 
                                                                        Text="&lt;img src='images\knob-search16.png' style='border-width:0' /&gt;" 
                                                                        UniqueName="Preview">
                                                                        <HeaderStyle Width="20px" />
                                                                        <ItemStyle Width="20px" />
                                                                    </telerik:GridButtonColumn>
                                                                </Columns>
                                                                <EditFormSettings>
                                                                    <EditColumn FilterControlAltText="Filter EditCommandColumn column">
                                                                    </EditColumn>
                                                                </EditFormSettings>
                                                            </MasterTableView>
                                                            <FilterMenu EnableImageSprites="False">
                                                            </FilterMenu>
                                                            <HeaderContextMenu CssClass="GridContextMenu GridContextMenu_Default">
                                                            </HeaderContextMenu>
                                                        </telerik:RadGrid>
                                                    </div>
                                                </div>
                                                </td>
                                            </tr>
                                            <tr class="GridFooter">
                                                <td align="center">
                                                    <telerik:RadButton ID="ConfermaButton" runat="server" Text="Ok" Width="100px" 
                                                        Skin="Office2007">
                                                        <Icon PrimaryIconUrl="images/checks.png" PrimaryIconLeft="5px" />
                                                    </telerik:RadButton>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>


              <asp:Panel ID="InserimentoCodicePinPanel" runat="server">


                  <table width="700px" cellpadding="5" cellspacing="5" border="0">
                      <tr>
                          <td>
                              <table class="ContainerWrapper" border="0" cellpadding="2" cellspacing="0" width="100%">

                                  <%--  HEADER--%>
                                  <tr>
                                      <td style="background-color: #BFDBFF; padding: 4px; height: 25px; border-bottom: 1px solid  #9ABBE8; border-top: 1px solid  #9ABBE8">
                                          <table style="width: 100%;">
                                              <tr>
                                                  <td align="left">&nbsp;<asp:Label ID="TitoloCodiceAttivazioneLabel" runat="server" Style="color: #00156E" Font-Bold="True"
                                                      Text="Inserisci Codice Attivazione" />
                                                  </td>


                                              </tr>

                                          </table>

                                      </td>

                                  </tr>

                                  <%-- CONTENT--%>
                                  <tr>
                                      <td class="ContainerMargin">

                                          <div id="CodicePinPanel" runat="server" style="padding: 2px 0px 0px 0px;">

                                              <table style="width: 100%; background-color: #BFDBFF; border: 1px solid #5D8CC9">


                                                  <tr>
                                                      <td>
                                                          <table cellpadding="0" cellspacing="0" width="100%" border="0"
                                                              style="background-color: #BFDBFF">
                                                              <tr>
                                                                  <td style="background-color: #FFFFFF; border: 1px solid #5D8CC9">
                                                                      <table style="width: 100%">
                                                                          <tr>
                                                                              <td align="left">&nbsp;<asp:Label ID="CodiceAttivazioneLabel" runat="server"  
                                                                                  Text="Inserisci il codice di attivazione inviato al numero di telefono da te fornito" />
                                                                              </td>
                                                                             
                                                                          </tr>
                                                                          <tr style="height:12px">
                                                                            <td></td>
                                                                          </tr>
                                                                          <tr>
                                                                              <td>
                                                                                  <table style="width:100%">
                                                                                      <tr>
                                                                                          <td align="left">&nbsp;<asp:Label ID="Label3" runat="server"
                                                                                              Text="Codice PIN" />
                                                                                          </td>
                                                                                          <td align="right">
                                                                                              <telerik:RadButton ID="RinviaCodiceButton" runat="server" Text="Rinvia Codice" Width="100px" ToolTip="Cliccando il pulsante riceverai un SMS contenente il codice PIN"
                                                                                                  Skin="Office2007">
                                                                                                 
                                                                                              </telerik:RadButton>
                                                                                          </td>
                                                                                      </tr>
                                                                                  </table>

                                                                              </td>
                                                                              
                                                                          </tr>

                                                                          <tr>
                                                                              <td align="left">
                                                                                    <telerik:RadTextBox ID="OtpTextBox" runat="server" Width="400px">
                                                                                    </telerik:RadTextBox>
                                                                              </td>
                                                                          </tr>

                                                                           <tr style="height:30px">
                                                                               <td align="left">&nbsp;<asp:Label ID="MessaggioErroreLabel" runat="server"  style="font-size: 16px"
                                                                                  Text="" />
                                                                              </td>
                                                                          </tr>

                                                                      </table>
                                                                  </td>
                                                              </tr>
                                                          </table>
                                                    </td>
                                                  </tr>




                                              </table>

                                          </div>



                                      </td>
                                  </tr>

                                  <tr>
                                      <td align="center" style="background-color: #BFDBFF; padding: 4px; border-bottom: 0px solid  #9ABBE8; border-top: 1px solid  #9ABBE8; height: 25px">
                                                                                              <telerik:RadButton ID="ConfermaOTPButton" runat="server" Text="Conferma" Width="100px" 
                                                        Skin="Office2007">
                                                       
                                                    </telerik:RadButton>
                                                
                                      </td>
                                  </tr>

                              </table>
                          </td>
                      </tr>
                  </table>



              </asp:Panel>

          

        </div>



    </center>


    </form>

     <script type="text/javascript">

         function RefreshImage() {
             var captcha = $find("<%=RadCaptcha1.ClientID %>");
            document.location = $get(captcha.get_id() + "_CaptchaLinkButton").href;
        }
  
    </script>

  
</body>
</html>
