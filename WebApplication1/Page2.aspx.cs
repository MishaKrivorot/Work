using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Image = System.Drawing.Image;

namespace WebApplication1
{
    public partial class Page2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.UrlReferrer == null || Request.UrlReferrer.ToString() == string.Empty)
            {
                Response.Redirect("page1.aspx");
            }
            if (!IsPostBack)
            {
                errorMessage.CssClass = "success";
            }
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(2000);
            string userEmail = userEmailTextBox.Value ?? "";
            string firstName = firstNameText.Value ?? "";
            string lastName = lastNameText.Value ?? "";
            string login = loginText.Value ?? "";
            string role = Request.Form["role"] ?? "";
            bool masterSport = Request.Form["masterSportbool"] == "on";
            bool candidateScience = Request.Form["candidateSciencebool"] == "on";
            bool doctorScience = Request.Form["doctorSciencebool"] == "on";
            string course = Request.Form["courselist"] ?? "";
            string faculty = Request.Form["facultylist"] ?? "";
            string department = Request.Form["departmentlist"] ?? "";
            string Userpassword = password.Value ?? "";
            if (string.IsNullOrWhiteSpace(userEmail) ||
                string.IsNullOrWhiteSpace(firstName) ||
                string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(login) ||
                string.IsNullOrWhiteSpace(Userpassword))
            {
                errorMessage.CssClass = "error";
                errorMessage.Text = "Please fill in all required fields: Email, First Name, Last Name, Login, and Password.";
                return;
            }
            byte[] photoBytes = null;
            if (photoUpload.HasFile)
            {
                using (var image = Image.FromStream(photoUpload.PostedFile.InputStream))
                {
                    var resizedImage = ResizeImage(image, 100, 150, 200, 300);
                    using (var ms = new MemoryStream())
                    {
                        resizedImage.Save(ms, ImageFormat.Jpeg);
                        photoBytes = ms.ToArray();
                    }
                }
            }
            else
            {
                errorMessage.CssClass = "error";
                errorMessage.Text = "Please upload a photo.";
                return;
            }

            Session["UserEmail"] = userEmail;
            // Check for unique email and login
            if (IsUniqueUserData(login, userEmail))
            {
                SaveUserDataToDatabase(firstName, lastName, login, userEmail, role, masterSport, candidateScience, doctorScience, course, faculty, department, photoBytes, Userpassword);
                Response.Redirect("Page3.aspx");
            }
            else
            {
                errorMessage.CssClass = "error";
                errorMessage.Text = "Login or Email already exists. Please use different values.";
            }
        }

        private Image ResizeImage(Image originalImage, int minWidth, int minHeight, int maxWidth, int maxHeight)
        {
            int width = originalImage.Width;
            int height = originalImage.Height;

            if (width < minWidth || height < minHeight)
            {
                width = minWidth;
                height = minHeight;
            }
            else if (width > maxWidth || height > maxHeight)
            {
                width = maxWidth;
                height = maxHeight;
            }

            var resizedImage = new Bitmap(width, height);
            using (var graphics = Graphics.FromImage(resizedImage))
            {
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(originalImage, 0, 0, width, height);
            }

            return resizedImage;
        }

        private bool IsUniqueUserData(string login, string email)
        {
            bool isUnique = true;
            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=Users-info;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM Users WHERE Login = @Login OR Email = @Email";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Login", login);
                    command.Parameters.AddWithValue("@Email", email);

                    connection.Open();
                    int count = (int)command.ExecuteScalar();

                    if (count > 0)
                    {
                        isUnique = false;
                    }
                }
            }

            return isUnique;
        }

        private void SaveUserDataToDatabase(string firstName, string lastName, string login, string email, string role, bool masterSport, bool candidateScience, bool doctorScience, string course, string faculty, string department, byte[] photo, string Userpassword)
        {
            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=Users-info;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Users (FirstName, LastName, Login, Email, Role, MasterSport, CandidateScience, DoctorScience, Course, Faculty, Department, Photo, Userpassword) " +
                               "VALUES (@FirstName, @LastName, @Login, @Email, @Role, @MasterSport, @CandidateScience, @DoctorScience, @Course, @Faculty, @Department, @Photo, @Userpassword)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FirstName", string.IsNullOrEmpty(firstName) ? (object)DBNull.Value : firstName);
                    command.Parameters.AddWithValue("@LastName", string.IsNullOrEmpty(lastName) ? (object)DBNull.Value : lastName);
                    command.Parameters.AddWithValue("@Userpassword", string.IsNullOrEmpty(Userpassword) ? (object)DBNull.Value : Userpassword);
                    command.Parameters.AddWithValue("@Login", string.IsNullOrEmpty(login) ? (object)DBNull.Value : login);
                    command.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(email) ? (object)DBNull.Value : email);
                    command.Parameters.AddWithValue("@Role", string.IsNullOrEmpty(role) ? (object)DBNull.Value : role);
                    command.Parameters.AddWithValue("@MasterSport", masterSport);
                    command.Parameters.AddWithValue("@CandidateScience", candidateScience);
                    command.Parameters.AddWithValue("@DoctorScience", doctorScience);
                    command.Parameters.AddWithValue("@Course", string.IsNullOrEmpty(course) ? (object)DBNull.Value : course);
                    command.Parameters.AddWithValue("@Faculty", string.IsNullOrEmpty(faculty) ? (object)DBNull.Value : faculty);
                    command.Parameters.AddWithValue("@Department", string.IsNullOrEmpty(department) ? (object)DBNull.Value : department);
                    command.Parameters.AddWithValue("@Photo", photo ?? (object)DBNull.Value);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(2000);
            Response.Redirect("Page1.aspx");
        }
    }
}
