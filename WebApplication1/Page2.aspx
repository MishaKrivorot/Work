<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Page2.aspx.cs" Inherits="WebApplication1.Page2" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Реєстрація</title>
    <style>
        body { 
            font-size: 20px; 
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
            padding: 20px;
            border-radius: 10px;
            box-shadow: 0px 0px 15px rgba(0, 0, 0, 0.2);
            width: 70%;
            max-width: 600px;
            font-size: 22px;
        }
        label {
            font-weight: bold;
            display: block;
            margin-top: 15px;
            text-align: left;
        }
        input[type="text"], input[type="email"], input[type="password"], select {
            width: 90%;
            padding: 10px;
            margin-top: 5px;
            margin-bottom: 15px;
            border-radius: 5px;
            border: 1px solid #ccc;
            font-size: 18px;
        }
        .button {
            width: 100px;
            height: 40px;
            font-size: 18px;
            font-weight: bold;
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
        .error {
            color: red;
            font-size: 18px;
            font-weight: bold;
        }
        .checkbox-group {
            display: flex;
            justify-content: space-around;
            margin-top: 10px;
        }
        .checkbox-group input {
            margin-top: 10px;
        }
    </style>
</head>
<body>
    <h2>Реєстрація користувача</h2>
    <form id="form2" runat="server" enctype="multipart/form-data">
        <label>Фотографія:</label>
        <asp:FileUpload ID="photoUpload" runat="server" Accept="image/jpeg, image/png" /><br />

        <label>Ім'я:</label><input type="text" ID="firstNameText" runat="server"/>
        <label>Прізвище:</label><input type="text" ID="lastNameText" runat="server"/><br />

        <label>Логін:</label><input type="text" ID="loginText" runat="server"/>
        <label>E-mail:</label><input type="email" id="userEmailTextBox" runat="server" /><br />

        <label>Пароль:</label><input type="password" ID="password" runat="server"/><br />

        <!-- Радіокнопки для вибору ролі -->
        <div class="checkbox-group">
            <label>
                <input type="radio" name="role" value="Студент" onclick="updateCheckboxes()" /> Студент
            </label>
            <label>
                <input type="radio" name="role" value="Викладач" checked onclick="updateCheckboxes()" /> Викладач
            </label>
            <label>
                <input type="radio" name="role" value="Н.-Д. персонал" onclick="updateCheckboxes()" /> Н.-Д. персонал
            </label>
        </div>

        <!-- Чекбокси з умовами доступності -->
        <div class="checkbox-group">
            <label><input type="checkbox" id="masterSportbool" checked runat="server"/> Майстер спорту</label>
            <label><input type="checkbox" id="candidateSciencebool" runat="server"/> Кандидат наук</label>
            <label><input type="checkbox" id="doctorSciencebool" runat="server"/> Доктор наук</label>
        </div><br />

        <label>Курс:</label>
        <select id="courselist" runat="server">
            <option>1 курс</option>
            <option>2 курс</option>
            <option>3 курс</option>
            <option>4 курс</option>
            <option>5 курс</option>
            <option>6 курс</option>
        </select><br />

        <label>Факультет:</label>
        <select id="facultylist" runat="server">
            <option>Механіко-математичний</option>
            <option>Радіофізичний</option>
            <option>Геологічний</option>
            <option>Історичний</option>
            <option>Філософський</option>
        </select><br />

        <label>Структурний підрозділ:</label>
        <select id="departmentlist" runat="server">
            <option>Наукова бібліотека</option>
            <option>Ботанічний сад</option>
            <option>Інформаційно-обчислювальний центр</option>
            <option>Ректорат</option>
        </select><br />

        <asp:Label ID="errorMessage" runat="server" CssClass="error"></asp:Label><br />

        <asp:Button ID="btnNext" runat="server" Text="ДАЛІ" OnClick="btnNext_Click" CssClass="btn" />
        <asp:Button ID="btnBack" runat="server" Text="НАЗАД" OnClick="btnBack_Click" CssClass="btn" />
        
        <script type="text/javascript">
            // Приховуємо кнопки та робимо перенаправлення з тайм-аутом
            function hideButtons() {
                document.getElementById("btnNext").style.visibility = 'hidden';
                document.getElementById("btnBack").style.visibility = 'hidden';
                document.getElementById("errorMessage").style.visibility = 'hidden';
            }

            // Установка стану чекбоксів в залежності від обраної ролі
            function updateCheckboxes() {
                const role = document.querySelector('input[name="role"]:checked').value;
                const masterSportCheckbox = document.getElementById("masterSportbool");
                const candidateScienceCheckbox = document.getElementById("candidateSciencebool");
                const doctorScienceCheckbox = document.getElementById("doctorSciencebool");
                const courseList = document.getElementById("courselist");
                const facultyList = document.getElementById("facultylist");
                const departmentList = document.getElementById("departmentlist");

                // "Майстер спорту" завжди активний
                masterSportCheckbox.disabled = false;

                // "Кандидат наук" активний для ролей Викладач та Н.-Д. персонал
                candidateScienceCheckbox.disabled = (role !== "Викладач" && role !== "Н.-Д. персонал");

                // "Доктор наук" активний тільки для ролі Викладач
                doctorScienceCheckbox.disabled = (role !== "Викладач");

                courseList.disabled = (role !== "Студент");
                facultyList.disabled = (role !== "Викладач" && role !== "Студент");
                departmentList.disabled = (role !== "Н.-Д. персонал");
            }

            // Виклик функції при завантаженні сторінки та зміні ролі
            window.onload = updateCheckboxes;
        </script>
    </form>
</body>
</html>
