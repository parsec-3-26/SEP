
Partial Class PopupPage
    Inherits System.Web.UI.MasterPage

    Public Sub RegisterComponent(ByVal script As String)
        Me.componentPlaceHolder.Controls.Add(New LiteralControl(script))
    End Sub

    Public Sub ImpostaLarghezzaHeader(ByVal width As String)
        Me.MAIN.Style.Remove("width")
        Me.MAIN.Style.Add("width", width & "px")
     End Sub

    Public Sub ImpostaAltezza(ByVal height As String)
        Me.MAIN.Style.Remove("height")
        Me.MAIN.Style.Add("height", height & "px")
    End Sub

    Public Property DescrizioneProcedura As String
        Set(ByVal value As String)
            Me.DescrizioneProceduraLabel.Text = value
        End Set
        Get
            Return Me.DescrizioneProceduraLabel.Text
        End Get
    End Property

End Class

