'Imports System.Web.Services
'Imports System.Web.Services.Protocols
'Imports System.ComponentModel
'Imports System.IO
'Imports System.Security
'Imports System.Security.Cryptography
'Imports System.Xml
'Imports System.Web
'Imports System.Configuration
'Imports System.Net
'Imports System.Net.Mail
'Imports System.Data.SqlClient

'' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
'' <System.Web.Script.Services.ScriptService()> _
'<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
'<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
'<ToolboxItem(False)> _
'Public Class AgentManagementWebServices
'    Inherits System.Web.Services.WebService

'    Public Function HelloWorld() As String
'        Return "Hello World"
'    End Function

'    '***************************************
'    'Function: ValidateAgentAndDevice
'    'Author: Faiz
'    'Date: 27/8/2015
'    'Summary: Validate with agent code and device ID 
'    '***************************************
'    <WebMethod()> _
'    Public Function ValidateAgentAndDevice(ByVal strAgentID As String, ByVal strDeviceID As String) As String

'        Dim isSuccess As Boolean = False
'        Dim objDBCom As New MySQLDBComponent.MySQLDBComponent(POSWeb.POSWeb_SQLConn)
'        Dim dt As New DataTable
'        Dim sqlS As String = ""

'        objDBCom.AddParameter("@AgentCode", SqlDbType.NVarChar, strAgentID)
'        objDBCom.AddParameter("@DeviceID", SqlDbType.NVarChar, strDeviceID)

'        objDBCom.ExecuteProcedure(dt, "SP_WS_Agent_ValidateAgentAndDevice")
'        objDBCom.Dispose()

'        If dt.Rows.Count <> 0 Then
'            Return "True"
'        Else
'            Return "False"
'        End If

'        dt.Dispose()

'    End Function

'    '***************************************
'    'Function: ValidateLogin
'    'Author: Faiz
'    'Date: 27/8/2015
'    'Summary: Validate with agent code, device ID and agent password
'    '***************************************
'    <WebMethod()> _
'    Public Function ValidateLogin(ByVal strAgentID As String, ByVal strPassword As String, ByVal strDeviceID As String) As String

'        Dim isSuccess As Boolean = False
'        Dim objDBCom As New MySQLDBComponent.MySQLDBComponent(POSWeb.POSWeb_SQLConn)
'        Dim dt As New DataTable
'        Dim dc As New DataColumn
'        Dim sqlS As String = String.Empty
'        Dim str As String = "True"
'        objDBCom.AddParameter("@AgentCode", SqlDbType.NVarChar, strAgentID)
'        objDBCom.AddParameter("@AgentPassword", SqlDbType.NVarChar, strPassword)
'        objDBCom.AddParameter("@UDID", SqlDbType.NVarChar, strDeviceID)

'        objDBCom.ExecuteProcedure(dt, "SP_WS_Agent_ValidateLogin")
'        objDBCom.Dispose()

'        If dt.Rows.Count > 0 Then
'            For Each dc In dt.Columns
'                str = str & " | " & dt.Rows(0).Item(dc.ToString())
'            Next
'            Return str
'        Else
'            Return "False"
'        End If

'        dt.Dispose()

'    End Function

'    '***************************************
'    'Function: SaveDocument
'    'Author: Faiz
'    'Date: 27/8/2015
'    'Summary: 1) Accept file in binary string
'    '         2) Call Decrypt function
'    '         3) if Complete.xml, check the file count in folder 
'    '           tally with xml content before insert in table
'    '         4) Remove files if incomplete
'    '***************************************
'    <WebMethod()> _
'    Public Function SaveDocument(ByVal strBinary As String, ByVal strDocName As String, ByVal strFolder As String, ByVal strSource As String, ByVal agentID As String, ByVal totalFile As String) As String
'        Dim strStatus As String
'        Dim strProposal As String
'        Dim strDir As String = "D:\Files\"

'        Dim script As String = ""

'        Try
'            strFolder = strDocName.Substring(0, strDocName.IndexOf("_")) & "\" 'get Proposal number from strDocName
'            strProposal = strDocName.Substring(0, strDocName.IndexOf("_")) & "|"
'            strSource = "IAPP (S)"

'            ' Store transferred file in location folder
'            'Dim bw As BinaryWriter
'            'Dim sIn() As Byte = Convert.FromBase64String(strBinary)

'            'If (Not System.IO.Directory.Exists(strDir & strFolder)) Then
'            '    System.IO.Directory.CreateDirectory(strDir & strFolder)
'            'End If

'            'bw = New BinaryWriter(File.Create(strDir & strFolder & strDocName)) 'Create the ecrypted file in local server location 
'            'bw.Write(sIn)
'            'bw.Close()

'            '*****************************************************************************
'            If DecryptFile(strDocName, strFolder, strSource, strDir) = True Then
'                strStatus = strProposal + "Document Successfully Received"
'            Else
'                strStatus = "Unsuccessful Decrypt & Transfer"
'            End If
'            '*****************************************************************************
'            'Delete all .enc file
'            If System.IO.Directory.GetFiles(strDir & strFolder).Length <> 0 Then
'                For Each sFile In System.IO.Directory.GetFiles(strDir & strFolder)
'                    If sFile.Contains("enc") Then
'                        System.IO.File.Delete(sFile)
'                    End If
'                Next sFile
'            End If

'            If strDocName.ToLower.Contains("complete.xml") Then
'                If (Not System.IO.Directory.Exists(strDir & strFolder & strDocName)) Then

'                    Dim count, counter As Integer
'                    Dim getPropNo As String = String.Empty
'                    Dim getAgentID As String = String.Empty
'                    Dim strChgExt As String = Replace(strDocName, ".enc", "")

'                    'Get file number from complete.xml
'                    'Using sr As StreamReader = New StreamReader(strDir & strFolder & strChgExt)
'                    '    Dim line As String
'                    '    ' Read and display lines from the file until the end of the file is reached. 
'                    '    line = sr.ReadLine()
'                    '    Dim words As String() = line.Split(New Char() {";"c})

'                    '    ' Use For Each loop over words and display them.
'                    '    For Each word As String In words
'                    '        If word.Length <> 0 Then 'Filter empty string
'                    '            If counter = 0 Then
'                    '                getAgentID = words(0) 'Read Agent ID
'                    '            ElseIf counter = 1 Then
'                    '                getPropNo = words(1) 'Read Proposal No
'                    '            End If
'                    '            If counter > 1 Then
'                    '                count = count + CInt(word)
'                    '            End If
'                    '        End If
'                    '        counter = counter + 1
'                    '    Next
'                    '    sr.Close()
'                    'End Using

'                    'Get file exist in folder
'                    Dim countFile As Integer
'                    For Each cntFile In System.IO.Directory.GetFiles(strDir & strFolder)
'                        If cntFile.ToString.ToLower.Contains("enc") Then
'                            'Do nothing
'                        Else
'                            countFile = countFile + 1
'                        End If
'                        'countFile = countFile + 1
'                    Next cntFile
'                    'Minus complete.xml file
'                    countFile = countFile - 1

'                    'Compare file exist in folder with PropNo_complete.xml
'                    'If (countFile = count) Then
'                    If (countFile = totalFile) Then

'                        'If IsExist(getAgentID) Then
'                        If IsExist(agentID) Then

'                            Dim objDBCom As New MySQLDBComponent.MySQLDBComponent(POSWeb.POSWeb_SQLConn)
'                            Dim dt As New DataTable
'                            Dim sqlS As String = ""

'                            'Insert in Submission_Trans table if success
'                            objDBCom.AddParameter("@AgentCode", SqlDbType.NVarChar, agentID)
'                            objDBCom.AddParameter("@ProposalNo", SqlDbType.NVarChar, strFolder.Replace("\", ""))
'                            objDBCom.AddParameter("@FileCount", SqlDbType.NVarChar, countFile)
'                            objDBCom.AddParameter("@Status", SqlDbType.NVarChar, "Submitted") 'update by workflow: Failed, Submitted, Received 
'                            'date submitted insert by stored proc
'                            'message 'update by workflow
'                            'policy no 'add by workflow

'                            objDBCom.ExecuteProcedure(dt, "SP_WS_Agent_SaveDocument")
'                            objDBCom.Dispose()

'                            'If ReadPR(strDir, strFolder, getPropNo & "_PR.xml") Then
'                            If ReadPR(strDir, strFolder, strFolder.Replace("\", "") & "_PR.xml") Then
'                                strStatus = "Documents successfully submitted"
'                            Else
'                                strStatus = "Incomplete documents process"
'                            End If

'                        Else
'                            strStatus = "Agent " & agentID & " does not exist"
'                        End If 'End IsExist()
'                    Else 'Remove file if not completed
'                        If System.IO.Directory.GetFiles(strDir & strFolder).Length <> 0 Then
'                            For Each sFile In System.IO.Directory.GetFiles(strDir & strFolder)
'                                System.IO.File.Delete(sFile)
'                            Next sFile
'                            strStatus = "Incomplete documents submission"
'                        End If
'                    End If 'End (countFile = count)

'                End If 'End folder not exist
'            End If 'End complete.xml
'            Return strStatus
'        Catch ex As Exception
'            Throw ex
'        End Try
'    End Function

'    'Public Function SaveDocument(ByVal strBinary As String, ByVal strDocName As String, ByVal strFolder As String, ByVal strSource As String) As String

'    '    Dim strStatus As String
'    '    Dim strProposal As String
'    '    Dim strDir As String = "D:\Files\"
'    '    'If strDocName.Contains(".enc") Then
'    '    'Else
'    '    'strDocName = strDocName & ".enc"
'    '    'End If
'    '    strFolder = strDocName.Substring(0, strDocName.IndexOf("_")) & "\" 'get Proposal number from strDocName
'    '    strProposal = strDocName.Substring(0, strDocName.IndexOf("_")) & "|"
'    '    strSource = "IAPP (S)"

'    '    ' Store transferred file in location folder
'    '    Dim bw As BinaryWriter
'    '    Dim sIn() As Byte = Convert.FromBase64String(strBinary)

'    '    If (Not System.IO.Directory.Exists(strDir & strFolder)) Then
'    '        System.IO.Directory.CreateDirectory(strDir & strFolder)
'    '    End If

'    '    bw = New BinaryWriter(File.Create(strDir & strFolder & strDocName)) 'Create the ecrypted file in local server location 
'    '    bw.Write(sIn)
'    '    bw.Close()

'    '    '*****************************************************************************
'    '    'Modified by jACOB
'    '    If DecryptFile(strDocName, strFolder, strSource, strDir) = True Then
'    '        strStatus = strProposal + "Document Successfully Received"
'    '        'strStatus = "Success Decrypt & Transfer"
'    '    Else
'    '        strStatus = "Unsuccessful Decrypt & Transfer"
'    '    End If
'    '    '*****************************************************************************

'    '    If strDocName.ToLower.Contains("complete.xml") Then
'    '        If (Not System.IO.Directory.Exists(strDir & strFolder & strDocName)) Then

'    '            Dim count, counter As Integer
'    '            Dim getPropNo As String = String.Empty
'    '            Dim getAgentID As String = String.Empty
'    '            Dim strChgExt As String = Replace(strDocName, ".enc", "")

'    '            'Get file number from complete.xml
'    '            Using sr As StreamReader = New StreamReader(strDir & strFolder & strChgExt)
'    '                Dim line As String
'    '                ' Read and display lines from the file until the end of the file is reached. 
'    '                line = sr.ReadLine()
'    '                Dim words As String() = line.Split(New Char() {";"c})

'    '                ' Use For Each loop over words and display them.
'    '                For Each word As String In words
'    '                    If word.Length <> 0 Then 'Filter empty string
'    '                        If counter = 0 Then
'    '                            getAgentID = words(0) 'Read Agent ID
'    '                        ElseIf counter = 1 Then
'    '                            getPropNo = words(1) 'Read Proposal No
'    '                        End If
'    '                        If counter > 1 Then
'    '                            count = count + CInt(word)
'    '                        End If
'    '                    End If
'    '                    counter = counter + 1
'    '                Next
'    '                sr.Close()
'    '            End Using

'    '            'Get file exist in folder
'    '            Dim countFile As Integer
'    '            For Each cntFile In System.IO.Directory.GetFiles(strDir & strFolder)
'    '                If cntFile.ToString.ToLower.Contains("xml") Then
'    '                    'Do nothing
'    '                Else
'    '                    countFile = countFile + 1
'    '                End If
'    '            Next cntFile

'    '            'Compare file exist in folder with PropNo_complete.xml
'    '            If (countFile = count) Then

'    '                If IsExist(getAgentID) Then

'    '                    Dim objDBCom As New MySQLDBComponent.MySQLDBComponent(POSWeb.POSWeb_SQLConn)
'    '                    Dim dt As New DataTable
'    '                    Dim sqlS As String = ""

'    '                    'Insert in Submission_Trans table if success
'    '                    objDBCom.AddParameter("@AgentCode", SqlDbType.NVarChar, getAgentID)
'    '                    objDBCom.AddParameter("@ProposalNo", SqlDbType.NVarChar, getPropNo)
'    '                    objDBCom.AddParameter("@FileCount", SqlDbType.NVarChar, countFile)
'    '                    objDBCom.AddParameter("@Status", SqlDbType.NVarChar, "Submitted") 'update by workflow: Failed, Submitted, Received 
'    '                    'date submitted insert by stored proc
'    '                    'message 'update by workflow
'    '                    'policy no 'add by workflow

'    '                    objDBCom.ExecuteProcedure(dt, "SP_WS_Agent_SaveDocument")
'    '                    objDBCom.Dispose()

'    '                    If ReadPR(strDir, strFolder, getPropNo & "_PR.xml") Then
'    '                        strStatus = "Documents successfully submitted"
'    '                    Else
'    '                        strStatus = "Incomplete documents process"
'    '                    End If

'    '                Else
'    '                    strStatus = "Agent " & getAgentID & " does not exist"
'    '                End If 'End IsExist()
'    '            Else 'Remove file if not completed
'    '                If System.IO.Directory.GetFiles(strDir & strFolder).Length <> 0 Then
'    '                    For Each sFile In System.IO.Directory.GetFiles(strDir & strFolder)
'    '                        System.IO.File.Delete(sFile)
'    '                    Next sFile
'    '                    strStatus = "Incomplete documents submission"
'    '                End If
'    '            End If 'End (countFile = count)

'    '        End If 'End folder not exist
'    '    End If 'End complete.xml
'    '    'strStatus = "Successfully upload file"
'    '    Return strStatus

'    'End Function

'    '***************************************
'    'Function: RefreshSubmission
'    'Author: Faiz
'    'Date: 8/9/2015
'    'Summary: 'Check in xml content between 
'    'policy no and status either it is accepted or rejected
'    '***************************************
'    Public Function RefreshSubmission(ByRef strPolNo) As String

'        Dim strStatus As String = String.Empty
'        Dim strMessage As String = String.Empty
'        Dim objDBCom As New MySQLDBComponent.MySQLDBComponent(POSWeb.POSWeb_SQLConn)
'        Dim dt As New DataTable

'        objDBCom.AddParameter("@PolicyNo", SqlDbType.NVarChar, strPolNo)

'        objDBCom.ExecuteProcedure(dt, "SP_WS_Agent_RefreshSubmission")
'        objDBCom.Dispose()

'        If dt.Rows.Count > 0 Then
'            strStatus = IIf(dt.Rows(0).Item("Status") Is DBNull.Value, 0, dt.Rows(0).Item("Status"))
'            strMessage = IIf(dt.Rows(0).Item("Message") Is DBNull.Value, 0, dt.Rows(0).Item("Message"))
'        End If

'        dt.Dispose()

'        Return "Status: " & strStatus & " Message: " & strMessage

'    End Function

'    '***************************************
'    'Function: RetrievePolicyNumber
'    'Author: Ain
'    'Date: 3/11/2015
'    'Summary: 'Check in xml content between 
'    'Return policy Number
'    '***************************************
'    <WebMethod()> _
'    Public Function RetrievePolicyNumber(ByVal agentCode As String, ByVal strPolNo As String) As String
'        Dim PolicyNo As String = String.Empty
'        'Dim objDBCom As New MySQLDBComponent.MySQLDBComponent(POSWeb.POSWeb_SQLConn)
'        'Dim dt As New DataTable
'        Dim result As Boolean = False
'        Dim result1 As String = ""
'        Dim reply As String = ""
'        Dim numPolicy As String = ""
'        Dim yearNow As String = DateTime.Now.ToString("yyyy")
'        Dim script As String = ""

'        Try
'            'Check if exits
'            result = POSWeb.exitsProposalNo(agentCode, strPolNo)
'            If result = True Then
'                'Check if policy exits
'                result1 = POSWeb.getValProposalNo(agentCode, strPolNo)
'                If result1 = "" Then
'                    'Generate policyNo
'                    numPolicy = "TL" + yearNow + POSWeb.policyNumberVal()
'                    'Change data table 'Submission_Trans'
'                    script = "update Submission_Trans set Status='Received',PolicyNo='" + numPolicy + "' where AgentCode='" + agentCode + "' and ProposalNo='" + strPolNo + "'"
'                    POSWeb.dataRun(script)
'                    reply = "Received|" + numPolicy + "|Success"
'                Else
'                    reply = "Received|" + result1 + "|Success"
'                End If
'            Else
'                reply = "Failed||Data Not Exits."
'            End If

'            'objDBCom.AddParameter("@ProposalNo", SqlDbType.NVarChar, strPolNo)
'            'objDBCom.ExecuteProcedure(dt, "SP_WS_Agent_RetrievePolicyNo")
'            'objDBCom.Dispose()
'            'If dt.Rows.Count > 0 Then
'            '    PolicyNo = IIf(dt.Rows(0).Item("PolicyNo") Is DBNull.Value, 0, dt.Rows(0).Item("PolicyNo"))
'            'End If
'            'dt.Dispose()
'        Catch ex As Exception
'            Throw ex
'        End Try
'        Return reply
'    End Function

'    '***************************************
'    'Function: IsExist
'    'Author: Faiz
'    'Date: 9/9/2015
'    'Summary: To search based on AgentCode
'    '***************************************
'    Private Function IsExist(ByVal strAgentCode) As Boolean

'        Dim objDBCom As New MySQLDBComponent.MySQLDBComponent(POSWeb.POSWeb_SQLConn)
'        Dim dt As New DataTable
'        Dim sqlS As String = ""
'        Dim isFound As Boolean = False

'        objDBCom.AddParameter("@AgentCode", SqlDbType.NVarChar, strAgentCode)

'        objDBCom.ExecuteProcedure(dt, "SP_WS_Agent_IsExist")
'        objDBCom.Dispose()

'        If dt.Rows.Count > 0 Then
'            isFound = True
'        End If

'        dt.Dispose()

'        Return isFound

'    End Function

'    '***************************************
'    'Function: ReadPR
'    'Author: Faiz
'    'Date: 10/9/2015
'    'Summary: To insert PR.xml data in database
'    '***************************************
'    Private Function ReadPR(ByVal strDir As String, ByVal strFolder As String, ByVal strDocName As String) As Boolean
'        Dim boolStatus As Boolean = False

'        boolStatus = Write_ProspectInfo(strDir, strFolder, strDocName)
'        Return boolStatus

'    End Function

'    '***************************************
'    'Function: Write_ProspectInfo
'    'Author: Faiz
'    'Date: 10/9/2015
'    'Summary: To insert PR.xml data in eProposal_LA_Details table
'    '***************************************
'    Private Function Write_ProspectInfo(ByVal strDir As String, ByVal strFolder As String, ByVal strDocName As String) As Boolean
'        Dim isSuccess As Boolean = False
'        Dim xmldoc As New XmlDataDocument()
'        Dim xmlnodeParty As XmlNodeList
'        Dim xmlnodeAddress As XmlNodeList
'        Dim xmlnodeContact As XmlNodeList
'        Dim i As Integer

'        Try
'            Dim fs As New FileStream(strDir & strFolder & strDocName, FileMode.Open, FileAccess.Read)
'            xmldoc.Load(fs)

'            xmlnodeParty = xmldoc.GetElementsByTagName("Party")
'            For i = 0 To xmlnodeParty.Count - 1
'                Dim objDBCom As New MySQLDBComponent.MySQLDBComponent(POSWeb.POSWeb_SQLConn)
'                Dim dt As New DataTable
'                Dim sqlS As String = ""

'                'ID auto increment by SQL
'                'EAPPID

'                objDBCom.AddParameter("@eProposalNo", SqlDbType.Text, strDocName.Substring(0, strDocName.IndexOf("_"))) 'eProposalNo
'                objDBCom.AddParameter("@PTypeCode", SqlDbType.Text, xmlnodeParty(i).ChildNodes.Item(0).InnerText.Trim()) 'PTypeCode
'                objDBCom.AddParameter("@LATitle", SqlDbType.Text, xmlnodeParty(i).ChildNodes.Item(4).InnerText.Trim()) 'LATitle
'                objDBCom.AddParameter("@LAName", SqlDbType.Text, xmlnodeParty(i).ChildNodes.Item(5).InnerText.Trim()) 'LAName
'                objDBCom.AddParameter("@LASex", SqlDbType.Text, xmlnodeParty(i).ChildNodes.Item(6).InnerText.Trim()) 'LASex
'                objDBCom.AddParameter("@LADOB", SqlDbType.Text, xmlnodeParty(i).ChildNodes.Item(7).InnerText.Trim()) 'LADOB
'                objDBCom.AddParameter("@LANewICNo", SqlDbType.Text, xmlnodeParty(i).ChildNodes.Item(24).ChildNodes.Item(1).InnerText.Trim()) 'LANewICNo
'                objDBCom.AddParameter("@LAOtherIDType", SqlDbType.Text, xmlnodeParty(i).ChildNodes.Item(25).ChildNodes.Item(0).InnerText.Trim()) 'LAOtherIDType
'                objDBCom.AddParameter("@LAOtherID", SqlDbType.Text, xmlnodeParty(i).ChildNodes.Item(25).ChildNodes.Item(1).InnerText.Trim()) 'LAOtherID
'                objDBCom.AddParameter("@LAMaritalStatus", SqlDbType.Text, xmlnodeParty(i).ChildNodes.Item(10).InnerText.Trim()) 'LAMaritalStatus
'                objDBCom.AddParameter("@LARace", SqlDbType.Text, xmlnodeParty(i).ChildNodes.Item(11).InnerText.Trim()) 'LARace
'                objDBCom.AddParameter("@LAReligion", SqlDbType.Text, xmlnodeParty(i).ChildNodes.Item(12).InnerText.Trim()) 'LAReligion
'                objDBCom.AddParameter("@LANationality", SqlDbType.Text, xmlnodeParty(i).ChildNodes.Item(13).InnerText.Trim()) 'LANationality
'                objDBCom.AddParameter("@LAOccupationCode", SqlDbType.Text, xmlnodeParty(i).ChildNodes.Item(14).InnerText.Trim()) 'LAOccupationCode
'                objDBCom.AddParameter("@LAExactDuties", SqlDbType.Text, xmlnodeParty(i).ChildNodes.Item(15).InnerText.Trim()) 'LAExactDuties
'                objDBCom.AddParameter("@LATypeOfBusiness", SqlDbType.Text, xmlnodeParty(i).ChildNodes.Item(16).InnerText.Trim()) 'LATypeOfBusiness
'                objDBCom.AddParameter("@LAEmployerName", SqlDbType.Text, xmlnodeParty(i).ChildNodes.Item(17).InnerText.Trim()) 'LAEmployerName
'                objDBCom.AddParameter("@LAYearlyIncome", SqlDbType.Text, xmlnodeParty(i).ChildNodes.Item(18).InnerText.Trim()) 'LAYearlyIncome
'                objDBCom.AddParameter("@LARelationship", SqlDbType.Text, xmlnodeParty(i).ChildNodes.Item(19).InnerText.Trim()) 'LARelationship

'                'POFlag
'                If xmlnodeParty(i).ChildNodes.Item(0).InnerText.Trim() = "PO" Then
'                    objDBCom.AddParameter("@POFlag", SqlDbType.Text, "Y")
'                Else
'                    objDBCom.AddParameter("@POFlag", SqlDbType.Text, "N")
'                End If

'                objDBCom.AddParameter("@CorrespondenceAddress", SqlDbType.Text, xmlnodeParty(i).ChildNodes.Item(22).InnerText.Trim()) 'CorrespondenceAddress
'                objDBCom.AddParameter("@ResidenceOwnRented", SqlDbType.Text, xmlnodeParty(i).ChildNodes.Item(21).InnerText.Trim()) 'ResidenceOwnRented

'                xmlnodeAddress = xmldoc.GetElementsByTagName("Address")

'                objDBCom.AddParameter("@ResidenceAddress1", SqlDbType.Text, xmlnodeAddress(0).ChildNodes.Item(1).InnerText.Trim()) 'ResidenceAddress1
'                objDBCom.AddParameter("@ResidenceAddress2", SqlDbType.Text, xmlnodeAddress(0).ChildNodes.Item(2).InnerText.Trim()) 'ResidenceAddress2
'                objDBCom.AddParameter("@ResidenceAddress3", SqlDbType.Text, xmlnodeAddress(0).ChildNodes.Item(3).InnerText.Trim()) 'ResidenceAddress3
'                objDBCom.AddParameter("@ResidenceTown", SqlDbType.Text, xmlnodeAddress(0).ChildNodes.Item(4).InnerText.Trim()) 'ResidenceTown
'                objDBCom.AddParameter("@ResidenceState", SqlDbType.Text, xmlnodeAddress(0).ChildNodes.Item(5).InnerText.Trim()) 'ResidenceState
'                objDBCom.AddParameter("@ResidencePostcode", SqlDbType.Text, xmlnodeAddress(0).ChildNodes.Item(6).InnerText.Trim()) 'ResidencePostcode
'                objDBCom.AddParameter("@ResidenceCountry", SqlDbType.Text, xmlnodeAddress(0).ChildNodes.Item(7).InnerText.Trim()) 'ResidenceCountry

'                objDBCom.AddParameter("@OfficeAddress1", SqlDbType.Text, xmlnodeAddress(1).ChildNodes.Item(1).InnerText.Trim()) 'OfficeAddress1
'                objDBCom.AddParameter("@OfficeAddress2", SqlDbType.Text, xmlnodeAddress(1).ChildNodes.Item(2).InnerText.Trim()) 'OfficeAddress2
'                objDBCom.AddParameter("@OfficeAddress3", SqlDbType.Text, xmlnodeAddress(1).ChildNodes.Item(3).InnerText.Trim()) 'OfficeAddress3
'                objDBCom.AddParameter("@OfficeTown", SqlDbType.Text, xmlnodeAddress(1).ChildNodes.Item(4).InnerText.Trim()) 'OfficeTown
'                objDBCom.AddParameter("@OfficeState", SqlDbType.Text, xmlnodeAddress(1).ChildNodes.Item(5).InnerText.Trim()) 'OfficeSTate
'                objDBCom.AddParameter("@OfficePostcode", SqlDbType.Text, xmlnodeAddress(1).ChildNodes.Item(6).InnerText.Trim()) 'OfficePostcode
'                objDBCom.AddParameter("@OfficeCountry", SqlDbType.Text, xmlnodeAddress(1).ChildNodes.Item(7).InnerText.Trim()) 'OfficeCountry

'                xmlnodeContact = xmldoc.GetElementsByTagName("Contact")

'                objDBCom.AddParameter("@ResidencePhoneNo", SqlDbType.Text, xmlnodeContact(0).ChildNodes.Item(1).InnerText.Trim()) 'ResidencePhoneNo
'                objDBCom.AddParameter("@OfficePhoneNo", SqlDbType.Text, xmlnodeContact(1).ChildNodes.Item(1).InnerText.Trim()) 'OfficePhoneNo
'                objDBCom.AddParameter("@FaxPhoneNo", SqlDbType.Text, xmlnodeContact(4).ChildNodes.Item(1).InnerText.Trim()) 'FaxPhoneNo
'                objDBCom.AddParameter("@MobilePhoneNo", SqlDbType.Text, xmlnodeContact(2).ChildNodes.Item(1).InnerText.Trim()) 'MobilePhoneNo
'                objDBCom.AddParameter("@EmailAddress", SqlDbType.Text, xmlnodeContact(3).ChildNodes.Item(1).InnerText.Trim()) 'EmailAddress

'                objDBCom.AddParameter("@PentalHealthStatus", SqlDbType.Text, xmlnodeParty(i).ChildNodes.Item(29).ChildNodes.Item(0).ChildNodes.Item(1).InnerText.Trim()) 'PentalHealthStatus
'                objDBCom.AddParameter("@PentalFemaleStatus", SqlDbType.Text, xmlnodeParty(i).ChildNodes.Item(29).ChildNodes.Item(1).ChildNodes.Item(1).InnerText.Trim()) 'PentalFemaleStatus
'                objDBCom.AddParameter("@PentalDeclarationStatus", SqlDbType.Text, xmlnodeParty(i).ChildNodes.Item(29).ChildNodes.Item(2).ChildNodes.Item(1).InnerText.Trim()) 'PentalDeclarationStatus

'                'LACompleteFlag
'                'AddPO
'                'CreatedAt
'                'UpdatedAt
'                'LASmoker
'                objDBCom.AddParameter("@ResidenceForeignAddressFlag", SqlDbType.Text, xmlnodeAddress(0).ChildNodes.Item(8).InnerText.Trim()) 'ResidenceForeignAddressFlag
'                objDBCom.AddParameter("@OfficeForeignAddressFlag", SqlDbType.Text, xmlnodeAddress(1).ChildNodes.Item(8).InnerText.Trim()) 'OfficeForeignAddressFlag
'                'POType
'                'MobilePhoneNoPrefix
'                'ResidencePhoneNoPrefix
'                'OfficePhoneNoPrefix
'                'FaxPhoneNoPrefix

'                objDBCom.AddParameter("@GST_Registered", SqlDbType.Text, xmlnodeParty(i).ChildNodes.Item(30).ChildNodes.Item(0).InnerText.Trim()) 'GST_Registered
'                objDBCom.AddParameter("@GST_RegistrationNo", SqlDbType.Text, xmlnodeParty(i).ChildNodes.Item(30).ChildNodes.Item(1).InnerText.Trim()) 'GST_RegistrationNo
'                objDBCom.AddParameter("@GST_RegistrationDate", SqlDbType.Text, xmlnodeParty(i).ChildNodes.Item(30).ChildNodes.Item(2).InnerText.Trim()) 'GST_RegistrationDate
'                objDBCom.AddParameter("@GST_Exampted", SqlDbType.Text, xmlnodeParty(i).ChildNodes.Item(30).ChildNodes.Item(3).InnerText.Trim()) 'GST_Exampted

'                'ProspectProfileChangesCounter
'                'ProspectProfileID

'                objDBCom.AddParameter("@HaveChildren", SqlDbType.NVarChar, xmlnodeParty(i).ChildNodes.Item(20).InnerText.Trim()) 'HaveChildren
'                objDBCom.AddParameter("@LABirthCountry", SqlDbType.NVarChar, xmlnodeParty(i).ChildNodes.Item(8).InnerText.Trim()) 'LABirthCountry
'                objDBCom.AddParameter("@MalaysianWithPOBox", SqlDbType.NVarChar, xmlnodeParty(i).ChildNodes.Item(23).InnerText.Trim()) 'MalaysianWithPOBox

'                objDBCom.AddParameter("@Residence_POBox", SqlDbType.Text, xmlnodeAddress(0).ChildNodes.Item(9).InnerText.Trim()) 'Residence_POBox
'                objDBCom.AddParameter("@Office_POBox", SqlDbType.Text, xmlnodeAddress(1).ChildNodes.Item(9).InnerText.Trim()) 'Office_POBox


'                If objDBCom.ExecuteProcedure(dt, "SP_WS_Agent_Write_ProspectInfo") Then
'                    isSuccess = True

'                End If
'                objDBCom.Dispose()
'            Next
'        Catch ex As Exception
'            Throw ex
'        End Try

'        Return isSuccess
'    End Function


'    Private Function Write_ProspectInfo2(ByVal strDir As String, ByVal strFolder As String, ByVal strDocName As String) As Boolean
'        Dim isSuccess As Boolean = False
'        Dim xmldoc As New XmlDataDocument()

'        Dim xmlnodeParty As XmlNodeList
'        Dim xmlnodeAddress As XmlNodeList
'        Dim xmlnodeContact As XmlNodeList
'        Dim xmlnodePental As XmlNodeList
'        Dim xmlnodeGST As XmlNodeList

'        Dim xmlParty As XmlElement
'        Dim xmlAddress As XmlElement
'        Dim xmlContact As XmlElement
'        Dim xmlPental As XmlElement
'        Dim xmlGST As XmlElement

'        xmlnodeParty = xmldoc.SelectNodes("/eApps/AssuredInfo/Party")
'        xmlnodeAddress = xmldoc.SelectNodes("/eApps/AssuredInfo/Party/Addresses/Address")
'        xmlnodeContact = xmldoc.SelectNodes("/eApps/AssuredInfo/Party/Contacts/Contact")
'        xmlnodePental = xmldoc.SelectNodes("/eApps/AssuredInfo/Party/PentalHealthDetails")
'        xmlnodeGST = xmldoc.SelectNodes("/eApps/AssuredInfo/Party/LAGST")

'        Dim i As Integer
'        Dim fs As New FileStream(strDir & strFolder & strDocName, FileMode.Open, FileAccess.Read)
'        xmldoc.Load(fs)
'        Dim str As String


'        'For i = 0 To xmlnodeParty.Count - 1
'        For Each xmlParty In xmlnodeParty
'            Dim objDBCom As New MySQLDBComponent.MySQLDBComponent(POSWeb.POSWeb_SQLConn)
'            Dim dt As New DataTable
'            Dim sqlS As String = ""

'            'ID auto increment by SQL
'            'EAPPID
'            ' str = xmlAddress.Attributes(""

'            objDBCom.AddParameter("@eProposalNo", SqlDbType.Text, strDocName.Substring(0, strDocName.IndexOf("_"))) 'eProposalNo
'            objDBCom.AddParameter("@PTypeCode", SqlDbType.Text, xmlParty.Item("PTypeCode").InnerText.Trim()) 'PTypeCode
'            objDBCom.AddParameter("@LATitle", SqlDbType.Text, xmlParty.Item("LATitle").InnerText.Trim()) 'LATitle
'            objDBCom.AddParameter("@LAName", SqlDbType.Text, xmlParty.Item("LAName").InnerText.Trim()) 'LAName
'            objDBCom.AddParameter("@LASex", SqlDbType.Text, xmlParty.Item("LASex").InnerText.Trim()) 'LASex
'            objDBCom.AddParameter("@LADOB", SqlDbType.Text, xmlParty.Item("LADOB").InnerText.Trim()) 'LADOB
'            objDBCom.AddParameter("@LANewICNo", SqlDbType.Text, xmlParty.Item("LANewIC").ChildNodes.Item(1).InnerText.Trim()) 'LANewICNo
'            objDBCom.AddParameter("@LAOtherIDType", SqlDbType.Text, xmlParty.Item("LAOtherID").ChildNodes.Item(0).InnerText.Trim()) 'LAOtherIDType
'            objDBCom.AddParameter("@LAOtherID", SqlDbType.Text, xmlParty.Item("LAOtherID").ChildNodes.Item(1).InnerText.Trim()) 'LAOtherID
'            objDBCom.AddParameter("@LAMaritalStatus", SqlDbType.Text, xmlParty.Item("LAMaritalStatus").InnerText.Trim()) 'LAMaritalStatus
'            objDBCom.AddParameter("@LARace", SqlDbType.Text, xmlParty.Item("LARace").InnerText.Trim()) 'LARace
'            objDBCom.AddParameter("@LAReligion", SqlDbType.Text, xmlParty.Item("LAReligion").InnerText.Trim()) 'LAReligion
'            objDBCom.AddParameter("@LANationality", SqlDbType.Text, xmlParty.Item("LANationality").InnerText.Trim()) 'LANationality
'            objDBCom.AddParameter("@LAOccupationCode", SqlDbType.Text, xmlParty.Item("LAOccupationCode").InnerText.Trim()) 'LAOccupationCode
'            objDBCom.AddParameter("@LAExactDuties", SqlDbType.Text, xmlParty.Item("LAExactDuties").InnerText.Trim()) 'LAExactDuties
'            objDBCom.AddParameter("@LATypeOfBusiness", SqlDbType.Text, xmlParty.Item("LATypeOfBusiness").InnerText.Trim()) 'LATypeOfBusiness
'            objDBCom.AddParameter("@LAEmployerName", SqlDbType.Text, xmlParty.Item("LAEmployerName").InnerText.Trim()) 'LAEmployerName
'            objDBCom.AddParameter("@LAYearlyIncome", SqlDbType.Text, xmlParty.Item("LAYearlyIncome").InnerText.Trim()) 'LAYearlyIncome
'            objDBCom.AddParameter("@LARelationship", SqlDbType.Text, xmlParty.Item("LARelationship").InnerText.Trim()) 'LARelationship

'            'POFlag
'            If xmlParty.Item("PTypeCode").InnerText.Trim() = "PO" Then
'                objDBCom.AddParameter("@POFlag", SqlDbType.Text, "Y")
'            Else
'                objDBCom.AddParameter("@POFlag", SqlDbType.Text, "N")
'            End If

'            objDBCom.AddParameter("@CorrespondenceAddress", SqlDbType.Text, xmlParty.Item("CorrespondenceAddress").InnerText.Trim()) 'CorrespondenceAddress
'            objDBCom.AddParameter("@ResidenceOwnRented", SqlDbType.Text, xmlParty.Item("ResidenceOwnRented").InnerText.Trim()) 'ResidenceOwnRented

'            For Each xmlAddress In xmlnodeAddress
'                str = xmlAddress.Item("Town").InnerText.Trim()
'                If xmlAddress.Attributes("Type").Value = "Residence" Then
'                    objDBCom.AddParameter("@ResidenceAddress1", SqlDbType.Text, xmlAddress.Item("Address1").InnerText.Trim()) 'ResidenceAddress1
'                    objDBCom.AddParameter("@ResidenceAddress2", SqlDbType.Text, xmlAddress.Item("Address2").InnerText.Trim()) 'ResidenceAddress2
'                    objDBCom.AddParameter("@ResidenceAddress3", SqlDbType.Text, xmlAddress.Item("Address3").InnerText.Trim()) 'ResidenceAddress3
'                    objDBCom.AddParameter("@ResidenceTown", SqlDbType.Text, xmlAddress.Item("Town").InnerText.Trim()) 'ResidenceTown
'                    objDBCom.AddParameter("@ResidenceState", SqlDbType.Text, xmlAddress.Item("State").InnerText.Trim()) 'ResidenceState
'                    objDBCom.AddParameter("@ResidencePostcode", SqlDbType.Text, xmlAddress.Item("Postcode").InnerText.Trim()) 'ResidencePostcode
'                    objDBCom.AddParameter("@ResidenceCountry", SqlDbType.Text, xmlAddress.Item("Country").InnerText.Trim()) 'ResidenceCountry
'                    objDBCom.AddParameter("@ResidenceForeignAddressFlag", SqlDbType.Text, xmlAddress.Item("ForeignAddress").InnerText.Trim()) 'ResidenceForeignAddressFlag
'                    Dim keyNodelst As XmlNodeList = xmlAddress.SelectNodes(".//POBoxFlag")
'                    If (keyNodelst.Count > 0) Then
'                        objDBCom.AddParameter("@Residence_POBox", SqlDbType.Text, xmlAddress.Item("POBoxFlag").InnerText.Trim()) 'Residence_POBox
'                    Else
'                        objDBCom.AddParameter("@Residence_POBox", SqlDbType.Text, "") 'Residence_POBox
'                    End If
'                ElseIf xmlAddress.Attributes("Type").Value = "Office" Then
'                    objDBCom.AddParameter("@OfficeAddress1", SqlDbType.Text, xmlAddress.Item("Address1").InnerText.Trim()) 'OfficeAddress1
'                    objDBCom.AddParameter("@OfficeAddress2", SqlDbType.Text, xmlAddress.Item("Address2").InnerText.Trim()) 'OfficeAddress2
'                    objDBCom.AddParameter("@OfficeAddress3", SqlDbType.Text, xmlAddress.Item("Address3").InnerText.Trim()) 'OfficeAddress3
'                    objDBCom.AddParameter("@OfficeTown", SqlDbType.Text, xmlAddress.Item("Town").InnerText.Trim()) 'OfficeTown
'                    objDBCom.AddParameter("@OfficeState", SqlDbType.Text, xmlAddress.Item("State").InnerText.Trim()) 'OfficeSTate
'                    objDBCom.AddParameter("@OfficePostcode", SqlDbType.Text, xmlAddress.Item("Postcode").InnerText.Trim()) 'OfficePostcode
'                    objDBCom.AddParameter("@OfficeCountry", SqlDbType.Text, xmlAddress.Item("Country").InnerText.Trim()) 'OfficeCountry
'                    objDBCom.AddParameter("@OfficeForeignAddressFlag", SqlDbType.Text, xmlAddress.Item("ForeignAddress").InnerText.Trim()) 'OfficeForeignAddressFlag
'                    Dim keyNodelst As XmlNodeList = xmlAddress.SelectNodes(".//POBoxFlag")
'                    If (keyNodelst.Count > 0) Then
'                        objDBCom.AddParameter("@Office_POBox", SqlDbType.Text, xmlAddress.Item("POBoxFlag").InnerText.Trim()) 'Office_POBox
'                    Else
'                        objDBCom.AddParameter("@Office_POBox", SqlDbType.Text, "") 'Office_POBox
'                    End If
'                End If
'            Next

'            For Each xmlContact In xmlnodeContact
'                If xmlContact.Attributes("Type").Value = "Residence" Then
'                    objDBCom.AddParameter("@ResidencePhoneNo", SqlDbType.Text, xmlContact.Item("ContactNo").InnerText.Trim()) 'ResidencePhoneNo
'                ElseIf xmlContact.Attributes("Type").Value = "Office" Then
'                    objDBCom.AddParameter("@OfficePhoneNo", SqlDbType.Text, xmlContact.Item("ContactNo").InnerText.Trim()) 'OfficePhoneNo
'                ElseIf xmlContact.Attributes("Type").Value = "Mobile" Then
'                    objDBCom.AddParameter("@MobilePhoneNo", SqlDbType.Text, xmlContact.Item("ContactNo").InnerText.Trim()) 'MobilePhoneNo
'                ElseIf xmlContact.Attributes("Type").Value = "Email" Then
'                    objDBCom.AddParameter("@EmailAddress", SqlDbType.Text, xmlContact.Item("ContactNo").InnerText.Trim()) 'EmailAddress
'                ElseIf xmlContact.Attributes("Type").Value = "Fax" Then
'                    objDBCom.AddParameter("@FaxPhoneNo", SqlDbType.Text, xmlContact.Item("ContactNo").InnerText.Trim()) 'FaxPhoneNo
'                End If
'            Next

'            Dim j As Integer
'            Dim strCode, strStatus As String
'            For j = 0 To xmlnodePental.Count - 1
'                strCode = xmlnodePental.Item(j).ChildNodes.Item(j).InnerText.Trim()
'                strStatus = xmlnodePental.Item(j).ChildNodes.Item(j).InnerText.Trim()
'                If strCode.Replace("False", "") = "MDTAUW01" Or strCode.Replace("True", "") = "MDTAUW01" Then
'                    objDBCom.AddParameter("@PentalHealthStatus", SqlDbType.Text, strStatus.Replace("MDTAUW01", "")) 'PentalHealthStatus
'                ElseIf strCode.Replace("False", "") = "MDTAUW02" Or strCode.Replace("True", "") = "MDTAUW02" Then
'                    objDBCom.AddParameter("@PentalHealthStatus", SqlDbType.Text, strStatus.Replace("MDTAUW01", ""))  'PentalFemaleStatus
'                ElseIf strCode.Replace("False", "") = "MDTAUW03" Or strCode.Replace("True", "") = "MDTAUW03" Then
'                    objDBCom.AddParameter("@PentalHealthStatus", SqlDbType.Text, strStatus.Replace("MDTAUW01", ""))  'PentalDeclarationStatus
'                End If
'            Next

'            'LACompleteFlag
'            'AddPO
'            'CreatedAt
'            'UpdatedAt
'            'LASmoker
'            'POType
'            'MobilePhoneNoPrefix
'            'ResidencePhoneNoPrefix
'            'OfficePhoneNoPrefix
'            'FaxPhoneNoPrefix

'            'If xmlnodeGST.Count > 1 Then
'            '    For Each xmlGST In xmlnodeGST
'            '        objDBCom.AddParameter("@GST_Registered", SqlDbType.Text, xmlGST.Item("GSTRegPerson").InnerText.Trim()) 'GST_Registered
'            '        objDBCom.AddParameter("@GST_RegistrationNo", SqlDbType.Text, xmlGST.Item("GSTRegNo").InnerText.Trim()) 'GST_RegistrationNo
'            '        objDBCom.AddParameter("@GST_RegistrationDate", SqlDbType.Text, xmlGST.Item("GSTRegDate").InnerText.Trim()) 'GST_RegistrationDate
'            '        objDBCom.AddParameter("@GST_Exampted", SqlDbType.Text, xmlGST.Item("GSTExempted").InnerText.Trim()) 'GST_Exampted
'            '    Next
'            'Else
'            '    objDBCom.AddParameter("@GST_Registered", SqlDbType.Text, "") 'GST_Registered
'            '    objDBCom.AddParameter("@GST_RegistrationNo", SqlDbType.Text, "") 'GST_RegistrationNo
'            '    objDBCom.AddParameter("@GST_RegistrationDate", SqlDbType.Text, "") 'GST_RegistrationDate
'            '    objDBCom.AddParameter("@GST_Exampted", SqlDbType.Text, "") 'GST_Exampted
'            'End If

'            objDBCom.AddParameter("@GST_Registered", SqlDbType.Text, xmlParty.Item("LAGST").ChildNodes.Item(0).ChildNodes.Item(0).InnerText.Trim) 'GST_Registered
'            objDBCom.AddParameter("@GST_RegistrationNo", SqlDbType.Text, xmlParty.Item("LAGST").ChildNodes.Item(1).ChildNodes.Item(0).InnerText.Trim) 'GST_RegistrationNo
'            objDBCom.AddParameter("@GST_RegistrationDate", SqlDbType.Text, xmlParty.Item("LAGST").ChildNodes.Item(2).ChildNodes.Item(0).InnerText.Trim) 'GST_RegistrationDate
'            objDBCom.AddParameter("@GST_Exampted", SqlDbType.Text, xmlParty.Item("LAGST").ChildNodes.Item(3).ChildNodes.Item(0).InnerText.Trim) 'GST_Exampted

'            'ProspectProfileChangesCounter
'            'ProspectProfileID

'            objDBCom.AddParameter("@HaveChildren", SqlDbType.NVarChar, xmlParty.Item("ChildFlag").InnerText.Trim()) 'HaveChildren
'            objDBCom.AddParameter("@LABirthCountry", SqlDbType.NVarChar, xmlParty.Item("LABirthCountry").InnerText.Trim()) 'LABirthCountry
'            objDBCom.AddParameter("@MalaysianWithPOBox", SqlDbType.NVarChar, xmlParty.Item("MalaysianWithPOBox").InnerText.Trim()) 'MalaysianWithPOBox

'            If objDBCom.ExecuteProcedure(dt, "SP_WS_Agent_Write_ProspectInfo") Then
'                isSuccess = True

'            End If
'            objDBCom.Dispose()
'        Next
'        Return isSuccess
'    End Function

'    '***************************************
'    'Function: DecryptFile
'    'Author: Faiz
'    'Date: 27/8/2015
'    'Summary: To decrypt binary string file from .enc to original extension
'    '***************************************
'    Public Function DecryptFile(ByVal strDocName As String, ByVal strFolder As String, ByVal strSource As String, ByVal strDir As String) As Boolean

'        Dim crypt As New Chilkat.Crypt2()

'        Dim success As Boolean
'        success = crypt.UnlockComponent("Anything for 30-day trial")
'        If (success <> True) Then
'            Return False
'            Exit Function
'        End If

'        crypt.CryptAlgorithm = "SHA1"
'        crypt.CipherMode = "cbc"
'        crypt.KeyLength = 256
'        crypt.PaddingScheme = 0

'        Dim key As String = "Pas5pr@se"
'        Dim iv As String = "@1B2c3D4e5F6g7H8"
'        Dim salt As String = "s@1tValue"

'        crypt.SetEncodedKey(key, "ascii")
'        crypt.SetEncodedIV(iv, "ascii")
'        crypt.SetEncodedSalt(salt, "ascii")

'        '  SHA1 Decrypt the file (the file may be any size because it will stream the file in/out
'        Dim strChgExt As String = Replace(strDocName, ".enc", "")

'        success = crypt.CkDecryptFile(strDir & strFolder & strDocName, strDir & strFolder & strChgExt)
'        'If (success <> True) Then
'        If (success = True) Then
'            Return False
'            Exit Function
'        Else
'            'If (Not System.IO.Directory.Exists(strDir & strFolder & strDocName)) Then
'            '    System.IO.File.Delete(strDir & strFolder & strDocName)
'            'End If
'            Return True
'        End If

'    End Function

'    '***************************************
'    'Function: EncryptFile
'    'Author: Faiz
'    'Date: 2/9/2015
'    'Summary: To encrypt binary string file to .enc (for testing purpose)
'    '***************************************

'    <WebMethod()> _
'    Public Function EncryptAll(ByVal strBinary As String, ByVal strDocName As String, ByVal strFolder As String, ByVal strSource As String, ByVal agentID As String, ByVal totalFile As String) As String
'        Dim strStatus As String = ""
'        Dim strDir As String = "D:\Files\"
'        strFolder = strDocName.Substring(0, strDocName.IndexOf("_")) & "\" 'get Proposal number from strDocName
'        Dim count As Integer
'        Dim counter As Integer
'        Dim getPN As String = String.Empty
'        Dim crypt As New Chilkat.Crypt2()

'        Dim success As Boolean
'        success = crypt.UnlockComponent("Anything for 30-day trial")
'        If (success <> True) Then
'            Return False
'            Exit Function
'        End If

'        crypt.CryptAlgorithm = "SHA1"
'        crypt.CipherMode = "cbc"
'        crypt.KeyLength = 256
'        crypt.PaddingScheme = 0

'        Dim key As String = "Pas5pr@se"
'        Dim iv As String = "@1B2c3D4e5F6g7H8"
'        Dim salt As String = "s@1tValue"

'        crypt.SetEncodedKey(key, "ascii")
'        crypt.SetEncodedIV(iv, "ascii")
'        crypt.SetEncodedSalt(salt, "ascii")

'        ' Store transferred file in location folder
'        Dim bw As BinaryWriter
'        Dim sIn() As Byte = Convert.FromBase64String(strBinary)

'        If (Not System.IO.Directory.Exists(strDir & strFolder)) Then
'            System.IO.Directory.CreateDirectory(strDir & strFolder)
'        End If

'        bw = New BinaryWriter(File.Create(strDir & strFolder & strDocName)) 'Create the ecrypted file in local server location 
'        bw.Write(sIn)
'        bw.Close()

'        Using sr As StreamReader = New StreamReader(strDir & strFolder & strDocName)
'            Dim line As String
'            line = sr.ReadLine()
'            Dim words As String() = line.Split(New Char() {";"c})

'            ' Use For Each loop over words and display them.
'            For Each word As String In words
'                If word.Length <> 0 Then 'Filter empty string
'                    If counter = 1 Then
'                        getPN = words(1) 'Read Proposal No
'                    End If
'                    If counter > 1 Then
'                        count = count + CInt(word)
'                    End If
'                End If
'                counter = counter + 1
'            Next
'            'count = count + 1 'Include Complete.xml in file count
'            sr.Close()
'        End Using

'        success = crypt.CkEncryptFile(strDir & strFolder & strDocName, strDir & strFolder & strDocName & ".enc")
'        If (success <> True) Then
'            Return False
'            Exit Function
'        Else
'            strStatus = SaveDocument(strBinary, strDocName, strFolder, strSource, agentID, totalFile)

'            'If (Not System.IO.Directory.Exists(strDir & strFolder & strDocName)) Then
'            '    System.IO.File.Delete(strDir & strFolder & strDocName)
'            'End If
'            'Return True
'        End If
'        Return strStatus
'    End Function


'    Public Function EncryptFile(ByVal strBinary As String, ByVal strDocName As String) As Boolean

'        Dim strStatus As Boolean = False
'        Dim strDir As String = "C:\Users\Admin\Desktop\Files\"
'        Dim strFolder As String = strDocName.Substring(0, strDocName.IndexOf("_")) & "\" 'get Proposal number from strDocName
'        Dim count As Integer
'        Dim counter As Integer
'        Dim getPN As String = String.Empty
'        Dim crypt As New Chilkat.Crypt2()

'        Dim success As Boolean
'        success = crypt.UnlockComponent("Anything for 30-day trial")
'        If (success <> True) Then
'            Return False
'            Exit Function
'        End If

'        crypt.CryptAlgorithm = "SHA1"
'        crypt.CipherMode = "cbc"
'        crypt.KeyLength = 256
'        crypt.PaddingScheme = 0

'        Dim key As String = "Pas5pr@se"
'        Dim iv As String = "@1B2c3D4e5F6g7H8"
'        Dim salt As String = "s@1tValue"

'        crypt.SetEncodedKey(key, "ascii")
'        crypt.SetEncodedIV(iv, "ascii")
'        crypt.SetEncodedSalt(salt, "ascii")

'        ' Store transferred file in location folder
'        Dim bw As BinaryWriter
'        Dim sIn() As Byte = Convert.FromBase64String(strBinary)

'        If (Not System.IO.Directory.Exists(strDir & strFolder)) Then
'            System.IO.Directory.CreateDirectory(strDir & strFolder)
'        End If

'        bw = New BinaryWriter(File.Create(strDir & strFolder & strDocName)) 'Create the ecrypted file in local server location 
'        bw.Write(sIn)
'        bw.Close()

'        Using sr As StreamReader = New StreamReader(strDir & strFolder & strDocName)
'            Dim line As String
'            line = sr.ReadLine()
'            Dim words As String() = line.Split(New Char() {";"c})

'            ' Use For Each loop over words and display them.
'            For Each word As String In words
'                If word.Length <> 0 Then 'Filter empty string
'                    If counter = 1 Then
'                        getPN = words(1) 'Read Proposal No
'                    End If
'                    If counter > 1 Then
'                        count = count + CInt(word)
'                    End If
'                End If
'                counter = counter + 1
'            Next
'            'count = count + 1 'Include Complete.xml in file count
'            sr.Close()
'        End Using

'        success = crypt.CkEncryptFile(strDir & strFolder & strDocName, strDir & strFolder & strDocName & ".enc")
'        If (success <> True) Then
'            Return False
'            Exit Function
'        Else
'            If (Not System.IO.Directory.Exists(strDir & strFolder & strDocName)) Then
'                System.IO.File.Delete(strDir & strFolder & strDocName)
'            End If
'            Return True
'        End If
'        Return success

'    End Function


'    '****************************************************************************************************
'    'Modified By Jacob 06/10/2015
'    'Write error log into text file

'    Public Sub WriteErrorLogFile(ByVal strError As String)
'        Dim FILE_NAME As String = "D:\AgentMgmt\AgentMgmt\ErrorLog"
'        Dim strFile As String = DateTime.Today.ToString("dd-MMM-yyyy") & "ErrorLog.txt"
'        Dim filePath As String = FILE_NAME + "\" + strFile
'        Dim fileExists As Boolean = File.Exists(filePath)

'        Using writer As New StreamWriter(filePath, True)
'            If Not fileExists Then
'                writer.WriteLine(strError)
'            End If
'            writer.WriteLine(strError)
'        End Using

'    End Sub
'    '****************************************************************************************************

'    '***************************************
'    'Function: Forgot Password
'    'Author: Eugene
'    'Date: 26/1/2016
'    'Summary: send password for user in case password forgotten
'    '***************************************
'    <WebMethod()> _
'    Public Function SendForgotPassword(ByVal strAgentId) As String
'        Dim ms As New Encryption
'        Dim isSuccess As Boolean = False
'        'Dim separated As String() = strAgentId.Split("?"c)
'        'Dim temp As String = separated(1)
'        'Dim separated2 As String() = temp.Split("="c)
'        'Dim temp2 As String = separated2(1)
'        Dim objDBcom = New MySQLDBComponent.MySQLDBComponent(POSWeb.POSWeb_SQLConn)
'        Dim dt As New DataTable
'        Dim userEmail As String = String.Empty
'        Dim password As String = String.Empty
'        Dim str As String = True

'        objDBcom.AddParameter("@AgentCode", SqlDbType.NVarChar, strAgentId)
'        objDBcom.ExecuteProcedure(dt, "SP_WS_Agent_Profile_ForgotPassword")
'        objDBcom.Dispose()

'        If dt.Rows.Count > 0 Then
'            userEmail = dt.Rows(0)("AgentEmail").ToString()
'            password = dt.Rows(0)("AgentPassword").ToString()
'            If Not String.IsNullOrEmpty(password) Then
'                Try
'                    Dim smtp As New SmtpClient()
'                    Dim message As New MailMessage("no-reply@bcalife.co.id", userEmail)
'                    message.Subject = "Password Recovery"
'                    message.Body = String.Format("Hi {0},<br /><br />Your password is {1}.<br /><br />Thank You.", strAgentId, ms.DecryptString(password, "1234567891123456"))
'                    message.IsBodyHtml = True
'                    smtp.Host = "smtp.office365.com"
'                    smtp.EnableSsl = True
'                    Dim NetworkCred As New NetworkCredential("no-reply@bcalife.co.id", "Jakarta1!")
'                    smtp.Credentials = NetworkCred
'                    smtp.Port = 587
'                    smtp.Timeout = 60000
'                    smtp.Send(message)
'                    Return str
'                Catch ex As Exception

'                End Try

'            Else
'                Return "False"
'            End If
'            dt.Dispose()
'        End If
'    End Function

'End Class