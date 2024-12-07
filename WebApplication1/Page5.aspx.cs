using System;
using System.Web.UI;

namespace WebApplication1
{
    public partial class Page5 : Page
    {
        protected string firstName;
        protected string lastName;
        protected string userLogin;
        protected string userEmail;
        protected byte[] photoBytes;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.UrlReferrer == null || Request.UrlReferrer.ToString() == string.Empty)
            {
                Response.Redirect("page1.aspx");
            }
            if (!IsPostBack)
            {
                if (Session["UserLogin"] == null || Session["UserEmail"] == null)
                {
                    Response.Redirect("Page1.aspx");
                    return;
                }

                firstName = Session["FirstName"] as string;
                lastName = Session["LastName"] as string;
                userLogin = Session["UserLogin"] as string;
                userEmail = Session["UserEmail"] as string;
                photoBytes = Session["UserPhoto"] as byte[];
            }
        }

        protected void Logout_Click(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(2000);
            Session.Clear();
            Response.Redirect("Page1.aspx");
        }
    }
}
