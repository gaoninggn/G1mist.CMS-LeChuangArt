using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace G1mist.CMS.Common
{
    public class RsaCryptoService
    {
        private readonly RSACryptoServiceProvider _privateKeyRsaProvider;
        private readonly RSACryptoServiceProvider _publicKeyRsaProvider;

        public RsaCryptoService(string privateKey, string publicKey = null)
        {
            if (!string.IsNullOrEmpty(privateKey))
            {
                _privateKeyRsaProvider = CreateRsaProviderFromPrivateKey(privateKey);
            }

            if (!string.IsNullOrEmpty(publicKey))
            {
                _publicKeyRsaProvider = CreateRsaProviderFromPublicKey(publicKey);
            }
        }

        public string Decrypt(string cipherText)
        {
            if (_privateKeyRsaProvider == null)
            {
                throw new Exception("_privateKeyRsaProvider is null");
            }
            return Encoding.UTF8.GetString(_privateKeyRsaProvider.Decrypt(Convert.FromBase64String(cipherText), false));
        }

        public string Encrypt(string text)
        {
            if (_publicKeyRsaProvider == null)
            {
                throw new Exception("_publicKeyRsaProvider is null");
            }
            return Convert.ToBase64String(_publicKeyRsaProvider.Encrypt(Encoding.UTF8.GetBytes(text), false));
        }

        private RSACryptoServiceProvider CreateRsaProviderFromPrivateKey(string privateKey)
        {
            var privateKeyBits = Convert.FromBase64String(privateKey);

            var rsa = new RSACryptoServiceProvider();
            var rsAparams = new RSAParameters();

            using (var binr = new BinaryReader(new MemoryStream(privateKeyBits)))
            {
                var twobytes = binr.ReadUInt16();
                switch (twobytes)
                {
                    case 0x8130:
                        binr.ReadByte();
                        break;
                    case 0x8230:
                        binr.ReadInt16();
                        break;
                    default:
                        throw new Exception("Unexpected value read binr.ReadUInt16()");
                }

                twobytes = binr.ReadUInt16();
                if (twobytes != 0x0102)
                    throw new Exception("Unexpected version");

                var bt = binr.ReadByte();
                if (bt != 0x00)
                    throw new Exception("Unexpected value read binr.ReadByte()");

                rsAparams.Modulus = binr.ReadBytes(GetIntegerSize(binr));
                rsAparams.Exponent = binr.ReadBytes(GetIntegerSize(binr));
                rsAparams.D = binr.ReadBytes(GetIntegerSize(binr));
                rsAparams.P = binr.ReadBytes(GetIntegerSize(binr));
                rsAparams.Q = binr.ReadBytes(GetIntegerSize(binr));
                rsAparams.DP = binr.ReadBytes(GetIntegerSize(binr));
                rsAparams.DQ = binr.ReadBytes(GetIntegerSize(binr));
                rsAparams.InverseQ = binr.ReadBytes(GetIntegerSize(binr));
            }

            rsa.ImportParameters(rsAparams);
            return rsa;
        }

        private static int GetIntegerSize(BinaryReader binr)
        {
            int count;
            var bt = binr.ReadByte();
            if (bt != 0x02)
                return 0;
            bt = binr.ReadByte();

            if (bt == 0x81)
                count = binr.ReadByte();
            else
                if (bt == 0x82)
                {
                    var highbyte = binr.ReadByte();
                    var lowbyte = binr.ReadByte();
                    byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                    count = BitConverter.ToInt32(modint, 0);
                }
                else
                {
                    count = bt;
                }

            while (binr.ReadByte() == 0x00)
            {
                count -= 1;
            }
            binr.BaseStream.Seek(-1, SeekOrigin.Current);
            return count;
        }

        public RSACryptoServiceProvider CreateRsaProviderFromPublicKey(string publicKeyString)
        {
            // encoded OID sequence for  PKCS #1 rsaEncryption szOID_RSA_RSA = "1.2.840.113549.1.1.1"
            byte[] seqOid = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };

            var x509Key = Convert.FromBase64String(publicKeyString);

            // ---------  Set up stream to read the asn.1 encoded SubjectPublicKeyInfo blob  ------
            using (var mem = new MemoryStream(x509Key))
            {
                using (var binr = new BinaryReader(mem))  //wrap Memory Stream with BinaryReader for easy reading
                {
                    var twobytes = binr.ReadUInt16();
                    switch (twobytes)
                    {
                        case 0x8130:
                            binr.ReadByte();    //advance 1 byte
                            break;
                        case 0x8230:
                            binr.ReadInt16();   //advance 2 bytes
                            break;
                        default:
                            return null;
                    }

                    var seq = binr.ReadBytes(15);
                    if (!CompareBytearrays(seq, seqOid))    //make sure Sequence for OID is correct
                        return null;

                    twobytes = binr.ReadUInt16();
                    switch (twobytes)
                    {
                        case 0x8103:
                            binr.ReadByte();    //advance 1 byte
                            break;
                        case 0x8203:
                            binr.ReadInt16();   //advance 2 bytes
                            break;
                        default:
                            return null;
                    }

                    var bt = binr.ReadByte();
                    if (bt != 0x00)     //expect null byte next
                        return null;

                    twobytes = binr.ReadUInt16();
                    switch (twobytes)
                    {
                        case 0x8130:
                            binr.ReadByte();    //advance 1 byte
                            break;
                        case 0x8230:
                            binr.ReadInt16();   //advance 2 bytes
                            break;
                        default:
                            return null;
                    }

                    twobytes = binr.ReadUInt16();
                    byte lowbyte;
                    byte highbyte = 0x00;

                    switch (twobytes)
                    {
                        case 0x8102:
                            lowbyte = binr.ReadByte();  // read next bytes which is bytes in modulus
                            break;
                        case 0x8202:
                            highbyte = binr.ReadByte(); //advance 2 bytes
                            lowbyte = binr.ReadByte();
                            break;
                        default:
                            return null;
                    }
                    byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };   //reverse byte order since asn.1 key uses big endian order
                    var modsize = BitConverter.ToInt32(modint, 0);

                    var firstbyte = binr.PeekChar();
                    if (firstbyte == 0x00)
                    {   //if first byte (highest order) of modulus is zero, don't include it
                        binr.ReadByte();    //skip this null byte
                        modsize -= 1;   //reduce modulus buffer size by 1
                    }

                    var modulus = binr.ReadBytes(modsize);   //read the modulus bytes

                    if (binr.ReadByte() != 0x02)            //expect an Integer for the exponent data
                        return null;
                    var expbytes = (int)binr.ReadByte();        // should only need one byte for actual exponent data (for all useful values)
                    var exponent = binr.ReadBytes(expbytes);

                    // ------- create RSACryptoServiceProvider instance and initialize with public key -----
                    var rsa = new RSACryptoServiceProvider();
                    var rsaKeyInfo = new RSAParameters
                    {
                        Modulus = modulus,
                        Exponent = exponent
                    };
                    rsa.ImportParameters(rsaKeyInfo);

                    return rsa;
                }

            }
        }

        private static bool CompareBytearrays(ICollection<byte> a, IList<byte> b)
        {
            if (a == null) throw new ArgumentNullException("a");
            if (a.Count != b.Count)
                return false;
            var i = 0;
            foreach (byte c in a)
            {
                if (c != b[i])
                    return false;
                i++;
            }
            return true;
        }
    }
}
