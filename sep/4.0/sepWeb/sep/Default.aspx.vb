
Partial Class _Default
    Inherits System.Web.UI.Page

    Private WithEvents MainPage As MainPage


    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        MainPage = CType(Me.Master, MainPage)
        MainPage.DescrizioneProcedura = ">"
        Me.PdfFrame.Visible = False
    End Sub

    Protected Sub Page_LoadComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.LoadComplete
        Dim modulo As ParsecAdmin.Modulo = Session("CurrentModule")
        If Not modulo Is Nothing Then
            MainPage.NomeModulo = modulo.Descrizione

            Me.VisualizzaSchedaTecnica(modulo)

        End If
    End Sub

    Private Sub VisualizzaSchedaTecnica(ByVal modulo As ParsecAdmin.Modulo)
        If Not modulo.Abilitato Then
            If Not String.IsNullOrEmpty(modulo.UrlSchedaTecnica) Then
                Me.PdfFrame.Visible = True
                Me.PdfFrame.Attributes.Add("src", modulo.UrlSchedaTecnica)
                MainPage.NomeModulo &= " - Scheda Tecnica"
            End If
        End If
    End Sub

    Protected Sub MasterPage_OnModify(ByVal sender As Object, ByVal e As System.EventArgs) Handles MainPage.OnModify
        Response.Write("Modifica")
    End Sub

    Protected Sub MasterPage_OnNew(ByVal sender As Object, ByVal e As System.EventArgs) Handles MainPage.OnNew
        Response.Write("Nuovo")
    End Sub


End Class