Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Globalization
Imports System.Security.Cryptography
Imports System.Data.SqlClient

Public Class POSWeb
    Public Shared POSWeb_SQLConn = ConfigurationManager.AppSettings("POSWeb_SQLConn")
    Public Shared conn As SqlConnection = New SqlConnection(ConfigurationManager.AppSettings("POSWeb_SQLConn").ToString())
    Public Shared comm As SqlCommand
    Public Shared dr As SqlDataReader

    Public Shared Sub DDL_LoadRules(ByRef Ddl As DropDownList, ByVal RulesName As String, Optional ByVal DefaultMsg As String = "")

        Dim objDBCom As New MySQLDBComponent.MySQLDBComponent(POSWeb_SQLConn)
        Dim dt As New DataTable

        Dim sqlS As String = "EXEC sp_helptext " & RulesName & ""

        If objDBCom.ExecuteSQL(dt, sqlS) Then

            If dt.Rows.Count > 0 Then
                For Each drRow As DataRow In dt.Rows

                    If Not drRow.Item(0) Is DBNull.Value Then

                        Dim st As String = drRow.Item(0)
                        st = st.Substring(st.IndexOf("@list IN (") + 10, st.Length - st.IndexOf("@list IN (") - 11)

                        Dim ch As Char() = {","c}
                        Dim str As String() = st.ToString.Trim.Split(ch)
                        Dim i As Integer
                        For i = 0 To str.Length - 1
                            If Not str(i).Trim = "" Then
                                Dim stt As String = str(i).Trim()
                                stt = stt.Substring(1, stt.Length - 2)
                                Ddl.Items.Add(New ListItem(stt, stt))
                            End If
                        Next i
                    End If

                    'Ddl.Items.Add(New ListItem(drRow(IIf(IsNumeric("PText"), Convert.ToInt32("PText"), "PText")), drRow(IIf(IsNumeric("PValue"), Convert.ToInt32("PValue"), "PValue"))))

                Next
            End If
            Ddl.Items.Insert(0, New ListItem(IIf(DefaultMsg <> "", DefaultMsg, "Select One"), "-1"))
        End If
        objDBCom.Dispose()
        dt.Dispose()
    End Sub
    Public Shared Sub DDL_LoadParameter(ByRef Ddl As DropDownList, ByVal Type As String, Optional ByVal DefaultMsg As String = "")

        Dim objDBCom As New MySQLDBComponent.MySQLDBComponent(POSWeb_SQLConn)
        Dim dt As New DataTable

        objDBCom.Parameters.Clear()
        objDBCom.AddParameter("i_PType", SqlDbType.Int, CType(Type, Integer))

        If objDBCom.ExecuteProcedure(dt, "Parameters_GetBy_Type") Then

            If dt.Rows.Count > 0 Then
                For Each drRow As DataRow In dt.Rows
                    Ddl.Items.Add(New ListItem(drRow.Item("PText"), drRow.Item("PValue")))
                    'Ddl.Items.Add(New ListItem(drRow(IIf(IsNumeric("PText"), Convert.ToInt32("PText"), "PText")), drRow(IIf(IsNumeric("PValue"), Convert.ToInt32("PValue"), "PValue"))))
                Next
            End If
            Ddl.Items.Insert(0, New ListItem(IIf(DefaultMsg <> "", DefaultMsg, "Select One"), "-1"))
        End If
        objDBCom.Dispose()
        dt.Dispose()
    End Sub

    Public Shared Sub DDLChild_LoadParameter(ByRef Ddl As DropDownList, ByVal parentid As Integer, ByVal Type As String, Optional ByVal DefaultMsg As String = "")

        Dim objDBCom As New MySQLDBComponent.MySQLDBComponent(POSWeb_SQLConn)
        Dim dt As New DataTable
        Ddl.Items.Clear()
        objDBCom.Parameters.Clear()
        objDBCom.AddParameter("i_PParentID", SqlDbType.Int, parentid)
        objDBCom.AddParameter("i_PType", SqlDbType.VarChar, Type)

        If objDBCom.ExecuteProcedure(dt, "Parameters_GetBy_Parent1") Then

            If dt.Rows.Count > 0 Then
                For Each drRow As DataRow In dt.Rows
                    Ddl.Items.Add(New ListItem(drRow.Item("PText"), drRow.Item("PValue")))
                    'Ddl.Items.Add(New ListItem(drRow(IIf(IsNumeric("PText"), Convert.ToInt32("PText"), "PText")), drRow(IIf(IsNumeric("PValue"), Convert.ToInt32("PValue"), "PValue"))))
                Next
            End If
            Ddl.Items.Insert(0, New ListItem(IIf(DefaultMsg <> "", DefaultMsg, "Select One"), "0"))
        End If
        objDBCom.Dispose()
        dt.Dispose()
    End Sub
    Public Shared Function FormatToDecimal(ByVal value As Object, Optional ByVal isPoint As Boolean = False) As String

        If value Is Nothing Or value Is DBNull.Value Then
            Return String.Empty
        Else
            Dim c As Double = CType(value, Double)
            Dim nfi As NumberFormatInfo = New CultureInfo("en-US", False).NumberFormat
            Dim s As String = c.ToString("N", nfi)
            If Not isPoint Then
                s = s.Substring(0, (s.Length - 3))
            End If
            Return s
        End If


    End Function
    Public Shared Sub DBLog_Add(ByVal moduleS As String, ByVal ops As String, ByVal infoS As String)
        Dim StreamLogFile As System.IO.FileStream
        Dim WriteLogFile As System.IO.StreamWriter
        Dim strCurrentLogFile As String

        Try
            strCurrentLogFile = AppDomain.CurrentDomain.BaseDirectory & "Log\DBLog_" & Now.ToString("yyyy-MM-dd") & ".txt"
            StreamLogFile = New System.IO.FileStream(strCurrentLogFile, IO.FileMode.Append, IO.FileAccess.Write, IO.FileShare.ReadWrite)
            WriteLogFile = New System.IO.StreamWriter(StreamLogFile)
            WriteLogFile.WriteLine(Now.ToString("hh:mm:sstt") & " -- " & moduleS & " -- " & ops & " -- " & infoS)
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

    Public Shared Function RandomString_Get() As String
        Dim arrPossibleChars As Char() = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray()

        Dim intPasswordLength As Integer = 6
        Dim stringPassword As String = Nothing
        Dim rand As System.Random = New Random
        Dim i As Integer = 0
        For i = 0 To intPasswordLength
            Dim intRandom As Integer = rand.Next(arrPossibleChars.Length)
            stringPassword = stringPassword & arrPossibleChars(intRandom).ToString()
        Next

        Return stringPassword

    End Function
    '******************************************************************
    '* Sub GetNumberValidatorScript
    '******************************************************************
    '* Sub name         : GetNumberValidatorScript
    '* Parameter        : n/a
    '* Return           : n/a
    '* By               : Faizal 
    '* Create Date      : 7/8/2008
    '* Update Date      : 8/10/2008
    '* Version          : 1.0.0
    '* Purpose          : Create number validator script
    '******************************************************************
    Public Shared Function GetNumberValidatorScript() As String
        Dim _str As String
        _str = "<script> "
        _str += " function FilterNumeric(){"
        ''  _str += " var key; if(window.event){ key = event.keyCode;}else if(event.which){ key = event.which;} return (key == 45 || key == 13 || key == 8 || key == 9 || key == 189 || (key >= 48 && key <= 58) )"
        _str += " if (event.keyCode<48 || event.keyCode>57) event.returnValue = false ;"
        _str += "}</script>"
        Return _str

    End Function

    '******************************************************************
    '* Sub GetDecimalValidatorScript
    '******************************************************************
    '* Sub name         : GetDecimalValidatorScript
    '* Parameter        : n/a
    '* Return           : n/a
    '* By               : Faizal 
    '* Create Date      : 7/8/2008
    '* Update Date      : 8/10/2008
    '* Version          : 1.0.0
    '* Purpose          : Create decimal input validator script
    '******************************************************************
    Public Shared Function GetDecimalNoPointValidatorScript() As String
        Dim _str As String
        _str = "<script> "
        _str += " function FilterDecimal(){"
        _str += " var re; "
        _str += "var ch=String.fromCharCode(event.keyCode);"
        _str += "if (event.keyCode<32)"
        _str += "{"
        _str += "return;"
        _str += "};"
        _str += "if( (event.keyCode<=57)&&(event.keyCode>=48))"
        _str += "{"
        _str += "if (!event.shiftKey)"
        _str += "{"
        _str += "return;"
        _str += "}"
        _str += "}"
        _str += "if ((ch=='.'))"
        _str += "{"
        _str += "return;"
        _str += "}"
        _str += "if ((ch==','))"
        _str += "{"
        _str += "return;"
        _str += "}"
        _str += "event.returnValue=false;"
        _str += "}</script>"
        Return _str

    End Function
    Public Shared Sub Alert(ByRef btn As System.Web.UI.Control, ByRef lbl_Alert As Label, ByVal msg As String, ByVal isSuccess As Boolean)

        If isSuccess Then
            lbl_Alert.Style("color") = "#333333"
        Else
            lbl_Alert.Style("color") = "red"
        End If
        lbl_Alert.Text = msg
        lbl_Alert.Style("font-style") = "italic"
        lbl_Alert.Style("display") = "inline"
        btn.Focus()


    End Sub
    Public Shared Function Record_Delete(ByVal mid As Integer, ByVal cid As Integer, ByVal moduleS As String, Optional ByVal userID As Integer = 0) As Boolean
        Dim returnBoolean As Boolean = False
        Dim objDBCom As New MySQLDBComponent.MySQLDBComponent(POSWeb_SQLConn)
        objDBCom.Parameters.Clear()

        If mid <> 0 Then
            objDBCom.AddParameter("i_mid", SqlDbType.Int, mid)
        End If
        If cid <> 0 Then
            objDBCom.AddParameter("i_cid", SqlDbType.Int, cid)
        End If
        If userID <> 0 Then
            objDBCom.AddParameter("i_Aud_UserID", SqlDbType.Int, userID)
        End If

        If objDBCom.ExecuteProcedure(moduleS & "_Delete") Then
            returnBoolean = True
        Else
            objDBCom.Rollback()
            returnBoolean = False
        End If
        objDBCom.Dispose()
        Return returnBoolean

    End Function

    Public Shared Function Zero_Add(ByVal inputNo As Integer, ByVal noDigit As Integer, ByVal sPrefix As String) As String

        Dim s As String = inputNo.ToString
        Dim s_length As Integer = s.Length
        Dim s_before As String = ""
        Dim i As Integer = 0
        While i < noDigit - s_length
            s_before += "0"
            i += 1
        End While

        Return sPrefix & s_before & s

    End Function

    Public Shared Function Parameter_GetText(ByVal parentid As Integer, ByVal id As Integer, ByVal Type As String) As String
        Dim rtnString As String = ""
        Dim objDBCom As New MySQLDBComponent.MySQLDBComponent(POSWeb_SQLConn)
        Dim dt As New DataTable

        objDBCom.Parameters.Clear()
        objDBCom.AddParameter("i_PParentID", SqlDbType.VarChar, parentid)
        objDBCom.AddParameter("i_PValue", SqlDbType.VarChar, id)
        objDBCom.AddParameter("i_PType", SqlDbType.Int, CType(Type, Integer))
        '' objDBCom.BeginTransaction()
        If objDBCom.ExecuteProcedure(dt, "Parameters_GetText") Then
            ''    objDBCom.Commit()
            If dt.Rows.Count > 0 Then
                rtnString = dt.Rows(0).Item("PText")
            End If
        End If
        objDBCom.Dispose()
        dt.Dispose()
        Return rtnString
    End Function
    Public Shared Function Parameter_GetOrder(ByRef id As Integer, ByVal Type As String) As String
        Dim rtnString As String = ""
        Dim objDBCom As New MySQLDBComponent.MySQLDBComponent(POSWeb_SQLConn)
        Dim dt As New DataTable

        objDBCom.Parameters.Clear()
        objDBCom.AddParameter("i_PValue", SqlDbType.VarChar, id)
        objDBCom.AddParameter("i_PType", SqlDbType.VarChar, Type)
        ''objDBCom.BeginTransaction()
        If objDBCom.ExecuteProcedure(dt, "Parameters_GetOrder") Then

            If dt.Rows.Count > 0 Then
                rtnString = dt.Rows(0).Item("POrder")
            End If
        End If
        objDBCom.Dispose()
        dt.Dispose()
        Return rtnString
    End Function

    Public Shared Function HaveExcess(ByRef Page1 As Page, Optional ByVal isPopUp As Boolean = False, Optional ByVal isPMSAccessRight As Boolean = False) As Boolean

        Dim rtnBoolean As Boolean = False

        If isPMSAccessRight Then

            If Page1.Session("PMSWeb_user") = "0" Or Page1.Session("PMSWeb_user") Is Nothing Then
                If isPopUp Then
                    Page1.Response.Redirect(ConfigurationManager.AppSettings("PMSWeb_WebSiteURL") & "LoginPopUp.aspx?expired=true&page=" & Page1.Request.Url.ToString & "")
                Else
                    Page1.Response.Redirect(ConfigurationManager.AppSettings("PMSWeb_WebSiteURL") & "Login.aspx?expired=true&page=" & Page1.Request.Url.ToString & "")
                End If
                rtnBoolean = False
            Else
                Page1.Session.Timeout = 60
                rtnBoolean = True
            End If

        Else



            Dim pName As String
            pName = HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath

            ''kalau page yang sama, x perlu check lagi
            ''If Page1.Session("Page") = pName Then
            ''    Page1.Response.Write("sessionsama")
            ''    Return True
            ''    Exit Function
            ''End If
            ''Page1.Session("Page") = pName


            ''Check Page is Public
            Dim IsPublic As Boolean = True
            Dim ModuleID As Integer = 0
            Dim dt As New DataTable
            Dim objDBCom As New MySQLDBComponent.MySQLDBComponent(POSWeb.POSWeb_SQLConn)
            If objDBCom.ExecuteSQL(dt, "SELECT ModuleID, IsPublic FROM TBL_PMSWEB_MOD where FileName = '" & pName & "'") Then
                If dt.Rows.Count > 0 Then
                    ModuleID = dt.Rows(0).Item(0)
                    IsPublic = dt.Rows(0).Item(1)
                    rtnBoolean = True
                Else
                    rtnBoolean = True
                End If

            Else
                rtnBoolean = True
            End If
            objDBCom.Dispose()
            dt.Dispose()


            If IsPublic Then
                ''Page1.Response.Write("IsPublic")
                Return True
                Exit Function
            End If


            If Not checkSessionNew(Page1, isPopUp) Then
                '' Page1.Response.Write("checkSessionNew false")
                Return False
                Exit Function
            End If



            If Not IsPublic Then
                Dim sql As String = ""
                sql &= "SELECT Count(*) from TBL_PMSWEB_MOD_ACCESS where ModuleID = " & ModuleID & ""
                sql &= " AND  SUSER = '" & Page1.Session("PMSWeb_user") & "' "

                Dim dt1 As New DataTable
                Dim objDBCom1 As New MySQLDBComponent.MySQLDBComponent(POSWeb.POSWeb_SQLConn)
                If objDBCom1.ExecuteSQL(dt1, sql) Then
                    If dt1.Rows(0).Item(0) > 0 Then
                        rtnBoolean = True
                    Else
                        rtnBoolean = False
                    End If
                Else
                    rtnBoolean = False
                End If
                objDBCom1.Dispose()
                dt1.Dispose()
            End If



            If Not rtnBoolean Then
                If isPopUp Then
                    Page1.Response.Redirect(ConfigurationManager.AppSettings("PMSWeb_WebSiteURL") & "LoginPopUp.aspx?expired=true&page=" & Page1.Request.Url.ToString & "&NoAccess=1")
                Else
                    Page1.Response.Redirect(ConfigurationManager.AppSettings("PMSWeb_WebSiteURL") & "Login.aspx?expired=true&page=" & Page1.Request.Url.ToString & "&NoAccess=1")
                End If
            End If

            ' rtnBoolean

        End If

        Return rtnBoolean


    End Function


    Public Shared Function checkSessionNew(ByRef Page1 As Page, Optional ByVal isPopUp As Boolean = False) As Boolean
        ' Public Function checkSession(ByVal RadAjax1 As RadAjaxManager, ByVal Page1 As Page, ByVal uSession As Integer, ByVal Mode As String, ByVal taxtBox1 As String)
        Dim rtnBoolean As Boolean = False
        If Page1.Session("PMSWeb_user") = "0" Then
            If isPopUp Then
                Page1.Response.Redirect(ConfigurationManager.AppSettings("PMSWeb_WebSiteURL") & "LoginPopUp.aspx?expired=true&page=" & Page1.Request.Url.ToString & "")
            Else
                Page1.Response.Redirect(ConfigurationManager.AppSettings("PMSWeb_WebSiteURL") & "Login.aspx?expired=true&page=" & Page1.Request.Url.ToString & "")
            End If
            rtnBoolean = False
        Else
            Page1.Session.Timeout = 60
            rtnBoolean = True
        End If

        Return rtnBoolean

    End Function



    Public Shared Function checkSession(ByRef Page1 As Page, ByVal uSession As Integer, ByVal uType As String, ByVal PageType As String, ByVal moduleS As String) As Boolean
        ' Public Function checkSession(ByVal RadAjax1 As RadAjaxManager, ByVal Page1 As Page, ByVal uSession As Integer, ByVal Mode As String, ByVal taxtBox1 As String)
        Dim rtnBoolean As Boolean = False
        If uSession = 0 Then
            Select Case PageType
                Case "master"
                    Page1.Response.Redirect(ConfigurationManager.AppSettings("PMSWeb_WebSiteURL") & "Login.aspx?expired=true&page=" & Page1.Request.Url.ToString & "&moduleS=" & moduleS & "")
                    rtnBoolean = False
                    Exit Function
                Case "internal"
                    Page1.Response.Redirect(ConfigurationManager.AppSettings("PMSWeb_WebSiteURL") & "Login.aspx?expired=true&page=" & Page1.Request.Url.ToString & "&moduleS=" & moduleS & "")
                    rtnBoolean = False
                    Exit Function
                Case "external"
                    Page1.Response.Redirect(ConfigurationManager.AppSettings("PMSWeb_WebSiteURL") & "Login.aspx?expired=true&page=" & Page1.Request.Url.ToString & "&moduleS=" & moduleS & "")
                    rtnBoolean = False
                    Exit Function
            End Select
            rtnBoolean = False
        Else
            Page1.Session.Timeout = 60
            rtnBoolean = True
        End If

        Return rtnBoolean

    End Function

    Public Shared Function Signature_Create(ByVal Key As String) As String
        Dim objSHA1 As New SHA1CryptoServiceProvider()

        objSHA1.ComputeHash(System.Text.Encoding.UTF8.GetBytes(Key.ToCharArray))

        Dim buffer() As Byte = objSHA1.Hash
        Dim HashValue As String = System.Convert.ToBase64String(buffer)

        Return HashValue
    End Function

    Public Shared Function MD5Encrypt(ByVal s As String) As String

        'The array of bytes that will contain the encrypted value of strPlainText
        Dim hashedDataBytes As Byte()

        'The encoder class used to convert strPlainText to an array of bytes
        Dim encoder As New UTF8Encoding()

        'Create an instance of the MD5CryptoServiceProvider class
        Dim md5Hasher As New MD5CryptoServiceProvider()

        'Call ComputeHash, passing in the plain-text string as an array of bytes
        'The return value is the encrypted value, as an array of bytes
        hashedDataBytes = md5Hasher.ComputeHash(encoder.GetBytes(s))

        Return Convert.ToBase64String(hashedDataBytes)
    End Function

    Public Shared Function SessionString_Get() As String

        Dim rString As String = Now.Year.ToString & _
               IIf(CType(Now.Month, Integer) < 10, "0" & Now.Month.ToString, Now.Month.ToString) & _
               IIf(CType(Now.Day, Integer) < 10, "0" & Now.Day.ToString, Now.Day.ToString) & _
               Now.Hour.ToString & _
               Now.Minute.ToString & _
               Now.Millisecond.ToString

        Return rString
    End Function

    Public Shared Sub IP_ADDRESS_SAVE(ByVal ip As String, ByVal pageS As String)

        Dim sqlS As String = ""
        sqlS &= "INSERT INTO TBL_PMS_PROG_VIS   ([IPAdd] ,[VisitDateTime],[Page]) values ('" & ip & "', getDate(),'" & pageS & "')"
        Dim objDBCom As New MySQLDBComponent.MySQLDBComponent(POSWeb.POSWeb_SQLConn)

        If objDBCom.ExecuteSQL(sqlS) Then

        Else

        End If
        objDBCom.Dispose()
    End Sub

    Public Shared Function exitsProposalNo(ByVal agentCode As String, ByVal proposalNo As String) As Boolean
        Dim result As Boolean = False
        Dim script As String = ""
        Try
            script = "select * from Submission_Trans where AgentCode='" + agentCode + "' and ProposalNo='" + proposalNo + "'"
            conn.Open()
            comm = New SqlCommand(script, conn)
            dr = comm.ExecuteReader
            If dr.HasRows Then
                result = True
            Else
                result = False
            End If
            conn.Close()
        Catch ex As Exception
            Throw ex
        End Try
        Return result
    End Function

    Public Shared Function getValProposalNo(ByVal agentCode As String, ByVal proposalNo As String) As String
        Dim result As String = ""
        Dim script As String = ""
        Try
            script = "select PolicyNo from Submission_Trans where AgentCode='" + agentCode + "' and ProposalNo='" + proposalNo + "' and PolicyNo is not null"
            conn.Open()
            comm = New SqlCommand(script, conn)
            dr = comm.ExecuteReader
            If dr.HasRows Then
                dr.Read()
                If dr.Item(0).ToString().Trim() <> "" Then
                    result = dr.Item(0).ToString().Trim()
                End If
            End If
            conn.Close()
        Catch ex As Exception
            Throw ex
        End Try
        Return result
    End Function

    Public Shared Function policyNumberVal() As String
        Dim result As String = ""
        Dim script As String = ""
        Dim count As Integer = 0
        Try
            script = "select top 1 right(PolicyNo,10) from Submission_Trans order by PolicyNo desc"
            conn.Open()
            comm = New SqlCommand(script, conn)
            dr = comm.ExecuteReader
            If dr.HasRows Then
                dr.Read()
                If dr.Item(0).ToString().Trim() <> "" Then
                    result = (Convert.ToInt32(dr.Item(0).ToString().Trim()) + 1).ToString("D10")
                Else
                    result = "0000000001"
                End If
                dr.Close()
            Else
                result = "0000000001"
            End If
            conn.Close()
        Catch ex As Exception
            Throw ex
        End Try
        Return result
    End Function

    Public Shared Sub dataRun(ByVal script As String)
        Try
            conn.Open()
            comm = New SqlCommand(script, conn)
            comm.ExecuteNonQuery()
            conn.Close()
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

End Class
