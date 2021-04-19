Partial Class NotificaSegnalazioniPage
    Inherits System.Web.UI.Page


   
    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        Me.CaricaSegnalazioni()
    End Sub



    Private Sub CaricaSegnalazioni()

        'Dim tasks As New ParsecWKF.TaskRepository

        'Dim watch = System.Diagnostics.Stopwatch.StartNew()

        'Dim t = tasks.GetTaskAttivoModuloById(2, ParsecAdmin.TipoModulo.WBT)

        'watch.Stop()
        'Dim elapsedMs = watch.ElapsedMilliseconds

        'watch = System.Diagnostics.Stopwatch.StartNew()
        'Dim taskAttivo As ParsecWKF.TaskAttivo = tasks.GetView(New ParsecWKF.TaskFiltro With {.IdUtente = 44, .IdModulo = ParsecAdmin.TipoModulo.WBT}).Where(Function(c) c.IdIstanza = 1).FirstOrDefault

        'watch.Stop()
        'Dim elapsedMs2 = watch.ElapsedMilliseconds

        Try

            Dim parametri As New ParsecAdmin.ParametriSegnalazioneRepository
            Dim parametro As ParsecAdmin.ParametriSegnalazione = parametri.GetQuery.FirstOrDefault
            parametri.Dispose()

            Dim username As String = ParsecCommon.CryptoUtil.Decrypt(parametro.NomeUtenteRicevente)
            Dim password As String = ParsecCommon.CryptoUtil.Decrypt(parametro.PasswordRicevente)
            Dim baseUrl As String = parametro.EndPointServizio

            Dim service As New ParsecWebServices.WhistleBlowingService(baseUrl)


            Dim ricevente As ParsecWebServices.User = service.GetUser(username, password)
            Dim tips As List(Of ParsecWebServices.Tip) = service.GetTips(ricevente.session_id).OrderByDescending(Function(c) c.creation_date).ToList

            Dim idSegnalazione As String = String.Empty
            Dim nuovaSegnalazione As ParsecAdmin.Segnalazione = Nothing

            Dim segnalazioni As New ParsecAdmin.SegnalazioneRepository

            For Each tip As ParsecWebServices.Tip In tips

                idSegnalazione = tip.internaltipid
                Dim segn = segnalazioni.Where(Function(c) c.GuidSegnalazione = idSegnalazione)
                If Not segn.Any Then
                    nuovaSegnalazione = New ParsecAdmin.Segnalazione With {.GuidSegnalazione = idSegnalazione, .DataScadenza = tip.expiration_date, .Stato = "NUOVA", .DataCreazione = tip.creation_date, .NumeroSeriale = tip.sequence_number}
                    segnalazioni.Add(nuovaSegnalazione)
                    segnalazioni.SaveChanges()
                    service.SetLabel(ricevente.session_id, tip.internaltipid, "NUOVA")

                    Me.CreaIstanzaSegnalazione(nuovaSegnalazione, parametro)

                End If
            Next

            segnalazioni.Dispose()


        Catch ex As Exception

        End Try


    End Sub




    Public Sub CreaIstanzaSegnalazione(ByVal segnalazione As ParsecAdmin.Segnalazione, ByVal parametri As ParsecAdmin.ParametriSegnalazione)

        Dim istanze As New ParsecWKF.IstanzaRepository
        Dim istanzaPrecedente As ParsecWKF.Istanza = istanze.GetQuery.Where(Function(c) c.IdDocumento = segnalazione.Id AndAlso c.IdModulo = ParsecAdmin.TipoModulo.WBT).FirstOrDefault


        'Se l'istanza del documento non è in iter
        If istanzaPrecedente Is Nothing Then

            Dim descrizioneDocumento As String = String.Format("WBT - {0} del {1}", segnalazione.NumeroSeriale, segnalazione.DataCreazione.ToShortDateString)

            Dim IdModelloIter As Integer = parametri.IdModelloIter

            Dim modelli As New ParsecWKF.ModelliRepository
            Dim modello As ParsecWKF.Modello = modelli.Where(Function(c) c.Id = IdModelloIter).FirstOrDefault
            modelli.Dispose()


            Dim adesso As DateTime = Now



            Dim statoIniziale As Integer = 1
            Dim statoDaEseguire As Integer = 5

            Dim idIstanza As Integer = 0

            'mittente del protocollo 
            Dim idMittente As Integer = parametri.IdDestinatarioIter


            Dim istanza As New ParsecWKF.Istanza
            istanza.Riferimento = descrizioneDocumento
            istanza.IdStato = statoIniziale
            istanza.DataInserimento = adesso
            istanza.DataScadenza = adesso '.AddDays(modelloIter.DurataIter)
            istanza.IdModello = modello.Id
            istanza.IdDocumento = segnalazione.Id

            istanza.Ufficio = idMittente
            'istanza.ContatoreGenerale = segnalazione.Id

            istanza.IdModulo = modello.RiferimentoModulo
            istanza.IdUtente = idMittente

            istanza.FileIter = modello.NomeFile

            Try

                istanze.Add(istanza)
                istanze.SaveChanges()
                istanze.Dispose()

                idIstanza = istanza.Id


                'MITTENTE E DESTINATARIO SOLO GLI STESSI

                '*******************************************************************************************************************************
                'Inserisco nei parametri del processo l'attore DESTINATARIO corrente.
                Dim parametriProcesso As New ParsecWKF.ParametriProcessoRepository
                Dim processo As New ParsecWKF.ParametroProcesso With {.IdProcesso = idIstanza, .Nome = "DESTINATARIO", .Valore = idMittente.ToString}
                parametriProcesso.Add(processo)
                parametriProcesso.SaveChanges()



                'Inserisco nei parametri del processo l'attore MITTENTE se non esiste.
                Dim parametroProcessoExist As Boolean = Not parametriProcesso.GetQuery.Where(Function(c) c.IdProcesso = idIstanza And c.Nome = "MITTENTE").FirstOrDefault Is Nothing
                If Not parametroProcessoExist Then
                    processo = New ParsecWKF.ParametroProcesso With {.IdProcesso = idIstanza, .Nome = "MITTENTE", .Valore = idMittente.ToString}
                    parametriProcesso.Add(processo)
                    parametriProcesso.SaveChanges()
                End If

                parametriProcesso.Dispose()
                '*******************************************************************************************************************************

                ' inserisco il task dell'istanza appena inserita
                Dim tasks As New ParsecWKF.TaskRepository

                Dim task As New ParsecWKF.Task
                task.IdIstanza = idIstanza
                task.Nome = ""


                Dim corrente As String = modello.StatoIniziale()
                task.Corrente = corrente
                task.Successivo = modello.StatoSuccessivo(corrente)

                task.Mittente = idMittente
                'task.Destinatario = IdDestinatario
                task.IdStato = statoDaEseguire
                task.DataInizio = adesso
                task.DataEsecuzione = adesso
                task.DataFine = adesso '.AddDays(modelloIter.DurataTask)
                task.Operazione = "NUOVO"
                task.Cancellato = False
                task.IdUtenteOperazione = idMittente

                Try

                    tasks.Add(task)
                    tasks.SaveChanges()

                    tasks.Dispose()

                    'Aggiungo il nuovo task
                    Me.Procedi(task, istanza)


                    Me.AggiungiUtenteVisibilita(istanza.IdDocumento, istanza.IdModulo, idMittente)

                Catch ex As Exception
                    Throw New ApplicationException(ex.Message)
                End Try

            Catch ex As Exception
                Throw New ApplicationException(ex.Message)
            End Try

        End If

    End Sub



    '*************************************************************************************************************
    'INSERISCO IL TASK AUTOMATICO
    '*************************************************************************************************************
    Private Sub Procedi(ByVal taskAttivo As ParsecWKF.Task, ByVal istanzaAttiva As ParsecWKF.Istanza)

        Dim statoEseguito As Integer = 6
        Dim statoDaEseguire As Integer = 5
        Dim statoIstanzaCompletato As Integer = 3

        Dim nomeFileIter As String = istanzaAttiva.FileIter
        Dim idUtente As Integer = taskAttivo.Mittente

        Dim tasks As New ParsecWKF.TaskRepository


        tasks.Attach(taskAttivo)
        'Dim taskAttivo As ParsecWKF.Task = tasks.Where(Function(c) c.Id = taskAttivo.Id).FirstOrDefault


        Dim actions = ParsecWKF.ModelloInfo.ReadActionInfo(taskAttivo.Corrente, nomeFileIter)


        Dim taskSuccessivoAutomatico As ParsecWKF.ParametroProcesso = actions(0).Parameters.Where(Function(c) c.Nome = "TaskSuccessivoAutomatico").FirstOrDefault

        Dim azione As ParsecWKF.Action = Nothing


        If Not taskSuccessivoAutomatico Is Nothing Then
            azione = ParsecWKF.ModelloInfo.ReadActionInfo(taskAttivo.Successivo, nomeFileIter).Where(Function(c) c.Name = taskSuccessivoAutomatico.Valore).FirstOrDefault
        Else
            azione = ParsecWKF.ModelloInfo.ReadActionInfo(taskAttivo.Successivo, nomeFileIter)(0)
        End If

        Dim operazione As String = "INIZIO"
        Dim procediAction = actions.Where(Function(c) c.Type = "INIZIO").FirstOrDefault
        If Not procediAction Is Nothing Then
            If Not String.IsNullOrEmpty(procediAction.Description) Then
                operazione = procediAction.Description.ToUpper
            End If
        End If



        '*************************************************************************************************************
        'AGGIORNO IL TASK PRECEDENTE
        '*************************************************************************************************************

        'If Not String.IsNullOrEmpty(Me.NoteInterneTextBox.Text) Then
        '    task.Note = Me.NoteInterneTextBox.Text
        'End If


        taskAttivo.IdStato = statoEseguito
        taskAttivo.DataEsecuzione = Now
        taskAttivo.Operazione = operazione
        taskAttivo.Destinatario = idUtente
        taskAttivo.Notificato = True
        tasks.SaveChanges()
        '*************************************************************************************************************

        '*************************************************************************************************************
        'INSERISCO IL NUOVO TASK
        '*************************************************************************************************************

        Dim statoSuccessivo As String = ParsecWKF.ModelloInfo.StatoSuccessivoAction(taskAttivo.Successivo, azione.Name, nomeFileIter)

        Dim durata As Integer = 1

        If Not String.IsNullOrEmpty(statoSuccessivo) Then
            'durata = ModelloInfo.DurataTaskIter(statoSuccessivo, taskAttivo.NomeFileIter)
        End If


        'ANNULLO L'ISTANZA
        If azione.Type = "FINE" Then
            Dim istanze As New ParsecWKF.IstanzaRepository
            Dim istanza As ParsecWKF.Istanza = istanze.GetQuery.Where(Function(c) c.Id = istanzaAttiva.Id).FirstOrDefault
            istanza.IdStato = statoIstanzaCompletato
            istanza.DataEsecuzione = Now
            istanze.SaveChanges()
            istanze.Dispose()
        End If

        Dim nuovotask As New ParsecWKF.Task
        nuovotask.IdIstanza = taskAttivo.IdIstanza
        nuovotask.TaskPadre = taskAttivo.Id

        nuovotask.Nome = taskAttivo.Corrente


        nuovotask.Successivo = ParsecWKF.ModelloInfo.StatoSuccessivoIter(statoSuccessivo, nomeFileIter)

        nuovotask.Mittente = idUtente

        If azione.Type = "FINE" Then
            nuovotask.IdStato = statoEseguito
            nuovotask.Corrente = "FINE"
            nuovotask.Destinatario = idUtente
            nuovotask.Operazione = "FINE"
            nuovotask.DataEsecuzione = Now
        Else
            nuovotask.IdStato = statoDaEseguire
            nuovotask.Corrente = statoSuccessivo
        End If

        nuovotask.DataInizio = Now

        'If Not String.IsNullOrEmpty(Me.NoteInterneTextBox.Text) Then
        '    nuovotask.Note = Me.NoteInterneTextBox.Text
        'End If


        nuovotask.DataFine = Now.AddDays(durata)
        nuovotask.Cancellato = False
        nuovotask.Notificato = False
        nuovotask.IdUtenteOperazione = idUtente

        tasks.Add(nuovotask)
        tasks.SaveChanges()
        '*************************************************************************************************************

        tasks.Dispose()

    End Sub


    Private Sub AggiungiUtenteVisibilita(ByVal idDocumento As Integer, ByVal idModulo As Integer, ByVal idUtente As Integer)

        Dim visibilitaDocumento As New ParsecAdmin.VisibilitaDocumentoRepository

        Dim utenti As New ParsecAdmin.UserRepository
        Dim utente As ParsecAdmin.Utente = utenti.Where(Function(c) c.Id = idUtente).FirstOrDefault
        utenti.Dispose()

        Dim utenteVisibilita As New ParsecAdmin.VisibilitaDocumento
        utenteVisibilita.AbilitaCancellaEntita = False
        utenteVisibilita.Descrizione = (If(utente.Username = Nothing, "", utente.Username) + " - " + If(utente.Cognome = Nothing, "", utente.Cognome) + " " + If(utente.Nome = Nothing, "", utente.Nome))
        utenteVisibilita.TipoEntita = 2
        utenteVisibilita.IdEntita = utente.Id
        utenteVisibilita.IdModulo = idModulo
        utenteVisibilita.LogIdUtente = utente.Id
        utenteVisibilita.LogDataOperazione = Now
        utenteVisibilita.IdDocumento = idDocumento

        visibilitaDocumento.Add(utenteVisibilita)
        visibilitaDocumento.SaveChanges()
        visibilitaDocumento.Dispose()
    End Sub

End Class