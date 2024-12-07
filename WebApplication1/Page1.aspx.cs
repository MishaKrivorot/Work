using System;
using System.Data.SqlClient;
using System.Web.UI;

namespace WebApplication1
{
    public partial class Page1 : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            errorMessage.CssClass = "success";
        }

        protected void Login_Click(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(2000);
            string userLogin = login.Value;
            string userPassword = password.Value;
            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=Users-info;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT FirstName, LastName, Email, Photo FROM Users WHERE Login = @Login AND Userpassword = @Password";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Login", userLogin);
                    command.Parameters.AddWithValue("@Password", userPassword);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        reader.Read();

                        // Store user data in session variables
                        Session["FirstName"] = reader["FirstName"].ToString();
                        Session["LastName"] = reader["LastName"].ToString();
                        Session["UserLogin"] = userLogin;
                        Session["UserEmail"] = reader["Email"].ToString();
                        Session["UserPhoto"] = reader["Photo"] as byte[]; // Assumes 'Photo' is stored as binary data in database

                        // Redirect to Page5
                        Response.Redirect("Page5.aspx");
                    }
                    else
                    {
                        // Unsuccessful login, show error message
                        errorMessage.CssClass = "error";
                    }
                }
            }
        }

        protected void Register_Click(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(2000);
            Response.Redirect("Page2.aspx");
        }
    }
}
