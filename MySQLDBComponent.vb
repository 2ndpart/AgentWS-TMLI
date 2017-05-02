
Imports System.Data
Imports System.Data.SqlClient

Namespace MySQLDBComponent
    Public Class MySQLDBComponent
        Implements IDisposable

#Region "Private Attributes"
        Private dbConn As SqlConnection
        Private dbCM As SqlCommand
        Private dbTrans As SqlTransaction
        Private disposedValue As Boolean = False        ' To detect redundant calls
#End Region

#Region "Constructors"
        Public Sub New()
            dbConn = New SqlConnection
            dbCM = New SqlCommand

            dbCM.Connection = dbConn
        End Sub

        Public Sub New(ByVal connectionString As String)
            dbConn = New SqlConnection(connectionString)
            dbCM = New SqlCommand

            dbCM.Connection = dbConn
        End Sub
#End Region

#Region "Property"
        Public Property Connection() As SqlConnection
            Get
                Return dbConn
            End Get
            Set(ByVal value As SqlConnection)
                dbConn = value
            End Set
        End Property

        Public Property ConnectionString() As String
            Get
                Return dbConn.ConnectionString
            End Get
            Set(ByVal Value As String)
                dbConn.ConnectionString = Value
            End Set
        End Property

        Public ReadOnly Property Parameters() As SqlParameterCollection
            Get
                Return dbCM.Parameters
            End Get
        End Property
#End Region

#Region "Public Methods"

        Public Function ExecuteSqlCommand(command As SqlCommand) As Boolean
            Try
                Dim conn As New SqlConnection
                conn = dbConn

                If conn.State = ConnectionState.Closed Or ConnectionState.Closed Then
                    conn.Open()
                End If
                command.Connection = conn
                Dim affectedRecored As Integer = command.ExecuteNonQuery()

                If affectedRecored > 0 Then
                    Return True
                Else
                    Return False
                End If

            Catch ex As Exception
                Return False
            End Try
        End Function

        Public Overloads Function AddParameter(ByVal parameterName As String, _
                                               ByVal value As Object) As Boolean
            Dim dbParam As SqlParameter

            Try
                dbParam = New SqlParameter()
                dbParam.ParameterName = parameterName
                dbParam.Value = value
                dbParam.Direction = ParameterDirection.Input
                dbCM.Parameters.Add(dbParam)
                Return True
            Catch ex As Exception
                Trace("AddParameter(String, Object) : " & ex.Message)
                Return False
            End Try
        End Function

        Public Overloads Function AddParameter(ByVal parameterName As String, _
                                               ByVal dbTypes As SqlDbType, _
                                               ByVal value As Object) As Boolean
            Dim dbParam As SqlParameter

            Try
                dbParam = New SqlParameter(parameterName, dbTypes)
                dbParam.Value = value
                dbParam.Direction = ParameterDirection.Input
                dbCM.Parameters.Add(dbParam)

                Return True
            Catch ex As Exception
                Trace("AddParameter(String, SqlDbType, Object) : " & ex.Message)
                Return False
            End Try
        End Function

        Public Overloads Function AddParameter(ByVal parameterName As String, _
                                               ByVal dbTypes As SqlDbType, _
                                               ByVal value As Object, _
                                               ByVal size As Integer) As Boolean
            Dim dbParam As SqlParameter

            Try
                dbParam = New SqlParameter(parameterName, dbTypes)
                dbParam.Value = value
                dbParam.Direction = ParameterDirection.Input
                If size > -1 Then
                    dbParam.Size = size
                End If
                dbCM.Parameters.Add(dbParam)
                Return True
            Catch ex As Exception
                Trace("AddParameter(String, SqlDbType, Object, Integer) : " & ex.Message)
                Return False
            End Try
        End Function

        Public Overloads Function AddParameter(ByVal parameterName As String, _
                                               ByVal dbTypes As SqlDbType, _
                                               ByVal direction As ParameterDirection) As Boolean
            Dim dbParam As SqlParameter

            Try
                dbParam = New SqlParameter(parameterName, dbTypes)
                dbParam.Direction = direction
                dbCM.Parameters.Add(dbParam)
                Return True
            Catch ex As Exception
                Trace("AddParameter(String, SqlDbType, ParameterDirection) : " & ex.Message)
                Return False
            End Try
        End Function

        Public Overloads Function AddParameter(ByVal parameterName As String, _
                                               ByVal dbTypes As SqlDbType, _
                                               ByVal value As Object, _
                                               ByVal direction As ParameterDirection) As Boolean
            Dim dbParam As SqlParameter

            Try
                dbParam = New SqlParameter(parameterName, dbTypes)
                dbParam.Value = value
                dbParam.Direction = direction
                dbCM.Parameters.Add(dbParam)
                Return True
            Catch ex As Exception
                Trace("AddParameter(String, SqlDbType, Object, ParameterDirection) : " & ex.Message)
                Return False
            End Try
        End Function

        Public Overloads Function AddParameter(ByVal parameterName As String, _
                                               ByVal dbTypes As SqlDbType, _
                                               ByVal direction As ParameterDirection, _
                                               ByVal size As Integer) As Boolean
            Dim dbParam As SqlParameter

            Try
                dbParam = New SqlParameter(parameterName, dbTypes)
                dbParam.Size = size
                dbParam.Direction = direction
                dbCM.Parameters.Add(dbParam)
                Return True
            Catch ex As Exception
                Trace("AddParameter(String, SqlDbType, Object, Integer, ParameterDirection) : " & ex.Message)
                Return False
            End Try
        End Function

        Public Overloads Function AddParameter(ByVal parameterName As String, _
                                               ByVal dbTypes As SqlDbType, _
                                               ByVal value As Object, _
                                               ByVal size As Integer, _
                                               ByVal direction As ParameterDirection) As Boolean
            Dim dbParam As SqlParameter

            Try
                dbParam = New SqlParameter(parameterName, dbTypes)
                dbParam.Value = value
                dbParam.Size = size
                dbParam.Direction = direction
                dbCM.Parameters.Add(dbParam)
                Return True
            Catch ex As Exception
                Trace("AddParameter(String, SqlDbType, Object, Integer, ParameterDirection) : " & ex.Message)
                Return False
            End Try
        End Function

        Public Overloads Function ExecuteProcedure(ByVal ProcedureName As String) As Boolean
            Try
                ' Initialize
                dbCM.Transaction = dbTrans
                dbCM.CommandText = ProcedureName
                dbCM.CommandType = CommandType.StoredProcedure
                ' Establish database connection
                If dbConn.State = ConnectionState.Broken Or _
                   dbConn.State = ConnectionState.Closed Then
                    dbConn.Open()
                End If
                ' Process...
                dbCM.ExecuteNonQuery()
                Return True
            Catch ex As Exception
                Trace("ExecuteProcedure(" & ProcedureName & ") : " & ex.Message)
                Return False
            End Try
        End Function

        Public Overloads Function ExecuteProcedure(ByRef DataRetriever As Object, ByVal ProcedureName As String) As Boolean
            Dim dbDA As SqlDataAdapter

            Try
                ' Initialize
                dbCM.Transaction = dbTrans
                dbCM.CommandText = ProcedureName
                dbCM.CommandType = CommandType.StoredProcedure
                dbDA = New SqlDataAdapter(dbCM)
                ' Establish database connection
                If dbConn.State = ConnectionState.Broken Or _
                   dbConn.State = ConnectionState.Closed Then
                    dbConn.Open()
                End If
                ' Process...
                If DataRetriever.GetType.Name = "DataTable" Or DataRetriever.GetType.Name = "DataSet" Then
                    dbDA.Fill(DataRetriever)
                Else
                    Throw New Exception("Invalid DataRetriever Object Assignment.")
                End If
                Return True
            Catch ex As Exception
                Trace("ExecuteProcedure(DataRetriever, " & ProcedureName & ") : " & ex.Message)
                Return False
            Finally
                dbDA = Nothing
            End Try
        End Function

        Public Overloads Function ExecuteProcedure(ByRef DataReader As SqlDataReader, ByVal ProcedureName As String) As Boolean
            Dim dbDR As SqlDataReader

            Try
                ' Initialize
                dbCM.Transaction = dbTrans
                dbCM.CommandText = ProcedureName
                dbCM.CommandType = CommandType.StoredProcedure
                ' Establish database connection
                If dbConn.State = ConnectionState.Broken Or _
                   dbConn.State = ConnectionState.Closed Then
                    dbConn.Open()
                End If
                ' Process...
                dbDR = dbCM.ExecuteReader
                DataReader = dbDR
                Return True
            Catch ex As Exception
                Trace("ExecuteProcedure(DataReader, " & ProcedureName & ") : " & ex.Message)
                Return False
            End Try
        End Function

        Public Overloads Function ExecuteSQL(ByVal SQLStatement As String) As Boolean
            Try
                ' Initialize
                dbCM.Transaction = dbTrans
                dbCM.CommandText = SQLStatement
                dbCM.CommandType = CommandType.Text
                ' Establish database connection
                If dbConn.State = ConnectionState.Broken Or _
                   dbConn.State = ConnectionState.Closed Then
                    dbConn.Open()
                End If
                ' Process...
                dbCM.ExecuteNonQuery()
                Return True
            Catch ex As Exception
                Trace("ExecuteSQL(" & SQLStatement & ") : " & ex.Message)
                Return False
            End Try
        End Function

        Public Overloads Function ExecuteSQL(ByRef DataRetriever As Object, ByVal SQLStatement As String) As Boolean
            Dim dbDA As SqlDataAdapter

            Try
                ' Initialize
                dbCM.Transaction = dbTrans
                dbCM.CommandText = SQLStatement
                dbCM.CommandType = CommandType.Text
                dbDA = New SqlDataAdapter(dbCM)
                ' Establish database connection
                If dbConn.State = ConnectionState.Broken Or _
                   dbConn.State = ConnectionState.Closed Then
                    dbConn.Open()
                End If
                ' Process...
                If DataRetriever.GetType.Name = "DataTable" Or DataRetriever.GetType.Name = "DataSet" Then
                    dbDA.Fill(DataRetriever)
                Else
                    Throw New Exception("Invalid DataRetriever Object Assignment.")
                End If
                Return True
            Catch ex As Exception
                Trace("ExecuteSQL(DataRetriever, " & SQLStatement & ") : " & ex.Message)
                Return False
            Finally
                dbDA = Nothing
            End Try
        End Function

        Public Overloads Function ExecuteSQL(ByRef DataReader As SqlDataReader, ByVal SQLStatement As String) As Boolean
            'Dim dbDR As SqlDataReader

            Try
                ' Initialize
                dbCM.Transaction = dbTrans
                dbCM.CommandText = SQLStatement
                dbCM.CommandType = CommandType.Text
                ' Establish database connection
                If dbConn.State = ConnectionState.Broken Or _
                   dbConn.State = ConnectionState.Closed Then
                    dbConn.Open()
                End If
                ' Process...
                'dbDR = dbCM.ExecuteReader()
                'DataReader = dbDR
                DataReader = dbCM.ExecuteReader()
                Return True
            Catch ex As Exception
                Trace("ExecuteSQL(DataReader, " & SQLStatement & ") : " & ex.Message)
                Return False
            End Try
        End Function

        Public Overloads Function ExecuteProcedureScalar(ByVal ProcedureName As String, ByRef ReturnValue As Object) As Boolean
            Try
                ' Initialize
                dbCM.Transaction = dbTrans
                dbCM.CommandText = ProcedureName
                dbCM.CommandType = CommandType.StoredProcedure
                ' Establish database connection
                If dbConn.State = ConnectionState.Broken Or _
                   dbConn.State = ConnectionState.Closed Then
                    dbConn.Open()
                End If
                ' Process...
                ReturnValue = dbCM.ExecuteScalar()
                Return True
            Catch ex As Exception
                Trace("ExecuteProcedureScalar(" & ProcedureName & ",ReturnValue) : " & ex.Message)
                Return False
            End Try
        End Function

        Public Overloads Function ExecuteSQLScalar(ByVal SQLStatement As String, ByRef ReturnValue As Object) As Boolean
            Try
                ' Initialize
                dbCM.Transaction = dbTrans
                dbCM.CommandText = SQLStatement
                dbCM.CommandType = CommandType.Text
                ' Establish database connection
                If dbConn.State = ConnectionState.Broken Or _
                   dbConn.State = ConnectionState.Closed Then
                    dbConn.Open()
                End If
                ' Process...
                ReturnValue = dbCM.ExecuteScalar()
                Return True
            Catch ex As Exception
                Trace("ExecuteSQLScalar(" & SQLStatement & ",ReturnValue) : " & ex.Message)
                Return False
            End Try
        End Function

        Public Sub BeginTransaction()
            ' Establish database connection
            If dbConn.State = ConnectionState.Broken Or _
               dbConn.State = ConnectionState.Closed Then
                dbConn.Open()
            End If
            ' Process...
            dbTrans = dbConn.BeginTransaction()
        End Sub

        Public Overloads Function Commit() As Boolean
            Try
                dbTrans.Commit()
            Catch ex As Exception
                Trace("Commit() : " & ex.Message)
            End Try
        End Function

        Public Overloads Function Rollback() As Boolean
            Try
                dbTrans.Rollback()
            Catch ex As Exception
                Trace("Rollback() : " & ex.Message)
            End Try
        End Function

        Public Function ConvertEmptyToNull(ByVal value As Object) As Object
            If Not IsDate(value) AndAlso value = String.Empty Then
                Return DBNull.Value
            Else
                Return value
            End If
        End Function

        Public Function ConvertNullToEmpty(ByVal value As Object) As String
            If value Is DBNull.Value Then
                Return String.Empty
            Else
                Return value
            End If
        End Function

        Public Function ConvertNumToNull(ByVal value As Integer) As Object
            If value = 0 Then
                Return DBNull.Value
            Else
                Return value
            End If
        End Function

        Public Function ConvertNullToNum(ByVal value As Object) As Integer
            If value Is DBNull.Value Then
                Return 0
            Else
                Return value
            End If
        End Function
#End Region

#Region "Private Methods"
        Private Sub Trace(ByVal ErrorMessage As String)
            Dim StreamLogFile As System.IO.FileStream
            Dim WriteLogFile As System.IO.StreamWriter
            Dim strCurrentLogFile As String

            Try
                strCurrentLogFile = AppDomain.CurrentDomain.BaseDirectory & "Log\MySQLDBComponent" & Now.ToString("yyyy-MM-dd") & ".txt_"
                StreamLogFile = New System.IO.FileStream(strCurrentLogFile, IO.FileMode.Append, IO.FileAccess.Write, IO.FileShare.ReadWrite)
                WriteLogFile = New System.IO.StreamWriter(StreamLogFile)
                WriteLogFile.WriteLine(Now.ToString("hh:mm:sstt") & " -- " & ErrorMessage)
                WriteLogFile.Close()
                StreamLogFile.Close()
            Catch ex As Exception
                If Not WriteLogFile Is Nothing Then
                    WriteLogFile.Close()
                End If
                If Not StreamLogFile Is Nothing Then
                    StreamLogFile.Close()
                End If
            End Try
        End Sub
#End Region

#Region "Dispose Methods"
        ' IDisposable
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: free unmanaged resources when explicitly called
                    If Not dbConn Is Nothing Then
                        If dbConn.State = ConnectionState.Open Then
                            dbConn.Close()
                        End If
                        dbConn.Dispose()
                    End If
                End If

                ' TODO: free shared unmanaged resources
                dbCM = Nothing
                dbConn = Nothing
            End If
            Me.disposedValue = True
        End Sub
#End Region

#Region " IDisposable Support "
        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region
    End Class
End Namespace
