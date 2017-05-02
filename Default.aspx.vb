Imports System.IO
Imports ICSharpCode.SharpZipLib.Zip
Imports Microsoft.VisualBasic.FileIO

Public Class _Default
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim file As HttpPostedFile = Request.Files("file")
        Dim folderName As String = Request.Params("folderName")
        Dim isResubmit As String = Request.Params("isResubmit")
        Try
            Dim fileName As String = folderName & ".zip"
            file.SaveAs("C:/Users/Public/" & fileName)
            Directory.CreateDirectory("C:/inetpub/wwwroot/SPAJFiles/" & folderName)
            'System.IO.File.Move("C:/Users/Public/files.zip", "C:/Users/Public/files/" & folderName & "/files.zip")
            FileSystem.MoveFile("C:/Users/Public/" & fileName, "C:/inetpub/wwwroot/SPAJFiles/" & folderName & "/" & fileName & ".zip", True)
            Dim unzipper As New FastZip
            Dim fileFilter As String = String.Empty

            If isResubmit = "true" Then
                unzipper.ExtractZip("C:/inetpub/wwwroot/SPAJFiles/" & folderName & "/" & fileName & ".zip", "C:/inetpub/wwwroot/SPAJFiles/" & folderName & "/resubmit", fileFilter)
            Else
                unzipper.ExtractZip("C:/inetpub/wwwroot/SPAJFiles/" & folderName & "/" & fileName & ".zip", "C:/inetpub/wwwroot/SPAJFiles/" & folderName, fileFilter)
            End If
        Catch ex As Exception
            Response.Write("Error on saving file : " + ex.Message)
        End Try
    End Sub

End Class