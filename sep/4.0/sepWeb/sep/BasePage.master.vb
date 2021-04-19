
Partial Class BasePage
    Inherits System.Web.UI.MasterPage

    Public Sub RegisterComponent(ByVal script As String)
        Dim cell As New TableCell
        cell.Width = New Unit(0)
        cell.HorizontalAlign = HorizontalAlign.Right

        cell.Controls.Add(New LiteralControl(script))
        Me.componentPlaceHolder.Rows(0).Cells.Add(cell)
    End Sub

    Public Sub ImpostaLarghezzaHeader(ByVal width As String)
        Me.MAIN.Style.Remove("width")
        Me.MAIN.Style.Add("width", width & "px")
    End Sub

    Public Sub NascondiHeader()
        Me.HEADER.Style.Add("display", "none")
    End Sub

    Public Property DescrizioneProcedura As String
        Set(ByVal value As String)
            Me.DescrizioneProceduraLabel.Text = value
        End Set
        Get
            Return Me.DescrizioneProceduraLabel.Text
        End Get
    End Property

    Protected Sub Page_Init(sender As Object, e As System.EventArgs) Handles Me.Init
        Me.corpo.Attributes.Add("onload", "self.focus()")
    End Sub
End Class

