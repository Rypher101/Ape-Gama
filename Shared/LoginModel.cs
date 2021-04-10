using System.Security.Cryptography;
using System.Text;

namespace ApeGama.Shared
{
    public class LoginModel
    {
        public string userEmalil { get; set; }
        public string userPassword { get; set; }
        public void ShaEnc()
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(userPassword));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                userPassword = builder.ToString();
            }
        }
    }
}
