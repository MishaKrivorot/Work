<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Page3.aspx.cs" Inherits="WebApplication1.Page3" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Верифікація</title>
    <style>
        body { 
            font-size: 22px; 
            font-weight: bold; 
            background-color: #f0f8ff; 
            padding: 30px;
            text-align: center;
        }
        h2 {
            font-size: 30px;
            font-weight: bold;
            color: #333;
        }
        form {
            display: inline-block;
            background-color: #ffffff;
            padding: 30px;
            border-radius: 10px;
            box-shadow: 0px 0px 15px rgba(0, 0, 0, 0.2);
            width: 70%;
            max-width: 500px;
        }
        label {
            font-size: 24px;
            font-weight: bold;
            display: block;
            margin-top: 20px;
        }
        input[type="text"] {
            width: 90%;
            padding: 12px;
            margin-top: 10px;
            margin-bottom: 20px;
            border-radius: 5px;
            border: 1px solid #ccc;
            font-size: 22px;
        }
        .btn {
            font-size: 22px;
            font-weight: bold;
            padding: 12px 24px;
            background-color: #007bff;
            color: white;
            border: none;
            border-radius: 5px;
            cursor: pointer;
            margin-top: 15px;
        }
        .btn:hover {
            background-color: #0056b3;
        }
    </style>
</head>
<body>
    <h2>Верифікація адреси електронної пошти</h2>
    <form id="form3" runat="server">
        <label>На Вашу адресу направлений одноразовий пароль. Введіть його в поле і натисніть "ДАЛІ":</label><br /><br />
        <input type="text" id="verificationCodeInput" runat="server" /><br /><br />

        <asp:Button ID="btnRegister" runat="server" Text="ЗАРЕЄСТРУВАТИ" OnClick="btnRegister_Click" CssClass="btn" OnClientClick="hideButtons();" />
        <asp:Button ID="btnBack" runat="server" Text="НАЗАД" OnClick="btnBack_Click" CssClass="btn" OnClientClick="hideButtons();" />
        
        <script type="text/javascript">
            function hideButtons() {
                document.getElementById("btnBack").style.visibility = 'hidden';
                document.getElementById("btnRegister").style.visibility = 'hidden';
            }
        </script>
    </form>
</body>
</html>
