using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication1
{
    public partial class Page4 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.UrlReferrer == null || Request.UrlReferrer.ToString() == string.Empty)
            {
                Response.Redirect("page1.aspx");
            }
            if (!IsPostBack)
            {
                if (Session["RegistrationSuccess"] != null && (bool)Session["RegistrationSuccess"])
                {
                    ResultLabel.Text = "РЕЄСТРАЦІЮ УСПІШНО ЗАВЕРШЕНО!";
                    ResultLabel.CssClass = "success";
                }
                else
                {
                    ResultLabel.Text = "ПОМИЛКА РЕЄСТРАЦІЇ!";
                    ResultLabel.CssClass = "error";
                }
            }
        }
        protected void btnBack_Click(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(2000);
            Response.Redirect("page1.aspx");
        }
    }
}