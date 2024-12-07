<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Page5.aspx.cs" Inherits="WebApplication1.Page5" %>
<!DOCTYPE html>
<html>
<head>
    <title>Сайт з авторизованим доступом</title>
    <style>
        body { 
            font-weight: bold; 
            font-size: 20px; 
            background-color: #f0f8ff; 
            text-align: center;
            padding: 20px;
        }
        h2 {
            font-size: 30px;
            font-weight: bold;
            color: #333;
        }
        form {
            display: inline-block;
            background-color: #ffffff;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0px 0px 15px rgba(0, 0, 0, 0.2);
            width: 80%;
            max-width: 600px;
        }
        div {
            font-size: 22px;
            margin-bottom: 15px;
        }
        .photo-container {
            border: 1px solid #000; 
            width: 200px;
            height: 300px; 
            margin: 10px auto;
            text-align: center;
        }
        .photo-container img {
            max-width: 100%;
            max-height: 100%;
        }
        .user-info {
            margin-top: 20px;
            font-size: 24px;
        }
        .user-info p {
            margin: 5px 0;
        }
        .button-container {
            margin-top: 20px;
        }
        .btn {
            font-size: 26px;
            font-weight: bold;
            padding: 15px 30px;
            color: white;
            background-color: #007bff;
            border: none;
            border-radius: 5px;
            cursor: pointer;
        }
        .btn:hover {
            background-color: #0056b3;
        }
    </style>
</head>
<body>
    <h2>Вітаємо Вас на нашому сайті,</h2>
    <h2><%= firstName %> <%= lastName %>!</h2>

    <form id="form1" runat="server">
        <div style="padding: 10px; text-align: center;">
            <div class="photo-container">
                <% if (photoBytes != null) { %>
                    <img src="data:image/jpeg;base64,<%= Convert.ToBase64String(photoBytes) %>" alt="Фото користувача" />
                <% } else { %>
                    <p>ФОТО</p>
                <% } %>
            </div>

            <div class="user-info">
                Логін: <%= userLogin %> <br />
                E-mail: <%= userEmail %>
            </div>
        </div>

        <div class="button-container">
            <asp:Button ID="btnBack" runat="server" Font-Size="XX-Large" 
            OnClientClick="hideButtons();" Text="ВИХІД" OnClick="Logout_Click" CssClass="btn" />
        </div>

        <script type="text/javascript">
            function hideButtons() {
                document.getElementById("btnBack").style.visibility = 'hidden';
            }
        </script>
    </form>
</body>
</html>
