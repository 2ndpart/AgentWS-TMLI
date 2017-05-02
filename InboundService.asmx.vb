Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports System.Xml
Imports System.IO
Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class InboundService
    Inherits System.Web.Services.WebService
    Dim insertLog As New Dictionary(Of String, Integer)()

    <WebMethod()>
    Public Function HelloWorld() As String
        Return "Hello World"
    End Function

    <WebMethod()>
    Public Function ReceiveStringToTable(xmlString As String) As Boolean

        Dim dSet As New DataSet
        Dim objDBCom As New MySQLDBComponent.MySQLDBComponent(POSWeb.POSWeb_SQLConn)
        Try
            dSet.ReadXml(New XmlTextReader(New System.IO.StringReader(xmlString)))
            Dim dTable As DataTable = dSet.Tables(0)

            If dTable.TableName = "Agent_profile" Then
                Dim dts As New DataTable
                Dim table As New DataTable
                Dim table2 As New DataTable
                Dim table3 As New DataTable

                Dim j As Integer = 0
                dTable.Columns.Remove("IndexNo")
                Dim extra As DataColumn = dTable.Columns.Add("IndexNo", System.Type.[GetType]("System.Boolean"))
                extra.SetOrdinal(2)

                For Each ro As DataRow In dTable.Rows
                    Dim i As Integer = 0
                    For Each col As DataColumn In dTable.Columns
                        If i = 12 Then
                            dTable.Rows(j)(i) = DateTime.Parse(dTable.Rows(j)(i))
                        ElseIf i = 13 Then
                            dTable.Rows(j)(i) = DateTime.Parse(dTable.Rows(j)(i))
                        ElseIf i = 26 Then
                            dTable.Rows(j)(i) = DateTime.Parse(dTable.Rows(j)(i))
                        ElseIf i = 27 Then
                            dTable.Rows(j)(i) = DateTime.Now
                        ElseIf i = 29 Then
                            dTable.Rows(j)(i) = DateTime.Parse(dTable.Rows(j)(i))
                        End If
                        i += 1
                    Next
                    j += 1
                Next

                Dim dtcloned As New DataTable
                dtcloned = dTable.Clone()
                dtcloned.Columns(12).DataType = GetType(DateTime)
                dtcloned.Columns(13).DataType = GetType(DateTime)
                dtcloned.Columns(26).DataType = GetType(DateTime) 'THIS ALSO
                dtcloned.Columns(27).DataType = GetType(DateTime) 'REMOVE IF XML DONT HAVE SINCE MPOS DIDNT INCLUDE THIS COLUMN
                dtcloned.Columns(29).DataType = GetType(DateTime) 'SAME AS THIS TOO
                For Each rt As DataRow In dTable.Rows
                    dtcloned.ImportRow(rt)
                Next

                Dim q1 As String = ""
                Dim x As String = ""
                q1 &= "select AgentCode, AgentEmail from Agent_profile"
                objDBCom.ExecuteSQL(dts, q1)
                For Each dtrow As DataRow In dts.Rows
                    For count As Integer = 0 To dts.Rows.Count - 1
                        Dim found As Boolean = True
                        Dim rowCount1 As Integer = dtcloned.Rows.Count - 1
                        For count1 As Integer = 0 To rowCount1
                            If count1 <= rowCount1 Then
                                If dts.Rows(count)(0).ToString() = dtcloned.Rows(count1)(3).ToString() Then
                                    'dtclone.Rows(count1).Delete()
                                    x = count1
                                    found = False
                                    If found = False Then
                                        dtcloned.Rows(x).Delete()
                                        rowCount1 -= 1
                                    End If
                                ElseIf dts.Rows(count)(1).ToString() = dtcloned.Rows(count1)(5).ToString() Then
                                    x = count1
                                    found = False
                                    If found = False Then
                                        dtcloned.Rows(x).Delete()
                                        rowCount1 -= 1
                                    End If
                                End If
                            End If
                        Next
                    Next
                Next

                Dim conn As String = ConfigurationManager.ConnectionStrings("constr").ConnectionString
                Using con As New SqlConnection(conn)
                    Using SqlBulkCopy As New SqlBulkCopy(con)
                        SqlBulkCopy.DestinationTableName = "dbo.Agent_profile"
                        con.Open()
                        SqlBulkCopy.WriteToServer(dtcloned)
                        con.Close()
                    End Using
                End Using

                If Not dtcloned.Rows.Count = 0 Then
                    For Each r As DataRow In dtcloned.Rows
                        Dim sql6 As String = "select AgentPassword from Agent_profile where AgentCode = '" + dtcloned.Rows(x)(14) + "'"
                        Dim sql7 As String = "select AgentStatus from Agent_profile where AgentCode = '" + dtcloned.Rows(x)(14) + "'"

                        objDBCom.ExecuteSQL(table, sql6)
                        If Not table.Rows.Count = 0 Then
                            Dim sql8 As String = "update Agent_profile set DirectSupervisorPassword = '" + table.Rows(0)(0).ToString + "' where AgentCode = '" + dtcloned.Rows(x)(3) + "'"
                            objDBCom.ExecuteSQL(sql8)
                        End If

                        objDBCom.ExecuteSQL(table3, sql7)
                        If Not table.Rows.Count = 0 Then
                            Dim sql9 As String = "update Agent_profile set DirectSupervisorStatus = '" + table3.Rows(0)(0).ToString + "' where AgentCode = '" + dtcloned.Rows(x)(3) + "'"
                            objDBCom.ExecuteSQL(sql9)
                        Else
                            Dim sql10 As String = "update Agent_profile set DirectSupervisorStatus = 'A' where AgentCode = '" + dtcloned.Rows(x)(3) + "'"
                            objDBCom.ExecuteSQL(sql10)
                        End If
                        x += 1
                    Next
                End If


                'UNDER THE ASSUMPTION OF ALL DATA WILL BE PAST FROM SINO TO HERE, I WILL SKIP THE LAST PART OF THE CODE IN THE IMPORTAGENT.ASPX AS IT DEEMS UNNECCESSARY.
                'UNTIL FUTHER EVALUTATION.

            ElseIf dTable.TableName = "Data_Cabang" Then
                Dim j As Integer = 0
                For Each ro As DataRow In dTable.Rows
                    Dim i As Integer = 0
                    For Each col As DataColumn In dTable.Columns
                        If i = 19 Then
                            dTable.Rows(j)(i) = DateTime.Now
                        End If
                        i += 1
                    Next
                    j += 1
                Next

                Dim dtcloned As New DataTable
                dtcloned = dTable.Clone()
                For Each col As DataColumn In dTable.Columns
                    If col.ColumnName.Equals("TanggalRollOut") Then
                        dtcloned.Columns(19).DataType = GetType(DateTime)
                    End If
                Next
                For Each rt As DataRow In dTable.Rows
                    dtcloned.ImportRow(rt)
                Next

                Dim query As String = "select * from Data_Cabang"
                Dim dt1 As New DataTable
                Dim x As Integer = 0
                objDBCom.ExecuteSQL(dt1, query)
                For count As Integer = 0 To dt1.Rows.Count - 1
                    Dim found As Boolean = True
                    Dim rowCount1 As Integer = dtcloned.Rows.Count - 1
                    For count1 As Integer = 0 To rowCount1
                        If count1 <= rowCount1 Then
                            If dt1.Rows(count)(7).ToString() = dtcloned.Rows(count1)(7).ToString() Then
                                'dtclone.Rows(count1).Delete()
                                x = count1
                                found = False
                                If found = False Then
                                    dtcloned.Rows(x).Delete()
                                    rowCount1 -= 1
                                End If
                            End If
                        End If
                    Next
                Next

                Dim conn As String = ConfigurationManager.ConnectionStrings("constr").ConnectionString
                Using con As New SqlConnection(conn)
                    Using SqlBulkCopy As New SqlBulkCopy(con)
                        SqlBulkCopy.DestinationTableName = "dbo.Data_Cabang"
                        con.Open()
                        SqlBulkCopy.WriteToServer(dtcloned)
                        con.Close()
                    End Using
                End Using

                Dim RC As Integer = 0
                If Not dtcloned.Rows.Count = 0 Then
                    For Each rt As DataRow In dtcloned.Rows
                        Dim query3 As String = "update Data_Cabang set TanggalRollOut = NULL, Status = 'A', IsNew = GETDATE(), RowStatus = 'N' where  " + dtcloned.Columns(0).ColumnName + " = '" + dtcloned.Rows(RC)(0).ToString() + "'"
                        objDBCom.ExecuteSQL(query3)
                        RC += 1
                    Next
                End If
                Dim q1 As String = ""
                Dim q2 As String = ""
                q1 &= "UPDATE Master_Info set VersionNo = VersionNo + 1, UpdatedDate = GETDATE() where TableName = 'Data_Cabang'"
                q2 &= "UPDATE Master_Info set VersionNo = VersionNo + 1, UpdatedDate = GETDATE() where TableName = 'Master_Info'"
                objDBCom.ExecuteSQL(q1)
                objDBCom.ExecuteSQL(q2)

            ElseIf dTable.TableName = "DataReferral" Then
                'Dim extra As DataColumn = dTable.Columns.Add("ID", System.Type.[GetType]("System.Boolean"))
                'extra.SetOrdinal(0)
                'NEED TO WAIT UNTIL THE SAMPEL DATA SEND TO US TO MAKE ADJUSTMENT
                Dim j As Integer = 0
                For Each ro As DataRow In dTable.Rows
                    Dim i As Integer = 0
                    For Each col As DataColumn In dTable.Columns
                        If i = 4 Then
                            dTable.Rows(j)(i) = DateTime.Parse(dTable.Rows(j)(i))
                            i += 1
                        End If
                    Next
                    j += 1
                Next

                Dim dtcloned As New DataTable
                dtcloned = dTable.Clone()
                For Each col As DataColumn In dTable.Columns
                    If col.ColumnName.Equals("DateOfBirth") Then
                        dtcloned.Columns(4).DataType = GetType(DateTime)
                    End If
                Next
                For Each rt As DataRow In dTable.Rows
                    dtcloned.ImportRow(rt)
                Next
                dtcloned.Columns.Remove("ID")
                For Each dRow As DataRow In dtcloned.Rows
                    InsertDataIntoDB(dRow)
                Next

                Dim q3 As String = ""
                Dim q4 As String = ""
                q3 &= "UPDATE Master_Info set VersionNo = VersionNo + 1, UpdatedDate = GETDATE() where TableName = 'DataReferral'"
                q4 &= "UPDATE Master_Info set VersionNo = VersionNo + 1, UpdatedDate = GETDATE() where TableName = 'Master_Info'"
                objDBCom.ExecuteSQL(q3)
                objDBCom.ExecuteSQL(q4)
            End If



            objDBCom.Dispose()
            Return True
        Catch ex As Exception
            Return False
        End Try

    End Function

    Private Sub InsertDataIntoDB(dataToInsert As DataRow)
        Dim objDBCom As New MySQLDBComponent.MySQLDBComponent(POSWeb.POSWeb_SQLConn)
        Dim objDBCom2 As New MySQLDBComponent.MySQLDBComponent(POSWeb.POSWeb_SQLConn)
        Dim insertStatement As String = "INSERT INTO DataReferral ([NIP],[Name],[BirthPlace],[DateOfBirth],[Details],[Status], IsNew, IsUpdate) VALUES (@nip,@name,@birthPlace,@dateOfBirth,@details,@status, GETDATE(), GETDATE())"
        objDBCom.Parameters.AddWithValue("@nip", dataToInsert("NIP"))
        objDBCom.Parameters.AddWithValue("@name", dataToInsert("Name"))
        objDBCom.Parameters.AddWithValue("@birthPlace", dataToInsert("BirthPlace"))
        objDBCom.Parameters.AddWithValue("@dateOfBirth", dataToInsert("DateOfBirth"))
        objDBCom.Parameters.AddWithValue("@details", dataToInsert("Details"))
        objDBCom.Parameters.AddWithValue("@status", dataToInsert("Status"))
        Try
            Dim affectedRecord = objDBCom.ExecuteSQL(insertStatement)
            objDBCom.Dispose()
            If affectedRecord = False Then
                Dim updateCommandString As String = "UPDATE DataReferral SET [Name]=@name, [BirthPlace]=@birthPlace,[DateOfBirth]=@dateOfBirth,[Details]=@details,[Status]=@status, IsUpdate = GETDATE() WHERE NIP=@nip"
                objDBCom2.Parameters.AddWithValue("@nip", dataToInsert("NIP"))
                objDBCom2.Parameters.AddWithValue("@name", dataToInsert("Name"))
                objDBCom2.Parameters.AddWithValue("@birthPlace", dataToInsert("BirthPlace"))
                objDBCom2.Parameters.AddWithValue("@dateOfBirth", dataToInsert("DateOfBirth"))
                objDBCom2.Parameters.AddWithValue("@details", dataToInsert("Details"))
                objDBCom2.Parameters.AddWithValue("@status", dataToInsert("Status"))
                Dim affectedURecord = objDBCom2.ExecuteSQL(updateCommandString)
                objDBCom2.Dispose()
                If affectedURecord = False Then

                End If
            End If
            insertLog.Add(dataToInsert("NIP").ToString(), affectedRecord)
        Catch ex As Exception

        End Try
    End Sub

End Class