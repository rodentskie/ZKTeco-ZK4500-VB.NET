Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports System.Windows.Forms
Imports AxZKFPEngXControl

Public Class Form1
    'Dim ZkFprint = New AxZKFPEngX
    Dim WithEvents ZkFprint As New AxZKFPEngX
    Dim Check As Boolean

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Controls.Add(ZkFprint)
        InitialAxZkfp()
    End Sub

    Private Sub InitialAxZkfp()
        Try
            If (ZkFprint.InitEngine = 0) Then
                ZkFprint.FPEngineVersion = "9"
                ZkFprint.EnrollCount = 3
                deviceSerial.Text = (deviceSerial.Text + (" " _
                            + (ZkFprint.SensorSN + (" Count: " _
                            + (ZkFprint.SensorCount.ToString + (" Index: " + ZkFprint.SensorIndex.ToString))))))
                ShowHintInfo("Device successfully connected")
            End If

        Catch ex As Exception
            ShowHintInfo(("Device init err, error: " + ex.Message))
        End Try


    End Sub

    Private Sub zkFprint_OnImageReceived()
        Dim g As Graphics = fpicture.CreateGraphics
        Dim bmp As Bitmap = New Bitmap(fpicture.Width, fpicture.Height)
        g = Graphics.FromImage(bmp)
        Dim dc As Integer = g.GetHdc.ToInt32
        ZkFprint.PrintImageAt(dc, 0, 0, bmp.Width, bmp.Height)
        g.Dispose()
        fpicture.Image = bmp
    End Sub




    Private Sub ShowHintInfo(ByVal s As String)
        prompt.Text = s
    End Sub

    Private Sub btnVerify_Click(sender As Object, e As EventArgs) Handles btnVerify.Click
        If ZkFprint.IsRegister Then
            ZkFprint.CancelEnroll
        End If

        ZkFprint.BeginCapture
        ShowHintInfo("Please give fingerprint sample.")
    End Sub

    Private Sub btnRegister_Click(sender As Object, e As EventArgs) Handles btnRegister.Click
        ZkFprint.CancelEnroll
        ZkFprint.EnrollCount = 3
        ZkFprint.BeginEnroll
        ShowHintInfo("Please give fingerprint sample.")
    End Sub

    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        fpicture.Image = Nothing
    End Sub

    Private Sub ZkFprint_OnImageReceived(sender As Object, e As IZKFPEngXEvents_OnImageReceivedEvent) Handles ZkFprint.OnImageReceived
        Dim g As Graphics = fpicture.CreateGraphics
        Dim bmp As Bitmap = New Bitmap(fpicture.Width, fpicture.Height)
        g = Graphics.FromImage(bmp)
        Dim dc As Integer = g.GetHdc.ToInt32
        ZkFprint.PrintImageAt(dc, 0, 0, bmp.Width, bmp.Height)
        g.Dispose()
        fpicture.Image = bmp
    End Sub

    Private Sub ZkFprint_OnFeatureInfo(sender As Object, e As IZKFPEngXEvents_OnFeatureInfoEvent) Handles ZkFprint.OnFeatureInfo
        Dim strTemp As String = String.Empty
        If (ZkFprint.EnrollIndex <> 1) Then
            If ZkFprint.IsRegister Then
                If ((ZkFprint.EnrollIndex - 1) _
                            > 0) Then
                    Dim eindex As Integer = (ZkFprint.EnrollIndex - 1)
                    strTemp = ("Please scan again ..." + eindex.ToString())
                End If

            End If

        End If

        ShowHintInfo(strTemp)
    End Sub

    Private Sub ZkFprint_OnEnroll(sender As Object, e As IZKFPEngXEvents_OnEnrollEvent) Handles ZkFprint.OnEnroll
        If e.actionResult Then
            Dim template As String = ZkFprint.EncodeTemplate1(e.aTemplate)
            txtTemplate.Text = template
            ShowHintInfo("Registration successful. You can verify now")
            btnRegister.Enabled = False
            btnVerify.Enabled = True
        Else
            ShowHintInfo("Error, please register again.")
        End If
    End Sub
End Class
