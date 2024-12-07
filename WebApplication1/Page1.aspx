<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Page1.aspx.cs" Inherits="WebApplication1.Page1" %>
<!DOCTYPE html>
<html>
<head>
    <title>Сайт з авторизованим доступом</title>
    <style>
        /* Basic styling for the page */
        body { 
            font-family: Arial, sans-serif; 
            background-color: #f0f8ff; 
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100vh; 
            margin: 0;
        }
        h2 {
            font-size: 30px;
            font-weight: bold;
            color: #333;
            margin-bottom: 20px;
        }
        form {
            background-color: #ffffff;
            padding: 30px;
            border-radius: 8px;
            box-shadow: 0px 0px 15px rgba(0, 0, 0, 0.2);
            width: 350px;
            font-weight: bold;
            font-size: 18px;
            margin: 0 auto;
            padding-right: 60px;
        }
        label {
            display: block;
            margin-bottom: 5px;
            font-size: 18px;
        }
        input[type="text"], input[type="password"] {
            width: 100%;
            padding: 12px;
            margin-bottom: 15px;
            border: 1px solid #ccc;
            border-radius: 5px;
            font-size: 16px;
            font-weight: bold;
        }
        .error {
            color: red;
            font-size: 16px;
            font-weight: bold;
            margin-bottom: 15px;
            display: block;
        }
        .success {
            color: green;
            font-size: 16px;
            font-weight: bold;
            display: none;
        }
        .button-container {
            display: flex;
            justify-content: space-around;
            margin-top: 15px;
            width: 380px;
        }
        .Button {
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
        .Button:hover {
            background-color: #0056b3;
}
    </style>
</head>
<body>
    <div>
        <h2>Сайт з авторизованим доступом</h2>
        <form id="form1" runat="server">
            <div>
                <label>Логін:</label>
                <input type="text" id="login" runat="server"/>
            </div>
            <div>
                <label>Пароль:</label>
                <input type="password" id="password" runat="server"/>
            </div>
            <asp:Label ID="errorMessage" runat="server" CssClass="error" Text="Логін або пароль введені неправильно"></asp:Label>
            <div class="button-container">
                <asp:Button ID="btnLogin" runat="server" Text="Вхід" OnClientClick="hideButtons();" CssClass="Button" onclick="Login_Click" />
                <asp:Button ID="btnRegister" runat="server" Text="Реєстрація" OnClientClick="hideButtons();" CssClass="Button" onclick="Register_Click" />
            </div>
        </form>
    </div>
    <script type="text/javascript">
        function hideButtons() {
            document.getElementById("btnLogin").style.visibility = 'hidden';
            document.getElementById("btnRegister").style.visibility = 'hidden';
            document.getElementById("errorMessage").style.visibility = 'hidden';
        }
    </script>
</body>
</html>
