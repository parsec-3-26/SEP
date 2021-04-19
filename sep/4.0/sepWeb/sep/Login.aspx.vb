Imports ParsecAdmin
Imports RS

Partial Class Login
    Inherits System.Web.UI.Page


    Private Enum TipoPannello
        Aggiornamento = 0
        Login = 1
        Avvisi = 2
        Otp = 3
    End Enum


    Public Property Avvisi() As List(Of ParsecAdmin.Avviso)
        Get
            Return CType(Session(CStr(ViewState("Avvisi_Ticks"))), Object)
        End Get
        Set(ByVal value As List(Of ParsecAdmin.Avviso))
            If ViewState("Avvisi_Ticks") Is Nothing Then
                ViewState("Avvisi_Ticks") = "Avvisi_" & Now.Ticks.ToString
            End If
            Session(CStr(ViewState("Avvisi_Ticks"))) = value
        End Set
    End Property


    Public Property NumeroTentativiOtp() As Integer
        Get
            Return CInt(Session(CStr(ViewState("NumeroTentativiOtp"))))
        End Get
        Set(ByVal value As Integer)
            If ViewState("NumeroTentativiOtp") Is Nothing Then
                ViewState("NumeroTentativiOtp") = "NumeroTentativiOtp_" & Now.Ticks.ToString
            End If
            Session(CStr(ViewState("NumeroTentativiOtp"))) = value
        End Set
    End Property


    Private Function AutenticaUtente() As ParsecAdmin.Utente

        Dim chk As CheckBox = CType(Me.Login1.FindControl("UsaUtenteWindows"), CheckBox)

        Dim userRepository As New ParsecAdmin.UserRepository

        Dim utente As ParsecAdmin.Utente = Nothing

        If chk.Checked AndAlso chk.Visible Then

            Dim username As TextBox = CType(Me.Login1.FindControl("UserName"), TextBox)
            Dim password As TextBox = CType(Me.Login1.FindControl("Password"), TextBox)
            Dim failureText As Literal = CType(Me.Login1.FindControl("FailureText"), Literal)
            Dim winFailureText As Literal = CType(Me.Login1.FindControl("WinFailureText"), Literal)

            Try
                Dim ldapPath = ParsecAdmin.WebConfigSettings.GetKey("LdapPath")
                Dim ldapDomain = ParsecAdmin.WebConfigSettings.GetKey("LdapDomain")

                Dim windowsUser As New ParsecAdmin.LdapWindowsUser(ldapPath)
                If windowsUser.Authenticate(ldapDomain, username.Text, password.Text) Then
                    Dim utenteSep = userRepository.Where(Function(c) c.GuidUtenteWindows = windowsUser.ObjectGuid And c.LogTipoOperazione Is Nothing).FirstOrDefault
                    If Not utenteSep Is Nothing Then
                        utente = userRepository.AuthenticateWindowsUser(utenteSep.Username)
                        Me.Login1.UserName = utenteSep.Username
                    End If
                End If
            Catch ex As Exception
                winFailureText.Text = ex.Message
                failureText.Visible = False
            End Try

        Else
            utente = userRepository.Authenticate(Me.Login1.UserName, Me.Login1.Password)
        End If

        userRepository.Dispose()

        Return utente

    End Function

    'Protected Sub Login1_Authenticate(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.AuthenticateEventArgs) Handles Login1.Authenticate

    '    Dim clientRepository As New ParsecAdmin.ClientRepository
    '    Dim detachmentRepository As New ParsecAdmin.DetachmentRepository
    '    Dim openHomePage As Boolean = False

    '    'Dim isLogged = (Not System.Web.HttpContext.Current.User Is Nothing) AndAlso System.Web.HttpContext.Current.User.Identity.IsAuthenticated
    '    'If isLogged Then
    '    '    Response.Redirect("~/AccessoNegatoPage.aspx")
    '    'End If


    '    Dim utente As ParsecAdmin.Utente = Me.AutenticaUtente


    '    If Not utente Is Nothing Then


    '        Dim distaccamentoUtente As New ParsecAdmin.UtenteDistaccamentoRepository

    '        Dim distaccamento As ParsecAdmin.UtenteDistaccamento = distaccamentoUtente.GetQuery.Where(Function(c) c.IdUtente = utente.Id And c.DistaccamentoPredefinito = True).FirstOrDefault
    '        If Not distaccamento Is Nothing Then
    '            ParsecUtility.Applicazione.DistaccamentoCorrente = (New ParsecAdmin.DetachmentRepository).GetQuery.Where(Function(c) c.Id = distaccamento.IdDistaccamento).FirstOrDefault
    '        End If

    '        Dim cliente As ParsecAdmin.Cliente = clientRepository.GetQuery.FirstOrDefault
    '        ParsecUtility.Applicazione.ClienteCorrente = cliente



    '        Dim moduleRepository As New ParsecAdmin.ModuleRepository
    '        Dim modulo As ParsecAdmin.Modulo = moduleRepository.GetQuery.Where(Function(c) c.Id = utente.IdModuloDefault).FirstOrDefault

    '        ParsecUtility.Applicazione.UtenteCorrente = utente
    '        ParsecUtility.Applicazione.ModuloCorrente = modulo

    '        If utente.PswNonSettata Then
    '            FormsAuthentication.SetAuthCookie(Me.Login1.UserName, False)
    '            Response.Redirect("ImpostaPasswordPage.aspx")
    '        Else

    '            If Me.VerificaPasswordScaduta(utente) Then
    '                FormsAuthentication.SetAuthCookie(Me.Login1.UserName, False)
    '                Response.Redirect("~/UI/Amministrazione/pages/user/OpzioniUtentePage.aspx" & "?pswexp=1")
    '            Else
    '                If utente.Bloccato Then
    '                    FormsAuthentication.SetAuthCookie(Me.Login1.UserName, False)
    '                    Response.Redirect("~/AccessoNegatoPage.aspx")
    '                Else

    '                    openHomePage = True
    '                    Me.SbloccaDocumenti(utente.Id)
    '                    Me.SbloccaRegistrazioni(utente.Id)
    '                    Me.SbloccaEmails(utente.Id)
    '                    Me.SbloccaCaselleEmail(utente.Id)
    '                    Me.SbloccaTasks(utente.Id)

    '                    Me.SbloccaRegistrazioniUtentiNonCollegati()
    '                    Me.SbloccaDocumentiUtentiNonCollegati()
    '                    Me.SbloccaEmailsUtentiNonCollegati()
    '                    Me.SbloccaCaselleEmailUtentiNonCollegati()
    '                    Me.SbloccaTasksUtentiNonCollegati()


    '                    'Avvio il rilevamento della scadenza della sessione.
    '                    ParsecUtility.SessioneCorrente.Initialize()


    '                    FormsAuthentication.SetAuthCookie(Me.Login1.UserName, False)


    '                    'Dim authTicket As New FormsAuthenticationTicket(2, Me.Login1.UserName, Now, Now.AddYears(14), True, "")
    '                    'Dim encryptedTicket = FormsAuthentication.Encrypt(authTicket)
    '                    'Dim cookie As New HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
    '                    'cookie.Expires = authTicket.Expiration
    '                    'HttpContext.Current.Response.Cookies.Set(cookie)


    '                    'Dim cc = Request.Cookies(FormsAuthentication.FormsCookieName)
    '                    'Dim ticket = FormsAuthentication.Decrypt(cc.Value)

    '                    ''Dim userAgent = Request.UserAgent.ToLower()

    '                    ''Dim aperturaHomePagePopup As Boolean = True

    '                    ''Try
    '                    ''    Boolean.TryParse(ParsecAdmin.WebConfigSettings.GetKey("AperturaHomePagePopup"), aperturaHomePagePopup)
    '                    ''Catch ex As Exception

    '                    ''End Try

    '                    ' ''If userAgent.ToLower.Contains("mozilla") Then
    '                    ''If Not aperturaHomePagePopup Then
    '                    ''    Response.Redirect("~/Default.aspx")
    '                    ''    'Response.Redirect(FormsAuthentication.GetRedirectUrl(Me.Login1.UserName, False))
    '                    ''    'FormsAuthentication.RedirectFromLoginPage(Me.Login1.UserName, False)

    '                    ''Else
    '                    ''    ParsecUtility.Utility.OpenWebSite(Me.scriptHolder)

    '                    ''End If


    '                    'ParsecUtility.UtentiConnessi.Add(utente.Id, utente.Cognome & " " & utente.Nome & "  - Ultimo accesso " & Now.ToLongDateString & " alle ore " & Now.ToShortTimeString)
    '                    ParsecUtility.UtentiConnessi.Add(utente.Id, utente.Cognome & " " & utente.Nome)


    '                    Me.RegistraEvento("LOGIN RIUSCITO", ParsecAdmin.TipoEventoLog.Login)
    '                    Me.RegistraAccessoUtente(utente)


    '                    '******************************************************************************************************************************
    '                    'Verifico il parametro EseguiTaskInBackground PRO
    '                    '******************************************************************************************************************************

    '                    'Dim parametri As New ParsecAdmin.ParametriRepository
    '                    'Dim parametro = parametri.GetByName("EseguiTaskInBackground", ParsecAdmin.TipoModulo.PRO)
    '                    'parametri.Dispose()

    '                    'Dim runJob As Boolean = False

    '                    'If Not parametro Is Nothing Then
    '                    '    runJob = (parametro.Valore = "1")
    '                    'End If


    '                    'If runJob Then

    '                    '    Dim info As ParsecUtility.JobInfo = Nothing

    '                    '    'Il primo utente che avvia l'Application
    '                    '    If Application("JobInfo") Is Nothing Then

    '                    '        Try

    '                    '            'info = New ParsecUtility.JobInfo With {.Lock = New Object, .StartDate = Now, .JobEnd = False, .Notified = False, .UserId = utente.Id}
    '                    '            info = New ParsecUtility.JobInfo With {.Lock = New Object, .JobEnd = False, .Notified = False, .UserId = utente.Id}

    '                    '            info.CurrentTasksToUpdate = ParsecAdmin.WebConfigSettings.GetKey("TaskCorrenteDaAggiornare") '"IN CARICO"

    '                    '            'TASK DA ESEGUIRE
    '                    '            info.TaskToExecute = ParsecAdmin.WebConfigSettings.GetKey("TaskDaEseguire")
    '                    '            info.ActionToExecute = ParsecAdmin.WebConfigSettings.GetKey("ActionDaEseguire")

    '                    '            Dim ruoloDestinatario As String = ParsecAdmin.WebConfigSettings.GetKey("RuoloDestinatarioTask")

    '                    '            Dim ruoli As New ParsecWKF.RuoloRepository
    '                    '            Dim utentiRuolo As New ParsecWKF.RuoloRelUtenteRepository(ruoli.Context)

    '                    '            Dim idUte = (From ruolo In ruoli.GetQuery
    '                    '                         Join utenteRuolo In utentiRuolo.GetQuery
    '                    '                         On ruolo.Id Equals utenteRuolo.IdRuolo
    '                    '                         Where ruolo.Descrizione = ruoloDestinatario
    '                    '                         Select utenteRuolo.IdUtente).FirstOrDefault

    '                    '            ruoli.Dispose()

    '                    '            If idUte.HasValue Then
    '                    '                info.IdConsignee = idUte.Value
    '                    '                Application("JobInfo") = info
    '                    '                runJob = True
    '                    '            Else
    '                    '                runJob = False
    '                    '            End If


    '                    '        Catch ex As Exception
    '                    '            runJob = False
    '                    '        End Try

    '                    '    Else
    '                    '        'info = CType(Application("JobInfo"), ParsecUtility.JobInfo)

    '                    '        'Dim minute = CInt(Now.Subtract(info.StartDate).TotalMinutes)
    '                    '        'If minute >= 1 Then
    '                    '        '    If info.JobEnd Then
    '                    '        '        info.UserId = utente.Id
    '                    '        '        info.StartDate = Now
    '                    '        '        info.JobEnd = False
    '                    '        '        info.Notified = False
    '                    '        '        runJob = True
    '                    '        '    End If

    '                    '        'End If
    '                    '    End If


    '                    '    If runJob Then

    '                    '        'Il thread viene chiuso dall'Application.End ?
    '                    '        Dim th As New System.Threading.Thread(New System.Threading.ParameterizedThreadStart(AddressOf GetFattureScadute))
    '                    '        th.Start(info)
    '                    '    End If

    '                    'End If

    '                    '******************************************************************************************************************************


    '                End If

    '            End If


    '        End If

    '    End If

    '    detachmentRepository.Dispose()
    '    clientRepository.Dispose()

    '    If openHomePage Then
    '        Dim userAgent = Request.UserAgent.ToLower()

    '        Dim aperturaHomePagePopup As Boolean = True

    '        Try
    '            Boolean.TryParse(ParsecAdmin.WebConfigSettings.GetKey("AperturaHomePagePopup"), aperturaHomePagePopup)
    '        Catch ex As Exception

    '        End Try

    '        'If userAgent.ToLower.Contains("mozilla") Then
    '        If Not aperturaHomePagePopup Then
    '            Response.Redirect("~/Default.aspx")
    '            'Response.Redirect(FormsAuthentication.GetRedirectUrl(Me.Login1.UserName, False))
    '            'FormsAuthentication.RedirectFromLoginPage(Me.Login1.UserName, False)

    '        Else
    '            ParsecUtility.Utility.OpenWebSite(Me.scriptHolder)

    '        End If
    '    End If

    'End Sub

    Protected Sub Login1_Authenticate(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.AuthenticateEventArgs) Handles Login1.Authenticate

        Dim clientRepository As New ParsecAdmin.ClientRepository
        Dim detachmentRepository As New ParsecAdmin.DetachmentRepository


        Dim openHomePage As Boolean = False


        'Dim isLogged = (Not System.Web.HttpContext.Current.User Is Nothing) AndAlso System.Web.HttpContext.Current.User.Identity.IsAuthenticated
        'If isLogged Then
        '    Response.Redirect("~/AccessoNegatoPage.aspx")
        'End If


        Dim utente As ParsecAdmin.Utente = Me.AutenticaUtente


        If Not utente Is Nothing Then


            Dim distaccamentoUtente As New ParsecAdmin.UtenteDistaccamentoRepository

            Dim distaccamento As ParsecAdmin.UtenteDistaccamento = distaccamentoUtente.GetQuery.Where(Function(c) c.IdUtente = utente.Id And c.DistaccamentoPredefinito = True).FirstOrDefault
            If Not distaccamento Is Nothing Then
                ParsecUtility.Applicazione.DistaccamentoCorrente = (New ParsecAdmin.DetachmentRepository).GetQuery.Where(Function(c) c.Id = distaccamento.IdDistaccamento).FirstOrDefault
            End If

            Dim cliente As ParsecAdmin.Cliente = clientRepository.GetQuery.FirstOrDefault
            ParsecUtility.Applicazione.ClienteCorrente = cliente



            Dim moduleRepository As New ParsecAdmin.ModuleRepository
            Dim modulo As ParsecAdmin.Modulo = moduleRepository.GetQuery.Where(Function(c) c.Id = utente.IdModuloDefault).FirstOrDefault

            ParsecUtility.Applicazione.UtenteCorrente = utente
            ParsecUtility.Applicazione.ModuloCorrente = modulo

            If utente.PswNonSettata Then
                FormsAuthentication.SetAuthCookie(Me.Login1.UserName, False)
                Response.Redirect("ImpostaPasswordPage.aspx")
            Else

                If Me.VerificaPasswordScaduta(utente) Then
                    FormsAuthentication.SetAuthCookie(Me.Login1.UserName, False)
                    Response.Redirect("~/UI/Amministrazione/pages/user/OpzioniUtentePage.aspx" & "?pswexp=1")
                Else
                    If utente.Bloccato Then
                        FormsAuthentication.SetAuthCookie(Me.Login1.UserName, False)
                        Response.Redirect("~/AccessoNegatoPage.aspx")
                    Else

                        '******************************************************************************************************************************
                        'INIZIO GESTIONE OTP
                        '******************************************************************************************************************************
                        Dim attivazioneAutenticazioneSecondoLivello As Boolean = Me.GetAttivazioneAutenticazioneSecondoLivello
                        Dim mittenteSms As String = Me.GetMittenteSms

                        If attivazioneAutenticazioneSecondoLivello AndAlso Not String.IsNullOrEmpty(mittenteSms) Then

                            Dim ip As String = Me.GetPublicIp()

                            'SE L'INDIRIZZO IP NON E' QUELLO DELL'AMMINISTRAZIONE
                            Dim elencoIndirizziIP As String() = cliente.IndirizzoIP.Split(New Char() {";"}, StringSplitOptions.RemoveEmptyEntries)
                            If Not elencoIndirizziIP.Contains(ip) Then


                                Dim failureText As Literal = CType(Me.Login1.FindControl("FailureText"), Literal)
                                Dim winFailureText As Literal = CType(Me.Login1.FindControl("WinFailureText"), Literal)

                                If Not String.IsNullOrEmpty(utente.Cellulare) Then

                                    Dim durataMinutiOtp As Integer = Me.GetDurataMinutiOtp
                                    Dim lunghezzaOtp As Integer = Me.GetLunghezzaOtp

                                    If utente.DataScadenzaOTP.HasValue Then

                                        'SE LA OTP NON E' SCADUTA
                                        If Now <= utente.DataScadenzaOTP Then
                                            'VISUALIZZO SOLO IL PANNELLO PER CONSENTIRE L'INSERIMENTO DEL CODICE PIN
                                            Me.VisualizzaPannello(TipoPannello.Otp)
                                            Exit Sub
                                        Else
                                            'SE LA OTP E' SCADUTA
                                            Try
                                                'GENERO IL NUOVO CODICE PIN
                                                Dim otp As String = Me.GenerateOTP(lunghezzaOtp)
                                                Dim encryptedOtp = ParsecCommon.CryptoUtil.Encrypt(otp)

                                                'LO INVIO VIA SMS
                                                Me.InviaSms("+39" + utente.Cellulare, mittenteSms, otp)

                                                'AGGIORNO LA TABELLA UTENTE
                                                Dim utenti As New UserRepository
                                                Dim ute = utenti.Where(Function(c) c.Id = utente.Id).FirstOrDefault
                                                ute.DataScadenzaOTP = Now.AddMinutes(durataMinutiOtp)
                                                ute.OTP = encryptedOtp
                                                utenti.SaveChanges()
                                                utenti.Dispose()

                                                Me.MessaggioErroreLabel.Text = "Sms inviato con successo."
                                                Me.MessaggioErroreLabel.ForeColor = Drawing.Color.Green

                                                'VISUALIZZO IL PANNELLO PER CONSENTIRE L'INSERIMENTO DEL CODICE PIN
                                                Me.VisualizzaPannello(TipoPannello.Otp)
                                                Exit Sub

                                            Catch ex As Exception
                                                winFailureText.Text = ex.Message
                                                failureText.Visible = False
                                                Exit Sub
                                            End Try

                                        End If
                                    Else


                                        Try

                                            Dim otp As String = Me.GenerateOTP(lunghezzaOtp)
                                            Dim encryptedOtp = ParsecCommon.CryptoUtil.Encrypt(otp)
                                            Me.InviaSms("+39" + utente.Cellulare, mittenteSms, otp)

                                            'AGGIORNO LA TABELLA UTENTE
                                            Dim utenti As New UserRepository
                                            Dim ute = utenti.Where(Function(c) c.Id = utente.Id).FirstOrDefault
                                            ute.DataScadenzaOTP = Now.AddMinutes(durataMinutiOtp)
                                            ute.OTP = encryptedOtp
                                            utenti.SaveChanges()
                                            utenti.Dispose()

                                            Me.MessaggioErroreLabel.Text = "Sms inviato con successo."
                                            Me.MessaggioErroreLabel.ForeColor = Drawing.Color.Green

                                            Me.VisualizzaPannello(TipoPannello.Otp)
                                            Exit Sub

                                        Catch ex As Exception
                                            winFailureText.Text = ex.Message
                                            failureText.Visible = False
                                            Exit Sub
                                        End Try

                                    End If
                                Else
                                    winFailureText.Text = "L'utente non possiede un numero di telefono."
                                    failureText.Visible = False
                                    Exit Sub
                                End If


                            End If

                        End If

                        '******************************************************************************************************************************
                        'FINE GESTIONE OTP
                        '******************************************************************************************************************************


                        openHomePage = True

                        Me.SbloccaDocumenti(utente.Id)
                        Me.SbloccaRegistrazioni(utente.Id)
                        Me.SbloccaEmails(utente.Id)
                        Me.SbloccaCaselleEmail(utente.Id)
                        Me.SbloccaTasks(utente.Id)

                        Me.SbloccaRegistrazioniUtentiNonCollegati()
                        Me.SbloccaDocumentiUtentiNonCollegati()
                        Me.SbloccaEmailsUtentiNonCollegati()
                        Me.SbloccaCaselleEmailUtentiNonCollegati()
                        Me.SbloccaTasksUtentiNonCollegati()


                        'Avvio il rilevamento della scadenza della sessione.
                        ParsecUtility.SessioneCorrente.Initialize()


                        FormsAuthentication.SetAuthCookie(Me.Login1.UserName, False)


                        'Dim authTicket As New FormsAuthenticationTicket(2, Me.Login1.UserName, Now, Now.AddYears(14), True, "")
                        'Dim encryptedTicket = FormsAuthentication.Encrypt(authTicket)
                        'Dim cookie As New HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
                        'cookie.Expires = authTicket.Expiration
                        'HttpContext.Current.Response.Cookies.Set(cookie)


                        'Dim cc = Request.Cookies(FormsAuthentication.FormsCookieName)
                        'Dim ticket = FormsAuthentication.Decrypt(cc.Value)




                        'ParsecUtility.UtentiConnessi.Add(utente.Id, utente.Cognome & " " & utente.Nome & "  - Ultimo accesso " & Now.ToLongDateString & " alle ore " & Now.ToShortTimeString)
                        ParsecUtility.UtentiConnessi.Add(utente.Id, utente.Cognome & " " & utente.Nome)


                        Me.RegistraEvento("LOGIN RIUSCITO", ParsecAdmin.TipoEventoLog.Login)
                        Me.RegistraAccessoUtente(utente)


                        '******************************************************************************************************************************
                        'Verifico il parametro EseguiTaskInBackground PRO
                        '******************************************************************************************************************************

                        'Dim parametri As New ParsecAdmin.ParametriRepository
                        'Dim parametro = parametri.GetByName("EseguiTaskInBackground", ParsecAdmin.TipoModulo.PRO)
                        'parametri.Dispose()

                        'Dim runJob As Boolean = False

                        'If Not parametro Is Nothing Then
                        '    runJob = (parametro.Valore = "1")
                        'End If


                        'If runJob Then

                        '    Dim info As ParsecUtility.JobInfo = Nothing

                        '    'Il primo utente che avvia l'Application
                        '    If Application("JobInfo") Is Nothing Then

                        '        Try

                        '            'info = New ParsecUtility.JobInfo With {.Lock = New Object, .StartDate = Now, .JobEnd = False, .Notified = False, .UserId = utente.Id}
                        '            info = New ParsecUtility.JobInfo With {.Lock = New Object, .JobEnd = False, .Notified = False, .UserId = utente.Id}

                        '            info.CurrentTasksToUpdate = ParsecAdmin.WebConfigSettings.GetKey("TaskCorrenteDaAggiornare") '"IN CARICO"

                        '            'TASK DA ESEGUIRE
                        '            info.TaskToExecute = ParsecAdmin.WebConfigSettings.GetKey("TaskDaEseguire")
                        '            info.ActionToExecute = ParsecAdmin.WebConfigSettings.GetKey("ActionDaEseguire")

                        '            Dim ruoloDestinatario As String = ParsecAdmin.WebConfigSettings.GetKey("RuoloDestinatarioTask")

                        '            Dim ruoli As New ParsecWKF.RuoloRepository
                        '            Dim utentiRuolo As New ParsecWKF.RuoloRelUtenteRepository(ruoli.Context)

                        '            Dim idUte = (From ruolo In ruoli.GetQuery
                        '                         Join utenteRuolo In utentiRuolo.GetQuery
                        '                         On ruolo.Id Equals utenteRuolo.IdRuolo
                        '                         Where ruolo.Descrizione = ruoloDestinatario
                        '                         Select utenteRuolo.IdUtente).FirstOrDefault

                        '            ruoli.Dispose()

                        '            If idUte.HasValue Then
                        '                info.IdConsignee = idUte.Value
                        '                Application("JobInfo") = info
                        '                runJob = True
                        '            Else
                        '                runJob = False
                        '            End If


                        '        Catch ex As Exception
                        '            runJob = False
                        '        End Try

                        '    Else
                        '        'info = CType(Application("JobInfo"), ParsecUtility.JobInfo)

                        '        'Dim minute = CInt(Now.Subtract(info.StartDate).TotalMinutes)
                        '        'If minute >= 1 Then
                        '        '    If info.JobEnd Then
                        '        '        info.UserId = utente.Id
                        '        '        info.StartDate = Now
                        '        '        info.JobEnd = False
                        '        '        info.Notified = False
                        '        '        runJob = True
                        '        '    End If

                        '        'End If
                        '    End If


                        '    If runJob Then

                        '        'Il thread viene chiuso dall'Application.End ?
                        '        Dim th As New System.Threading.Thread(New System.Threading.ParameterizedThreadStart(AddressOf GetFattureScadute))
                        '        th.Start(info)
                        '    End If

                        'End If

                        '******************************************************************************************************************************


                    End If

                End If


            End If

        End If

        detachmentRepository.Dispose()
        clientRepository.Dispose()


        Session("LoginEseguito") = "1"

        If openHomePage Then
            Dim userAgent = Request.UserAgent.ToLower()

            Dim aperturaHomePagePopup As Boolean = True

            Try
                Boolean.TryParse(ParsecAdmin.WebConfigSettings.GetKey("AperturaHomePagePopup"), aperturaHomePagePopup)
            Catch ex As Exception

            End Try

            'If userAgent.ToLower.Contains("mozilla") Then
            If Not aperturaHomePagePopup Then
                Response.Redirect("~/Default.aspx")
                'Response.Redirect(FormsAuthentication.GetRedirectUrl(Me.Login1.UserName, False))
                'FormsAuthentication.RedirectFromLoginPage(Me.Login1.UserName, False)

            Else
                ParsecUtility.Utility.OpenWebSite(Me.scriptHolder)

            End If
        End If

    End Sub



    Private Sub RegistraEvento(ByVal descrizione As String, tipologia As ParsecAdmin.TipoEventoLog)
        Dim logs As New ParsecAdmin.LogEventoRepository
        Dim log As New ParsecAdmin.LogEvento
        log.Descrizione = descrizione.ToUpper
        log.IdTipolgia = tipologia
        logs.Save(log)
        logs.Dispose()
    End Sub

    Private Sub RegistraAccessoUtente(ByVal utente As ParsecAdmin.Utente)
        Dim logs As New ParsecAdmin.LogUltimiAccessiUtenteRepository
        Dim log = logs.GetQuery.Where(Function(c) c.IdUtente = utente.Id).FirstOrDefault
        If log Is Nothing Then
            log = New ParsecAdmin.LogUltimiAccessiUtente
            log.DataPenultimoAccesso = Now
            log.DataUltimoAccesso = Now
            log.IdUtente = utente.Id
        Else
            log.DataUltimoAccesso = Now
        End If
        logs.Save(log)
        logs.Dispose()
    End Sub

    Private Sub SbloccaEmails(idUtente As Integer)
        Dim emails As New ParsecPro.EmailArrivoRepository
        Dim emailBoccate = emails.GetQuery.Where(Function(c) c.IdUtenteLock = idUtente And c.Lockata = True).ToList
        For Each email In emailBoccate
            email.Lockata = False
            emails.SaveChanges()
        Next
        emails.Dispose()
    End Sub


    'Private Sub GetFattureScadute(ByVal info As ParsecUtility.JobInfo)


    '    Dim lock = info.Lock


    '    Try

    '        Threading.Monitor.Enter(lock)


    '        Dim statoTaskEseguito As Integer = 6
    '        Dim statoTaskDaEseguire As Integer = 5

    '        Dim fatture As New ParsecPro.FatturaElettronicaRepository
    '        Dim fattureScadute = fatture.Where(Function(c) c.IdStato = CInt(ParsecPro.StatoFattura.Protocollata)).ToList
    '        fattureScadute = fattureScadute.Where(Function(c) (c.MessaggioSdI.DataRicezioneInvio.AddDays(16) - Now).Days < 1).ToList

    '        Dim tasks As New ParsecWKF.TaskRepository

    '        'Dim deleghe As New ParsecAdmin.DelegheRepository
    '        'Dim deleganti = deleghe.Where(Function(c) c.IdDelegato = idUtente).Select(Function(c) c.IdDelegante).ToList
    '        'deleganti.Add(idUtente)
    '        'deleghe.Dispose()


    '        Dim task As ParsecWKF.Task = Nothing

    '        Dim istanze As New ParsecWKF.IstanzaRepository
    '        Dim istanza As ParsecWKF.Istanza = Nothing
    '        Dim idRegistrazione As Integer = 0
    '        Dim idIstanza As Integer = 0




    '        Dim taskCorrenteDaAggiornare = info.CurrentTasksToUpdate '"IN CARICO"

    '        'TASK DA ESEGUIRE
    '        Dim taskDaEseguire As String = info.TaskToExecute
    '        Dim azioneDaEseguire As String = info.ActionToExecute
    '        Dim idDestinatario = info.IdConsignee


    '        'Dim idDestinatario As Integer = 739 '67

    '        Dim statoSuccessivo As String = String.Empty
    '        Dim action As ParsecWKF.Action = Nothing

    '        For Each fatt In fattureScadute

    '            Try

    '                idRegistrazione = fatt.IdRegistrazione

    '                istanza = istanze.Where(Function(c) c.IdDocumento = idRegistrazione And c.IdModulo = ParsecAdmin.TipoModulo.PRO And c.Cancellato = False).FirstOrDefault

    '                If Not istanza Is Nothing Then

    '                    idIstanza = istanza.Id

    '                    Dim taskCorrenteDaAggiornareList = taskCorrenteDaAggiornare.Split(",").Select(Function(c) c.Trim.ToUpper).ToList

    '                    'task = tasks.Where(Function(c) c.IdIstanza = idIstanza And c.Corrente.ToUpper = taskCorrenteDaAggiornare And c.IdStato = statoTaskDaEseguire And deleganti.Contains(c.Mittente)).FirstOrDefault

    '                    task = tasks.Where(Function(c) c.IdIstanza = idIstanza And taskCorrenteDaAggiornareList.Contains(c.Corrente.ToUpper) And c.IdStato = statoTaskDaEseguire).FirstOrDefault

    '                    If Not task Is Nothing Then

    '                        '********************************************************************************************************************************************
    '                        'AGGIORNO IL TASK CORRENTE
    '                        '********************************************************************************************************************************************
    '                        action = ParsecWKF.ModelloInfo.ReadActionInfo(taskDaEseguire, istanza.FileIter).Where(Function(c) c.Name = azioneDaEseguire).FirstOrDefault()

    '                        task.IdUtenteOperazione = task.Mittente 'idUtente
    '                        task.IdStato = statoTaskEseguito
    '                        task.DataEsecuzione = Now
    '                        task.Destinatario = idDestinatario
    '                        task.Operazione = action.Description.ToUpper
    '                        task.Notificato = True
    '                        tasks.SaveChanges()
    '                        '********************************************************************************************************************************************

    '                        '********************************************************************************************************************************************
    '                        'AGGIUNGO UN NUOVO TASK (ESEGUO UN INVIA AVANTI)
    '                        '********************************************************************************************************************************************
    '                        statoSuccessivo = ParsecWKF.ModelloInfo.StatoSuccessivoAction(taskDaEseguire, azioneDaEseguire, istanza.FileIter)

    '                        Dim nuovoTask As New ParsecWKF.Task
    '                        nuovoTask.IdIstanza = idIstanza
    '                        nuovoTask.Nome = taskDaEseguire
    '                        nuovoTask.Corrente = statoSuccessivo
    '                        nuovoTask.Successivo = ParsecWKF.ModelloInfo.StatoSuccessivoIter(statoSuccessivo, istanza.FileIter)
    '                        nuovoTask.Mittente = idDestinatario
    '                        nuovoTask.TaskPadre = task.Id
    '                        nuovoTask.DataInizio = task.DataInizio
    '                        nuovoTask.DataFine = task.DataFine

    '                        Try
    '                            Dim durata As Integer = 0
    '                            If Not String.IsNullOrEmpty(statoSuccessivo) Then
    '                                durata = ParsecWKF.ModelloInfo.DurataTaskIter(statoSuccessivo, istanza.FileIter)
    '                            End If
    '                            nuovoTask.DataFine = Now.AddDays(durata)
    '                        Catch ex As Exception
    '                        End Try

    '                        nuovoTask.IdStato = statoTaskDaEseguire
    '                        nuovoTask.Notificato = False
    '                        nuovoTask.Cancellato = False
    '                        nuovoTask.Note = task.Note
    '                        nuovoTask.IdUtenteOperazione = task.Mittente ' idUtente
    '                        tasks.Add(nuovoTask)
    '                        tasks.SaveChanges()

    '                        '********************************************************************************************************************************************

    '                        '********************************************************************************************************************************************
    '                        'AGGIORNO LO STATO DELLA FATTURA
    '                        '********************************************************************************************************************************************
    '                        fatt.IdStato = ParsecPro.StatoFattura.Accettata
    '                        fatture.SaveChanges()
    '                        '********************************************************************************************************************************************

    '                        info.JobCount += 1


    '                    End If

    '                End If


    '            Catch ex As Exception

    '            End Try

    '        Next

    '        fatture.Dispose()
    '        tasks.Dispose()
    '        istanze.Dispose()



    '    Catch ex As Exception

    '    Finally

    '        Threading.Monitor.Exit(lock)
    '        info.JobEnd = True

    '        'IO.File.WriteAllText("D:\a.txt", info.StartDate.ToShortTimeString & " " & Now.ToShortTimeString)
    '    End Try






    'End Sub

    Private Sub SbloccaTasks(idUtente As Integer)
        'Elimino tutti i task bloccati dall'utente corrente.
        Dim taskBloccati As New ParsecWKF.LockTaskRepository
        taskBloccati.DeleteAll(idUtente)
        taskBloccati.Dispose()
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

    Private Sub SbloccaDocumenti(idUtente As Integer)
        Dim documentiBloccati As New ParsecAtt.LockDocumentoRepository
        'Elimino tutti i documenti bloccati dall'utente corrente.
        documentiBloccati.DeleteAll(idUtente)
        documentiBloccati.Dispose()
    End Sub

    Private Sub SbloccaRegistrazioni(idUtente As Integer)
        Dim registrazioniBloccate As New ParsecPro.LockRegistrazioneRepository
        'Elimino tutti i protocolli bloccati dall'utente corrente.
        registrazioniBloccate.DeleteAll(idUtente)
        registrazioniBloccate.Dispose()
    End Sub

    Private Sub SbloccaCaselleEmail(idUtente As Integer)
        'Elimino tutte le casella di posta bloccate dall'utente corrente.
        Dim caselleEmail As New ParsecPro.LockCasellaEmailRepository
        caselleEmail.DeleteAll(idUtente)
        caselleEmail.Dispose()
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

    Private Sub VisualizzaPannello(ByVal tipo As TipoPannello)
        Me.LoginPanel.Visible = (tipo = TipoPannello.Login)
        Me.AvvisiPanel.Visible = (tipo = TipoPannello.Avvisi)
        Me.AggiornamentoPanel.Visible = (tipo = TipoPannello.Aggiornamento)
        Me.InserimentoCodicePinPanel.Visible = (tipo = TipoPannello.Otp)
    End Sub

    Protected Sub AvvisiGridView_ItemCommand(ByVal sender As Object, ByVal e As Telerik.Web.UI.GridCommandEventArgs) Handles AvvisiGridView.ItemCommand
        If e.CommandName = "Preview" Then
            Me.DownloadFile(e.Item)
        End If
    End Sub

    Protected Sub AvvisiGridView_ItemDataBound(ByVal sender As Object, ByVal e As Telerik.Web.UI.GridItemEventArgs) Handles AvvisiGridView.ItemDataBound
        Dim btn As LinkButton = Nothing
        If TypeOf e.Item Is Telerik.Web.UI.GridDataItem Then
            Dim dataItem As Telerik.Web.UI.GridDataItem = e.Item
            Dim avviso As ParsecAdmin.Avviso = CType(e.Item.DataItem, ParsecAdmin.Avviso)
            If String.IsNullOrEmpty(avviso.NomeFile) Then
                dataItem("Preview").Controls(0).Visible = False
            Else
                dataItem("Preview").Controls(0).Visible = True
                btn = CType(dataItem("Preview").Controls(0), LinkButton)
                btn.ToolTip = "Apri/Salva allegato"
            End If

            Dim ip As String = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName).AddressList.FirstOrDefault(Function(c) c.AddressFamily = System.Net.Sockets.AddressFamily.InterNetwork).ToString

            Dim avvisiLetti As New ParsecAdmin.AvvisiIndirizziIPRepository
            Dim avvisoLetto As ParsecAdmin.AvvisoIndirizzoIP = avvisiLetti.GetAvvisoLetto(avviso.Id, ip)
            avvisiLetti.Dispose()
            Dim img As Image = CType(dataItem("Flag").Controls(0), Image)
            If Not avvisoLetto Is Nothing Then
                img.ImageUrl = "~\images\flag_yellow16.png"
                img.ToolTip = "Avviso già letto"
            Else
                img.Visible = False
            End If
        End If
    End Sub

    Private Sub DownloadFile(ByVal item As Telerik.Web.UI.GridDataItem)
        Dim id As Integer = item.OwnerTableView.DataKeyValues(item.ItemIndex)("Id")

        Dim avvisi As New ParsecAdmin.AvvisiRepository
        Dim avviso As ParsecAdmin.Avviso = avvisi.GetQuery.Where(Function(c) c.Id = id).FirstOrDefault
        Dim pathAvviso As String = System.Configuration.ConfigurationManager.AppSettings("PathAvviso")
        Dim file As New IO.FileInfo(pathAvviso & avviso.NomeFile)
        If file.Exists Then
            FormsAuthentication.SetAuthCookie(Me.Login1.UserName, False)
            Session("AttachmentFullName") = file.FullName
            Dim pageUrl As String = "~/UI/Amministrazione/pages/user/DownloadPage.aspx"
            ParsecUtility.Utility.PageReload(pageUrl, True)
        Else
            ParsecUtility.Utility.MessageBox("L'allegato selezionato non esiste!", True)
        End If

        Dim ip As String = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName).AddressList.FirstOrDefault(Function(c) c.AddressFamily = System.Net.Sockets.AddressFamily.InterNetwork).ToString
        Dim avvisiLetti As New ParsecAdmin.AvvisiIndirizziIPRepository
        If avvisiLetti.GetAvvisoLetto(avviso.Id, ip) Is Nothing Then
            Dim avvisoLetto As New ParsecAdmin.AvvisoIndirizzoIP With {.IdAvviso = id, .IndirizzoIP = ip}
            avvisiLetti.Add(avvisoLetto)
            avvisiLetti.SaveChanges()
            avvisiLetti.Dispose()
        End If

    End Sub

    Private Function InAggiornamento() As Boolean
        Dim parametri As New ParsecAdmin.ParametriRepository
        Dim parametro As ParsecAdmin.Parametri = parametri.GetByName("InAggiornamento", ParsecAdmin.TipoModulo.SEP)
        parametri.Dispose()
        Return If(parametro Is Nothing, False, parametro.Valore = "1")
    End Function

    Private Function VerificaPasswordScaduta(ByVal utente As ParsecAdmin.Utente) As Boolean
        Dim parametri As New ParsecAdmin.ParametriRepository
        Dim parametro As ParsecAdmin.Parametri = parametri.GetByName("numGiorniValiditaPsw", ParsecAdmin.TipoModulo.SEP)
        parametri.Dispose()
        If Not parametro Is Nothing Then
            If parametro.Valore <> "-1" Then
                Dim ts As TimeSpan = Now - utente.DataUltimoSettaggioPsw
                Dim numeroGiorni As Integer = 0
                If ts.TotalDays >= 0.0 Then
                    numeroGiorni = CInt(Math.Floor(ts.TotalDays))
                Else
                    numeroGiorni = CInt(Math.Ceiling(ts.TotalDays))
                End If
                If numeroGiorni >= CInt(parametro.Valore) Then Return True
            End If
        End If
        Return False
    End Function





    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init


        If Me.InAggiornamento Then
            Me.VisualizzaPannello(TipoPannello.Aggiornamento)
        Else

            Dim moduli As New ParsecAdmin.ModuleRepository
            Dim modulo = moduli.GetQuery.Where(Function(c) c.Id = 9 And c.Abilitato = True).FirstOrDefault

            'luca 02/07/2020
            '' ''If Not modulo Is Nothing Then
            '' ''    Me.PassaggioNuovoAnnoModuloMessi()
            '' ''End If

            modulo = moduli.GetQuery.Where(Function(c) c.Id = 2 And c.Abilitato = True).FirstOrDefault

            If Not modulo Is Nothing Then
                Me.PassaggioNuovoAnnoModuloProtocollo()
            End If

            modulo = moduli.GetQuery.Where(Function(c) c.Id = 3 And c.Abilitato = True).FirstOrDefault
            If Not modulo Is Nothing Then
                Me.PassaggioNuovoAnnoModuloAtti()
            End If


            'modulo = moduli.GetQuery.Where(Function(c) c.Id = ParsecAdmin.ModuliEnumeration.Contratti And c.Abilitato = True).FirstOrDefault
            'If Not modulo Is Nothing Then
            '    Me.PassaggioNuovoAnnoModuloContratti()
            'End If

            moduli.Dispose()

            Dim avvisi As New ParsecAdmin.AvvisiRepository
            Me.Avvisi = avvisi.GetView(New ParsecAdmin.FiltroAvviso With {.Visibile = True})
            avvisi.Dispose()
            If Me.Avvisi.Count > 0 Then
                Me.VisualizzaPannello(TipoPannello.Avvisi)
                Me.AvvisoInfoLabel.Visible = False
            Else
                Me.VisualizzaPannello(TipoPannello.Login)
            End If
        End If


        If Not Me.Page.IsPostBack Then
            Dim parametri As New ParsecAdmin.ParametriRepository
            Dim parametro = parametri.GetByName("AbilitaLoginTramiteCredenzialiWindows", ParsecAdmin.TipoModulo.SEP)
            parametri.Dispose()

            Dim visualizzaCheck As Boolean = False

            If Not parametro Is Nothing Then
                visualizzaCheck = (parametro.Valore = "1")
            End If
            Dim chk As CheckBox = CType(Me.Login1.FindControl("UsaUtenteWindows"), CheckBox)
            Dim lbl As Label = CType(Me.Login1.FindControl("UsaUtenteWindowsLabel"), Label)
            chk.Visible = visualizzaCheck
            lbl.Visible = visualizzaCheck
        End If



    End Sub

    Protected Sub Page_LoadComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.LoadComplete
        Me.AvvisiGridView.DataSource = Me.Avvisi
        Me.AvvisiGridView.DataBind()
    End Sub

    Protected Sub ConfermaButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ConfermaButton.Click

        Dim avvisiLetti As New ParsecAdmin.AvvisiIndirizziIPRepository


        Dim avvisi As New ParsecAdmin.AvvisiRepository
        Dim avvisiVisibili = avvisi.GetView(New ParsecAdmin.FiltroAvviso With {.Visibile = True})

        Dim avvisiVisibiliConAllegati As List(Of ParsecAdmin.Avviso) = avvisiVisibili.Where(Function(c) c.NomeFile <> "").ToList
        Dim avvisiVisibiliSenzaAllegato As List(Of ParsecAdmin.Avviso) = avvisiVisibili.Where(Function(c) c.NomeFile = "").ToList

        Dim numeroAllegati As Integer = avvisiVisibiliConAllegati.Count
        Dim avvisoLetto As ParsecAdmin.AvvisoIndirizzoIP = Nothing
        Dim ip As String = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName).AddressList.FirstOrDefault(Function(c) c.AddressFamily = System.Net.Sockets.AddressFamily.InterNetwork).ToString
        If numeroAllegati > 0 Then
            For Each avviso As ParsecAdmin.Avviso In avvisiVisibiliConAllegati
                avvisoLetto = avvisiLetti.GetAvvisoLetto(avviso.Id, ip)
                If avvisoLetto Is Nothing Then
                    Exit For
                End If
            Next
            If Not avvisoLetto Is Nothing Then
                Me.VisualizzaPannello(TipoPannello.Login)
            Else
                ParsecUtility.Utility.MessageBox("Per proseguire è necessario aprire/salvare l'allegato dell'avviso!", True)
            End If
        Else

            'Gli avvisi senza allegato li marco come letti
            For Each avvisoSenzaAllegato In avvisiVisibiliSenzaAllegato
                If avvisiLetti.GetAvvisoLetto(avvisoSenzaAllegato.Id, ip) Is Nothing Then
                    avvisoLetto = New ParsecAdmin.AvvisoIndirizzoIP With {.IdAvviso = avvisoSenzaAllegato.Id, .IndirizzoIP = ip}
                    avvisiLetti.Add(avvisoLetto)
                    avvisiLetti.SaveChanges()
                End If
            Next


            Me.VisualizzaPannello(TipoPannello.Login)
        End If

        For Each avvisoVisibile As ParsecAdmin.Avviso In Me.Avvisi
            'Se l'avviso è scaduto lo nascondo.
            'Todo aggiungere DataScadenza
            If Now.Date >= avvisoVisibile.DataScadenza.Date Then
                Dim avvisoDaNascondere = avvisi.GetById(avvisoVisibile.Id)
                If Not avvisoDaNascondere Is Nothing Then
                    avvisoDaNascondere.Visibile = False
                    avvisi.SaveChanges()
                End If
            End If
        Next


        avvisi.Dispose()
        avvisiLetti.Dispose()
    End Sub


    Private Sub PassaggioNuovoAnnoModuloAtti()

        Dim annoCorrente As Integer = Now.Year
        Dim annoEsercizio As Integer = 0
        Dim parametri As New ParsecAdmin.ParametriRepository
        Dim parametro As ParsecAdmin.Parametri = parametri.GetByName("AnnoCorrente", ParsecAdmin.TipoModulo.ATT)
        If Not parametro Is Nothing Then
            Integer.TryParse(parametro.Valore, annoEsercizio)
        End If
        parametri.Dispose()

        If annoCorrente <> annoEsercizio Then
            Dim contenutoAvviso = "Per registrare un atto ammistrativo per l'anno " & annoCorrente.ToString & " è necessario generare i contatori. Contattare l'amministratore."
            Dim avvisi As New ParsecAdmin.AvvisiRepository
            Dim avviso = avvisi.Where(Function(c) c.Contenuto = contenutoAvviso).FirstOrDefault
            If avviso Is Nothing Then
                avviso = New ParsecAdmin.Avviso
                avviso.Contenuto = contenutoAvviso
                avviso.Data = Now
                avviso.Visibile = True
                avviso.DataScadenza = Now.AddDays(31)
                avvisi.Save(avviso)
            End If
            avvisi.Dispose()
        End If
    End Sub

    Private Sub PassaggioNuovoAnnoModuloProtocollo()
        Dim annoCorrente As Integer = -1
        Dim annoSuccessivo As Integer = Now.Year
        Dim avvisi As New ParsecAdmin.AvvisiRepository
        Dim esercizi As New ParsecPro.EsercizioRepository
        annoCorrente = esercizi.GetAnnoEsercizioCorrente
        If annoCorrente <> -1 Then
            If annoSuccessivo > annoCorrente Then
                Try
                    esercizi.AggiornaAnnoEsercizio(annoSuccessivo)

                    Dim avviso As New ParsecAdmin.Avviso
                    avviso.Contenuto = "Il Registro PROTOCOLLO è stato aggiornato all'anno " & annoSuccessivo.ToString
                    avviso.Data = Now
                    avviso.Visibile = True
                    avviso.DataScadenza = Now.AddDays(31)
                    avvisi.Save(avviso)

                Catch ex As Exception

                End Try
            End If
        End If
        avvisi.Dispose()
        esercizi.Dispose()
    End Sub

    'luca 02/07/2020
    '' ''Private Sub PassaggioNuovoAnnoModuloMessi()

    '' ''    Dim annoCorrente As Integer = -1
    '' ''    Dim annoSuccessivo As Integer = Now.Year
    '' ''    Dim contatori As New ParsecMES.ContatoriAlboRepository
    '' ''    Dim avvisi As New ParsecAdmin.AvvisiRepository

    '' ''    annoCorrente = contatori.GetAnnoEsercizioCorrente
    '' ''    If annoCorrente <> -1 Then
    '' ''        If annoSuccessivo > annoCorrente Then
    '' ''            Try
    '' ''                contatori.AggiornaAnnoEsercizio(annoSuccessivo)

    '' ''                Dim avviso As New ParsecAdmin.Avviso
    '' ''                avviso.Contenuto = "Il Registro MESSI è stato aggiornato all'anno " & annoSuccessivo.ToString
    '' ''                avviso.Data = Now
    '' ''                avviso.Visibile = True
    '' ''                avviso.DataScadenza = Now.AddDays(31)
    '' ''                avvisi.Save(avviso)

    '' ''            Catch ex As Exception

    '' ''            End Try

    '' ''        End If
    '' ''    Else
    '' ''        'Me.AnnoSuccessivoLabel.Text = "Contattare l'amministratore, non è definito l'anno di esercizio per l'Albo Pretorio."
    '' ''    End If
    '' ''    contatori.Dispose()

    '' ''    annoCorrente = -1

    '' ''    Dim contatoriNotifiche As New ParsecMES.ContatoriNotificheRepository
    '' ''    annoCorrente = contatoriNotifiche.GetAnnoEsercizioCorrente
    '' ''    If annoCorrente <> -1 Then
    '' ''        If annoSuccessivo > annoCorrente Then
    '' ''            Try
    '' ''                contatoriNotifiche.AggiornaAnnoEsercizio(annoSuccessivo)
    '' ''                Dim avviso As New ParsecAdmin.Avviso
    '' ''                avviso.Contenuto = "Il Registro NOTIFICHE è stato aggiornato all'anno " & annoSuccessivo.ToString
    '' ''                avviso.Data = Now
    '' ''                avviso.Visibile = True
    '' ''                avviso.DataScadenza = Now.AddDays(31)
    '' ''                avvisi.Save(avviso)
    '' ''            Catch ex As Exception

    '' ''            End Try

    '' ''        End If
    '' ''    Else
    '' ''        'Me.AnnoSuccessivoLabel.Text = "Contattare l'amministratore, non è definito l'anno di esercizio per l'Albo Pretorio."
    '' ''    End If

    '' ''    contatoriNotifiche.Dispose()

    '' ''    avvisi.Dispose()




    '' ''End Sub


    'Private Sub PassaggioNuovoAnnoModuloContratti()

    '    Dim annoCorrente As Integer = -1
    '    Dim annoSuccessivo As Integer = Now.Year
    '    Dim avvisi As New ParsecAdmin.AvvisiRepository
    '    Dim esercizi As New ParsecContratti.ContatorePropostaRepository
    '    annoCorrente = esercizi.GetAnnoEsercizioCorrente
    '    If annoCorrente <> -1 Then
    '        If annoSuccessivo > annoCorrente Then
    '            Try
    '                esercizi.AggiornaAnnoEsercizio(annoSuccessivo)

    '                Dim avviso As New ParsecAdmin.Avviso
    '                avviso.Contenuto = "Il Repertorio CONTRATTI è stato aggiornato all'anno " & annoSuccessivo.ToString
    '                avviso.Data = Now
    '                avviso.Visibile = True
    '                avviso.DataScadenza = Now.AddDays(31)
    '                avvisi.Save(avviso)

    '            Catch ex As Exception

    '            End Try
    '        End If
    '    End If
    '    avvisi.Dispose()
    '    esercizi.Dispose()

    'End Sub

    Private Function GetPublicIp() As String
        Dim ip As String = String.Empty
        Dim ipList As String = HttpContext.Current.Request.ServerVariables("HTTP_X_FORWARDED_FOR")
        If Not String.IsNullOrEmpty(ipList) Then
            ip = ipList.Split(","c)(0)
        Else
            ip = HttpContext.Current.Request.ServerVariables("REMOTE_ADDR").ToString
        End If
        Return ip
    End Function

    Private Function GenerateOTP(ByVal length As Integer) As String

        Dim numbers As String = "1234567890"

        Dim otp As String = String.Empty
        For i As Integer = 0 To length - 1
            Dim character As String = String.Empty
            Do
                Dim index As Integer = New Random().Next(0, numbers.Length)
                character = numbers.ToCharArray()(index).ToString()
            Loop While otp.IndexOf(character) <> -1
            otp &= character
        Next
        Return otp
    End Function


    Protected Sub ConfermaOTPButton_Click(sender As Object, e As EventArgs) Handles ConfermaOTPButton.Click


        Dim maxTentativi As Integer = 3

        Dim parametri As New ParsecAdmin.ParametriRepository
        Dim parametro = parametri.GetByName("NumeroTentativiOtp", ParsecAdmin.TipoModulo.SEP)
        parametri.Dispose()
        If Not parametro Is Nothing Then
            Try
                maxTentativi = CInt(parametro.Valore)
                If maxTentativi < 3 Then
                    maxTentativi = 3
                End If
            Catch ex As Exception
            End Try
        End If

        If Not String.IsNullOrEmpty(Me.OtpTextBox.Text) Then

            Dim utente As ParsecAdmin.Utente = CType(ParsecUtility.Applicazione.UtenteCorrente, ParsecAdmin.Utente)

            Dim utenti As New ParsecAdmin.UserRepository
            Dim ute As ParsecAdmin.Utente = utenti.Where(Function(c) c.Id = utente.Id).FirstOrDefault
            utenti.Dispose()

            Dim otp = ParsecCommon.CryptoUtil.Decrypt(ute.OTP)

            If otp = Me.OtpTextBox.Text Then
                If Me.NumeroTentativiOtp >= maxTentativi Then
                    Me.MessaggioErroreLabel.Text = "Numero massimo di tentativi superato."
                    Me.MessaggioErroreLabel.ForeColor = Drawing.Color.Red
                Else
                    Me.NumeroTentativiOtp = 0

                    Me.EseguiLogin(ute)

                    'AGGIORNO LA TABELLA UTENTE
                    utenti = New UserRepository
                    ute = utenti.Where(Function(c) c.Id = utente.Id).FirstOrDefault
                    ute.DataScadenzaOTP = Now
                    utenti.SaveChanges()
                    utenti.Dispose()

                End If

            Else
                If VerificaNumeroTentativi(maxTentativi) Then
                    Me.MessaggioErroreLabel.Text = "Numero massimo di tentativi superato."
                    Me.MessaggioErroreLabel.ForeColor = Drawing.Color.Red
                Else
                    Me.MessaggioErroreLabel.Text = "Il codice di attivazione inserito non è corretto. Verifica e riprova."
                    Me.MessaggioErroreLabel.ForeColor = Drawing.Color.Red
                End If

            End If
        Else
            Me.MessaggioErroreLabel.Text = "E' necessario inserire il codice di attivazione."
            Me.MessaggioErroreLabel.ForeColor = Drawing.Color.Red

        End If

    End Sub


    Private Function VerificaNumeroTentativi(ByVal maxTentativi As Integer) As Boolean
        Dim res As Boolean = False
        Me.NumeroTentativiOtp += 1

        If Me.NumeroTentativiOtp >= maxTentativi Then
            res = True
        End If

        Return res
    End Function

    Private Sub EseguiLogin(ByVal utente As ParsecAdmin.Utente)

        Me.SbloccaDocumenti(utente.Id)
        Me.SbloccaRegistrazioni(utente.Id)
        Me.SbloccaEmails(utente.Id)
        Me.SbloccaCaselleEmail(utente.Id)
        Me.SbloccaTasks(utente.Id)

        Me.SbloccaRegistrazioniUtentiNonCollegati()
        Me.SbloccaDocumentiUtentiNonCollegati()
        Me.SbloccaEmailsUtentiNonCollegati()
        Me.SbloccaCaselleEmailUtentiNonCollegati()
        Me.SbloccaTasksUtentiNonCollegati()


        'Avvio il rilevamento della scadenza della sessione.
        ParsecUtility.SessioneCorrente.Initialize()


        FormsAuthentication.SetAuthCookie(Me.Login1.UserName, False)

        ParsecUtility.UtentiConnessi.Add(utente.Id, utente.Cognome & " " & utente.Nome)


        Me.RegistraEvento("LOGIN RIUSCITO", ParsecAdmin.TipoEventoLog.Login)
        Me.RegistraAccessoUtente(utente)

        Session("LoginEseguito") = "1"


        Dim userAgent = Request.UserAgent.ToLower()

        Dim aperturaHomePagePopup As Boolean = True

        Try
            Boolean.TryParse(ParsecAdmin.WebConfigSettings.GetKey("AperturaHomePagePopup"), aperturaHomePagePopup)
        Catch ex As Exception

        End Try


        If Not aperturaHomePagePopup Then
            Response.Redirect("~/Default.aspx")
        Else
            ParsecUtility.Utility.OpenWebSite(Me.scriptHolder)

        End If


    End Sub

    Private Sub InviaSms(ByVal destinatario As String, ByVal mittente As String, ByVal otp As String)

        Dim connection As SMSCConnection = Nothing

        Try

            Dim encryptedPassword As String = ParsecAdmin.WebConfigSettings.GetKey("ArubaSmsPassword")
            Dim encryptedUsername As String = ParsecAdmin.WebConfigSettings.GetKey("ArubaSmsUsername")


            Dim username As String = ParsecCommon.CryptoUtil.Decrypt(encryptedUsername)
            Dim password As String = ParsecCommon.CryptoUtil.Decrypt(encryptedPassword)
            Dim hostname As String = ParsecAdmin.WebConfigSettings.GetKey("ArubaSmsHostname")
            Dim port As Integer = CInt(ParsecAdmin.WebConfigSettings.GetKey("ArubaSmsPort"))

            connection = New SMSCHTTPConnection(username, password, hostname, port, Nothing, Nothing)

            Try

                Dim sms As New SMS()
                sms.addSMSRecipient(destinatario)

                sms.Message = String.Format(Me.GetMessaggioSms, otp)
                sms.SMSSender = mittente
                sms.setImmediate()

                Dim result As SendResult = connection.sendSMS(sms)

            Catch smscre As SMSCRemoteException
                Throw New ApplicationException("Impossibile inviare l'sms per il seguente motivo: " + smscre.Message)
            Catch smsce As SMSCException
                Throw New ApplicationException("Impossibile inviare l'sms per il seguente motivo: " + smsce.Message)
            End Try


        Catch ex As SMSCException
            Throw New ApplicationException("Impossibile inviare l'sms per il seguente motivo: " + ex.Message)
        End Try



        connection.logout()
    End Sub

    Protected Sub RinviaCodiceButton_Click(sender As Object, e As EventArgs) Handles RinviaCodiceButton.Click

        Try

            Dim utente As ParsecAdmin.Utente = CType(ParsecUtility.Applicazione.UtenteCorrente, ParsecAdmin.Utente)

            Dim parametri As New ParsecAdmin.ParametriRepository

            Dim lunghezzaOtp As Integer = 7

            Dim para = parametri.GetByName("LunghezzaOtp", ParsecAdmin.TipoModulo.SEP)

            If Not para Is Nothing Then
                Try
                    lunghezzaOtp = CInt(para.Valore)
                    If lunghezzaOtp < 7 Then
                        lunghezzaOtp = 7
                    End If
                Catch ex As Exception
                End Try
            End If

            Dim durataMinutiOtp As Integer = 15

            para = parametri.GetByName("DurataMinutiOtp", ParsecAdmin.TipoModulo.SEP)

            If Not para Is Nothing Then
                Try

                    durataMinutiOtp = CInt(para.Valore)
                    If durataMinutiOtp < 15 Then
                        durataMinutiOtp = 15
                    End If

                Catch ex As Exception
                End Try
            End If

            Dim mittenteSms As String = String.Empty

            para = parametri.GetByName("MittenteSms", ParsecAdmin.TipoModulo.SEP)
            If Not para Is Nothing Then
                mittenteSms = para.Valore
            End If

            parametri.Dispose()

            Dim otp As String = Me.GenerateOTP(lunghezzaOtp)
            Dim encryptedOtp = ParsecCommon.CryptoUtil.Encrypt(otp)

            Me.InviaSms("+39" + utente.Cellulare, mittenteSms, otp)

            'AGGIORNO LA TABELLA UTENTE
            Dim utenti As New UserRepository
            Dim ute = utenti.Where(Function(c) c.Id = utente.Id).FirstOrDefault
            ute.DataScadenzaOTP = Now.AddMinutes(durataMinutiOtp)
            ute.OTP = encryptedOtp
            utenti.SaveChanges()
            utenti.Dispose()

            Me.MessaggioErroreLabel.Text = "Sms inviato con successo."
            Me.MessaggioErroreLabel.ForeColor = Drawing.Color.Green

        Catch ex As Exception
            MessaggioErroreLabel.Text = ex.Message
        End Try



    End Sub

    Private Function GetAttivazioneAutenticazioneSecondoLivello() As Boolean
        Dim attivazioneAutenticazioneSecondoLivello As Boolean = False

        Dim parametri As New ParsecAdmin.ParametriRepository
        Dim parametro = parametri.GetByName("AttivazioneAutenticazioneSecondoLivello", ParsecAdmin.TipoModulo.SEP)
        parametri.Dispose()

        If Not parametro Is Nothing Then
            Try
                attivazioneAutenticazioneSecondoLivello = CBool(parametro.Valore)
            Catch ex As Exception
            End Try
        End If

        Return attivazioneAutenticazioneSecondoLivello
    End Function

    Private Function GetDurataMinutiOtp() As Integer

        Dim durataMinutiOtp As Integer = 15

        Dim parametri As New ParsecAdmin.ParametriRepository
        Dim parametro = parametri.GetByName("DurataMinutiOtp", ParsecAdmin.TipoModulo.SEP)
        parametri.Dispose()

        If Not parametro Is Nothing Then
            Try
                durataMinutiOtp = CInt(parametro.Valore)
                If durataMinutiOtp < 15 Then
                    durataMinutiOtp = 15
                End If

            Catch ex As Exception
            End Try
        End If
        Return durataMinutiOtp
    End Function

    Private Function GetLunghezzaOtp() As Integer
        Dim lunghezzaOtp As Integer = 7

        Dim parametri As New ParsecAdmin.ParametriRepository
        Dim parametro = parametri.GetByName("LunghezzaOtp", ParsecAdmin.TipoModulo.SEP)
        parametri.Dispose()

        If Not parametro Is Nothing Then
            Try
                lunghezzaOtp = CInt(parametro.Valore)
                If lunghezzaOtp < 7 Then
                    lunghezzaOtp = 7
                End If
            Catch ex As Exception
            End Try
        End If
        Return lunghezzaOtp
    End Function

    Private Function GetMittenteSms() As String
        Dim mittenteSms As String = String.Empty

        Dim parametri As New ParsecAdmin.ParametriRepository
        Dim parametro = parametri.GetByName("MittenteSms", ParsecAdmin.TipoModulo.SEP)
        parametri.Dispose()

        If Not parametro Is Nothing Then
            mittenteSms = parametro.Valore
        End If
        Return mittenteSms
    End Function

    Private Function GetMessaggioSms() As String
        Dim messaggio As String = "IL TUO PIN E': {0}"

        Dim parametri As New ParsecAdmin.ParametriRepository
        Dim parametro = parametri.GetByName("MessaggioSms", ParsecAdmin.TipoModulo.SEP)
        parametri.Dispose()

        If Not parametro Is Nothing Then
            Try
                messaggio = parametro.Valore

            Catch ex As Exception
            End Try
        End If
        Return messaggio
    End Function




End Class