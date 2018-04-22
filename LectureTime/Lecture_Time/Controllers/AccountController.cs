using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Lecture_Time.Models;
using System.DirectoryServices.Protocols;
using System.Net;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Lecture_Time.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        #region ctors
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext _db;

        private LectureTimeDbApi LTDbApi = new LectureTimeDbApi();

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationDbContext db )
        {
            UserManager = userManager;
            SignInManager = signInManager;
            Db = db;
        }

        public ApplicationDbContext Db
        {
            get
            {
                return _db ?? HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            }
            private set
            {
                _db = value;
            }
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        #endregion

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            

            if (!Helpers.ValidateLDAP(model))
            {
                ModelState.AddModelError("CredentialsError", "Invalid credentials.");
                return View(model);
            }

            string[] userData = Helpers.GetUserData(model.UserName);
            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = userData[2],
            };

            if (UserManager.FindByName(model.UserName) == null)
            {
                var registerResult = await UserManager.CreateAsync(user, model.Password);
                if (registerResult.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    // Refactorize this
                    var userModel = new Models.LTUser
                    {
                        UserName = model.UserName,
                        FirstName = userData[0],
                        LastName = userData[1],
                        Email = userData[2],
                        IsLecturer = userData[2].Contains("@st") ? false : true,
                        IsStudent = userData[2].Contains("@st") ? true : false,
                    };
                    if (userModel.IsLecturer)
                    {
                        LTDbApi.AddLecturer(new Lecturer
                        {
                            LTUser = userModel,                          
                        });
                    }
                    else
                    {
                        LTDbApi.AddUser(userModel);
                    }
                    return RedirectToAction("Index", "Home");
                }

            }

            // TODO: get this done
            //#if DEBUG   
            //var userModel2 = new Models.LTUser
            //{
            //    UserName = model.UserName,
            //    FirstName = userData[0],
            //    LastName = userData[1],
            //    Email = userData[2],
            //    IsLecturer = true,
            //    IsStudent = true,
            //    IsAdmin = true
            //};
            //LTDbApi.AddLecturer(new Lecturer
            //{
            //    LTUser = userModel2,
            //});
            //#else
            //#endif


            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }


        protected override void Dispose(bool disposing)
        {

            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

       

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}