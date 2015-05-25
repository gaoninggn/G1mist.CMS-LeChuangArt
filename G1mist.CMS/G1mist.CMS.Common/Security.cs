using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace G1mist.CMS.Common
{
    public class Security
    {
        /// <summary>
        /// MD5　32位加密
        /// </summary>
        /// <param name="str">要加密的字符串</param>
        /// <param name="salt">加密盐</param>
        /// <returns></returns>
        public static string SetMD5(string str, string salt)
        {
            string cl = str + salt;
            string pwd = "";
            MD5 md5 = MD5.Create();//实例化一个md5对像
            // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 

                pwd = pwd + s[i].ToString("X");

            }
            return pwd;
        }

        /// <summary>
        /// 生成加密盐,定死生成6位加密盐
        /// </summary>
        /// <returns></returns>
        public static string GetPwdSalt()
        {
            var random = new Random();
            var salt = string.Empty;

            const string allChar = "0,1,2,3,4,5,6,7,8,9,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,W,X,Y,Z,a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z";

            var allCharArray = allChar.Split(',');

            for (int i = 0; i < 6; i++)
            {
                var seed = random.Next(61);
                salt += allCharArray[seed];
            }
            return salt;
        }

        #region AES 加密解密
        /// <summary>
        /// 获取密钥
        /// </summary>
        private static string Key
        {
            get { return @")O[NB]6,YF}+efcaj{+oESb9d8>Z'e9M"; }
        }

        /// <summary>
        /// 获取向量
        /// </summary>
        private static string Iv
        {
            get { return @"L+\~f4,Ir)b$=pkf"; }
        }

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="plainStr">明文字符串</param>
        /// <returns>密文</returns>
        private static string AesEncrypt(string plainStr)
        {
            var bKey = Encoding.UTF8.GetBytes(Key);
            var bIV = Encoding.UTF8.GetBytes(Iv);
            var byteArray = Encoding.UTF8.GetBytes(plainStr);

            string encrypt = null;
            var aes = Rijndael.Create();
            try
            {
                using (var mStream = new MemoryStream())
                {
                    using (var cStream = new CryptoStream(mStream, aes.CreateEncryptor(bKey, bIV), CryptoStreamMode.Write))
                    {
                        cStream.Write(byteArray, 0, byteArray.Length);
                        cStream.FlushFinalBlock();
                        encrypt = Convert.ToBase64String(mStream.ToArray());
                    }
                }
            }
            catch
            { }
            aes.Clear();

            return encrypt;
        }

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="plainStr">明文字符串</param>
        /// <param name="returnNull">加密失败时是否返回 null，false 返回 String.Empty</param>
        /// <returns>密文</returns>
        public static string AesEncrypt(string plainStr, bool returnNull)
        {
            string encrypt = AesEncrypt(plainStr);
            return returnNull ? encrypt : (encrypt ?? String.Empty);
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="encryptStr">密文字符串</param>
        /// <returns>明文</returns>
        private static string AesDecrypt(string encryptStr)
        {
            byte[] bKey = Encoding.UTF8.GetBytes(Key);
            byte[] bIV = Encoding.UTF8.GetBytes(Iv);
            byte[] byteArray = Convert.FromBase64String(encryptStr);

            string decrypt = null;
            Rijndael aes = Rijndael.Create();
            try
            {
                using (MemoryStream mStream = new MemoryStream())
                {
                    using (CryptoStream cStream = new CryptoStream(mStream, aes.CreateDecryptor(bKey, bIV), CryptoStreamMode.Write))
                    {
                        cStream.Write(byteArray, 0, byteArray.Length);
                        cStream.FlushFinalBlock();
                        decrypt = Encoding.UTF8.GetString(mStream.ToArray());
                    }
                }
            }
            catch { }
            aes.Clear();

            return decrypt;
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="encryptStr">密文字符串</param>
        /// <param name="returnNull">解密失败时是否返回 null，false 返回 String.Empty</param>
        /// <returns>明文</returns>
        public static string AesDecrypt(string encryptStr, bool returnNull)
        {
            string decrypt = AesDecrypt(encryptStr);
            return returnNull ? decrypt : (decrypt ?? String.Empty);
        }
        #endregion

    }
}
