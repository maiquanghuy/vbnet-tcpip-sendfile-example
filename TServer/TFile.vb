Imports System.IO

Public Class TFile
    Public Shared Function CheckFile(FullPath As String) As Boolean
        If System.IO.File.Exists(FullPath) Then
            'The file exists
            CheckFile = True
        Else
            'File doesn't exist
            CheckFile = False
        End If
    End Function
End Class
