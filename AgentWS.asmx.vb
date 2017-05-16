Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports System.IO
Imports System.Security
Imports System.Security.Cryptography
Imports System.Xml
Imports System.Globalization
Imports System.Web
Imports System.Configuration
Imports System.Net
Imports System.Net.Mail
Imports System.Data.SqlClient
Imports System.Text
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class AgentWS
    Inherits System.Web.Services.WebService

    <WebMethod()>
    Public Function GetSPAJNumber() As String
        Return SPAJNumberGenerator.GetSPAJNumber
    End Function

    '<WebMethod()>
    'Public Function GetAllHTMLFiles() As List(Of String)
    '    Dim folderPath As String = "C:\inetpub\wwwroot\HTMLFiles"
    '    Dim returnVal As New List(Of String)
    '    Dim fileList As String() = Directory.GetFiles(folderPath)

    '    For i As Integer = 0 To fileList.Length - 1
    '        returnVal.Add(Path.GetFileName(fileList(i)))
    '    Next

    '    Return returnVal
    'End Function

    <WebMethod()>
    Public Function GetAgentPassword(strAgentId As String, key As String)
        If key = "TMC0nn3ct" Then
            Dim isSuccess As Boolean = False
            Dim objDBcom = New MySQLDBComponent.MySQLDBComponent(POSWeb.POSWeb_SQLConn)
            Dim dt As New DataTable
            Dim password As String = String.Empty
            Dim str As String = String.Empty

            objDBcom.AddParameter("@AgentCode", SqlDbType.NVarChar, strAgentId)
            objDBcom.ExecuteProcedure(dt, "TMLI_WS_Agent_Profile_ForgotPassword")
            objDBcom.Dispose()

            If dt.Rows.Count > 0 Then
                password = dt.Rows(0)("AgentPassword").ToString()

                str = DecryptString(password, "1234567891123456")
                Return str
            Else
                Return "Invalid Agent Code"
            End If

        End If
    End Function

    <WebMethod()>
    Public Function GetAllHTMLFiles() As DataTable
        Dim objDBCom As New MySQLDBComponent.MySQLDBComponent(POSWeb.POSWeb_SQLConn)
        Dim dTable As New DataTable
        Dim selectStat As String = "SELECT [Id]
                                  ,[SPAJId]
                                  ,[FolderName]
                                  ,[FileName]
                                  ,[Status]
                                  ,[SPAJSection]
                                  ,[ProductCode]
                                  ,[ValidDate]
                                  ,[DateCreated]
                                  ,[DateModified]
                                  FROM [TMLI_SPAJHTML_Form]"
        objDBCom.ExecuteSQL(dTable, selectStat)
        dTable.TableName = "HTMLFiles"
        Return dTable
    End Function

    <WebMethod()>
    Public Function CreateBackupFolder(agentID As String) As String
        Dim folderName As String = agentID & Now.ToString("ddMMyyyy_hhmmss")
        Try
            Directory.CreateDirectory("C:/inetpub/wwwroot/files/" & folderName)
            If Directory.Exists("C:/inetpub/wwwroot/files/" & folderName) Then
                Return folderName
            End If
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    <WebMethod()>
    Public Function UpdateOnPostUploadBackupFile(agentCode As String, agentName As String, UDID As String, AppVersion As String, backupFolder As String) As String
        Dim objDBCom As New MySQLDBComponent.MySQLDBComponent(POSWeb.POSWeb_SQLConn)
        Dim selectCountStatement As String = "SELECT COUNT(Agent_ID) FROM TMLI_Device_Management WHERE Agent_ID =" & agentCode
        Dim dTable As New DataTable
        objDBCom.ExecuteSQL(dTable, selectCountStatement)
        Try
            If dTable.Rows(0)(0) > 0 Then
                Dim comm As New SqlCommand("UPDATE TMLI_Device_Management SET Agent_Name=@name, [TMLI_version]=@version, [Last_UploadDate]=@uploadDate, [UDID]=@udid, [Backup_URL]=@backupURL, [Status]=Uploaded WHERE Agent_ID=@agentCode")
                comm.Parameters.AddWithValue("@name", agentName)
                comm.Parameters.AddWithValue("@version", AppVersion)
                comm.Parameters.AddWithValue("@uploadDate", Now)
                comm.Parameters.AddWithValue("@udid", UDID)
                comm.Parameters.AddWithValue("@backupURL", "http://tmcuat.tokiomarine-life.co.id/files/" & backupFolder & "/" & backupFolder & ".zip")
                comm.Parameters.AddWithValue("@agentCode", agentCode)
                objDBCom.ExecuteSqlCommand(comm)
                Return "Saved"
            Else
                Dim comm As New SqlCommand("INSERT INTO TMLI_Device_Management ([Agent_ID],[UDID],[Agent_Name],[TMLI_version],[Last_UploadDate],[Status],[Backup_URL]) VALUES (@agentCode,@udid,@agentName,@version,@uploadDate,@status,@backupURL)")
                comm.Parameters.AddWithValue("@agentName", agentName)
                comm.Parameters.AddWithValue("@version", AppVersion)
                comm.Parameters.AddWithValue("@uploadDate", Now)
                comm.Parameters.AddWithValue("@udid", UDID)
                comm.Parameters.AddWithValue("@status", "Uploaded")
                comm.Parameters.AddWithValue("@backupURL", "http://tmcuat.tokiomarine-life.co.id/files/" & backupFolder & "/" & backupFolder & ".zip")
                comm.Parameters.AddWithValue("@agentCode", agentCode)
                objDBCom.ExecuteSqlCommand(comm)
                Return "Saved"
            End If
        Catch ex As Exception
            Return ex.Message
        End Try

    End Function

    <WebMethod()>
    Public Function GetBackupFileLink(agentCode As String) As String
        Dim objDBCom As New MySQLDBComponent.MySQLDBComponent(POSWeb.POSWeb_SQLConn)
        Dim selectDownloadLink = "SELECT Backup_URL FROM TMLI_Device_Management WHERE Agent_ID =" & agentCode
        Dim dTable As New DataTable
        objDBCom.ExecuteSQL(dTable, selectDownloadLink)

        If dTable.Rows.Count > 0 Then
            Return dTable.Rows(0)("Backup_URL")
        Else
            Return "No Data Found For This Device"
        End If
    End Function

    <WebMethod()>
    Public Function UpdateOnPostDownloadBackupFile(agentCode As String, UDID As String) As Boolean
        Dim objDBCom As New MySQLDBComponent.MySQLDBComponent(POSWeb.POSWeb_SQLConn)
        Dim updateString = "UPDATE TMLI_Device_Management SET [Last_DownloadDate]=@downloadDate, [UDID]=@udid, [Status] = 'Downloaded' WHERE [Agent_ID]=@agentCode"
        Dim comm As New SqlCommand(updateString)
        comm.Parameters.AddWithValue("@downloadDate", Now)
        comm.Parameters.AddWithValue("@udid", UDID)
        comm.Parameters.AddWithValue("@agentCode", agentCode)
        objDBCom.ExecuteSqlCommand(comm)
    End Function

    <WebMethod()>
    Public Function SaveSAMMasterData(SAM_ID As String, SAM_Number As String, SAM_CustomerID As String, SAM_Type As String, SAM_ID_CFF As String, SAM_ID_ProductRecommendation As String,
                                      SAM_ID_Video As String, SAM_ID_Illustration As String, SAM_ID_Application As String, SAM_DateCreated As DateTime, SAM_DateModified As DateTime, SAM_Comments As String, SAM_Status As String, SAM_NextMeeting As DateTime)

        Dim objDBCom As New MySQLDBComponent.MySQLDBComponent(POSWeb.POSWeb_SQLConn)
        Try
            Dim insertStatement As String = "INSERT INTO SAM_Master VALUES
           (@SAM_ID,
            @SAM_Number,
            @SAM_CustomerID,
            @SAM_Type,
            @SAM_ID_CFF,
            @SAM_ID_ProductRecommendation,
            @SAM_ID_Video,
            @SAM_ID_Illustration,
            @SAM_ID_Application,
            @SAM_DateCreated,
            @SAM_DateModified,
            @SAM_Comments,
            @SAM_Status,
            @SAM_NextMeeting)"

            Dim comm As New SqlCommand(insertStatement)
            comm.Parameters.AddWithValue("@SAM_ID", SAM_ID)
            comm.Parameters.AddWithValue("@SAM_Number", SAM_Number)
            comm.Parameters.AddWithValue("@SAM_CustomerID", SAM_CustomerID)
            comm.Parameters.AddWithValue("@SAM_Type", SAM_Type)
            comm.Parameters.AddWithValue("@SAM_ID_CFF", SAM_ID_CFF)
            comm.Parameters.AddWithValue("@SAM_ID_ProductRecommendation", SAM_ID_ProductRecommendation)
            comm.Parameters.AddWithValue("@SAM_ID_Video", SAM_ID_Video)
            comm.Parameters.AddWithValue("@SAM_ID_Illustration", SAM_ID_Illustration)
            comm.Parameters.AddWithValue("@SAM_ID_Application", SAM_ID_Application)
            comm.Parameters.AddWithValue("@SAM_DateCreated", SAM_DateCreated)
            comm.Parameters.AddWithValue("@SAM_DateModified", SAM_DateModified)
            comm.Parameters.AddWithValue("@SAM_Comments", SAM_Comments)
            comm.Parameters.AddWithValue("@SAM_Status", SAM_Status)
            comm.Parameters.AddWithValue("@SAM_NextMeeting", SAM_NextMeeting)

            Return objDBCom.ExecuteSqlCommand(comm)

        Catch ex As Exception
            Return False
        End Try
        Return ""
    End Function

    <WebMethod()>
    Public Function SaveSAMDetailData(SAMDetail_ID As String, SAM_ID As String, SAM_Number As String, SAMDetail_MeetingDate As Date, SAMDetail_MeetingTime As DateTime, SAMDetail_MeetingNotes As String,
                                      SAMDetail_MeetingLocation As String, SAMDetail_MeetingDuration As Integer, SAMDetail_MeetingStatus As String, SAMDetail_MeetingComments As String) As Boolean

        Dim objDBCom As New MySQLDBComponent.MySQLDBComponent(POSWeb.POSWeb_SQLConn)
        Dim insertStatement As String = "INSERT INTO SAM_Details VALUES
            (
                @SAMDetail_ID,
                @SAM_ID,
                @SAM_Number,
                @SAMDetail_MeetingDate,
                @SAMDetail_MeetingTime,
                @SAMDetail_MeetingNotes,
                @SAMDetail_MeetingLocation,
                @SAMDetail_MeetingDuration,
                @SAMDetail_MeetingStatus,
                @SAMDetail_MeetingComments
            ) "

        Try
            Dim comm As New SqlCommand(insertStatement)
            comm.Parameters.AddWithValue("@SAMDetail_ID", SAMDetail_ID)
            comm.Parameters.AddWithValue("@SAM_ID", SAM_ID)
            comm.Parameters.AddWithValue("@SAM_Number", SAM_Number)
            comm.Parameters.AddWithValue("@SAMDetail_MeetingDate", SAMDetail_MeetingDate)
            comm.Parameters.AddWithValue("@SAMDetail_MeetingTime", SAMDetail_MeetingTime)
            comm.Parameters.AddWithValue("@SAMDetail_MeetingNotes", SAMDetail_MeetingNotes)
            comm.Parameters.AddWithValue("@SAMDetail_MeetingLocation", SAMDetail_MeetingLocation)
            comm.Parameters.AddWithValue("@SAMDetail_MeetingDuration", SAMDetail_MeetingDuration)
            comm.Parameters.AddWithValue("@SAMDetail_MeetingStatus", SAMDetail_MeetingStatus)
            comm.Parameters.AddWithValue("@SAMDetail_MeetingComments", SAMDetail_MeetingComments)
            objDBCom.ExecuteSqlCommand(comm)

            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    <WebMethod()>
    Public Function GetSAMMasterData() As DataTable
        Dim selectStatement As String = "SELECT * FROM SAM_Master"
        Dim dTable As New DataTable
        Dim objDBCom As New MySQLDBComponent.MySQLDBComponent(POSWeb.POSWeb_SQLConn)
        objDBCom.ExecuteSQL(dTable, selectStatement)
        Return dTable
    End Function

    <WebMethod()>
    Public Function GetSAMDetailData() As DataTable
        Dim selectStatement As String = "SELECT * FROM SAM_Details"
        Dim dTable As New DataTable
        Dim objDBCom As New MySQLDBComponent.MySQLDBComponent(POSWeb.POSWeb_SQLConn)
        objDBCom.ExecuteSQL(dTable, selectStatement)
        Return dTable
    End Function

    <WebMethod()>
    Public Function GetFileLink(spajNumber As String) As String
        Dim page As New Page
        Try
            Dim objDBCom As New MySQLDBComponent.MySQLDBComponent(POSWeb.POSWeb_SQLConn)
            Dim selectData As String = "SELECT FileLocation FROM TMLI_TBT_SPAJ_ESUBMISSION WHERE SPAJCode =" & spajNumber
            Dim dTable As New DataTable
            objDBCom.ExecuteSQL(dTable, selectData)

            If dTable.Rows.Count > 0 Then
                Dim relativePath = "~/SPAJFiles" & "/" & dTable.Rows(0)("FileLocation").ToString
                Return page.ResolveClientUrl(relativePath)
            Else
                Return "No file for this spaj number"
            End If

        Catch ex As Exception
            Return ex.Message
        End Try
        Return ""
    End Function

    <WebMethod()>
    Public Function OnPostUploadSPAJFiles(spajCode As String, productName As String, polisOwner As String, isResubmit As String, agentCode As String) As String
        Dim objDBCom As New MySQLDBComponent.MySQLDBComponent(POSWeb.POSWeb_SQLConn)
        Dim emailAddress As String

        Try
            Dim updateStatement As String = ""
            Dim selectCountStatement As String = "SELECT SPAJCode FROM TMLI_TBT_SPAJ_ESUBMISSION WHERE SPAJCode =" & spajCode
            Dim dTable As New DataTable
            objDBCom.ExecuteSQL(dTable, selectCountStatement)
            Dim serverURL = ConfigurationManager.AppSettings.Get("ServerURL")
            If dTable.Rows.Count > 0 Then

                If dTable.Rows(0)(0) > 0 Then
                    'meaning record already exist
                    updateStatement = "UPDATE TMLI_TBT_SPAJ_ESUBMISSION SET FileLocation=@fileLocation, SubmittedDate=@submitDate, ProductName=@productName, PolisOwner=@polisOwner WHERE SPAJCode=@spajCode"
                    Dim command As New SqlCommand(updateStatement)
                    command.Parameters.AddWithValue("@fileLocation", serverURL & "/SPAJFiles/" & spajCode)
                    command.Parameters.AddWithValue("@submitDate", Now)
                    command.Parameters.AddWithValue("@productName", productName)
                    command.Parameters.AddWithValue("@polisOwner", polisOwner)
                    command.Parameters.AddWithValue("@spajCode", spajCode)
                    objDBCom.ExecuteSqlCommand(command)
                Else
                    updateStatement = "INSERT INTO TMLI_TBT_SPAJ_ESUBMISSION (SPAJCode, FileLocation, SubmittedDate, ProductName, PolisOwner, AgentCode) VALUES (@spajCode,@fileLocation,@submitDate,@productName,@polisOwner,@agentCode)"
                    Dim command As New SqlCommand(updateStatement)
                    command.Parameters.AddWithValue("@fileLocation", serverURL & "/SPAJFiles/" & spajCode)
                    command.Parameters.AddWithValue("@submitDate", Now)
                    command.Parameters.AddWithValue("@productName", productName)
                    command.Parameters.AddWithValue("@polisOwner", polisOwner)
                    command.Parameters.AddWithValue("@spajCode", spajCode)
                    command.Parameters.AddWithValue("@agentCode", agentCode)
                    objDBCom.ExecuteSqlCommand(command)
                End If

                'Logic to read the JSON and pass it to DTR
                Dim jsonFilePath As String = "C:/inetpub/wwwroot/SPAJFiles/" & spajCode & "/" & spajCode & ".json"
                Dim spajFilesPath As String = "C:/inetpub/wwwroot/SPAJFiles/" & spajCode
                Dim jsonObject As JObject = JObject.Parse(File.ReadAllText(jsonFilePath))
                Dim channel As JObject = jsonObject("")
                Dim fileList As String() = Directory.GetFiles(spajFilesPath)

                For Each fileName As String In fileList
                    'A Section
                    Dim newFileName As String = fileName.Replace("C:/inetpub/wwwroot", serverURL)
                    If fileName.Contains("_a1") Then
                        channel.Property("spaj_a1").Value = newFileName
                    End If
                    If fileName.Contains("_a2") Then
                        channel.Property("spaj_a2").Value = newFileName
                    End If
                    If fileName.Contains("_a3") Then
                        channel.Property("spaj_a3").Value = newFileName
                    End If
                    If fileName.Contains("_a4") Then
                        channel.Property("spaj_a4").Value = newFileName
                    End If
                    If fileName.Contains("_a5") Then
                        channel.Property("spaj_a5").Value = newFileName
                    End If
                    'B Section
                    If fileName.Contains("_b1") Then
                        channel.Property("spaj_b1").Value = newFileName
                    End If
                    If fileName.Contains("_b2") Then
                        channel.Property("spaj_b2").Value = newFileName
                    End If
                    If fileName.Contains("_b3") Then
                        channel.Property("spaj_b3").Value = newFileName
                    End If
                    If fileName.Contains("_b4") Then
                        channel.Property("spaj_b4").Value = newFileName
                    End If
                    If fileName.Contains("_b5") Then
                        channel.Property("spaj_b5").Value = newFileName
                    End If
                Next
                File.WriteAllText(jsonFilePath, channel.ToString)
                Dim jsonString As String = File.ReadAllText(jsonFilePath)

                'Send the JSON to DTR
                Dim request As WebRequest = WebRequest.Create("http://192.168.1.119:9090/TMConnectWS/webapi/submission/create")
                ' Set the Method property of the request to POST.  
                request.Method = "POST"
                Dim byteArray As Byte() = Encoding.UTF8.GetBytes(jsonString)
                ' Set the ContentType property of the WebRequest.  
                request.ContentType = "application/json"
                ' Set the ContentLength property of the WebRequest.  
                request.ContentLength = byteArray.Length
                ' Get the request stream.  
                Dim dataStream As Stream = request.GetRequestStream()
                ' Write the data to the request stream.  
                dataStream.Write(byteArray, 0, byteArray.Length)
                ' Close the Stream object.  
                dataStream.Close()
                ' Get the response.  
                Dim response As WebResponse = request.GetResponse()
                ' Display the status.  
                Console.WriteLine(CType(response, HttpWebResponse).StatusDescription)
                ' Get the stream containing content returned by the server.  
                dataStream = response.GetResponseStream()
                ' Open the stream using a StreamReader for easy access.  
                Dim reader As New StreamReader(dataStream)
                ' Read the content.  
                Dim responseFromServer As String = reader.ReadToEnd()
                ' Display the content.  
                Console.WriteLine(responseFromServer)
                ' Clean up the streams.  
                reader.Close()
                dataStream.Close()
                response.Close()
                Return "Success"
            Else
                updateStatement = "INSERT INTO TMLI_TBT_SPAJ_ESUBMISSION (SPAJCode, FileLocation, SubmittedDate, ProductName, PolisOwner) VALUES (@spajCode,@fileLocation,@submitDate,@productName,@polisOwner)"
                Dim command As New SqlCommand(updateStatement)
                command.Parameters.AddWithValue("@fileLocation", "https://tmcuat.tokiomarine-life.co.id/SPAJFiles/" & spajCode)
                command.Parameters.AddWithValue("@submitDate", Now)
                command.Parameters.AddWithValue("@productName", productName)
                command.Parameters.AddWithValue("@polisOwner", polisOwner)
                command.Parameters.AddWithValue("@spajCode", spajCode)
                objDBCom.ExecuteSqlCommand(command)

                Dim jsonFilePath As String = "C:/inetpub/wwwroot/SPAJFiles/" & spajCode & "/" & spajCode & ".json"
                Dim spajFilesPath As String = "C:/inetpub/wwwroot/SPAJFiles/" & spajCode
                Dim jsonObject As JObject = JObject.Parse(File.ReadAllText(jsonFilePath))
                Dim channel As JObject = jsonObject("")
                Dim fileList As String() = Directory.GetFiles(spajFilesPath)

                For Each fileName As String In fileList
                    'A Section
                    Dim newFileName As String = fileName.Replace("C:/inetpub/wwwroot", "https://tmcuat.tokiomarine-life.co.id")
                    If fileName.Contains("_a1") Then
                        channel.Property("spaj_a1").Value = newFileName
                    End If
                    If fileName.Contains("_a2") Then
                        channel.Property("spaj_a2").Value = newFileName
                    End If
                    If fileName.Contains("_a3") Then
                        channel.Property("spaj_a3").Value = newFileName
                    End If
                    If fileName.Contains("_a4") Then
                        channel.Property("spaj_a4").Value = newFileName
                    End If
                    If fileName.Contains("_a5") Then
                        channel.Property("spaj_a5").Value = newFileName
                    End If
                    'B Section
                    If fileName.Contains("_b1") Then
                        channel.Property("spaj_b1").Value = newFileName
                    End If
                    If fileName.Contains("_b2") Then
                        channel.Property("spaj_b2").Value = newFileName
                    End If
                    If fileName.Contains("_b3") Then
                        channel.Property("spaj_b3").Value = newFileName
                    End If
                    If fileName.Contains("_b4") Then
                        channel.Property("spaj_b4").Value = newFileName
                    End If
                    If fileName.Contains("_b5") Then
                        channel.Property("spaj_b5").Value = newFileName
                    End If
                Next
                File.WriteAllText(jsonFilePath, channel.ToString)
                Dim jsonString As String = File.ReadAllText(jsonFilePath)

                'Send the JSON to DTR
                Dim request As WebRequest = WebRequest.Create("http://192.168.1.119:9090/TMConnectWS/webapi/submission/create")
                ' Set the Method property of the request to POST.  
                request.Method = "POST"
                Dim byteArray As Byte() = Encoding.UTF8.GetBytes(jsonString)
                ' Set the ContentType property of the WebRequest.  
                request.ContentType = "application/json"
                ' Set the ContentLength property of the WebRequest.  
                request.ContentLength = byteArray.Length
                ' Get the request stream.  
                Dim dataStream As Stream = request.GetRequestStream()
                ' Write the data to the request stream.  
                dataStream.Write(byteArray, 0, byteArray.Length)
                ' Close the Stream object.  
                dataStream.Close()
                ' Get the response.  
                Dim response As WebResponse = request.GetResponse()
                ' Display the status.  
                Console.WriteLine(CType(response, HttpWebResponse).StatusDescription)
                ' Get the stream containing content returned by the server.  
                dataStream = response.GetResponseStream()
                ' Open the stream using a StreamReader for easy access.  
                Dim reader As New StreamReader(dataStream)
                ' Read the content.  
                Dim responseFromServer As String = reader.ReadToEnd()
                ' Display the content.  
                Console.WriteLine(responseFromServer)
                ' Clean up the streams.  
                reader.Close()
                dataStream.Close()
                response.Close()
                Return "Success"
            End If
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    <WebMethod()>
    Public Function CreateSPAJFolder(spajNumber As String) As String
        Try
            Dim folderName As String = Path.Combine(Server.MapPath("SPAJFiles"), spajNumber)

            If Directory.Exists(folderName) Then
                Return "~/SPAJFiles/" & spajNumber
            Else
                Directory.CreateDirectory(folderName)
                Return "~/SPAJFiles/" & spajNumber
            End If
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    <WebMethod()>
    Public Function UploadFile(fileByte As Byte(), fileName As String, usedAs As String) As Boolean
        Try
            Dim temporaryFileName = Path.Combine(Server.MapPath("TemporaryFileUpload"), fileName)
            Dim memStream As New MemoryStream(fileByte)
            Dim fileStream As New FileStream(temporaryFileName, FileMode.Create)
            memStream.WriteTo(fileStream)

            memStream.Close()
            fileStream.Close()
            fileStream.Dispose()

            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    <WebMethod()>
    Public Function SendErrorLogMessage(strParam As String) As String
        Dim objDBCom As New MySQLDBComponent.MySQLDBComponent(POSWeb.POSWeb_SQLConn)
        Dim selectQuery As String = "SELECT [String_Data], [Error_msg], [Exec_Group] FROM [TMLI_AgentLog] WHERE [Exec_ID] = '" & strParam & "' AND Error_msg <> 'Success'"
        Dim dtResult As New DataTable
        Try
            objDBCom.ExecuteSQL(dtResult, selectQuery)

            Dim stringData As String = dtResult.Rows(0)("String_Data").ToString
            Dim errorMsg As String = dtResult.Rows(0)("Error_msg").ToString
            Dim execGroup As String = dtResult.Rows(0)("Exec_Group").ToString

            Dim message As String = "<HTML><BODY>
                                        <H2>Error On Receiving Following Data</H2>
                                        </ br>
                                        Error Data        :" & stringData & "
                                        </ br>
                                        Exception Message :" & errorMsg & "
                                        </ br>
                                        Exception Date    :" & execGroup & "
                                        </ br>
                                        Exception ID      :" & strParam & "
                                    </BODY></HTML>"

            objDBCom = New MySQLDBComponent.MySQLDBComponent(CStr(POSWeb.POSWeb_SQLConn))
            Dim sqlInsert As String = "INSERT INTO TMLI_SendEmail VALUES (@id,@emailTo,@content,@status)"
            Dim comm As New SqlCommand(sqlInsert)
            comm.Parameters.AddWithValue("@id", strParam)
            comm.Parameters.AddWithValue("@emailTo", "muh.rais@tokiomarine-life.co.id") 'muh.rais@tokiomarine-life.co.id
            comm.Parameters.AddWithValue("@content", message)
            comm.Parameters.AddWithValue("@status", "Sending")
            Dim result As Boolean = objDBCom.ExecuteSqlCommand(comm)

            If result Then
                Dim processinfo As New ProcessStartInfo()
                processinfo.WorkingDirectory = "C:\MailSender"
                processinfo.FileName = "java.exe"
                processinfo.Arguments = "-jar TestJar.jar " & strParam

                Dim myProcess As New Process()
                processinfo.UseShellExecute = False
                processinfo.RedirectStandardOutput = True
                myProcess.StartInfo = processinfo
                myProcess.Start()
                Dim myStreamReader As StreamReader = myProcess.StandardOutput
                ' Read the standard output of the spawned process. 
                Dim finalString As String
                Dim myString As String
                Do
                    myString = myStreamReader.ReadLine()
                    finalString += myString
                Loop Until (myStreamReader.EndOfStream)
                myProcess.WaitForExit()
                myProcess.Close()

                If finalString.Contains("Exception") Then
                    Return finalString
                Else
                    Return "Success"
                End If
            Else
                Return "Error saving record to Database"
            End If
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    <WebMethod()>
    Public Function HelloWorld() As String
        Return "Hello World"
    End Function

    ''' <summary>
    ''' Function to return image as base64String
    ''' </summary>
    ''' <returns></returns>
    <WebMethod()>
    Public Function GetAllProductInfo() As DataTable
        'Dim webRootPath As String = Server.MapPath("~")
        Dim productRootFolderName = "ProductRoot"
        Dim productRootLocation As String = ConfigurationManager.AppSettings("ProductRoot").ToString()
        Dim folderName As String = productRootLocation

        Dim dTable As New DataTable
        dTable.TableName = "ProductInformation"
        dTable.Columns.Add("FilePath")
        dTable.Columns.Add("FileSize")

        Try
            Dim DirList As New ArrayList
            Dim Dirs() As String = Directory.GetDirectories(folderName)
            DirList.AddRange(Dirs)
            For Each Dir As String In Dirs
                GetDirectories(Dir, DirList)
            Next

            Dim dr As DataRow = dTable.NewRow

            dr("FilePath") = ""
            dr("FileSize") = 0

            dTable.Rows.Add(dr)

            For Each item In DirList


                Dim splitFolder As String() = item.Split("\")
                Dim itemindexFolder As Int32 = Array.IndexOf(splitFolder, productRootFolderName)
                Dim itemAllFolder As Int32 = splitFolder.Length

                Dim files As String() = Directory.GetFiles(item)

                For Each str As String In files
                    Dim fullPath As String = ""
                    Dim fileName As String = IO.Path.GetFileName(str)
                    Dim fi As New IO.FileInfo(str)

                    Dim split As String() = str.Split("\")
                    Dim itemindex As Int32 = Array.IndexOf(split, productRootFolderName)
                    Dim itemAll As Int32 = split.Length

                    For index As Integer = itemindex To itemAll
                        If itemindex = itemAll Then
                            Exit For
                        End If
                        fullPath += split(itemindex).ToString() + "\"
                        itemindex = itemindex + 1
                    Next

                    dTable.Rows.Add(fullPath.TrimEnd("\"), fi.Length)
                Next

                If files.Count = 0 Then
                    Dim fullPath As String = ""
                    For index As Integer = itemindexFolder To itemAllFolder
                        If itemindexFolder = itemAllFolder Then
                            Exit For
                        End If
                        fullPath += splitFolder(itemindexFolder).ToString() + "\"
                        itemindexFolder = itemindexFolder + 1
                    Next
                    dTable.Rows.Add(fullPath, 0)
                End If
            Next
        Catch ex As Exception
            Dim a As String = ex.ToString
        End Try
        Return dTable
    End Function

    Sub GetDirectories(ByVal StartPath As String, ByRef DirectoryList As ArrayList)
        Dim Dirs() As String = Directory.GetDirectories(StartPath)
        DirectoryList.AddRange(Dirs)

        For Each Dir As String In Dirs
            GetDirectories(Dir, DirectoryList)
        Next
    End Sub

    <WebMethod()>
    Public Function SendEmailInvitation(ByVal strusername As String) As String
        Dim objDBCom As New MySQLDBComponent.MySQLDBComponent(POSWeb.POSWeb_SQLConn)
        Dim dt As New DataTable
        Dim dt2 As New DataTable
        Dim strpassword As String = "select AgentPassword from TMLI_Agent_Profile where AgentCode = '" + strusername + "'"
        Dim stremail As String = "select AgentEmail from TMLI_Agent_Profile where AgentCode = '" + strusername + "'"
        objDBCom.ExecuteSQL(dt, strpassword)
        objDBCom.ExecuteSQL(dt2, stremail)
        objDBCom.Dispose()
        Try
            Dim useremail As String = "tmconnect.no-reply@tokiomarine-life.co.id"
            Dim newUserName As String = "tmli\tmconnect.no-reply"
            Dim userpass As String = "tmc0nn3ct!"
            Dim body As String = ""
            Using sr = New StreamReader(Server.MapPath("~\EmailTemplate.html"))
                body = sr.ReadToEnd()
            End Using

            Dim ipadimage As String = "http://simpleicon.com/wp-content/uploads/ipad-portrait.png"
            Dim desktopimage As String = "https://image.freepik.com/free-icon/desktop-button_318-25227.jpg"
            Dim messageBody As String = String.Format(body, "https://tmconnect.tokiomarine-life.co.id/anvoaiwenvwae0v-wevjhweivnawuen12j3n12m%20asjkdfna%20sdjf%20123.html", ipadimage, "http://www.google.com",
                                                      desktopimage, strusername, DecryptString(dt.Rows(0)(0).ToString(), "1234567891123456"))
            'Dim smtp As New SmtpClient()
            'Dim message As New MailMessage(useremail, dt2.Rows(0)(0).ToString())

            'message.Subject = "Aktivasi Aplikasi TM Connect"
            'message.Body = String.Format(body, "https://tmconnect.tokiomarine-life.co.id/anvoaiwenvwae0v-wevjhweivnawuen12j3n12m%20asjkdfna%20sdjf%20123.html", ipadimage, "http://www.google.com",
            '    desktopimage, strusername, DecryptString(dt.Rows(0)(0).ToString(), "1234567891123456"))
            'message.IsBodyHtml = True
            ''message.IsBodyHtml = True
            'smtp.Host = "192.168.1.27"
            'smtp.EnableSsl = False
            'smtp.DeliveryMethod = SmtpDeliveryMethod.Network
            'smtp.UseDefaultCredentials = False
            ''Dim NetworkCred As 
            'smtp.Credentials = New NetworkCredential(newUserName, userpass)
            'smtp.Port = 25
            'smtp.Timeout = 60000
            'smtp.Send(message)

            Dim messageId = strusername
            objDBCom = New MySQLDBComponent.MySQLDBComponent(CStr(POSWeb.POSWeb_SQLConn))
            Dim sqlInsert As String = "INSERT INTO TMLI_SendEmail VALUES (@id,@emailTo,@content,@status)"
            Dim comm As New SqlCommand(sqlInsert)
            comm.Parameters.AddWithValue("@id", strusername)
            comm.Parameters.AddWithValue("@emailTo", dt2.Rows(0)(0).ToString())
            comm.Parameters.AddWithValue("@content", messageBody)
            comm.Parameters.AddWithValue("@status", "Sending")
            Dim result As Boolean = objDBCom.ExecuteSqlCommand(comm)

            If result Then
                Dim processinfo As New ProcessStartInfo()
                processinfo.WorkingDirectory = "C:\MailSender"
                processinfo.FileName = "java.exe"
                processinfo.Arguments = "-jar TestJar.jar " & messageId

                Dim myProcess As New Process()
                processinfo.UseShellExecute = False
                processinfo.RedirectStandardOutput = True
                myProcess.StartInfo = processinfo
                myProcess.Start()
                Dim myStreamReader As StreamReader = myProcess.StandardOutput
                ' Read the standard output of the spawned process. 
                Dim finalString As String
                Dim myString As String
                Do
                    myString = myStreamReader.ReadLine()
                    finalString += myString
                Loop Until (myStreamReader.EndOfStream)
                myProcess.WaitForExit()
                myProcess.Close()

            Else

            End If

            Return "True"
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    '***************
    'Function: Get hierarcy of the Agent
    'Author: YOSI
    'Date: 6/1/2017
    'Summary: Get hierarcy of the Agent
    '***************
    <WebMethod()>
    Public Function GetAgentHierarcy(ByVal strAgentCode As String) As DataTable
        Dim isSuccess As Boolean = False
        Dim objDBCom As New MySQLDBComponent.MySQLDBComponent(POSWeb.POSWeb_SQLConn)
        Dim dt As New DataTable
        Dim dc As New DataColumn
        Dim rt As String = ""

        'Dim str As String = "True"

        Try
            objDBCom.AddParameter("@AgentCode", SqlDbType.VarChar, strAgentCode)
            objDBCom.ExecuteProcedure(dt, "GetAgentHierarcy")

            dt.TableName = "TMLI_Hierarcy"

            Return dt

            objDBCom.Dispose()
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    '***************

    ''' <summary>
    ''' Function to return image as base64String
    ''' </summary>
    ''' <returns></returns>
    <WebMethod()>
    Public Function GetAllBackgroundImage() As DataTable
        Dim dTable As New DataTable
        dTable.TableName = "BackgroundImage"
        dTable.Columns.Add("FileName")
        dTable.Columns.Add("FileBase64String")

        Try
            'Dim fileLocation As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BackgroundImage")
            Dim fileLocation As String = "C:\BackgroundImages"
            Dim files As String() = Directory.GetFiles(fileLocation)

            For Each file As String In files
                Dim dr As DataRow = dTable.NewRow

                Dim fileBytes As Byte() = System.IO.File.ReadAllBytes(file)
                Dim base64StringFile As String = Convert.ToBase64String(fileBytes)
                Dim fileName As String = Path.GetFileName(file)

                dr("FileName") = fileName
                dr("FileBase64String") = base64StringFile

                dTable.Rows.Add(dr)
            Next
        Catch ex As Exception

        End Try
        Return dTable
    End Function

    '***************************************
    'Function: ValidateLogin
    'Author: Faiz
    'Date: 27/8/2015
    'Summary: Validate with agent code, device ID and agent password
    'Edited by: Eugene on 17/2/2016 - Johan on 11/9/2016
    '***************************************
    <WebMethod()>
    Public Function ValidateLogin(ByVal strAgentID As String, ByVal strPassword As String, ByVal strUDID As String, ByRef strStatus As String) As DataTable

        Dim isSuccess As Boolean = False
        Dim objDBCom As New MySQLDBComponent.MySQLDBComponent(POSWeb.POSWeb_SQLConn)
        Dim dt As New DataTable
        Dim dt2 As New DataTable
        Dim dc As New DataColumn

        Try
            objDBCom.AddParameter("@AgentCode", SqlDbType.NVarChar, strAgentID)
            objDBCom.AddParameter("@AgentPassword", SqlDbType.NVarChar, strPassword)
            objDBCom.AddParameter("@UDID", SqlDbType.NVarChar, strUDID)
            objDBCom.ExecuteProcedure(dt, "TMLI_WS_Agent_ValidateLogin")

            'Dim conn As New SqlConnection(POSWeb.POSWeb_SQLConn)

            If dt.Rows.Count <> 0 Then

                Dim str As String = "select AgentPassword, AgentStatus from TMLI_Agent_Profile where AgentCode = '" + dt.Rows(0)(14).ToString() + "'"
                objDBCom.ExecuteSQL(dt2, str)
                If dt2.Rows.Count > 0 Then
                    If Not dt2.Rows(0)(0).ToString.Equals("") Then
                        dt.Rows(0)("DirectSupervisorPassword") = dt2.Rows(0)(0).ToString()
                        dt.Rows(0)("DirectSupervisorStatus") = dt2.Rows(0)(1).ToString()
                    Else
                        dt.Rows(0)("DirectSupervisorPassword") = " "
                        dt.Rows(0)("DirectSupervisorStatus") = dt2.Rows(0)(1).ToString()
                    End If
                Else
                    dt.Rows(0)("DirectSupervisorPassword") = " "
                    dt.Rows(0)("DirectSupervisorStatus") = " "
                End If


                strStatus = "True"
                dt.TableName = "TMLI_Agent_Profile"
                dt.AcceptChanges()
            Else
                strStatus = "False"
                dt.TableName = "TMLI_Agent_Profile"
                dt.AcceptChanges()
            End If
            Return dt
            objDBCom.Dispose()
            dt.Dispose()
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    '***************************************
    'Function: Forgot Password
    'Author: Eugene
    'Date: 26/1/2016
    'Summary: send password for user in case password forgottenP
    '***************************************
    <WebMethod()>
    Public Function SendForgotPassword(ByVal strAgentId As String) As String
        Dim isSuccess As Boolean = False
        Dim objDBcom = New MySQLDBComponent.MySQLDBComponent(POSWeb.POSWeb_SQLConn)
        Dim objDBcommand = New MySQLDBComponent.MySQLDBComponent(POSWeb.POSWeb_SQLConn)
        Dim dt As New DataTable
        Dim userEmail As String = String.Empty
        Dim password As String = String.Empty
        Dim personalEmail As String = String.Empty
        Dim str As String = True

        objDBcom.AddParameter("@AgentCode", SqlDbType.NVarChar, strAgentId)
        objDBcom.ExecuteProcedure(dt, "TMLI_WS_Agent_Profile_ForgotPassword")
        objDBcom.Dispose()

        If dt.Rows.Count > 0 Then
            userEmail = dt.Rows(0)("AgentEmail").ToString()
            password = dt.Rows(0)("AgentPassword").ToString()
            personalEmail = dt.Rows(0)("emailPersonal").ToString()

            If Not String.IsNullOrEmpty(password) Then

                Dim dataTable As New DataTable
                objDBcommand.ExecuteSQL(dataTable, "SELECT COUNT(EmailID) FROM TMLI_SendEmail WHERE EmailID =" & strAgentId)

                Dim emailText As String = String.Format("Dengan Hormat,<br />" &
                                            "Anda mengajukan proses Forget Password pada aplikasi TMConnect <br /><br /> " &
                                            "<STRONG>User Name : {0}</STRONG><br /> " &
                                            "<STRONG>Password  : {1}</STRONG><br /><br /> " &
                                            "Jika anda mengalami kesulitan dalam melakukan proses Forget Password, <br />" &
                                            "mohon hubungi helpdesk kami di tmconnect.helpdesk@tokiomarine-life.co.id <br />" &
                                            "Hormat Kami <br /> <br />" &
                                            "PT Tokio Marine Life Insurance Indonesia", "Kode Agen Anda", DecryptString(password, "1234567891123456"))
                Dim result As Boolean
                If dataTable.Rows(0)(0) > 0 Then
                    Dim sqlInsert As String = "UPDATE TMLI_SendEmail SET EmailTo=@emailTo, EmailContent=@content, EmailStatus=@status, EmailSubject=@emailSubject, EmailTime=@emailTime WHERE EmailID=@id"
                    Dim comm As New SqlCommand(sqlInsert)
                    comm.Parameters.AddWithValue("@id", strAgentId)
                    comm.Parameters.AddWithValue("@emailTo", userEmail + ";" + personalEmail)
                    comm.Parameters.AddWithValue("@content", emailText)
                    comm.Parameters.AddWithValue("@status", "Sending")
                    comm.Parameters.AddWithValue("@emailSubject", "Password Recovery")
                    comm.Parameters.AddWithValue("@emailTime", Now)
                    result = objDBcommand.ExecuteSqlCommand(comm)
                Else
                    Dim sqlInsert As String = "INSERT INTO TMLI_SendEmail VALUES (@id,@emailTo,@content,@status,@emailSubject,@emailTime)"
                    Dim comm As New SqlCommand(sqlInsert)
                    comm.Parameters.AddWithValue("@id", strAgentId)
                    comm.Parameters.AddWithValue("@emailTo", userEmail + ";" + personalEmail)
                    comm.Parameters.AddWithValue("@content", emailText)
                    comm.Parameters.AddWithValue("@status", "Sending")
                    comm.Parameters.AddWithValue("@emailSubject", "Password Recovery")
                    comm.Parameters.AddWithValue("@emailTime", Now)
                    result = objDBcommand.ExecuteSqlCommand(comm)
                End If

                If result Then
                    Dim processinfo As New ProcessStartInfo()
                    processinfo.WorkingDirectory = "C:\MailSender"
                    processinfo.FileName = "java.exe"
                    processinfo.Arguments = "-jar TestJar.jar " & strAgentId

                    Dim myProcess As New Process()
                    processinfo.UseShellExecute = False
                    processinfo.RedirectStandardOutput = True
                    myProcess.StartInfo = processinfo
                    myProcess.Start()
                    Dim myStreamReader As StreamReader = myProcess.StandardOutput
                    ' Read the standard output of the spawned process. 
                    Dim finalString As String
                    Dim myString As String
                    Do
                        myString = myStreamReader.ReadLine()
                        finalString += myString
                    Loop Until (myStreamReader.EndOfStream)
                    myProcess.WaitForExit()
                    myProcess.Close()
                    Return str
                End If
                'Dim adminmail As String = System.Configuration.ConfigurationManager.AppSettings("emailAddress").ToString()
                'Dim userpass As String = System.Configuration.ConfigurationManager.AppSettings("emailPassword").ToString()
                'Dim smtp As New SmtpClient()
                'Dim message As New MailMessage(adminmail, userEmail)
                'message.Subject = "Password Recovery"
                'message.Body = String.Format("Dengan Hormat,<br />" &
                '                            "Anda mengajukan proses Forget Password pada aplikasi TMConnect <br /> " &
                '                            "<STRONG>User Name : {0}</STRONG><br /> " &
                '                            "<STRONG>Password  : {1}.</STRONG><br /><br /> " &
                '                            "Jika anda mengalami kesulitan dalam melakukan proses Forget Password, <br />" &
                '                            "mohon hubungi helpdesk kami di tmconnect.helpdesk@tokiomarine-life.co.id <br />" &
                '                            "Hormat Kami <br /> <br />" &
                '                            "PT Tokio Marine Life Insurance Indonesia", "Kode Agen Anda", DecryptString(password, "1234567891123456"))

                'message.IsBodyHtml = True  
                'smtp.Host = "192.168.1.27"
                'smtp.EnableSsl = False
                'Dim NetworkCred As New NetworkCredential("tmli\tmconnect.no-reply", userpass)
                'smtp.Credentials = NetworkCred
                'smtp.Port = 25
                'smtp.Send(message)
                Return str
            Else
                Return "False"
            End If
            dt.Dispose()
        End If
    End Function

    '***************************************
    'Function: LoginFirstTime
    'Author: Johan
    'Date: 22/03/2017
    'Summary: Used to check whether the UDID for underlying agent is null or not.
    '***************************************
    <WebMethod()>
    Public Function IsAgentHaveUDID(strAgentId As String) As Boolean
        Dim objDBCom As New MySQLDBComponent.MySQLDBComponent(POSWeb.POSWeb_SQLConn)
        Dim query As String = "select case when TMLI_Agent_Profile.UDID IS NULL THEN 0 ELSE 1 END FROM TMLI_Agent_Profile where AgentCode = '" & strAgentId & "'"
        Dim dTable As New DataTable
        objDBCom.ExecuteSQL(dTable, query)
        If dTable.Rows.Count > 0 Then
            Return True
        Else
            Return False
        End If
    End Function

    '***************************************
    'Function: LoginFirstTime
    'Author: Eugene
    'Date: 15/2/2016
    'Summary: User login for the first time.
    '***************************************
    <WebMethod()>
    Public Function ReceiveFirstLogin(ByVal strAgentId As String, ByVal strAgentPass As String, ByVal strNewPass As String, ByVal strUID As String, ByRef strStatus As String) As DataTable
        Dim isSuccess As Boolean = False
        Dim objDBCom As New MySQLDBComponent.MySQLDBComponent(POSWeb.POSWeb_SQLConn)
        Dim dt As New DataTable
        Dim dt2 As New DataTable
        Dim dc As New DataColumn
        Dim rt As String = ""

        'Dim str As String = "True"

        Try
            objDBCom.AddParameter("@AgentCode", SqlDbType.NVarChar, strAgentId)
            objDBCom.AddParameter("@AgentPass", SqlDbType.NVarChar, strAgentPass)
            objDBCom.AddParameter("@NewPass", SqlDbType.NVarChar, strNewPass)
            objDBCom.AddParameter("@UDID", SqlDbType.NVarChar, strUID)

            objDBCom.ExecuteProcedure(dt, "TMLI_WS_Agent_ValidateFirstTime")

            'modified by Johan @ 6 march 2017 to accomodate user Level RD

            If dt.Rows.Count > 0 Then
                Dim str As String = "Select AgentPassword, AgentStatus from TMLI_Agent_Profile where AgentCode = '" + dt.Rows(0)(14).ToString() + "'"
                objDBCom.ExecuteSQL(dt2, str)

                If dt2.Rows.Count > 0 Then
                    If Not dt2.Rows(0)(0).ToString.Equals("") Then
                        dt.Rows(0)("DirectSupervisorPassword") = dt2.Rows(0)(0).ToString()
                        dt.Rows(0)("DirectSupervisorStatus") = dt2.Rows(0)(1).ToString()
                    Else
                        dt.Rows(0)("DirectSupervisorPassword") = " "
                        dt.Rows(0)("DirectSupervisorStatus") = dt2.Rows(0)(1).ToString()
                    End If
                Else
                    dt.Rows(0)("DirectSupervisorPassword") = " "
                    dt.Rows(0)("DirectSupervisorStatus") = " "
                End If


                strStatus = "True"
                dt.TableName = "TMLI_Agent_Profile"
                dt.AcceptChanges()
                Return dt
            Else
                strStatus = "False"
            End If

            objDBCom.Dispose()
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    '***************************************


    '***************************************
    'Function: Change Password
    'Author: Eugene
    'Date: 19/2/2016
    'Summary: User change password.
    '***************************************
    <WebMethod()>
    Public Function ChangePassword(ByVal strAgentId As String, ByVal strPassword As String, ByVal strNewPass As String, ByVal strUDID As String) As String
        Dim isSuccess As Boolean = False
        Dim objDBCom As New MySQLDBComponent.MySQLDBComponent(POSWeb.POSWeb_SQLConn)
        Dim dt As New DataTable

        If strUDID = "WEB" Then
            Try
                Dim updateString As String = "UPDATE TMLI_Agent_Profile SET AgentPassword=@password WHERE AgentCode =@code and AgentPassword = @AgentPass"
                Dim comm As New SqlCommand(updateString)
                comm.Parameters.AddWithValue("@password", strNewPass)
                comm.Parameters.AddWithValue("@code", strAgentId)
                comm.Parameters.AddWithValue("@AgentPass", strPassword)
                If objDBCom.ExecuteSqlCommand(comm) Then
                    Return True
                Else
                    Return False
                End If
            Catch ex As Exception
                Return False
                Throw ex
            End Try
        Else
            Try
                objDBCom.AddParameter("@AgentCode", SqlDbType.NVarChar, strAgentId)
                objDBCom.AddParameter("@AgentPass", SqlDbType.NVarChar, strPassword)
                objDBCom.AddParameter("@NewPass", SqlDbType.NVarChar, strNewPass)
                objDBCom.AddParameter("@UDID", SqlDbType.NVarChar, strUDID)

                objDBCom.ExecuteProcedure(dt, "TMLI_WS_Agent_ChangePassword")
                objDBCom.Dispose()

                If dt.Rows.Count > 0 Then
                    Return True
                Else
                    Return False
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End If

    End Function

    <WebMethod()>
    Public Function GetSPAJAndVA() As String
        Dim testParam As New List(Of String)
        testParam.Add("7682981738")
        testParam.Add("7682981738")
        testParam.Add("7682981738")
        testParam.Add("7682981738")
        SPAJNumberGenerator.WriteSyariahTXT(testParam)
        Dim result As String = SPAJNumberGenerator.GetSPAJNumber()


        Return result
    End Function
    '***************************************


    '***************************************
    'Function: FullSyncTable
    'Author: Eugene
    'Date: 22/2/2016
    'Summary: Sync table fully when user login
    '***************************************
    <WebMethod()>
    Public Function FullSyncTable(ByVal strAgentCode As String, ByRef strStatus As String) As DataSet
        Dim isSuccess As Boolean = False
        Dim objDBCom As New MySQLDBComponent.MySQLDBComponent(POSWeb.POSWeb_SQLConn)
        Dim dt As New DataTable
        Dim dc As New DataSet
        Dim dt2 As New DataTable
        Dim dt3 As New DataTable
        Dim ag As New DataTable
        Dim i As Integer = 0
        Dim j As String = 0

        Try
            Dim findagent As String = "select [Level]
                  ,[Atasan]
                  ,[IndexNo]
                  ,[AgentCode]
                  ,[AgentName]
                  ,[AgentEmail]
                  ,[AgentStatus]
                  ,[AgentAddr1]
                  ,[AgentAddr2]
                  ,[AgentAddr3]
                  ,[AgentContactNumber]
                  ,[AgentPassword]
                  ,[LicenseStartDate]
                  ,[LicenseExpiryDate]
                  ,[DirectSupervisorCode]
                  ,[DirectSupervisorName]
                  ,[BranchCode]
                  ,[BranchName]
                  ,[KCU]
                  ,[KanwilCode]
                  ,[Kanwil]
                  ,[ChannelCode]
                  ,[ChannelName]
                  ,[KodeCabang]
                  ,[Cabang]
                  ,[FirstLogin]
                  ,[LastLogonDate]
                  ,[LastLogoutDate]
                  ,[UDID]
                  ,[LastSyncDate]
                  ,[DeviceStatus]
                  ,[DirectSupervisorPassword]
                  ,[Admin]
                  ,[AdminPassword]
                  ,[DirectSupervisorStatus]
                  ,[LGIVNAME]
                  ,[CLTSEX]
                  ,[CLTDOB]
                  ,[SECUITYNO]
                  ,[SALUTL]
                  ,[NATLTY]
                  ,[MARRYD]
                  ,[ADDRTYPE]
                  ,[CLTADDR04]
                  ,[CLTADDR05]
                  ,[CLTPCODE]
                  ,[CTRYCODE]
                  ,[CLTPHONE01]
                  ,[CLTPHONE02]
                  ,[TAXMETH]
                  ,[XREFNO]
                  ,[ZRELIGN]
                  ,[TLAGLICNO]
                  ,[TLICEXPDT]
                  ,[TLAGLICNO_S]
                  ,[TLICEXPDT_S]
                  ,[BANKKEY]
                  ,[BANKACOUNT]
                  ,[BANKACCDSC]
                  ,[UPDATEDATE]
                  ,[InvitationDate]
                  ,[RegisterDate]
                  ,[DevicePlatform]
                  ,[TSALESUNT_FIRST]
                  ,[TSALESUNT_LAST] from TMLI_Agent_Profile where AgentCode = '" + strAgentCode.ToString() + "'"
            objDBCom.ExecuteSQL(ag, findagent)
            If Not ag.Rows.Count.Equals(0) Then
                'Stored procedure to list down all related tables that need to be pass to another device
                objDBCom.AddParameter("@agentcode", SqlDbType.NVarChar, strAgentCode)
                objDBCom.ExecuteProcedure(dc, "TMLI_WS_Agent_FullSync")
                objDBCom.Parameters.Clear()
                'The below stored procedure will list all the names of the tables in the database in 
                'same order and numbers as the above stored procedure\

                objDBCom.ExecuteProcedure(dt, "TMLI_WS_Agent_Extra")
                If dt.Rows.Count > 0 Then
                    'For Each rows In dt.Rows
                    For Each Table As DataTable In dc.Tables
                        dc.Tables(i).TableName = dt.Rows(j)(0).ToString()
                        If Table.TableName.Equals("TMLI_Master_Info") Or Table.TableName.Equals("TMLI_Data_Version") Or Table.TableName.Equals("TMLI_Score_Prospect_Age") Or Table.TableName.Equals("TMLI_Score_Prospect_Annual_Income") Or Table.TableName.Equals("TMLI_Score_Prospect_Gender") Or Table.TableName.Equals("TMLI_Score_Prospect_Marital_Status") Or Table.TableName.Equals("TMLI_Score_Prospect_Referral") Or Table.TableName.Equals("TMLI_Score_Prospect_Source_Income") Then

                        ElseIf Table.TableName.Equals("TMLI_eProposal_Marital_Status") Or Table.TableName.Equals("TMLI_eProposal_Nationality") Or Table.TableName.Equals("TMLI_eProposal_Religion") Or Table.TableName.Equals("TMLI_Data_Cabang") Or Table.TableName.Equals("TMLI_eProposal_Relation") Or Table.TableName.Equals("TMLI_Bank") Or Table.TableName.Equals("TMLI_Score_Prospect_Occupation") Or Table.TableName.Equals("TMLI_eProposal_OCCP") Then
                            Table.Columns.Remove("item_time")
                            'ElseIf Table.TableName.Equals("TMLI_eProposal_Relation") Then
                            '    Table.Columns.Remove("IsUpdate")
                        ElseIf Table.TableName.Equals("TMLI_eProposal_OCCP") Then
                            Table.Columns.Remove("item_time")
                            'Table.Columns.Remove("RiskClass")
                        Else
                            Table.Columns.Remove("IsNew")
                            Table.Columns.Remove("IsUpdate")
                            Table.Columns.Remove("RowStatus")
                        End If
                        i += 1
                        j += 1
                    Next
                    objDBCom.AddParameter("@agentcode", SqlDbType.NVarChar, strAgentCode)
                    objDBCom.ExecuteProcedure(dt2, "TMLI_WS_Agent_SelectAgent")

                    Dim str As String = "select AgentPassword, AgentStatus from TMLI_Agent_Profile where AgentCode = '" + dt2.Rows(0)(14).ToString() + "'"
                    objDBCom.ExecuteSQL(dt3, str)

                    If dt3.Rows.Count > 0 Then
                        If Not dt3.Rows(0)(0).ToString.Equals("") Then
                            dt2.Rows(0)("DirectSupervisorPassword") = dt3.Rows(0)(0).ToString()
                            dt2.Rows(0)("DirectSupervisorStatus") = dt3.Rows(0)(1).ToString()
                        Else
                            dt2.Rows(0)("DirectSupervisorPassword") = " "
                            dt2.Rows(0)("DirectSupervisorStatus") = dt3.Rows(0)(1).ToString()
                        End If
                    Else
                        dt2.Rows(0)("DirectSupervisorPassword") = " "
                        dt2.Rows(0)("DirectSupervisorStatus") = " "
                    End If

                    objDBCom.Dispose()
                    dt2.TableName = "TMLI_Agent_Profile"
                    dt2.AcceptChanges()
                    dc.Tables.Add(dt2)
                    'Next
                    strStatus = "True"
                    Return dc
                Else
                    strStatus = "No Record"
                End If
            Else
                strStatus = "No Record"
            End If
        Catch ex As Exception
            Throw ex
        End Try

    End Function
    '***************************************

    '***************************************
    'Function: CheckVersion
    'Author: Eugene
    'Date: 22/2/2016
    'Summary: Check version of each table
    '***************************************
    <WebMethod()>
    Public Function CheckVersion(ByVal strVesion As String, ByRef strStatus As String) As DataTable
        Dim isSuccess As Boolean = False
        Dim objDBCom As New MySQLDBComponent.MySQLDBComponent(POSWeb.POSWeb_SQLConn)
        Dim dt As New DataTable
        Dim dt2 As New DataTable
        Dim x As String = ""
        Dim sql As String = ""
        Dim sql2 As String = ""
        strVesion = 1

        sql = "Select VersionNo from TMLI_Master_Info where TableName = 'TMLI_Master_Info'"
        objDBCom.ExecuteSQL(dt2, sql)
        x = dt2.Rows(0)(0)
        If strVesion <> x Then
            sql2 = "select * from TMLI_Master_Info where TableName != 'TMLI_Master_Info'"
            objDBCom.ExecuteSQL(dt, sql2)
            objDBCom.Dispose()
            dt.TableName = "Master_Info"
            strStatus = "True"
            Return dt
        Else
            strStatus = "False"
        End If

    End Function
    '***************************************

    '***************************************
    'Function: Login API
    'Created: 4/3/2016
    'Summary: For testing purpose
    '***************************************
    <WebMethod()>
    Public Function LoginAPI(ByVal strAgentCode As String, ByVal strPass As String, ByVal strStatus As String) As String
        Dim isSuccess As Boolean = False
        Dim objDBCom As New MySQLDBComponent.MySQLDBComponent(POSWeb.POSWeb_SQLConn)
        Dim dt As New DataTable

        Try
            objDBCom.AddParameter("@agentcode", SqlDbType.NVarChar, strAgentCode)
            objDBCom.AddParameter("@agentpass", SqlDbType.NVarChar, strPass)
            objDBCom.ExecuteProcedure(dt, "TMLI_LoginAPI")
            objDBCom.Dispose()

            If dt.Rows.Count <> 0 Then
                strStatus = "True"
                Return strStatus
            Else
                strStatus = "False"
                Return strStatus
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    '***************************************

    '***************************************
    'Function: Version Checker
    'Created: 10/3/2016
    'Summary: check the version of the application
    '***************************************
    <WebMethod()>
    Public Function VersionChecker(ByVal strVersion As String) As String
        Dim isSuccess As Boolean = False
        Dim sql As String = ""
        sql &= "select MAX(VersionNo) as NewestVersion from TMLI_Version_checker"
        Dim objDBCom As New MySQLDBComponent.MySQLDBComponent(POSWeb.POSWeb_SQLConn)
        Dim dt As New DataTable

        Try
            objDBCom.ExecuteSQL(dt, sql)
            objDBCom.Dispose()
            If Not dt.Rows(0)(0).ToString.Equals("") Then
                If Convert.ToDouble(dt.Rows(0)(0)) > Convert.ToDouble(strVersion) Then
                    Return "True"
                ElseIf Convert.ToDouble(dt.Rows(0)(0)) <= Convert.ToDouble(strVersion) Then
                    Return "False"
                End If
            Else
                Return "False"
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    '***************************************

    <WebMethod()>
    Public Function SendStringMessage(message As String) As String
        ' Create a request using a URL that can receive a post.   
        Dim request As WebRequest = WebRequest.Create("http://192.168.1.119:9090/TMConnectWS/webapi/submission/create")
        ' Set the Method property of the request to POST.  
        request.Method = "POST"
        ' Create POST data and convert it to a byte array.  
        'Dim postData As String = "{""branch_id"ri":""GD  "",""agent_code"":""60002407"",""policy_holder"":""Test Policy Holder Web Service 01"",""insured"":""Test Send Web Service 01"",""phone_no"":""085691882088"",""email"":""hito.mait@gmail.com"",""spaj_no"":""810000000003"",""paymode"":""SA"",""product_id"":""IWA1"",""currency"":""IDR"",""premium"":""3999999.99"",""top_reg"":""2000000.00"",""top_sekaligus"":""3000000.00"",""spaj_a1"":"",""spaj_a2"":"",""spaj_a3"":"",""spaj_a4"":"",""spaj_a5"":"",""spaj_b1"":"",""spaj_b2"":"",""spaj_b3"":"",""spaj_b4"":"",""spaj_b5"":""}"

        '' this is applied for SPAJ Testing
        Dim spajObject As New TMLISPAJSubmissionClass
        spajObject.branch_id = "1001M"
        spajObject.agent_code = "80003799"
        spajObject.policy_holder = "Johan Regar"
        spajObject.insured = "Test Send Johan"
        spajObject.phone_no = "085691882088"
        spajObject.email = "js.regar@gmail.com"
        spajObject.spaj_no = "811100000001"
        spajObject.paymode = "SA"
        spajObject.product_id = "IWA1"
        spajObject.currency = "IDR"
        spajObject.premium = "39999999.99"
        spajObject.top_reg = "2000000.00"
        spajObject.top_sekaligus = "3000000.00"
        spajObject.spaj_a1 = "http://192.168.1.119/SPAJFiles/81000000001/a1.pdf"
        spajObject.spaj_a2 = "http://192.168.1.119/SPAJFiles/81000000001/a2.pdf"
        spajObject.spaj_a3 = ""
        spajObject.spaj_a4 = ""
        spajObject.spaj_a5 = ""
        spajObject.spaj_b1 = ""
        spajObject.spaj_b2 = ""
        spajObject.spaj_b3 = ""
        spajObject.spaj_b4 = ""
        spajObject.spaj_b5 = ""
        spajObject.user_upload = "80003799"
        spajObject.birth_date_user = "1990/04/02"
        spajObject.ilustration_no = "80001601170221145899"
        spajObject.va_number = "11110811100000001"
        spajObject.type_va = "S"
        '' this is applied for SPAJ Number Sync testing
        'Dim spajList As New List(Of String)
        'spajList.Add("810000000001")
        'spajList.Add("820000000001")
        'spajList.Add("830000000001")
        'spajList.Add("840000000001")
        'spajList.Add("850000000001")
        'spajList.Add("860000000001")
        'spajList.Add("870000000001")
        'spajList.Add("880000000001")
        'spajList.Add("890000000001")
        'spajList.Add("811000000001")
        'spajList.Add("812000000001")
        'spajList.Add("813000000001")
        'spajList.Add("814000000001")
        'spajList.Add("815000000001")
        'Dim spajObject As New TMLIInitialSPAJClass
        'spajObject.spaj_no = spajList

        Dim serializedSpajObject As String = JsonConvert.SerializeObject(spajObject)

        'Dim byteArray As Byte() = Encoding.UTF8.GetBytes(serializedSpajObject)
        '' Set the ContentType property of the WebRequest.  
        'request.ContentType = "application/json"
        '' Set the ContentLength property of the WebRequest.  
        'request.ContentLength = byteArray.Length
        '' Get the request stream.  
        'Dim dataStream As Stream = request.GetRequestStream()
        '' Write the data to the request stream.  
        'dataStream.Write(byteArray, 0, byteArray.Length)
        '' Close the Stream object.  
        'dataStream.Close()
        '' Get the response.  
        'Dim response As WebResponse = request.GetResponse()
        '' Display the status.  
        'Console.WriteLine(CType(response, HttpWebResponse).StatusDescription)
        '' Get the stream containing content returned by the server.  
        'dataStream = response.GetResponseStream()
        '' Open the stream using a StreamReader for easy access.  
        'Dim reader As New StreamReader(dataStream)
        '' Read the content.  
        'Dim responseFromServer As String = reader.ReadToEnd()
        '' Display the content.  
        'Console.WriteLine(responseFromServer)
        '' Clean up the streams.  
        'reader.Close()
        'dataStream.Close()
        'response.Close()
    End Function

    Public Function EncryptString(plainSourceStringToEncrypt As String, passPhrase As String) As String
        'Set up the encryption objects
        Using acsp As AesCryptoServiceProvider = GetProvider(Encoding.[Default].GetBytes(passPhrase))
            Dim sourceBytes As Byte() = Encoding.ASCII.GetBytes(plainSourceStringToEncrypt)
            Dim ictE As ICryptoTransform = acsp.CreateEncryptor()

            'Set up stream to contain the encryption
            Dim msS As New MemoryStream()

            'Perform the encrpytion, storing output into the stream
            Dim csS As New CryptoStream(msS, ictE, CryptoStreamMode.Write)
            csS.Write(sourceBytes, 0, sourceBytes.Length)
            csS.FlushFinalBlock()

            'sourceBytes are now encrypted as an array of secure bytes
            Dim encryptedBytes As Byte() = msS.ToArray()
            '.ToArray() is important, don't mess with the buffer
            'return the encrypted bytes as a BASE64 encoded string
            Return Convert.ToBase64String(encryptedBytes)
        End Using
    End Function

    '***************************************

    Public Function DecryptString(base64StringToDecrypt As String, passphrase As String) As String
        'Set up the encryption objects
        Using acsp As AesCryptoServiceProvider = GetProvider(Encoding.[Default].GetBytes(passphrase))
            Dim RawBytes As Byte() = Convert.FromBase64String(base64StringToDecrypt)
            Dim ictD As ICryptoTransform = acsp.CreateDecryptor()

            'RawBytes now contains original byte array, still in Encrypted state

            'Decrypt into stream
            Dim msD As New MemoryStream(RawBytes, 0, RawBytes.Length)
            Dim csD As New CryptoStream(msD, ictD, CryptoStreamMode.Read)
            'csD now contains original byte array, fully decrypted

            'return the content of msD as a regular string
            Return (New StreamReader(csD)).ReadToEnd()
        End Using
    End Function

    '***************************************

    Private Function GetProvider(key As Byte()) As AesCryptoServiceProvider
        Dim result As New AesCryptoServiceProvider()
        result.BlockSize = 128
        result.KeySize = 128
        result.Mode = CipherMode.CBC
        result.Padding = PaddingMode.PKCS7

        result.GenerateIV()
        result.IV = New Byte() {0, 0, 0, 0, 0, 0,
         0, 0, 0, 0, 0, 0,
         0, 0, 0, 0}

        Dim RealKey As Byte() = GetKey(key, result)
        result.Key = RealKey
        ' result.IV = RealKey;
        Return result
    End Function

    '***************************************

    Private Function GetKey(suggestedKey As Byte(), p As SymmetricAlgorithm) As Byte()
        Dim kRaw As Byte() = suggestedKey
        Dim kList As New List(Of Byte)()

        For i As Integer = 0 To p.LegalKeySizes(0).MinSize - 1 Step 8
            kList.Add(kRaw((i / 8) Mod kRaw.Length))
        Next
        Dim k As Byte() = kList.ToArray()
        Return k
    End Function

    '***************************************
    'Function: ChangeUDID
    'Author: Eugene
    'Date: 3/6/2016
    'Summary: Change the UDID for each Agent
    '***************************************
    <WebMethod()>
    Public Function ChangeUDID(ByVal strAgentcode As String, ByVal strUDID As String, ByRef strStatus As String) As String
        Dim isSuccess As Boolean = False
        Dim objDBCom As New MySQLDBComponent.MySQLDBComponent(POSWeb.POSWeb_SQLConn)
        Dim dt As New DataTable

        objDBCom.AddParameter("@AgentCode", SqlDbType.VarChar, strAgentcode)
        objDBCom.AddParameter("@UDID", SqlDbType.VarChar, strUDID)
        objDBCom.ExecuteProcedure(dt, "TMLI_UpdateAgentUDID")

        objDBCom.Dispose()
        If dt.Rows.Count <> 0 Then
            strStatus = "True"
            Return strStatus
        Else
            strStatus = "False"
            Return strStatus
        End If
    End Function

    '***************************************
    'Function: SyncDataReferral
    'Author: Eugene
    'Date: 3/6/2016
    'Summary: Sync date using updateddate
    '***************************************
    <WebMethod()>
    Public Function Syncdatareferral(ByVal strUpdateDate As String, ByRef strstatus As String) As DataTable
        Dim objDBCom As New MySQLDBComponent.MySQLDBComponent(POSWeb.POSWeb_SQLConn)
        Dim dt As New DataTable

        Dim query As String = "select * from TMLI_DataReferral where IsUpdate > (SELECT CONVERT(datetime, SWITCHOFFSET(CONVERT(datetimeoffset, @strUpdateDate), DATENAME(TzOffset, SYSDATETIMEOFFSET()))))"
        objDBCom.AddParameter("@strUpdateDate", SqlDbType.VarChar, strUpdateDate)
        objDBCom.ExecuteSQL(dt, query)

        objDBCom.Dispose()
        If dt.Rows.Count <> 0 Then
            dt.Columns("IsNew").ColumnName = "CreateTime"
            dt.Columns("IsUpdate").ColumnName = "UpdateTime"
            dt.Columns.Remove("RowStatus")
            strstatus = "True"
            dt.TableName = "TMLI_DataReferral"
            Return dt
        Else
            strstatus = "False"
        End If
    End Function

    '***************************************
    'Function: SyncDataVersion
    'Author: Eugene
    'Date: 3/8/2016
    'Summary: Sync Data Version Table
    '***************************************
    <WebMethod()>
    Public Function SyncDataVersion(ByVal Version As String, ByRef strStatus As String) As String
        Dim objDBCom As New MySQLDBComponent.MySQLDBComponent(POSWeb.POSWeb_SQLConn)
        Dim dt As New DataTable
        Dim q As String = "select MAX(Version) as NewestVersion from TMLI_Data_Version"
        objDBCom.ExecuteSQL(dt, q)
        objDBCom.Dispose()
        Try
            objDBCom.ExecuteSQL(dt, q)
            objDBCom.Dispose()
            If dt.Rows.Count <> 0 Then
                If Convert.ToDouble(dt.Rows(0)(0)) > Convert.ToDouble(Version) Then
                    Return "True"
                ElseIf Convert.ToDouble(dt.Rows(0)(0)) <= Convert.ToDouble(Version) Then
                    Return "False"
                End If
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    '***************************************

    '***************************************
    'Function: AdminLogin
    'Created: 15/3/2016
    'Summary: Admin login
    '***************************************
    <WebMethod()>
    Public Function AdminLogin(ByVal stradmin As String, ByVal stradminpass As String, ByVal strStatus As String) As String
        Dim isSuccess As Boolean = False
        Dim objDBCom As New MySQLDBComponent.MySQLDBComponent(POSWeb.POSWeb_SQLConn)
        Dim dt As New DataTable

        Dim SQL As String = "select distinct * from TMLI_Agent_Profile where Admin = @admincode and AdminPassword = @adminpass"

        objDBCom.AddParameter("@admincode", SqlDbType.VarChar, stradmin)
        objDBCom.AddParameter("@adminpass", SqlDbType.VarChar, stradminpass)
        objDBCom.ExecuteSQL(dt, SQL)
        objDBCom.Dispose()
        If dt.Rows.Count <> 0 Then
            strStatus = "True"
            Return strStatus
        Else
            strStatus = "False"
            Return strStatus
        End If

    End Function
    '***************************************
    <WebMethod()>
    Public Function encryptPassAgent(strString As String) As String
        Dim result As String = EncryptString(strString, "1234567891123456")
        If Not result.Equals(String.Empty) Then
            Return result
        End If
    End Function

End Class