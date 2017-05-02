Imports System
Imports System.IO
Imports System.Net
Imports System.Text
Public Class TMLIWSWrapper

    Shared Function SendStringData(url As String, data As String) As String
        Dim request As WebRequest = WebRequest.Create(url)
        ' Set the Method property of the request to POST.  
        request.Method = "POST"
        ' Create POST data and convert it to a byte array.  
        Dim postData As String = "This is a test that posts this string to a Web server."
        Dim byteArray As Byte() = Encoding.UTF8.GetBytes(data)
        ' Set the ContentType property of the WebRequest.  
        request.ContentType = "application/x-www-form-urlencoded"
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

        Return responseFromServer
    End Function

    Public Shared Function SendESubmissionData(jsonMessage As String) As String

    End Function
End Class
