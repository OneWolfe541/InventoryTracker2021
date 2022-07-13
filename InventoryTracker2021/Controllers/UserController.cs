using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using InventoryTracker2021.Context;
using InventoryTracker2021.Models;
using InventoryTracker2021.Models.Email;

namespace InventoryTracker2021.Controllers
{
    public class UserController : Controller
    {
        private InventoryContext _inventory = new InventoryContext();

        // Login
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(User _user)
        {
            _user.binPasswordHash = Crypto.SHA256(_user.chrUser.ToString() + _user.binPasswordHash.ToString());
            var usr = _inventory.Users.Where(u => u.chrUser == _user.chrUser && u.binPasswordHash == _user.binPasswordHash).FirstOrDefault();
            if (usr != null)
            {
                Session["UserID"] = usr.intUserID.ToString();
                Session["Username"] = usr.chrUser.ToString();
                return RedirectToAction("Index", "Locations");
            }
            else
            {
                ModelState.AddModelError("", "Username or Password is incorrect.");
            }
            return View();
        }

        // LoggedIn
        public ActionResult LoggedIn()
        {
            if (Session["UserID"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        // Logout
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }

        // Registration
        public ActionResult Register()
        {
            if (Session["UserID"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
            //return View();
        }

        [HttpPost]
        public ActionResult Register(User userToCreate)
        {
            if (Session["UserID"] != null)
            {
                if (ModelState.IsValid)
                {
                    User newUser = new User();
                    newUser.intUserID = userToCreate.intUserID;
                    newUser.chrUser = userToCreate.chrUser;
                    newUser.chrEmail = userToCreate.chrEmail;
                    //MD5 md5 = new MD5CryptoServiceProvider();
                    //Byte[] originalBytes = ASCIIEncoding.Default.GetBytes(userToCreate.binPasswordHash);
                    //Byte[] encodedBytes = md5.ComputeHash(originalBytes);
                    //userToCreate.binPasswordHash = Convert.ToBase64String(encodedBytes);
                    newUser.binPasswordHash = Crypto.SHA256(userToCreate.chrUser.ToString() + userToCreate.binPasswordHash.ToString());
                    _inventory.Users.Add(newUser);
                    _inventory.SaveChanges();
                }
                ModelState.Clear();
                ViewBag.Message = userToCreate.chrUser + " account successfully register.";
                return View();
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

        public ActionResult Details()
        {
            if (Session["UserID"] != null)
            {
                int id = Int32.Parse(Session["UserID"].ToString());

                var user = _inventory.Users.Where(u => u.intUserID == id).FirstOrDefault();

                return View(user);
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
            //return View();
        }

        [HttpPost]
        public ActionResult Details(User userToUpdate)
        {
            if (Session["UserID"] != null)
            {
                var user = _inventory.Users.Where(u => u.intUserID == userToUpdate.intUserID).FirstOrDefault();

                //user.chrUser = userToUpdate.chrUser;
                user.chrEmail = userToUpdate.chrEmail;
                user.binResetToken = null;
                //user.dtmResetRequestDate = null; // Save the request date for tracking 

                try
                {
                    _inventory.SaveChangesAsync();

                    Session["Username"] = user.chrUser.ToString();

                    ModelState.Clear();
                    ViewBag.Message = userToUpdate.chrUser + " account successfully updated.";
                }
                catch
                {
                    ModelState.Clear();
                    ViewBag.Message = userToUpdate.chrUser + " account could not be updated.";
                }
                
                return View(user);
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

        public ActionResult ForgotPassword(string name)
        {
            ViewBag.UserName = name;

            return View();
        }

        public async Task<JsonResult> PasswordRequest(string user)
        {
            User _user;

            _user = _inventory.Users.Where(u => u.chrUser == user || u.chrEmail == user).FirstOrDefault();

            if(_user == null)
            {
                return null;
            }
            else
            {
                // Generate new guid
                _user.binResetToken = Guid.NewGuid().ToString();
                _user.dtmResetRequestDate = DateTime.Now;

                try
                {
                    await _inventory.SaveChangesAsync();
                }
                catch
                {
                    return null;
                }

                var EmailBody = new PasswordRequestBody();
                var body = EmailBody.Body;
                var message = new MailMessage();
                message.To.Add(new MailAddress(_user.chrEmail));
                message.From = new MailAddress("noreply@electionpeople.com");  // replace with valid value
                message.Subject = "Forgotten Storage Inventory Password";
                string userName = _user.chrUser;
                string uniqueId = "https://www.electionpeople.com/storageinventory2demo/User/Reset?xuin=" + _user.binResetToken;

                string htmlBody = "<html><body><h1>Picture</h1><br><img src=\"cid:filename\"></body></html>";
                AlternateView av1Html = AlternateView.CreateAlternateViewFromString
                   (htmlBody, null, MediaTypeNames.Text.Html);

                String path = Server.MapPath("~\\Images\\AESContactInfo.png");

                try
                {
                    LinkedResource inline = new LinkedResource(path, "image/png");
                    inline.ContentId = Guid.NewGuid().ToString();
                    av1Html.LinkedResources.Add(inline);
                }
                catch (Exception e)
                {
                    return null;
                }

                Attachment att1 = null;
                try
                {
                    att1 = new Attachment(path);
                    att1.ContentDisposition.Inline = true;
                }
                catch (Exception e)
                {
                    return null;
                }

                try
                {
                    //message.Body = body;
                    message.Body = string.Format(body,
                        userName,
                        uniqueId,
                        att1.ContentId);
                }
                catch (Exception e)
                {
                    return null;
                }
                message.IsBodyHtml = true;
                message.Attachments.Add(att1);

                using (var smtp = new SmtpClient())
                {
                    var credential = new NetworkCredential
                    {
                        UserName = "noreply@gmail.com",  // replace with valid value
                        Password = "XXXX"  // replace with valid value
                    };
                    smtp.Credentials = credential;
                    smtp.Host = "XXXX";
                    smtp.Port = 0000;
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(message);
                }
            }

            var redirectUrl = new UrlHelper(Request.RequestContext).Action("RequestSent", "User", new { name = user });
            return Json(new { Url = redirectUrl });
        }

        public ActionResult RequestSent(string name)
        {
            ViewBag.UserName = name;

            return View();
        }

        public ActionResult Reset(string xuin)
        {
            if (xuin != null || xuin != Session["UserID"].ToString())
            {
                // Get user from the unique id
                var user = _inventory.Users.Where(u => u.binResetToken == xuin || u.intUserID.ToString() == xuin).FirstOrDefault();

                if (user != null)
                {
                    // Check the date the request was sent
                    DateTime startTime = (DateTime)(user.dtmResetRequestDate??DateTime.MinValue);
                    DateTime endTime = DateTime.Now;

                    // Get the difference
                    TimeSpan timeSpan = endTime.Subtract(startTime);

                    // Check for thirty minute window
                    if (timeSpan.Minutes <= 30)
                    //if(true)
                    {
                        PasswordResetModel newPassword = new PasswordResetModel();
                        newPassword.UserID = user.intUserID;
                        newPassword.UserName = user.chrUser;

                        return View(newPassword);
                    }
                    else
                    {
                        user.binResetToken = null;
                        user.dtmResetRequestDate = null;                        

                        try
                        {
                            _inventory.SaveChanges();
                        }
                        catch
                        {

                        }

                        return RedirectToAction("RequestExpired");
                    }
                }
                else
                {
                    return RedirectToAction("RequestExpired");
                }
            }
            else
            {
                return RedirectToAction("RequestExpired");
            }
        }

        //public ActionResult Reset()
        //{
        //    if (Session["UserID"] != null)
        //    {
        //        return View();
        //    }
        //    else
        //    {
        //        return RedirectToAction("Login");
        //    }
        //}

        [HttpPost]
        public async Task<ActionResult> Reset(PasswordResetModel userToChange)
        {
            var user = _inventory.Users.Where(u => u.intUserID == userToChange.UserID).FirstOrDefault();

            if (user != null)
            {
                // Check password 1
                if (userToChange.Password1 != null && userToChange.Password1 != "")
                {
                    // Check password 2
                    if (userToChange.Password2 != null && userToChange.Password2 != "")
                    {
                        if (userToChange.Password1 == userToChange.Password2)
                        {
                            // Create password hash
                            user.binPasswordHash = Crypto.SHA256(userToChange.UserName.ToString() + userToChange.Password1.ToString());
                            user.binResetToken = null;
                            //user.dtmResetRequestDate = null; // Dont delete the request date, it still records the last time a password change was requested
                            user.dtmPasswordChangedDate = DateTime.Now;

                            // Save new password and delete change request fields
                            try
                            {
                                _inventory.SaveChanges();
                            }
                            catch
                            {
                                PasswordResetModel newPassword = new PasswordResetModel();
                                newPassword.UserID = user.intUserID;
                                newPassword.UserName = user.chrUser;

                                ModelState.AddModelError("", "Password change failed.");
                                return View(newPassword);
                            }

                            // Send password reset success email (Confirmation email)
                            await Task.Run(() => SuccessEmail(user));

                            return RedirectToAction("Success");
                        }
                        else
                        {
                            PasswordResetModel newPassword = new PasswordResetModel();
                            newPassword.UserID = user.intUserID;
                            newPassword.UserName = user.chrUser;

                            ModelState.AddModelError("", "Passwords do not match.");
                            return View(newPassword);
                        }
                    }
                    else
                    {
                        PasswordResetModel newPassword = new PasswordResetModel();
                        newPassword.UserID = user.intUserID;
                        newPassword.UserName = user.chrUser;

                        ModelState.AddModelError("", "Please confirm the password.");
                        return View(newPassword);
                    }
                }
                else
                {
                    PasswordResetModel newPassword = new PasswordResetModel();
                    newPassword.UserID = user.intUserID;
                    newPassword.UserName = user.chrUser;

                    ModelState.AddModelError("", "Please enter a valid password.");
                    return View(newPassword);
                }
            }
            else
            {
                return RedirectToAction("RequestExpired");
            }
        }

        public async void SuccessEmail(User user)
        {
            var EmailBody = new PasswordConfirmationBody();
            var body = EmailBody.Body;
            var message = new MailMessage();
            message.To.Add(new MailAddress(user.chrEmail));
            message.From = new MailAddress("noreply@electionpeople.com");  // replace with valid value
            message.Subject = "Storage Inventory Password Changed";
            string userName = user.chrUser;

            string htmlBody = "<html><body><h1>Picture</h1><br><img src=\"cid:filename\"></body></html>";
            AlternateView av1Html = AlternateView.CreateAlternateViewFromString
               (htmlBody, null, MediaTypeNames.Text.Html);

            String path = Server.MapPath("~\\Images\\AESContactInfo.png");

            try
            {
                LinkedResource inline = new LinkedResource(path, "image/png");
                inline.ContentId = Guid.NewGuid().ToString();
                av1Html.LinkedResources.Add(inline);
            }
            catch
            {
                return;
            }

            Attachment att1 = null;
            try
            {
                att1 = new Attachment(path);
                att1.ContentDisposition.Inline = true;
            }
            catch
            {
                return;
            }

            try
            {
                //message.Body = body;
                message.Body = string.Format(body,
                    userName,
                    att1.ContentId);
            }
            catch
            {
                return;
            }
            message.IsBodyHtml = true;
            message.Attachments.Add(att1);

            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = "noreply@electionpeople.com",    // replace with valid value
                    Password = "@u+0v0+3***"                    // replace with valid value
                };
                smtp.Credentials = credential;
                smtp.Host = "smtp.office365.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(message);
            }
        }

        public ActionResult RequestExpired()
        {
            return View();
        }

        public ActionResult Success()
        {
            return View();
        }
    }
}