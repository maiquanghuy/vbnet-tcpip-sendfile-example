Imports System
Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Net.Sockets

Public Class Module1
    Private Const BUFFER_SIZE As Integer = 1024
    Private Const PORT_NUMBER As Integer = 9999
    Shared encoding As ASCIIEncoding = New ASCIIEncoding()

    Public Shared Sub Main()
        Try
            ' 1. connect            
            Dim socket As Socket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            socket.Connect("127.0.0.1", PORT_NUMBER)
            Dim ns As NetworkStream = New NetworkStream(socket)
            Dim reader As StreamReader = New StreamReader(ns)
            Dim writer As StreamWriter = New StreamWriter(ns)
            writer.AutoFlush = True

            Console.WriteLine("Connected to TServer.")

            While True
                Console.Write("Enter your file name: ")
                Dim str As String = Console.ReadLine()

                ' 2. send
                writer.WriteLine(str)

                ' 3. receive
                str = reader.ReadLine()

                'File exist
                If str.Equals("OK") Then
                    str = reader.ReadLine()
                    Dim FileName As String = str

                    Dim buff As Byte() = New Byte(1023) {}
                    'receive file size
                    Dim dataInfo As Integer = socket.Receive(buff, buff.Length, SocketFlags.None)
                    Dim s As String = encoding.GetString(buff, 0, dataInfo)
                    Dim Size As Long = Long.Parse(s)
                    Dim data As Byte() = New Byte(Size - 1) {}
#Region "receive one time"
                    'Dim rec As Int32 = socket.Receive(data, data.Length, SocketFlags.None)
#End Region

#Region "read from buffer then receive"
                    Dim buffer As Byte() = New Byte(1023) {}
                    Dim count As Long = 0
                    Dim i As Integer

                    While count < Size

                        If Size - count < buffer.Length Then
                            buffer = New Byte(Size - count - 1) {}
                        End If

                        socket.Receive(buffer, buffer.Length, SocketFlags.None)

                        For i = 0 To buffer.Length - 1
                            data(count) = buffer(i)
                            count += 1
                        Next
                    End While
#End Region

                    Console.WriteLine("Receive success")
                    File.WriteAllBytes(FileName, data)
                    Console.WriteLine("BYE")
                    Exit While
                    'File not exist
                ElseIf str.Equals("NG") Then
                    Console.WriteLine("File is not exist!")
                End If

                If str.ToUpper().Equals("BYE") Then Exit While
            End While
            writer.Close()
            reader.Close()
            socket.Close()
        Catch ex As Exception
            Console.WriteLine("Error: {0}", ex)
        End Try

        Console.Read()
    End Sub
End Class
