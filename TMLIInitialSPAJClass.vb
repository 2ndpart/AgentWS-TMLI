Public Class TMLIInitialSPAJClass
    Dim _spaj_no As List(Of String)

    Public Property spaj_no() As List(Of String)
        Get
            Return _spaj_no
        End Get
        Set(ByVal value As List(Of String))
            _spaj_no = value
        End Set
    End Property
End Class
