using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

namespace WebApplication1
{
    public partial class Page3 : System.Web.UI.Page
    {
        private static string generatedCode;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.UrlReferrer == null || Request.UrlReferrer.ToString() == string.Empty)
            {
                Response.Redirect("page1.aspx");
            }
            if (!IsPostBack)
            {
                // Generate one-time password
                generatedCode = GenerateRandomCode();

                // Send email with the code
                string userEmail = Session["UserEmail"] as string;
                if (!string.IsNullOrEmpty(userEmail))
                {
                    SendVerificationEmail(userEmail, generatedCode);
                }
            }
        }

        private string GenerateRandomCode()
        {
            Random rand = new Random();
            return rand.Next(100000, 999999).ToString(); // 6-digit code
        }

        private void SendVerificationEmail(string email, string code)
        {
            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("krivorots", "qvwv oydc rdhr qffq"),
                    EnableSsl = true,
                };

                smtpClient.Send("krivorots@gmail.com", email, "Verification Code", $"Ваш одноразовий пароль: {code}");
            }
            catch (Exception ex)
            {
                // Log or handle error
                Response.Write("Помилка відправки листа: " + ex.Message);
            }
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(2000);
            string userEmail = Session["UserEmail"] as string;
            if (verificationCodeInput.Value == generatedCode)
            {
                // Registration successful
                Session["RegistrationSuccess"] = true;
                Response.Redirect("page4.aspx");
            }
            else
            {
                // Registration failed, delete the user's record
                Session["RegistrationSuccess"] = false;
                DeleteUserRecord(userEmail);
                Response.Redirect("page4.aspx");
            }
        }

        private void DeleteUserRecord(string email)
        {
            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=Users-info;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Users WHERE Email = @Email";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(2000);
            string userEmail = Session["UserEmail"] as string;
            DeleteUserRecord(userEmail);
            Response.Redirect("page1.aspx");
        }
    }
}
