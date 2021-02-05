using System;
using System.Collections.Generic;
using System.Data;
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
    public partial class Registration : System.Web.UI.Page
    {
        string ASConnection = System.Configuration.ConfigurationManager.ConnectionStrings["ASConnection"].ConnectionString;
        static string finalHash;
        static string salt;
        byte[] Key;
        byte[] IV;

        public string success { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void createAccount()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ASConnection))
                {
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO Account VALUES (@FirstName,@LastName,@Email,@CCNo,@CVV,@ExpiryDate,@PasswordHash,@PasswordSalt,@DoB,@IV,@Key,@LockStatus,@LockCheckTimer,@LockLeftTimer)"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@FirstName", fName_textbox.Text.Trim());
                            cmd.Parameters.AddWithValue("@LastName", lName_textbox.Text.Trim());
                            cmd.Parameters.AddWithValue("@Email", email_textbox.Text.Trim());
                            cmd.Parameters.AddWithValue("@CCNo", Convert.ToBase64String(encryptData(ccNo_textbox.Text.Trim())));
                            cmd.Parameters.AddWithValue("@CVV", Convert.ToBase64String(encryptData(cvv_textbox.Text.Trim())));
                            cmd.Parameters.AddWithValue("@ExpiryDate", ccED_textbox.Text.Trim());
                            cmd.Parameters.AddWithValue("@PasswordHash", finalHash);
                            cmd.Parameters.AddWithValue("@PasswordSalt", salt);
                            cmd.Parameters.AddWithValue("@DoB", dob_textbox.Text.Trim());
                            cmd.Parameters.AddWithValue("@IV", Convert.ToBase64String(IV));
                            cmd.Parameters.AddWithValue("@Key", Convert.ToBase64String(Key));
                            cmd.Parameters.AddWithValue("@LockStatus", 0);
                            cmd.Parameters.AddWithValue("@LockCheckTimer", DateTime.Now);
                            cmd.Parameters.AddWithValue("@LockLeftTimer", DateTime.Now);
                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

        }

        protected byte[] encryptData(string data)
        {
            byte[] cipherText = null;
            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;
                ICryptoTransform encryptTransform = cipher.CreateEncryptor();
                //ICryptoTransform decryptTransform = cipher.CreateDecryptor();
                byte[] plainText = Encoding.UTF8.GetBytes(data);
                cipherText = encryptTransform.TransformFinalBlock(plainText, 0, plainText.Length);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }
            return cipherText;
        }

        protected void register_button_Click1(object sender, EventArgs e)
        { 
            if (ValidateCaptcha())
            {
                if (fName_textbox.Text.ToString() == "" && lName_textbox.Text.ToString() == "")
                {
                    checker_label.Text = "please enter your name details";
                }
                if (email_textbox.Text.ToString() == "")
                {
                    checker_label.Text = "please enter your email";
                }
                if (ccNo_textbox.Text.ToString() == "" && cvv_textbox.Text.ToString() == "" && ccED_textbox.Text.ToString() == "")
                {
                    checker_label.Text = "please enter your credit card details";
                }
                if (dob_textbox.Text.ToString() == "")
                {
                    checker_label.Text = "please enter your date of birth";
                }
                if (password_textbox.Text.ToString() == "")
                {
                    checker_label.Text = "please enter a password";
                }
                if (fName_textbox.Text.ToString() == "" && lName_textbox.Text.ToString() == "" && email_textbox.Text.ToString() == "" && ccNo_textbox.Text.ToString() == "" && cvv_textbox.Text.ToString() == "" && ccED_textbox.Text.ToString() == "" && dob_textbox.Text.ToString() == "" && password_textbox.Text.ToString() == "")
                {
                    checker_label.Text = "actually enter something";
                }
                else
                {
                    //string pwd = get value from your Textbox
                    string pwd = password_textbox.Text.ToString().Trim(); ;

                    //Generate random "salt"
                    RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                    byte[] saltByte = new byte[8];

                    //Fills array of bytes with a cryptographically strong sequence of random values.
                    rng.GetBytes(saltByte);
                    salt = Convert.ToBase64String(saltByte);

                    SHA512Managed hashing = new SHA512Managed();

                    string pwdWithSalt = pwd + salt;
                    byte[] plainHash = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwd));
                    byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));

                    finalHash = Convert.ToBase64String(hashWithSalt);

                    RijndaelManaged cipher = new RijndaelManaged();
                    cipher.GenerateKey();
                    Key = cipher.Key;
                    IV = cipher.IV;

                    createAccount();
                    display_label.Text = "account successfully created";
                }
            }
            else
            {
                checker_label.Text = "bot";
            }
        }

        protected void route_button_Click1(object sender, EventArgs e)
        {
            Response.Redirect("Login.aspx", false);
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
    }
}