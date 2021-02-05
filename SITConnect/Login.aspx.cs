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
    public partial class Login : System.Web.UI.Page
    {
        string ASConnection = System.Configuration.ConfigurationManager.ConnectionStrings["ASConnection"].ConnectionString;
        public string success { get; set; }

        static int loginCounter = 0;
        static int loginLeft = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void login_button_Click(object sender, EventArgs e)
        {
            if (ValidateCaptcha())
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

                    string status = checkLockStatus(HttpUtility.HtmlEncode(email_textbox.Text.ToString().Trim()));
                    bool checker = bool.Parse(status);

                    try
                    {
                        if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                        {
                            string pwdWithSalt = pwd + dbSalt;
                            byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                            string userHash = Convert.ToBase64String(hashWithSalt);

                            if (checker == false)
                            {
                                if (userHash.Equals(dbHash))
                                {
                                    Session["Email"] = email_info;
                                    string guid = Guid.NewGuid().ToString();
                                    Session["AuthToken"] = guid;
                                    Response.Cookies.Add(new HttpCookie("AuthToken", guid));
                                    Response.Redirect("Profile.aspx", false);
                                }
                                else
                                {
                                    if (loginCounter == 3)
                                    {
                                        lockStatus(email_info);
                                        loginCounter = 0;
                                        lockStartTimer(DateTime.Now, email_info);
                                        lockEndTimer(DateTime.Now.AddMinutes(5), email_info);
                                        TimeSpan remainingTime = lockTimeLeft(DateTime.Now, DateTime.Now.AddMinutes(5));

                                        catch_label.Text = "your account has been locked due to 3 failed attempts <br> lock duration - " + remainingTime + " minutes";
                                    }

                                    else if (checker == true)
                                    {
                                        catch_label.Text = "your account is locked";
                                    }

                                    else
                                    {
                                        loginCounter = loginCounter + 1;
                                        loginLeft = 4 - loginCounter;
                                        catch_label.Text = "wrong email or password";
                                    }
                                }
                            }

                            else if (checker == true)
                            {
                                TimeSpan remainingTime = lockTimeLeft(DateTime.Now, DateTime.Now.AddMinutes(5));

                                catch_label.Text = "your account has been locked due to 3 failed attempts <br> lock duration - " + remainingTime + " minutes";

                                if (remainingTime <= TimeSpan.Zero)
                                {
                                    unlockStatus(email_textbox.Text);
                                }
                            }
                        }
                        else
                        {
                            catch_label.Text = "invalid password";
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.ToString());
                    }
                    finally { }
                }
            }
            else
            {
                catch_label.Text = "bot";
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

        private string checkLockStatus(string email_info)
        {
            string s = null;

            SqlConnection connection = new SqlConnection(ASConnection);
            string sql = "SELECT LockStatus FROM Account WHERE Email=@Email_Info";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email_Info", email_info);

            connection.Open();
            try
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        s = reader["LockStatus"].ToString();
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

        private void unlockStatus(string email_info)
        {
            SqlConnection connection = new SqlConnection(ASConnection);
            string sql = "UPDATE Account SET LockStatus=0 WHERE EMAIL=@Email_info";
            connection.Open();
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email_info", email_info);
            command.ExecuteNonQuery();
        }

        private void lockStatus(string email_info)
        {
            SqlConnection connection = new SqlConnection(ASConnection);
            string sql = "UPDATE Account SET LockStatus=1 WHERE EMAIL=@Email_info";
            connection.Open();
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email_info", email_info);
            command.ExecuteNonQuery();
        }

        private void lockStartTimer(DateTime now, string email_info)
        {
            SqlConnection connection = new SqlConnection(ASConnection);
            string sql = "UPDATE Account SET LockCheckTimer=@time_now WHERE Email=@Email_info";
            connection.Open();
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@time_now", now);
            command.Parameters.AddWithValue("@Email_Info", email_info);
            command.ExecuteNonQuery();       
        }

        private void lockEndTimer(DateTime now, string email_info)
        {
            SqlConnection connection = new SqlConnection(ASConnection);
            string sql = "UPDATE Account SET LockLeftTimer=@time_now WHERE Email=@Email_info";
            connection.Open();
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@time_now", now);
            command.Parameters.AddWithValue("@Email_info", email_info);
            command.ExecuteNonQuery();
        }

        private DateTime lockEndTime(string email_info)
        {
            string time = null;
            SqlConnection connection = new SqlConnection(ASConnection);
            string sql = "SELECT LockLeftTimer FROM Account WHERE Email=@Email_info";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email_info", email_info);
            connection.Open();
            try
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        time = reader["LockLeftTimer"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }
            DateTime remainingTime = Convert.ToDateTime(time);
            return remainingTime;
        }


        private TimeSpan lockTimeLeft(DateTime startTime, DateTime endTime)
        {
            TimeSpan remainingTime = endTime - startTime;
            return remainingTime;
        }

        public bool ValidateCaptcha()
        {
            bool result = true;

            string captchaResponse = Request.Form["g-recaptcha-response"];

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://www.google.com/recaptcha/api/siteverify?secret=6Lf9ukoaAAAAAKAFHes0TPGI8qL8cNetqqmg2MuF &response=" + captchaResponse);

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

        protected void register_button_Click(object sender, EventArgs e)
        {
            Response.Redirect("Registration.aspx", false);
        }
    }
}