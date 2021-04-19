Partial Class NotificaPage
    Inherits System.Web.UI.Page


  

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        If Not Request.QueryString("checkDocumenti") Is Nothing Then

            Response.ClearHeaders()
            Response.Clear()


            Dim utenteCorrente As ParsecAdmin.Utente = CType(ParsecUtility.Applicazione.UtenteCorrente, ParsecAdmin.Utente)
            If Not utenteCorrente Is Nothing Then

                Dim tasks As New ParsecWKF.TaskRepository
                Dim taskAttivi = tasks.GetView(New ParsecWKF.TaskFiltro With {.IdUtente = utenteCorrente.Id}).Where(Function(c) c.Notificato = False)

                If taskAttivi.Count > 0 Then
                    Dim sb As New StringBuilder()
                    If taskAttivi.Count = 1 Then
                        sb.Append("<div>Sulla scrivania c'è " & taskAttivi.Count.ToString & " nuovo documento.")
                    Else
                        sb.Append("<div>Sulla scrivania ci sono " & taskAttivi.Count.ToString & " nuovi documenti.")
                    End If

                    sb.Append("</div>")
                    Response.Write(sb.ToString())

                    For Each t In taskAttivi
                        Dim taskId As Integer = t.Id
                        Dim task As ParsecWKF.Task = tasks.GetQuery.Where(Function(c) c.Id = taskId).FirstOrDefault
                        task.Notificato = True

                    Next
                    tasks.SaveChanges()
                    tasks.Dispose()


                Else
                    Response.Write("")
                End If
            Else
                Response.Write("")
            End If

        End If

        Response.End()
    End Sub
End Class