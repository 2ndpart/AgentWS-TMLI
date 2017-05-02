Imports System.Data.SqlClient
Imports System.IO

Public Class SPAJNumberGenerator

    Shared NonSyariahCode = "00709" 'BCA
    Shared SyariahCode = "1111" 'Muamalat
    Shared firstDigit = "8"
    Shared digitSeparator = "-"
    Shared firstRunningNumber = 0
    Shared secondRunningNumber = 0
    Shared thirdRunningNumber = 0
    Shared fourthRunningNumber = 0
    Shared fifthRunningNumber = 0
    Shared sixRunningNumber = 0
    Shared seventhRunningNumber = 0
    Shared eighthRunningNumber = 0
    Shared ninethRunningNumber = 0
    Shared tenthRunningNumber = 1
    Shared elevendRunningNumber = 0

    Shared SyariahVAList As New List(Of String)
    Shared NonSyariahVAList As New List(Of String)

    Shared objDBCom As New MySQLDBComponent.MySQLDBComponent(POSWeb.POSWeb_SQLConn)

    Public Shared Sub GenerateSPAJAndVA(counter As Integer)
        SyariahVAList.Clear()
        NonSyariahVAList.Clear()

        For i As Integer = 0 To counter
            Dim resultVal As String = GetSPAJNumber()
            Dim splittedCode As String() = resultVal.Split("-")

            NonSyariahVAList.Add(splittedCode(0))
            SyariahVAList.Add(splittedCode(1))
        Next
        WriteNonSyariahTXT(NonSyariahVAList)
        WriteSyariahTXT(SyariahVAList)
    End Sub
    Public Shared Function GetSPAJNumber() As String

        Dim selectComm As String = "SELECT * FROM TMLI_SPAJNumber_Master"
        Dim dTableSelect As New DataTable
        objDBCom.ExecuteSQL(dTableSelect, selectComm)
        Dim isNewData As Boolean = True

        If dTableSelect.Rows.Count > 0 Then
            isNewData = False
        End If

        If Not isNewData Then
            firstDigit = dTableSelect.Rows(0)("FirstDigit")
            firstRunningNumber = dTableSelect.Rows(0)("FirstRunningNumber")
            secondRunningNumber = dTableSelect.Rows(0)("SecondRunningNumber")
            thirdRunningNumber = dTableSelect.Rows(0)("ThirdRunningNumber")
            fourthRunningNumber = dTableSelect.Rows(0)("FourthRunningNumber")
            fifthRunningNumber = dTableSelect.Rows(0)("FifthRunningNumber")
            sixRunningNumber = dTableSelect.Rows(0)("SixthRunningNumber")
            seventhRunningNumber = dTableSelect.Rows(0)("SeventhRunningNumber")
            eighthRunningNumber = dTableSelect.Rows(0)("EighthRunningNumber")
            ninethRunningNumber = dTableSelect.Rows(0)("NinthRunningNumber")
            tenthRunningNumber = dTableSelect.Rows(0)("TenthRunningNumber")
            elevendRunningNumber = dTableSelect.Rows(0)("EleventhRunningNumber")
        End If

        If firstRunningNumber > 9 Then
            firstRunningNumber = 1
            secondRunningNumber += 1

            If secondRunningNumber > 9 Then
                secondRunningNumber = 1
                thirdRunningNumber += 1

                If thirdRunningNumber > 9 Then
                    thirdRunningNumber = 1
                    fourthRunningNumber += 1

                    If fourthRunningNumber > 9 Then
                        fourthRunningNumber = 1
                        fifthRunningNumber += 1

                        If fifthRunningNumber > 9 Then
                            fifthRunningNumber = 1
                            sixRunningNumber += 1

                            If sixRunningNumber > 9 Then
                                sixRunningNumber = 1
                                seventhRunningNumber += 1

                                If seventhRunningNumber > 9 Then
                                    seventhRunningNumber = 1
                                    eighthRunningNumber += 1

                                    If eighthRunningNumber > 9 Then
                                        eighthRunningNumber = 1
                                        ninethRunningNumber += 1

                                        If ninethRunningNumber > 9 Then
                                            ninethRunningNumber = 1
                                            tenthRunningNumber += 1
                                        Else
                                            ninethRunningNumber += 1
                                            ninethRunningNumber = 1
                                            tenthRunningNumber += 1
                                        End If
                                    Else
                                        eighthRunningNumber += 1
                                        If eighthRunningNumber = 10 Then
                                            eighthRunningNumber = 1
                                            ninethRunningNumber += 1
                                        End If
                                    End If
                                Else
                                    seventhRunningNumber += 1
                                    If seventhRunningNumber = 10 Then
                                        seventhRunningNumber = 1
                                        eighthRunningNumber += 1
                                    End If
                                End If
                            Else
                                sixRunningNumber += 1
                                If sixRunningNumber = 10 Then
                                    sixRunningNumber = 1
                                    seventhRunningNumber += 1
                                End If
                            End If
                        Else
                            fifthRunningNumber += 1
                            If fifthRunningNumber = 10 Then
                                fifthRunningNumber = 1
                                sixRunningNumber += 1
                            End If
                        End If
                    Else
                        fourthRunningNumber += 1

                        If fourthRunningNumber = 10 Then
                            fourthRunningNumber = 1
                            fifthRunningNumber += 1
                        End If
                    End If
                Else
                    thirdRunningNumber += 1
                    If thirdRunningNumber = 10 Then
                        thirdRunningNumber = 1
                        fourthRunningNumber += 1
                    End If
                End If
            Else
                secondRunningNumber += 1
                If secondRunningNumber = 10 Then
                    secondRunningNumber = 1
                    thirdRunningNumber += 1
                End If
            End If
        Else
            firstRunningNumber += 1
            If firstRunningNumber = 10 Then
                firstRunningNumber = 1
                secondRunningNumber += 1
                If secondRunningNumber > 9 Then
                    secondRunningNumber = 1
                    thirdRunningNumber += 1
                    If thirdRunningNumber > 9 Then
                        thirdRunningNumber = 1
                        fourthRunningNumber += 1
                        If fourthRunningNumber > 9 Then
                            fourthRunningNumber = 1
                            fifthRunningNumber += 1
                            If fifthRunningNumber > 9 Then
                                fifthRunningNumber = 1
                                sixRunningNumber += 1
                                If sixRunningNumber > 9 Then
                                    sixRunningNumber = 1
                                    seventhRunningNumber += 1
                                    If eighthRunningNumber > 9 Then
                                        eighthRunningNumber = 1
                                        ninethRunningNumber += 1
                                        If ninethRunningNumber > 9 Then
                                            ninethRunningNumber = 1
                                            tenthRunningNumber += 1
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
            End If
        End If
        Dim firstSecond = firstDigit & firstRunningNumber
        Dim firstSecondInt = Convert.ToInt32(firstSecond) + (seventhRunningNumber + eighthRunningNumber + ninethRunningNumber + tenthRunningNumber)
        elevendRunningNumber = 29102012 Mod firstSecondInt
        elevendRunningNumber = elevendRunningNumber.ToString()(1)

        Dim returnVal As String = firstDigit & firstRunningNumber & secondRunningNumber & thirdRunningNumber & fourthRunningNumber & fifthRunningNumber & sixRunningNumber & seventhRunningNumber & eighthRunningNumber & ninethRunningNumber & tenthRunningNumber & elevendRunningNumber



        If Not isNewData Then
            ' update existing record
            Dim command As New SqlCommand("UPDATE TMLI_SPAJNumber_Master SET FirstDigit=@firstDigit,FirstRunningNumber=@firstNumber,SecondRunningNumber=@secondNumber,ThirdRunningNumber=@thirdNumber,FourthRunningNumber=@fourthNumber,FifthRunningNumber=@fifthNumber,SixthRunningNumber=@sixthNumber,SeventhRunningNumber=@seventhNumber,EighthRunningNumber=@eighthNumber,NinthRunningNumber=@ninthNumber,TenthRunningNumber=@tenthNumber,EleventhRunningNumber=@eleventhNumber")
            command.Parameters.AddWithValue("@firstDigit", firstDigit)
            command.Parameters.AddWithValue("@firstNumber", firstRunningNumber)
            command.Parameters.AddWithValue("@secondNumber", secondRunningNumber)
            command.Parameters.AddWithValue("@thirdNumber", thirdRunningNumber)
            command.Parameters.AddWithValue("@fourthNumber", fourthRunningNumber)
            command.Parameters.AddWithValue("@fifthNumber", fifthRunningNumber)
            command.Parameters.AddWithValue("@sixthNumber", sixRunningNumber)
            command.Parameters.AddWithValue("@seventhNumber", seventhRunningNumber)
            command.Parameters.AddWithValue("@eighthNumber", eighthRunningNumber)
            command.Parameters.AddWithValue("@ninthNumber", ninethRunningNumber)
            command.Parameters.AddWithValue("@tenthNumber", tenthRunningNumber)
            command.Parameters.AddWithValue("@eleventhNumber", elevendRunningNumber)
            objDBCom.ExecuteSqlCommand(command)
        Else
            ' insert new record
            Dim command As New SqlCommand("INSERT INTO TMLI_SPAJNumber_Master VALUES (@firstDigit,@firstNumber,@secondNumber,@thirdNumber,@fourthNumber,@fifthNumber,@sixthNumber,@seventhNumber,@eighthNumber,@ninthNumber,@tenthNumber,@eleventhNumber)")
            command.Parameters.AddWithValue("@firstDigit", firstDigit)
            command.Parameters.AddWithValue("@firstNumber", firstRunningNumber)
            command.Parameters.AddWithValue("@secondNumber", secondRunningNumber)
            command.Parameters.AddWithValue("@thirdNumber", thirdRunningNumber)
            command.Parameters.AddWithValue("@fourthNumber", fourthRunningNumber)
            command.Parameters.AddWithValue("@fifthNumber", fifthRunningNumber)
            command.Parameters.AddWithValue("@sixthNumber", sixRunningNumber)
            command.Parameters.AddWithValue("@seventhNumber", seventhRunningNumber)
            command.Parameters.AddWithValue("@eighthNumber", eighthRunningNumber)
            command.Parameters.AddWithValue("@ninthNumber", ninethRunningNumber)
            command.Parameters.AddWithValue("@tenthNumber", tenthRunningNumber)
            command.Parameters.AddWithValue("@eleventhNumber", elevendRunningNumber)
            objDBCom.ExecuteSqlCommand(command)
        End If

        ' insert new record for SPAJ and VA Number
        Dim insertCommand As New SqlCommand("INSERT INTO TMLI_SPAJNUMBER_VA (SPAJNumber, SyariahVA, NonSyariahVA, Status, GeneratedDate) VALUES (@spajNumber, @syariahVA, @nonsyariahVA, @status, @generatedDate)")
        insertCommand.Parameters.AddWithValue("@spajNumber", returnVal)
        insertCommand.Parameters.AddWithValue("@syariahVA", SyariahCode & "0" & returnVal)
        insertCommand.Parameters.AddWithValue("@nonsyariahVA", NonSyariahCode & returnVal)
        insertCommand.Parameters.AddWithValue("@status", "Added")
        insertCommand.Parameters.AddWithValue("@generatedDate", Now)
        objDBCom.ExecuteSqlCommand(insertCommand)

        Return returnVal & "-" & SyariahCode & returnVal & "-" & NonSyariahCode & "0" & returnVal
    End Function

    Public Shared Function WriteNonSyariahTXT(Parameter As List(Of String)) As Boolean
        Dim txtFileName As String = Path.GetTempPath & "\" & "UPLOADREQ_" & Now.ToString("ddMMyyy_hhmmss") & ".txt"

        Using sw As New StreamWriter(File.Open(txtFileName, FileMode.OpenOrCreate))

            'write header
            Dim header As String = "0" & NonSyariahCode & "I" & Now.ToString("ddMMyyyy")
            sw.WriteLine(header)
            'write content
            For Each vaNumber As String In Parameter
                sw.WriteLine(vaNumber & "," & "PT Tokio Marine Life Insurance" & ",0,Premi Asuransi")
            Next

            'write footer
            Dim footer As String = "2" & NonSyariahCode & "0001500"
            sw.WriteLine(footer)
        End Using

    End Function

    Public Shared Function WriteSyariahTXT(parameter As List(Of String)) As Boolean
        Dim txtFileName As String = Path.GetTempPath & "\ADD_" & SyariahCode & "_" & Now.ToString("yyyyMMddhhmmss") & ".txt"

        Using sw As New StreamWriter(File.Open(txtFileName, FileMode.OpenOrCreate))

            For Each vaNumber As String In parameter
                sw.WriteLine(vaNumber & "," & "PT Tokio Marine Life Insurance" & ",0,Premi Asuransi")
            Next
        End Using
    End Function
End Class
