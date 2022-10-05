namespace LoginService
{
    public class AuthCodeGenerator
    {
        public const char _CHR_SEPARATOR = '`';

        private static readonly Dictionary<string, string> codes = new()
        {
            { "0", "u" },
            { "1", "o" },
            { "2", "i" },
            { "3", "e" },
            { "4", "a" },
            { "5", "z" },
            { "6", "y" },
            { "7", "x" },
            { "8", "w" },
            { "9", "v" },
            { "A", "t" },
            { "a", "s" },
            { "B", "r" },
            { "b", "q" },
            { "C", "p" },
            { "c", "n" },
            { "D", "m" },
            { "d", "l" },
            { "E", "k" },
            { "e", "j" },
            { "F", "h" },
            { "f", "g" },
            { "G", "f" },
            { "g", "d" },
            { "H", "c" },
            { "h", "b" },
            { "I", "0" },
            { "i", "9" },
            { "J", "8" },
            { "j", "7" },
            { "K", "6" },
            { "k", "5" },
            { "L", "4" },
            { "l", "3" },
            { "M", "2" },
            { "m", "1" },
            { "N", "B" },
            { "n", "C" },
            { "O", "D" },
            { "o", "F" },
            { "P", "G" },
            { "p", "H" },
            { "Q", "J" },
            { "q", "K" },
            { "R", "L" },
            { "r", "M" },
            { "S", "N" },
            { "s", "P" },
            { "T", "Q" },
            { "t", "R" },
            { "U", "S" },
            { "u", "T" },
            { "V", "V" },
            { "v", "W" },
            { "W", "X" },
            { "w", "Y" },
            { "X", "Z" },
            { "x", "A" },
            { "Y", "E" },
            { "y", "I" },
            { "Z", "O" },
            { "z", "U" },
        };

        /// <summary>
        /// convert user data to authentication code
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static string Encrypt(Models.User user)
        {
            // combine username`password
            var userInfo = $"{user.Username}{_CHR_SEPARATOR}{user.Password}".Replace(" ", string.Empty);
            // convert user info by codes mapping
            var authenticationCode = string.Empty;
            for (var i = 0; i < userInfo.Length; i++)
            {
                try // replace match
                {
                    authenticationCode += codes[userInfo.Substring(i, 1)];
                }
                catch // if no match, just append it
                {
                    authenticationCode += userInfo.Substring(i, 1);
                }
            }

            return authenticationCode;
        }

        /// <summary>
        /// convert authentication code to user data
        /// </summary>
        /// <param name="authenticationCode"></param>
        /// <returns></returns>
        public static Models.User? Decrypt(string authenticationCode)
        {
            if (string.IsNullOrWhiteSpace(authenticationCode)) return null;
            // split username`password
            var authCode = authenticationCode.Split(new char[] { _CHR_SEPARATOR });
            // return nothing if invalid authentication code
            if (authCode.Length < 2) return null;
            // parse as user data
            return new()
            {
                Username = authCode[0],
                Password = authCode[1]
            };
        }

        /// <summary>
        /// decipher keys based on codes mapping
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static string Decipher(string keys)
        {
            var values = string.Empty;
            for (var i = 0; i < keys.Length; i++)
            {
                values += codes.FirstOrDefault(x => x.Value.Equals(keys.Substring(i, 1))).Key;
            }
            return values;
        }
    }
}
