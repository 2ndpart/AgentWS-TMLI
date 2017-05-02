Imports System.Security.Cryptography
Imports System.IO
Imports System.Configuration

Public Class LoginSite
    Inherits System.Web.UI.Page
    Dim urlTemplate As String = ConfigurationManager.AppSettings("install.link")

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        VersionNo()
    End Sub


    Protected Sub VersionNo()
        Dim objDBCom As New MySQLDBComponent.MySQLDBComponent(POSWeb.POSWeb_SQLConn)
        Dim sql As String = "select MAX(VersionNo) as NewestVersion from TMLI_Version_checker"
        Dim dt As New DataTable

        objDBCom.ExecuteSQL(dt, sql)
        objDBCom.Dispose()
        lbl_Versionno.Text = dt.Rows(0)(0).ToString()
    End Sub

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
        result.IV = New Byte() {0, 0, 0, 0, 0, 0, _
         0, 0, 0, 0, 0, 0, _
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

    Protected Sub btn_login_Click(sender As Object, e As EventArgs) Handles btn_login.Click
        Dim ms As New AgentWS

        Dim strStatus = ""
        If txt_user.Text <> "" And txt_password.Text <> "" Then
            Label1.Text = ms.LoginAPI(txt_user.Text, EncryptString(txt_password.Text, "1234567891123456"), strStatus)
            If Label1.Text = "True" Then
                Dim installerLocation As String = ConfigurationManager.AppSettings("install.link")
                Response.Redirect(installerLocation)
            Else
                Label1.Text = "Username or Password incorrect."
            End If
        Else
            Label1.Text = "Invalid Input!"
        End If
    End Sub
End Class