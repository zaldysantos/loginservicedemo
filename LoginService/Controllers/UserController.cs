using Microsoft.AspNetCore.Mvc;

namespace LoginService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        public UserController()
        {
        }

        [HttpPost]
        [Route("login")]
        public ViewModels.Response Login([FromBody] Models.User user)
        {
            var requestContext = HttpContext.Request;

            user ??= new Models.User();
            user.Username = string.IsNullOrWhiteSpace(user.Username) ? string.Empty : user.Username.Trim().ToUpper();
            user.Password = string.IsNullOrWhiteSpace(user.Password) ? string.Empty : user.Password.Trim();

            var response = new ViewModels.Response
            {
                Service = $"{requestContext.Method} {requestContext.Path}",
                Args = user,
                Timestamp = DateTime.Now,
                Success = false,
                Data = null,
                Message = "Login failed."
            };

            try
            {
                var data = Data.Users.GetUserByUsername(user.Username); // find user data
                // check if password is correct
                data = data == null ? null : (string.IsNullOrWhiteSpace(data.Password) ? null : (data.Password.Trim().Equals(user.Password) ? data : null)); 

                response.Success = data != null; // if user data found, assume success
                if (response.Success)
                {
                    response.Data = AuthCodeGenerator.Encrypt(user); // generate authentication code
                    response.Message = "You are now logged-in.";

                    user.Password = "(redacted)"; // hide the password
                    response.Args = user; // the user data
                }
            }
            catch (Exception err)
            {
                response.Message = $"{err.Message} {Environment.NewLine} {err.StackTrace}";
            }

            return response;
        }

        [HttpGet]
        [Route("getAll")]
        public ViewModels.Response GetAll([FromHeader] string authenticationCode)
        {
            var requestContext = HttpContext.Request;
            // validate authentication code
            var isAuthenticated = Data.Users.IsAuthenticated(authenticationCode);

            return new ViewModels.Response
            {
                Service = $"{requestContext.Method} {requestContext.Path}",
                Args = authenticationCode,
                Timestamp = DateTime.Now,
                Success = isAuthenticated,
                Data = Data.Users.GetUsers(authenticationCode),
                Message = isAuthenticated ? "Success" : "Failed"
            };
        }
    }
}
