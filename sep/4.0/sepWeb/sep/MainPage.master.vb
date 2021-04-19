#Region "namespaces importati"

Imports System.Data.Entity
Imports ParsecAdmin
Imports System.Globalization

#End Region

Partial Class MainPage
    Inherits System.Web.UI.MasterPage

#Region "Dichiarazioni"

    Public Delegate Sub ToolbarEvent(ByVal sender As Object, ByVal e As EventArgs)
    Public Event OnNew As ToolbarEvent
    Public Event OnModify As ToolbarEvent
    Private infoVociMenu As List(Of ParsecAdmin.MenuitemInfo)
    Public Property ExpressDate As String = String.Empty
    Public Property LoginDate As String = String.Empty
    Public Property AttivaNotifica As String = CBool(ParsecAdmin.WebConfigSettings.GetKey("AttivaNotifica")).ToString.ToLower

#End Region

#Region "Property"



    Public ReadOnly Property GetSegnalazioni As String
        Get
            Dim res As String = "0"
            Try
               
               If Not Session("LoginEseguito") Is Nothing Then
                    Session("LoginEseguito") = Nothing

                    Dim moduli As New ParsecAdmin.ModuleRepository
                    Dim modulo = moduli.Where(Function(c) c.Id = ParsecAdmin.TipoModulo.WBT).FirstOrDefault
                    moduli.Dispose()

                    If Not modulo Is Nothing Then
                        If modulo.Abilitato Then
                            Dim utenteCorrente As ParsecAdmin.Utente = ParsecUtility.Applicazione.UtenteCorrente
                            Dim parametri As New ParsecAdmin.ParametriSegnalazioneRepository
                            Dim parametro = parametri.GetQuery.FirstOrDefault
                            parametri.Dispose()
                            If parametro.IdDestinatarioIter = utenteCorrente.Id Then
                                res = "1"
                            End If
                        End If
                    End If
                End If


            Catch ex As Exception

            End Try


            Return res
        End Get
       
    End Property




    Public Property SessioneScaduta As Boolean
        Get
            If ViewState("SessioneScaduta") Is Nothing Then
                ViewState("SessioneScaduta") = False
            End If
            Return ViewState("SessioneScaduta")
        End Get
        Set(ByVal value As Boolean)
            ViewState("SessioneScaduta") = value
        End Set
    End Property

    Public Property NomeModulo As String
        Set(ByVal value As String)
            Me.NomeModuloLabel.Text = value
        End Set
        Get
            Return Me.NomeModuloLabel.Text
        End Get
    End Property

    Public Property DescrizioneProcedura As String
        Set(ByVal value As String)
            Me.DescrizioneProceduraLabel.Text = value
        End Set
        Get
            Return Me.DescrizioneProceduraLabel.Text
        End Get
    End Property

    Public ReadOnly Property HiddenElimina As HiddenField
        Get
            Return CType(Me.FindControl("hflVerificaElimina"), HiddenField)
        End Get
    End Property

    Public ReadOnly Property Content As ContentPlaceHolder
        Get
            Return CType(Me.FindControl("MainContent"), ContentPlaceHolder)
        End Get
    End Property

#End Region

#Region "Eventi della pagina"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        'If CheckForSessionTimeout() Then
        '    SessioneScaduta = True
        '    Response.Redirect("~/TimeoutPage.aspx")
        'End If
        If ParsecUtility.Applicazione.UtenteCorrente Is Nothing Then
            SessioneScaduta = True
            Response.Redirect("~/TimeoutPage.aspx")
        End If
        'Dim cc = Request.Cookies(FormsAuthentication.FormsCookieName)
        'Dim ticket = FormsAuthentication.Decrypt(cc.Value)
        'QUESTA RIGA RIATTIVA IL FORMS AUTHENTICATION TIMEOUT (ticket + cookie)
        'System.Web.Security.FormsAuthentication.SetAuthCookie(CType(ParsecUtility.Applicazione.UtenteCorrente, ParsecAdmin.Utente).Nome, False)
        'Dim d As DateTime = DateTime.Now
        'LoginDate = d.ToString("u", DateTimeFormatInfo.InvariantInfo).Replace("Z", "")
        'Dim sessionTimeout As Integer = Session.Timeout - 1
        'Dim dateExpress As DateTime = d.AddMinutes(sessionTimeout)
        'ExpressDate = dateExpress.ToString("u", DateTimeFormatInfo.InvariantInfo).Replace("Z", "")
        If Not Me.Page.IsPostBack Then
            Dim risorsa As String = HttpContext.Current.Request.Url.AbsolutePath
            Me.RegistraEvento("RICHIESTA RISORSA: '" & risorsa.ToUpper & "'", TipoEventoLog.Timeout)
        End If

        If CultureInfo.InvariantCulture.DateTimeFormat.TimeSeparator = "." Or CultureInfo.CurrentCulture.DateTimeFormat.TimeSeparator = "." Or CultureInfo.CurrentUICulture.DateTimeFormat.TimeSeparator = "." Then
            Dim ci As New CultureInfo("it-IT", True)
            ci.DateTimeFormat.TimeSeparator = ":"
            System.Threading.Thread.CurrentThread.CurrentCulture = ci
            System.Threading.Thread.CurrentThread.CurrentUICulture = ci
        End If

      End Sub



    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        If ParsecUtility.SessioneCorrente.IsExpired Then
            Session.Abandon()
            FormsAuthentication.SignOut()
            FormsAuthentication.RedirectToLoginPage()
        End If
        'Dim pageUrl As String = "~/LogoutPage.aspx"
        'ParsecUtility.Utility.ExpireSession(pageUrl)
        Dim utente As ParsecAdmin.Utente = ParsecUtility.Applicazione.UtenteCorrente
        Dim cliente As ParsecAdmin.Cliente = ParsecUtility.Applicazione.ClienteCorrente
        Dim distaccamento As ParsecAdmin.Distaccamento = ParsecUtility.Applicazione.DistaccamentoCorrente
        Dim modulo As ParsecAdmin.Modulo = ParsecUtility.Applicazione.ModuloCorrente
        Dim idModulo As Integer = modulo.Id

        If modulo.Abilitato Then
            Me.LoadMenu(utente.Id, idModulo)
        End If

        Me.PannelloContenitore.Visible = (modulo.Id = 11 Or modulo.Id = 4 Or modulo.Id = 15)
        If Not IsPostBack AndAlso (modulo.Id = 11 OrElse modulo.Id = 4 OrElse modulo.Id = 15 OrElse modulo.Id = 5) Then
            'Me.CaricaAnniEsercizio()
            'Me.CaricaDistretti(utente.Id)
            If modulo.Id = 4 Then
                AnnoEsecizioLabel.Visible = False
                AnnoEsecizioComboBox.Visible = False

                DistrettoLabel.Text = "Comune"
                With DistrettoComboBox
                    .Width = 220
                    .Enabled = utente.SuperUser
                End With
            End If
            If modulo.Id = 15 Then
                DistrettoLabel.Visible = False
                DistrettoComboBox.Visible = False
            End If
            'If modulo.Id = 5 Then
            '    Response.Redirect("~/UI/PraticheEdilizie/pages/user/GestioneCalendarioAppuntamenti.aspx")
            'End If
        End If
        Dim moduleRepository As New ParsecAdmin.ModuleRepository
        Dim moduli As List(Of ParsecAdmin.Modulo) = moduleRepository.GetModuliVisibili()
        ToolBarModuli.DataSource = moduli
        ToolBarModuli.DataBind()
        moduleRepository.Dispose()
        Me.DescrizioneUtenteLabel.Text = utente.Titolo + " " + utente.Nome + " " + utente.Cognome
        Me.DescrizioneClienteLabel.Text = cliente.Descrizione
        Me.DataCorrenteLabel.Text = Date.Now.ToLongDateString
    End Sub

    Protected Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Unload
        Dim utente As ParsecAdmin.Utente = ParsecUtility.Applicazione.UtenteCorrente
        Dim signout As Boolean = False
        If utente Is Nothing Then
            signout = True
        End If
        'Se l'utente corrente è stato disconnesso tramite la pagina UtentiConnessiPage 
        'o se la sessione corrente è scaduta
        'La sessione non è scaduta
        If Not SessioneScaduta Then
            If Not ParsecUtility.UtentiConnessi.Items Is Nothing Then
                If Not ParsecUtility.UtentiConnessi.Items.ContainsKey(utente.Id) Then
                    signout = True
                End If
            End If
        End If
       
    End Sub

#End Region

#Region "Eventi degli oggetti della pagina"

    Protected Sub AnnoEsecizioComboBox_SelectedIndexChanged(ByVal sender As Object, ByVal e As Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs) Handles AnnoEsecizioComboBox.SelectedIndexChanged
        ParsecUtility.Applicazione.AnnoCorrente = CInt(AnnoEsecizioComboBox.SelectedItem.Value)
    End Sub

    Protected Sub DistrettoComboBox_SelectedIndexChanged(ByVal sender As Object, ByVal e As Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs) Handles DistrettoComboBox.SelectedIndexChanged
        Dim detachmentRepository As New ParsecAdmin.DetachmentRepository
        Dim idDistaccamento As Integer = CInt(DistrettoComboBox.SelectedItem.Value)
        Dim distaccamento As ParsecAdmin.Distaccamento = detachmentRepository.GetQuery.Where(Function(c) c.Id = idDistaccamento).FirstOrDefault
        ParsecUtility.Applicazione.DistaccamentoCorrente = distaccamento
        'Session("BolletteFiltrate" + CType(Session("CurrentUser"), Utente).Id.ToString) = Nothing
        detachmentRepository.Dispose()
        Try
            Response.Redirect("../../../../default.aspx")
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub LogoutButton_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles LogoutButton.Click
        Dim utente As ParsecAdmin.Utente = ParsecUtility.Applicazione.UtenteCorrente
        Me.RegistraEvento("LOGOUT RIUSCITO", TipoEventoLog.Logout)
        ParsecUtility.UtentiConnessi.Delete(utente.Id)
        Me.SbloccaRegistrazioniUtentiNonCollegati()
        Me.SbloccaDocumentiUtentiNonCollegati()
        Me.SbloccaEmailsUtentiNonCollegati()
        Me.SbloccaCaselleEmailUtentiNonCollegati()
        Me.SbloccaTasksUtentiNonCollegati()

        
        Session.Abandon()

        FormsAuthentication.SignOut()
        FormsAuthentication.RedirectToLoginPage()

       
    End Sub

    Protected Sub ToolBarModuli_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles ToolBarModuli.ItemCommand
        Dim utente As ParsecAdmin.Utente = ParsecUtility.Applicazione.UtenteCorrente
        Dim modulo As ParsecAdmin.Modulo = ParsecUtility.Applicazione.ModuloCorrente
        Dim moduleId As Integer = CInt(e.CommandArgument)
        If modulo.Id <> moduleId Then
            ' LoadMenu(utente.Id, moduleId)
            Dim moduleRepository As New ParsecAdmin.ModuleRepository
            Dim newModule As ParsecAdmin.Modulo = moduleRepository.GetQuery.Where(Function(c) c.Id = moduleId).FirstOrDefault
            ParsecUtility.Applicazione.ModuloCorrente = newModule
            moduleRepository.Dispose()
            'If newModule.Abilitato Then
            Response.Redirect(newModule.HomePage)
            'Else
            '    Response.Redirect(newModule.HomePage & "?a=0")
            'End If

        End If
    End Sub

    'Protected Sub OnToolbarButtonCommand(sender As Object, e As CommandEventArgs)
    '    Dim utente As ParsecAdmin.Utente = ParsecUtility.Applicazione.UtenteCorrente
    '    Dim modulo As ParsecAdmin.Modulo = ParsecUtility.Applicazione.ModuloCorrente
    '    Dim moduleId As Integer = CInt(e.CommandArgument)
    '    If modulo.Id <> moduleId Then
    '        ' LoadMenu(utente.Id, moduleId)
    '        Dim moduleRepository As New ParsecAdmin.ModuleRepository
    '        Dim newModule As ParsecAdmin.Modulo = moduleRepository.GetQuery.Where(Function(c) c.Id = moduleId).FirstOrDefault
    '        ParsecUtility.Applicazione.ModuloCorrente = newModule
    '        moduleRepository.Dispose()
    '        Response.Redirect(newModule.HomePage)
    '    End If
    'End Sub

    Protected Sub ToolBarModuli_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles ToolBarModuli.ItemDataBound
        Dim utente As ParsecAdmin.Utente = ParsecUtility.Applicazione.UtenteCorrente
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            'Dim moduliProfiliAssociatiAdUtente As List(Of Integer) = (New ParsecAdmin.ProfileRepository).GetModuliProfiloAbilitati(utente.Id)
            'If moduliProfiliAssociatiAdUtente.Contains(CType(e.Item.DataItem, ParsecAdmin.Modulo).Id) Then
            '    'CType(e.Item.FindControl("openModule"), Telerik.Web.UI.RadButton).Attributes.Add("onclick", "window.location.href='';window.location.href='" & CType(e.Item.DataItem, ParsecAdmin.Modulo).HomePage & "';")
            '    'window.location.replace(""); 
            'End If
            CType(e.Item.FindControl("openModule"), Telerik.Web.UI.RadButton).Text = CType(e.Item.DataItem, ParsecAdmin.Modulo).Descrizione
            CType(e.Item.FindControl("openModule"), Telerik.Web.UI.RadButton).ToolTip = CType(e.Item.DataItem, ParsecAdmin.Modulo).DescrizioneDettagliata

        End If
    End Sub

#End Region

#Region "Metodi"

    Private Sub BuildMenu(ByVal panelItem As Telerik.Web.UI.RadPanelItem, ByVal menuItemInfos As List(Of ParsecAdmin.MenuitemInfo), ByVal level As Integer)
        If Not menuItemInfos Is Nothing Then
            For Each menuItem In menuItemInfos
                Dim currentId As Integer = menuItem.Id
                Dim enabledMenuItems = (From mi In Me.infoVociMenu Where (mi.Visible <> -1 Or (mi.Visible = -1 And mi.ProcedureID = -1)) And mi.ParentID = currentId).ToList
                Dim childItem As New Telerik.Web.UI.RadPanelItem(menuItem.Description)
                childItem.NavigateUrl = menuItem.Url

                childItem.ImageUrl = menuItem.Icon
                If (menuItem.Visible <> -1 AndAlso menuItem.ProcedureID <> -1) OrElse (menuItem.ProcedureID = -1 AndAlso enabledMenuItems.Count > 0) Then
                    If menuItem.ParentID = 0 Then RadPanelBar1.Items.Add(childItem) Else panelItem.Items.Add(childItem)
                End If
                If enabledMenuItems.Count > 0 Then Me.BuildMenu(childItem, menuItem.Children, level + 1)
            Next
        End If
    End Sub

    Private Sub CaricaDistretti(ByVal idUtente As Integer)
        Dim detachmentRepository As New ParsecAdmin.DetachmentRepository
        Me.DistrettoComboBox.DataTextField = "Descrizione"
        Me.DistrettoComboBox.DataValueField = "Id"
        Dim distaccamenti As List(Of ParsecAdmin.Distaccamento) = detachmentRepository.GetDistaccamenti(idUtente)
        If distaccamenti.Count > 0 Then
            Me.DistrettoComboBox.DataSource = detachmentRepository.GetDistaccamenti(idUtente)
            Me.DistrettoComboBox.DataBind()
            If ParsecUtility.Applicazione.DistaccamentoCorrente Is Nothing Then
                Me.DistrettoComboBox.FindItemByValue(detachmentRepository.GetIdDistaccamentoPredefinito(idUtente)).Selected = True
                Dim idDistaccamento As Integer = CInt(DistrettoComboBox.SelectedItem.Value)
                ParsecUtility.Applicazione.DistaccamentoCorrente = detachmentRepository.GetQuery.Where(Function(c) c.Id = idDistaccamento).FirstOrDefault
            Else
                Me.DistrettoComboBox.FindItemByValue(CType(ParsecUtility.Applicazione.DistaccamentoCorrente, Distaccamento).Id).Selected = True
            End If

        End If
        detachmentRepository.Dispose()
    End Sub

    Private Sub CaricaAnniEsercizio()
        Dim annoEsecizioRepository As New ParsecAdmin.AnnoEsecizioRepository
        Me.AnnoEsecizioComboBox.DataTextField = "Anno"
        Me.AnnoEsecizioComboBox.DataValueField = "Anno"
        Me.AnnoEsecizioComboBox.DataSource = annoEsecizioRepository.GetAnni
        Me.AnnoEsecizioComboBox.DataBind()
        If ParsecUtility.Applicazione.AnnoCorrente = 0 Then
            Dim annoCorrente As Integer = annoEsecizioRepository.GetAnnoEsercizioCorrente
            Me.AnnoEsecizioComboBox.FindItemByValue(annoCorrente).Selected = True
            ParsecUtility.Applicazione.AnnoCorrente = annoCorrente
        Else
            Me.AnnoEsecizioComboBox.FindItemByValue(ParsecUtility.Applicazione.AnnoCorrente).Selected = True
        End If

        annoEsecizioRepository.Dispose()
    End Sub

    Public Sub CaricaAnniEsercizio(dataSource As Object, textField As String, valueField As String, valueSelected As String)
        Me.AnnoEsecizioComboBox.DataTextField = textField
        Me.AnnoEsecizioComboBox.DataValueField = valueField
        Me.AnnoEsecizioComboBox.DataSource = dataSource
        Me.AnnoEsecizioComboBox.DataBind()
        Me.AnnoEsecizioComboBox.FindItemByValue(valueSelected).Selected = True
    End Sub

    Public Function CheckForSessionTimeout() As Boolean
        If (Not Context.Session Is Nothing AndAlso Context.Session.IsNewSession) Then
            Dim cookieHeader As String = Page.Request.Headers("Cookie")
            If Not cookieHeader Is Nothing AndAlso cookieHeader.IndexOf("ASP.NET_SessionId") >= 0 Then
                Return True
            Else
                Return False
            End If
        Else
            Return False
        End If
    End Function

    Private Sub LoadMenu(ByVal userId As Integer, ByVal moduleId As Integer)
        Dim menuitemRepository As New ParsecAdmin.MenuItemRepository
        Me.infoVociMenu = menuitemRepository.GetInfoMenuItems(userId, moduleId)
        Dim menu As New ParsecAdmin.HieararchyMenu
        Dim menuItemInfos As List(Of ParsecAdmin.MenuitemInfo) = menu.Build(Me.infoVociMenu)
        Me.RadPanelBar1.Items.Clear()
        Me.BuildMenu(Nothing, menuItemInfos, 0)
        menuitemRepository.Dispose()
    End Sub

    Public Sub RegisterComponent(ByVal script As String)
        Dim cell As New TableCell
        cell.Width = New Unit(30)
        cell.Controls.Add(New LiteralControl(script))
        Me.componentPlaceHolder.Rows(0).Cells.Add(cell)
    End Sub

    Public Sub RegisterScript(ByVal script As String)
        Me.scriptPlaceHolder.Controls.Add(New LiteralControl(script))
    End Sub

    Private Sub RegistraEvento(ByVal descrizione As String, tipologia As ParsecAdmin.TipoEventoLog)
        Dim logs As New ParsecAdmin.LogEventoRepository
        Dim log As New ParsecAdmin.LogEvento
        log.Descrizione = descrizione.ToUpper
        log.IdTipolgia = tipologia
        logs.Save(log)
        logs.Dispose()
    End Sub

    Private Sub SbloccaCaselleEmailUtentiNonCollegati()
        If Not ParsecUtility.UtentiConnessi.Items Is Nothing Then
            Dim caselleEmail As New ParsecPro.LockCasellaEmailRepository
            Dim idUtenti = caselleEmail.GetQuery.Select(Function(c) c.IdUtente).ToList
            For Each idUtente In idUtenti
                If Not ParsecUtility.UtentiConnessi.Items.ContainsKey(idUtente) Then
                    caselleEmail.DeleteAll(idUtente)
                End If
            Next
            caselleEmail.Dispose()
        End If
    End Sub

    Private Sub SbloccaDocumentiUtentiNonCollegati()
        If Not ParsecUtility.UtentiConnessi.Items Is Nothing Then
            Dim documentiBloccati As New ParsecAtt.LockDocumentoRepository
            Dim idUtenti = documentiBloccati.GetQuery.Select(Function(c) c.IdUtente).ToList
            For Each idUtente In idUtenti
                If Not ParsecUtility.UtentiConnessi.Items.ContainsKey(idUtente) Then
                    documentiBloccati.DeleteAll(idUtente)
                End If
            Next
            documentiBloccati.Dispose()
        End If
    End Sub

    Private Sub SbloccaEmailsUtentiNonCollegati()
        If Not ParsecUtility.UtentiConnessi.Items Is Nothing Then
            Dim emails As New ParsecPro.EmailArrivoRepository
            Dim emailBoccate = emails.GetQuery.Where(Function(c) c.Lockata = True).ToList
            For Each email In emailBoccate
                If Not ParsecUtility.UtentiConnessi.Items.ContainsKey(email.IdUtenteLock) Then
                    email.Lockata = False
                    emails.SaveChanges()
                End If
            Next
            emails.Dispose()
        End If
    End Sub

    Private Sub SbloccaRegistrazioniUtentiNonCollegati()
        If Not ParsecUtility.UtentiConnessi.Items Is Nothing Then
            Dim registrazioniBloccate As New ParsecPro.LockRegistrazioneRepository
            Dim idUtenti = registrazioniBloccate.GetQuery.Select(Function(c) c.IdUtente).ToList
            For Each idUtente In idUtenti
                If Not ParsecUtility.UtentiConnessi.Items.ContainsKey(idUtente) Then
                    registrazioniBloccate.DeleteAll(idUtente)
                End If
            Next
            registrazioniBloccate.Dispose()
        End If
    End Sub

    Private Sub SbloccaTasksUtentiNonCollegati()
        If Not ParsecUtility.UtentiConnessi.Items Is Nothing Then

            Dim taskBloccati As New ParsecWKF.LockTaskRepository
            Dim idUtenti = taskBloccati.GetQuery.Select(Function(c) c.IdUtente).ToList
            For Each idUtente In idUtenti
                If Not ParsecUtility.UtentiConnessi.Items.ContainsKey(idUtente) Then
                    taskBloccati.DeleteAll(idUtente)
                End If
            Next
            taskBloccati.Dispose()
        End If
    End Sub

#End Region

End Class