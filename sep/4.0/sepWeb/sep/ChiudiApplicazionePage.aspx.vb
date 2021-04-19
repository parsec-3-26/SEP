Partial Class ChiudiApplicazionePage
    Inherits System.Web.UI.Page


    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If Not Request.QueryString("id") Is Nothing Then
            Dim idUtente As Integer = CInt(Request.QueryString("id"))
            ParsecUtility.UtentiConnessi.Delete(idUtente)
            Me.SbloccaRegistrazioniUtentiNonCollegati()
            Me.SbloccaDocumentiUtentiNonCollegati()
            Me.SbloccaEmailsUtentiNonCollegati()
            Me.SbloccaCaselleEmailUtentiNonCollegati()
            Me.SbloccaTasksUtentiNonCollegati()
            Exit Sub
        End If
    End Sub


    Private Sub SbloccaTasks(idUtente As Integer)
        'Elimino tutti i task bloccati dall'utente corrente.
        Dim taskBloccati As New ParsecWKF.LockTaskRepository
        taskBloccati.DeleteAll(idUtente)
        taskBloccati.Dispose()
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

End Class