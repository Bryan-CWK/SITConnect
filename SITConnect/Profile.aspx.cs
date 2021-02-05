using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SITConnect
{
    public partial class Profile : System.Web.UI.Page
    {
        string ASConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ASConnection"].ConnectionString;
        byte[] Key;
        byte[] IV;
        byte[] CCNo;
        string email_info = null;

        protected void Page_Load(object sender, EventArgs e)
        {

            if (Session["Email"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
            {
                if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    Response.Redirect("Login.aspx", false);
                }
                if (Session["Email"] != null)
                {
                    email_info = (string)Session["Email"];

                    displayUserProfile(email_info);
                }
            }
            else
            {
                Response.Redirect("Login.aspx", false);
            }
            
        }

        protected string decryptData(byte[] cipherText)
        {
            string plainText = null;
            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;
                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptTransform = cipher.CreateDecryptor();
                // Create the streams used for decryption
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptTransform, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string
                            plainText = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }
            return plainText;
        }

        protected void displayUserProfile(string email_info)
        {
            SqlConnection connection = new SqlConnection(ASConnectionString);
            string sql = "SELECT * FROM Account WHERE Email=@Email_Info";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email_Info", email_info);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["Email"] != DBNull.Value)
                        {
                            displayEmail_label.Text = reader["Email"].ToString();

                            if (reader["FirstName"] != DBNull.Value && reader["LastName"] != DBNull.Value)
                            {
                                string firstname = reader["FirstName"].ToString();
                                string lastname = reader["LastName"].ToString();
                                displayName_label.Text = firstname + lastname;

                                if (reader["CCNo"] != DBNull.Value)
                                {
                                    CCNo = Convert.FromBase64String(reader["CCNo"].ToString());

                                    if (reader["IV"] != DBNull.Value)
                                    {
                                        IV = Convert.FromBase64String(reader["IV"].ToString());
                                    }

                                    if (reader["Key"] != DBNull.Value)
                                    {
                                        Key = Convert.FromBase64String(reader["Key"].ToString());
                                    }
                                }
                            }
                        }
                    }
                    displayCCNo_label.Text = decryptData(CCNo);
                }
            }//try
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                connection.Close();
            }
        }

        protected void logout_button_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();

            Response.Redirect("Login.aspx", false);

            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
            }
            if (Request.Cookies["AuthToken"] != null)
            {
                Response.Cookies["AuthToken"].Value = string.Empty;
                Response.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-20);
            }

        }
    }
}