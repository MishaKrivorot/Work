<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Page4.aspx.cs" Inherits="WebApplication1.Page4" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Реєстрація</title>
    <style>
        /* General page styling */
        body {
            font-size: 22px; /* Set a large font size for body text */
            font-weight: bold; /* Make text bold */
            background-color: #f0f8ff; /* Light blue background */
            margin: 0;
            padding: 0;
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100vh; /* Make the body take full height of the screen */
            text-align: center; /* Ensure text is centered */
        }

        /* Label styling */
        asp\:Label {
            font-size: 30px; /* Larger font size for the label */
            font-weight: bold; /* Ensure bold text */
            color: #333; /* Dark text color for better readability */
            margin-bottom: 20px;
        }

        /* Button styling */
        .button {
            width: 200px;
            height: 50px;
            font-size: 22px;
            font-weight: bold;
            padding: 12px 24px;
            background-color: #4CAF50;
            color: white;
            border: none;
            border-radius: 5px;
            cursor: pointer;
            margin-top: 15px;
        }

        /* Button hover effect */
        .button:hover {
            background-color: #45a049; /* Slightly darker green on hover */
        }

        /* Error and success messages */
        .success {
            color: green;
            font-size: 28px;
            font-weight: bold;
        }

        .error {
            color: red;
            font-size: 28px;
            font-weight: bold;
        }
    </style>
</head>
<body>
    <form id="form4" runat="server">
        <!-- Display result message -->
        <asp:Label ID="ResultLabel" runat="server" Font-Bold="True" Font-Size="Large"></asp:Label><br /><br />
        
        <!-- Centered Back Button -->
        <asp:Button ID="btnBack" runat="server" class="button" 
            Text="НА ГОЛОВНУ" OnClientClick="hideButtons();" OnClick="btnBack_Click" />
        
        <script type="text/javascript">
            // Hide the back button and result message on page load or after an event
            function hideButtons() {
                document.getElementById("btnBack").style.visibility = 'hidden';
                document.getElementById("ResultLabel").style.visibility = 'hidden';
            }
        </script>
    </form>
</body>
</html>
