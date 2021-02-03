using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SITConnect
{
    // int loginAttempt = 0;
    // int loginLeft = 0;

    public partial class Login : System.Web.UI.Page
    {
        string ASConnection = System.Configuration.ConfigurationManager.ConnectionStrings["ASConnection"].ConnectionString;
        public string success { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void login_button_Click(object sender, EventArgs e)
        {
            if (email_textbox.Text.ToString() == "")
            {
                checker_label.Text = "please enter a valid email";
            }
            if (password_textbox.Text.ToString() == "")
            {
                checker_label.Text = "please enter a valid password";
            }
            if (email_textbox.Text.ToString() == "" && password_textbox.Text.ToString() == "")
            {
                checker_label.Text = "please enter your profile details";
            }
            else
            {
                string pwd = password_textbox.Text.ToString().Trim();
                string email_info = email_textbox.Text.ToString().Trim();
                SHA512Managed hashing = new SHA512Managed();
                string dbHash = getDBHash(email_info);
                string dbSalt = getDBSalt(email_info);
                try
                {
                    if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                    {
                        string pwdWithSalt = pwd + dbSalt;
                        byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                        string userHash = Convert.ToBase64String(hashWithSalt);
                        if (userHash.Equals(dbHash))
                        {
                            Session["Email"] = email_info;
                            Response.Redirect("Profile.aspx", false);
                        }

                        else
                        {
                            checker_label.Text = "Userid or password is not valid. Please try again.";
                            Response.Redirect("Login.aspx", false);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.ToString());
                }
                finally { }
            }
        }

        protected string getDBHash(string email_info)
        {
            string h = null;
            SqlConnection connection = new SqlConnection(ASConnection);
            string sql = "select PasswordHash FROM Account WHERE Email=@Email_Info";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email_Info", email_info);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["PasswordHash"] != null)
                        {
                            if (reader["PasswordHash"] != DBNull.Value)
                            {
                                h = reader["PasswordHash"].ToString();
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return h;
        }

        protected string getDBSalt(string email_info)
        {
            string s = null;
            SqlConnection connection = new SqlConnection(ASConnection);
            string sql = "select PASSWORDSALT FROM Account WHERE Email=@Email_Info";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email_Info", email_info);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["PASSWORDSALT"] != null)
                        {
                            if (reader["PASSWORDSALT"] != DBNull.Value)
                            {
                                s = reader["PASSWORDSALT"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return s;
        }

        public bool ValidateCaptcha()
        {
            bool result = true;

            string captchaResponse = Request.Form["g-recaptcha-response"];

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create
            (" https://www.google.com/recaptcha/api/siteverify?secret=6LfgFUUaAAAAABwf3QXQ7t9YnbioM3Hxvv-XoNrU & response=" + captchaResponse);

            try
            {
                using (WebResponse wResponse = req.GetResponse())
                {
                    using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                    {
                        string jsonResponse = readStream.ReadToEnd();

                        JavaScriptSerializer js = new JavaScriptSerializer();
                        Login jsonObject = js.Deserialize<Login>(jsonResponse);
                        result = Convert.ToBoolean(jsonObject.success);
                    }
                }
                return result;
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }
    }
}