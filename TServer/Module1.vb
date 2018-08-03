Imports System
Imports System.Text
Imports System.Net
Imports System.Net.Sockets
Imports System.IO

Public Class Module1
    Private Const BUFFER_SIZE As Integer = 1024
    Private Const PORT_NUMBER As Integer = 9999
    Shared encoding As ASCIIEncoding = New ASCIIEncoding()

    Public Shared Sub Main()
        Try
            Dim address As IPAddress = IPAddress.Parse("127.0.0.1")
            Dim listener As TcpListener = New TcpListener(address, PORT_NUMBER)
            listener.Start()
            Console.WriteLine("Server started on {0}", listener.LocalEndpoint)
            Console.WriteLine("Waiting for a connection...")
            Dim socket As Socket = listener.AcceptSocket()
            Console.WriteLine("Connection received from {0}", socket.RemoteEndPoint)

            Dim ns As NetworkStream = New NetworkStream(socket)
            Dim reader As StreamReader = New StreamReader(ns)
            Dim writer As StreamWriter = New StreamWriter(ns)
            writer.AutoFlush = True

            Dim data As Byte() = New Byte(1023) {}

            While True
                Dim FileName As String = reader.ReadLine()

                If FileName Is Nothing Then
                    Console.WriteLine("BYE")
                    writer.WriteLine("BYE")
                    Exit While
                End If

                If FileName.ToUpper.Equals("EXIT") Then
                    Console.WriteLine("BYE")
                    writer.WriteLine("BYE")
                    Exit While
                End If

                Console.WriteLine("FileName = " + FileName)
                Dim SourcePath As String = Environment.CurrentDirectory
                Dim FullPath As String = System.IO.Path.Combine(SourcePath, FileName)

                Dim FileExists As Boolean = TFile.CheckFile(FullPath)

                If (FileExists) Then
                    Dim arraydata As Byte() = File.ReadAllBytes(FullPath)
                    writer.WriteLine("OK")
                    writer.WriteLine(FileName)


                    Dim buff As Byte() = New Byte(1023) {}
                    Dim fileinfo As FileInfo = New FileInfo(FullPath)
                    Dim filesize As Long = fileinfo.Length
                    Dim datainfo As String = filesize.ToString()
                    buff = encoding.GetBytes(datainfo)
                    'send file size
                    socket.Send(buff, buff.Length, SocketFlags.None)
                    'send file data
#Region "send one time"
                    'socket.Send(arraydata, arraydata.Length, SocketFlags.None)
#End Region

#Region "read data into buffer then send"
                    Dim buffer As Byte() = New Byte(1023) {}
                    Dim fSize As Byte() = New Byte(fileinfo.Length - 1) {}
                    Dim i As Integer
                    Dim count As Long = 0
                    Dim fs As FileStream = New FileStream(FileName, FileMode.Open)
                    fs.Read(fSize, 0, fSize.Length)
                    fs.Close()
                    While count < filesize
                        If filesize - count < buffer.Length Then
                            buffer = New Byte(filesize - count - 1) {}
                        End If
                        For i = 0 To buffer.Length - 1
                            buffer(i) = fSize(count)
                            count += 1
                        Next
                        socket.Send(buffer, buffer.Length, SocketFlags.None)
                    End While
#End Region
                    Console.WriteLine("Send file " + FileName + " to client success!")
                Else
                    writer.WriteLine("NG")
                End If

            End While
            socket.Close()
            listener.Stop()
        Catch ex As Exception
            Console.WriteLine("Error: {0}", ex)
        End Try

        Console.Read()
    End Sub
End Class
