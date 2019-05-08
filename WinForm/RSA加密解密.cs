using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace DataConsole
{
    class RSAConvert
    {
        /// <summary>
        /// RSA解密算法
        /// </summary>
        /// <param name="encryptedInput"></param>
        /// <returns></returns>
        public static string RsaDecrypt(string encryptedInput)
        {
            try
            {
                if (string.IsNullOrEmpty(encryptedInput))
                {
                    return string.Empty;
                }
                using (var rsaProvider = new RSACryptoServiceProvider())
                {
                    var inputBytes = Convert.FromBase64String(encryptedInput);
                    rsaProvider.FromXmlString("<RSAKeyValue><Modulus>iSGL1eyBMxzpuL3YLvEVnPYrdPYIFwijQDBLXEDOJF09ZkMhJ2xHhlWDsiVk3do1YaU6MTEOK/afRCndWzApQAPyoVI0Av5fWHUtzBQGQuuVeOwcegk2neG2aNRNQ+sdazo0HuJQDcVpwKcGOED2BhcRJkA98jchK4d/FZTS9hk=</Modulus><Exponent>AQAB</Exponent><P>3EZ3p8+ggITL87UtOAD5Zct7obyxiFbF5BnbJr+yZbUbxZmf9TKqbwGwulq8xTJVnhYGM+hMgYqnx+puAm3SWw==</P><Q>n18K40E3r9WzPlbhIAEs2SiY68iZGCmzR/0n6PR48NtFSGsSDRIR632oeftBPfN7W4+kUIehL2gt9RgnRH8bmw==</Q><DP>kxgrh1BLKgeD+rad/6wG30dGw/axxw3LEEuD4RhaFTkf1pCEFMVEsuQ6E/fL3xOBwROMCNWzYT4qVIHj/JNByQ==</DP><DQ>fhrFUbbcKm0NDKnQLvPfGeHuxrsOsLjmXHMrbkBrpiHKu5fFS8Rdm5ntgr/WCwArPvL1EKeRWiK0Iri+8YsmOw==</DQ><InverseQ>FvOxbsfBlV7lEyJGyPFfu/Q2Dg2czzm2dfQZSaSm6LE6l9yrc+xmju6NwG8nkgm9tejhC64hEpKbEi0cjbq2Qg==</InverseQ><D>FxGmpZFI1uFpTCPbx2HVQfeDrgRprf5NAFJfiyB3zVRGLPrkC+7CRY4DPqfdxRidXFTgakAXYzv05RGp5FpAxf1GBAR6HQxVcntBpNqJUo2UBP+IrXgPFDPdodAl9SWgaHKwc79pCARVdJutm86kRRsy5rjcWpR8HQCYWzk/lmU=</D></RSAKeyValue>");
                    int bufferSize = rsaProvider.KeySize / 8;
                    var buffer = new byte[bufferSize];
                    using (MemoryStream inputStream = new MemoryStream(inputBytes),
                         outputStream = new MemoryStream())
                    {
                        while (true)
                        {
                            int readSize = inputStream.Read(buffer, 0, bufferSize);
                            if (readSize <= 0)
                            {
                                break;
                            }

                            var temp = new byte[readSize];
                            Array.Copy(buffer, 0, temp, 0, readSize);
                            var rawBytes = rsaProvider.Decrypt(temp, false);
                            outputStream.Write(rawBytes, 0, rawBytes.Length);
                        }
                        return Encoding.UTF8.GetString(outputStream.ToArray());
                    }
                }
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}
