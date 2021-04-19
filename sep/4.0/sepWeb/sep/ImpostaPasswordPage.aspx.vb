Imports ParsecAdmin

Partial Class ImpostaPasswordPage
    Inherits System.Web.UI.Page

    Private Sub Convalida()
        Dim message As String = ""
        Dim lunghezzaMinima As Integer = 5
        Dim parametriRepository As New ParsecAdmin.ParametriRepository
        Dim parametro As ParsecAdmin.Parametri = parametriRepository.GetByName("numCaratteriPswUtente")
        If Not parametro Is Nothing Then
            lunghezzaMinima = CInt(parametro.Valore)
        End If
        parametriRepository.Dispose()

        If String.IsNullOrEmpty(Me.NuovaPasswordTextBox.Text) OrElse String.IsNullOrEmpty(Me.ConfermaPasswordTextBox.Text) Then
            message = "E' necessario inserire la password sia nel campo 'Nuova password' sia nel campo 'Conferma password'!"
            Throw New ApplicationException(message)
        Else
            If Me.NuovaPasswordTextBox.Text <> Me.ConfermaPasswordTextBox.Text Then
                message = "Nei campi 'Nuova password' e 'Conferma password' è necessario inserire la stessa password!"
                Throw New ApplicationException(message)
            End If
        End If
        If ParsecUtility.Utility.CheckPassword(Me.NuovaPasswordTextBox.Text) Then
            message = "La nuova password non è valida!"
            Throw New ApplicationException(message)
        End If
        If Me.NuovaPasswordTextBox.Text.Length < lunghezzaMinima Then
            message = "La nuova password non può essere inferiore a " & CStr(lunghezzaMinima) & " caratteri!"
            Throw New ApplicationException(message)
        End If
        Dim utenteCollegato As ParsecAdmin.Utente = CType(ParsecUtility.Applicazione.UtenteCorrente, ParsecAdmin.Utente)
        If Not utenteCollegato Is Nothing Then
            If utenteCollegato.Username = Me.NuovaPasswordTextBox.Text Then
                message = "La nuova password non può essere uguale allo username dell'utente!"
                Throw New ApplicationException(message)
            End If
        End If
        Dim userRepository As New ParsecAdmin.UserRepository
        If userRepository.PasswordGiaUsata(utenteCollegato.Id, Me.NuovaPasswordTextBox.Text) Then
            message = "La nuova password specificata è stata già utilizzata dall'utente!"
            Throw New ApplicationException(message)
        End If
        userRepository.Dispose()
    End Sub

    Private Sub CreaCertificato(ByVal utente As Utente, ByVal fullPath As String)

        Dim organizzazione As String = String.Empty

        Dim cliente = CType(ParsecUtility.Applicazione.ClienteCorrente, Cliente)
        If Not cliente Is Nothing Then
            organizzazione = cliente.Descrizione
        End If

        Dim commonName As String = String.Empty

        If String.IsNullOrEmpty(utente.Cognome) Then
            commonName = utente.Cognome
        End If

        If Not String.IsNullOrEmpty(utente.Nome) Then
            If Not String.IsNullOrEmpty(commonName) Then
                commonName &= " " & utente.Nome
            Else
                commonName &= utente.Nome
            End If
        End If

        If Not String.IsNullOrEmpty(utente.CodiceFiscale) Then
            If Not String.IsNullOrEmpty(commonName) Then
                commonName &= "/" & utente.CodiceFiscale
            Else
                commonName &= utente.CodiceFiscale
            End If
        End If

        If String.IsNullOrEmpty(commonName) Then
            commonName = utente.Cognome
        End If
        ParsecUtility.SelfSignCertificate.Create(fullPath, utente.Cognome, utente.Nome, utente.Email, organizzazione, commonName, utente.PasswordHash)
        'ParsecUtility.SelfSignCertificate.Create(fullPath, utente.Cognome, utente.Nome, utente.Email, organizzazione, commonName, Now, New DateTime(Now.Year + 5, Now.Month, Now.Day), utente.PasswordHash)
    End Sub

    Protected Sub SalvaButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles SalvaButton.Click
        Dim message As String = "Operazione conclusa con successo! " & vbCrLf & " Riaccedere al Sistema con la nuova password!."
        Try
            Me.Convalida()
            Dim utente As ParsecAdmin.Utente = Nothing
            Try
                Dim utenteCollegato As ParsecAdmin.Utente = CType(ParsecUtility.Applicazione.UtenteCorrente, ParsecAdmin.Utente)
                Dim userRepository As New ParsecAdmin.UserRepository
                utente = userRepository.GetQuery.Where(Function(u) u.Id = utenteCollegato.Id).FirstOrDefault
                Dim oldPassword = utente.PasswordHash
                utente.PswNonSettata = False
                utente.DataUltimoSettaggioPsw = Now
                Dim password = ParsecUtility.Utility.CalcolaHash(Me.NuovaPasswordTextBox.Text)
                utente.PasswordHash = password
                userRepository.SaveChanges()

                Dim nomefileCertificato As String = String.Format("Certificato{0}{1}", utente.Id.ToString.PadLeft(7, "0"), ".cer")

                Try
                    Dim fullPath As String = ParsecAdmin.WebConfigSettings.GetKey("PathCertificati") & nomefileCertificato
                    If Not IO.Directory.Exists(ParsecAdmin.WebConfigSettings.GetKey("PathCertificati")) Then
                        IO.Directory.CreateDirectory(ParsecAdmin.WebConfigSettings.GetKey("PathCertificati"))
                    End If

                    If Not IO.File.Exists(fullPath) Then
                        Me.CreaCertificato(utente, fullPath)
                    Else
                        If Not password.SequenceEqual(oldPassword) Then
                            Me.CreaCertificato(utente, fullPath)
                        End If
                    End If
                Catch ex As Exception
                    ParsecUtility.Utility.MessageBox("Impossibile creare il certificato per il seguente motivo: " & vbCrLf & ex.Message, False)
                Finally
                    utente.NomefileCertificato = nomefileCertificato
                    userRepository.SaveChanges()
                End Try

                userRepository.Dispose()



            Catch ex As Exception
                ParsecUtility.Utility.MessageBox(ex.Message, True)
            Finally
                ParsecUtility.Utility.MessageBox(message, False)
                Response.Redirect("~/Login.aspx")
                'Session.Abandon()
                'FormsAuthentication.SignOut()
                'FormsAuthentication.RedirectToLoginPage()
            End Try
        Catch ex As Exception
            ParsecUtility.Utility.MessageBox(ex.Message, True)
        End Try
    End Sub

End Class