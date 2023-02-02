using AntalyaTaksiAccount.Models;
using BilgeCryptoHelpers;
using BilgeCryptoHelpers.PasswordControlUtilsHelpers;
using AntalyaTaksiAccount.Models;
//using AntalyaTaksiAccount.Models.DummyModels;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;

namespace AntalyaTaksiAccount.Utils
{
    public class Helper
    {
        

        public static bool PasswordControl(string pass)
        {
            var Rules = new List<IRule>();

            IRule lowerCaseRule = new ClassicalRule()
            {
                MinValidValue = 2,
                ErrorMessage = "Not Enough LowerCase Letters Present",
                RegexValue = "[^a-z]",
                RuleName = "LowerCase"
            };
            Rules.Add(lowerCaseRule);

            Rule upperCaseRule = new ClassicalRule()
            {
                MinValidValue = 1,
                ErrorMessage = "Not Enough UpperCase Letters Present",
                RegexValue = "[^A-Z]",
                RuleName = "UpperCase"
            };
            Rules.Add(upperCaseRule);

            Rule digitRule = new ClassicalRule()
            {
                MinValidValue = 1,
                ErrorMessage = "Not Enough Digits  Present",
                RegexValue = "[^0-9]",
                RuleName = "Digit"
            };
            Rules.Add(digitRule);

            Rule symbolRules = new ClassicalRule()
            {
                MinValidValue = 1,
                ErrorMessage = "Not Enough Symbols  Present",
                RegexValue = "[^.:^,; *?= !&\\-_]",
                RuleName = "Symbols"
            };
            Rules.Add(symbolRules);

            
            PasswordControlUtils passwordControlUtils = new PasswordControlUtils();
            passwordControlUtils.Rules = Rules;

            (bool result, List<string> errorResults) = passwordControlUtils.IsPasswordValidWithAllMessageWithRules(pass);
            return result;
        }
        public static bool UnicEmailControl(string email,ATAccountContext aTAccountContext)
        {
            try
            {
                if (string.IsNullOrEmpty(email.Trim()))
                {
                    return false;
                }
                if (!email.Contains("@"))
                {
                    return false;
                }
                var any= aTAccountContext.AllUsers.Any(c => c.Activity == 1 && c.MailAdress == email);
                return !any;
                    
            }
            catch (Exception)
            {
                return false;
            }
        }


        public static bool UnicIdNoControl(string idNo,ATAccountContext aTAccountContext)
        {
            if (string.IsNullOrEmpty(idNo))
            {
                return false;
            }

            try
            {
                var any = aTAccountContext.Drivers.Any(c => c.Activity == 1 && c.IdNo == idNo);
                return !any;

            }
            catch (Exception)
            {
                return false;
            }

        }

        public static bool UnicPhoneNumberControl(string phoneNumber, ATAccountContext aTAccountContext) 
        {
            if (string.IsNullOrEmpty(phoneNumber))
            {
                return false;
            }

            try
            {
                var any = aTAccountContext.Drivers.Any(c => c.Activity == 1 && c.AllUser.Phone == phoneNumber);
                return !any;

            }
            catch (Exception)
            {
                return false;
            }

        }

     //should be send mail codes of service from masteraccountapi
        public static string GeneratePassword()
        {
            try
            {
                string newPass = "";
                string[] passBigChar = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "R", "S", "T", "U", "V", "Y", "Z" };
                string[] passSmallChar = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "r", "s", "t", "u", "v", "y", "z" };
                string[] passNum = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
                string[] passSym = new string[] { "*", ".", "!", "-", "?" };
                List<string[]> arrays = new List<string[]>() { passBigChar, passNum, passSym, passSmallChar };
                Random random = new Random();
                for (int i = 0; i < 8; i++)
                {
                    string[] gecici = arrays[random.Next(0, 3)];
                    newPass = newPass + gecici[random.Next(0, gecici.Length - 1)];
                }
                return newPass;
            }
            catch (Exception)
            {

                return "";
            }
        }
        public static string PasswordEncode(string sData)
        {
            try
            {

                byte[] bytesToBeEncrypted = Encoding.UTF8.GetBytes(sData); // sifrelenecek veri
                byte[] passwordBytes = Encoding.UTF8.GetBytes("testsalt"); //salt
                passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

                byte[] bytesEncrypted = AES_Encrypt(bytesToBeEncrypted, passwordBytes);
                string encryptedResult = Convert.ToBase64String(bytesEncrypted); // sifreli veri

                return encryptedResult;

            }
            catch (Exception)
            {

                return "";
            }
        }
        public static string PasswordDecode(string sData)

        {
            try
            {
                byte[] bytesToBeDecrypted = Convert.FromBase64String(sData); // sifreli veri
                byte[] passwordBytesdecrypt = Encoding.UTF8.GetBytes("testsalt"); // salt
                passwordBytesdecrypt = SHA256.Create().ComputeHash(passwordBytesdecrypt);

                byte[] bytesDecrypted = AES_Decrypt(bytesToBeDecrypted, passwordBytesdecrypt);
                string decryptedResult = Encoding.UTF8.GetString(bytesDecrypted); // veri
                return decryptedResult;
            }
            catch (Exception ex)
            {

                return "";
            }
        }
        public static byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;
            byte[] saltBytes = new byte[] { 2, 1, 7, 3, 6, 4, 8, 5 };
            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;
                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);
                    AES.Mode = CipherMode.CBC;
                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }
        public static byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes = null;
            byte[] saltBytes = new byte[] { 2, 1, 7, 3, 6, 4, 8, 5 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;
                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);
                    AES.Mode = CipherMode.CBC;
                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }
                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }
        
    }
}
