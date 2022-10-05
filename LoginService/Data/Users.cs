namespace LoginService.Data
{
    public class Users
    {
        private static readonly Models.User[] UsersData = new[] // mock user data
        {
            new Models.User() { Username = "user1", Password = "password1", DOB = new DateTime(1980, 01, 31), Gender = "M", Name = "Dick Black", Address = "Melbourne, AU", Email = "bigblack_d@hotmail.com", Phone = "0987654321" },
            new Models.User() { Username = "user2", Password = "password2", DOB = new DateTime(1979, 12, 31), Gender = "M", Name = "Tiny Johnson", Address = "Manila, PH", Email = "teejay1979@yehey.com.ph", Phone = "0864297531" },
            new Models.User() { Username = "user3", Password = "password3", DOB = new DateTime(2001, 06, 30), Gender = "F", Name = "Nadjia Cole", Address = "Riyadh, SA", Email = "nadjiacole@google.com", Phone = "0192837465" },
        };

        /// <summary>
        /// get user by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static Models.User? GetUserByUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return null;
            // not case-sensitive
            return UsersData
                .Where(u => u.Username.Trim().ToUpper().Equals(username.Trim().ToUpper()))
                .FirstOrDefault();
        }

        /// <summary>
        /// get all users
        /// </summary>
        /// <param name="authenticationCode"></param>
        /// <returns></returns>
        public static IEnumerable<Models.User>? GetUsers(string authenticationCode)
        {
            // must be authenticated to retrieve this data
            return IsAuthenticated(authenticationCode) ? UsersData.AsEnumerable() : null;
        }

        /// <summary>
        /// validate authentication code
        /// </summary>
        /// <param name="authenticationCode"></param>
        /// <returns></returns>
        public static bool IsAuthenticated(string authenticationCode)
        {
            if (string.IsNullOrWhiteSpace(authenticationCode)) return false;
            // split username`password
            var authCode = authenticationCode.Split(new char[] { AuthCodeGenerator._CHR_SEPARATOR });
            // find the user data
            var user = GetUserByUsername(AuthCodeGenerator.Decipher(authCode[0])); 
            // validate if authentication code is matched with user data
            return user == null 
                ? false 
                : (string.IsNullOrWhiteSpace(user.Password) 
                    ? false 
                    : user.Password.Equals(AuthCodeGenerator.Decipher(authCode[1])));
        }
    }
}
