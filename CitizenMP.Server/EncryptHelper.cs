// Decompiled with JetBrains decompiler
// Type: EncryptHelper
// Assembly: CitizenMP.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 05F7001E-4DA4-4F15-A443-96D9D1B18E6C
// Assembly location: C:\Users\MEGA\Downloads\Programs\CitizenMP.Server.exe

using System;
using System.Security.Cryptography;
using System.Text;

public class EncryptHelper
{
  private const string AESKey = "SA#%^433@!#$&#$%";
  private const string RSAPublicKey = "<RSAKeyValue><Modulus>36nIT5kA8A1N84POjl6T/sz+kRU8kDbUO0VKzpBl5dSVhoemlMa1YXa8X6gEXx6hicqazNbtSSLjqHvo4FEPRm8L8QS1U7bQ/DydMWcj96FirKbeFLJZGIAhBCZFDODWN9TAF/kHmyL6logvmuyvINDPv6voLN6YzXmt6FgleBk=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
  private const string RSAPrivateKey = "<RSAKeyValue><Modulus>36nIT5kA8A1N84POjl6T/sz+kRU8kDbUO0VKzpBl5dSVhoemlMa1YXa8X6gEXx6hicqazNbtSSLjqHvo4FEPRm8L8QS1U7bQ/DydMWcj96FirKbeFLJZGIAhBCZFDODWN9TAF/kHmyL6logvmuyvINDPv6voLN6YzXmt6FgleBk=</Modulus><Exponent>AQAB</Exponent><P>5z0V7GaoJZ/BZMZ755ynqe1wu/Bj7nB3Nq7eE56eYvcjRPHIQBUoy8C0dsuX+SOE0O72Xa64Z4hnAw9G2167Ww==</P><Q>950IhQUK0hyyLXIlEB1T1O1P5aapEScPq8cFpAbsawPefklgFiK4S87XWL9K/b1JISfo7XexJ0nlq2j29WSYmw==</Q><DP>fa4x0D8rfOeLkV5f0c7PQgiPkVZiuiHeaZY5lahMpbV1Me/Hyyy086lVbIvTmdG4SmbW+KwSBhOZCYywEmM2qQ==</DP><DQ>uVwQmKNhqlBZAbRFEn8h1m+gM+ZDAdgf3xOpoVSdfq7yy87Z4zgyhm1cv87TsIcWS3+42quTLjofd+WnmaOoqQ==</DQ><InverseQ>4VGAgecowM/Ub+4kF3fWVN/k1+kepTdwDXkMept9XKbar08lmtNGyvMZU/9awwCYZZfs6l1gFyWLLKVm3g1UFQ==</InverseQ><D>W9Ie6xacPPCpVNSCww3u4gcUZ0l5oJbx0BdlW6IKQy1f6WfdKmzdX9LYCMk4ajhwBtqHbJq7tW++WJfuBdEhXHt6CMHw0y4+5tBVErq6nwkqFeogu/dV4ksBSwI/N644El1k3uBPOUigvDkLXn5qIrlO2sa+JUml65ABSy3jS4U=</D></RSAKeyValue>";

  public static string AESEncrypt(string value, string _aeskey = "SA#%^433@!#$&#$%")
  {
    if (string.IsNullOrEmpty(_aeskey))
      _aeskey = "SA#%^433@!#$&#$%";
    byte[] bytes1 = Encoding.UTF8.GetBytes(_aeskey);
    byte[] bytes2 = Encoding.UTF8.GetBytes(value);
    RijndaelManaged rijndaelManaged = new RijndaelManaged();
    rijndaelManaged.Key = bytes1;
    rijndaelManaged.Mode = CipherMode.ECB;
    rijndaelManaged.Padding = PaddingMode.PKCS7;
    byte[] inArray = rijndaelManaged.CreateEncryptor().TransformFinalBlock(bytes2, 0, bytes2.Length);
    return Convert.ToBase64String(inArray, 0, inArray.Length);
  }

  public static string AESDecrypt(string value, string _aeskey = "SA#%^433@!#$&#$%")
  {
    try
    {
      if (string.IsNullOrEmpty(_aeskey))
        _aeskey = "SA#%^433@!#$&#$%";
      byte[] bytes = Encoding.UTF8.GetBytes(_aeskey);
      byte[] inputBuffer = Convert.FromBase64String(value);
      RijndaelManaged rijndaelManaged = new RijndaelManaged();
      rijndaelManaged.Key = bytes;
      rijndaelManaged.Mode = CipherMode.ECB;
      rijndaelManaged.Padding = PaddingMode.PKCS7;
      return Encoding.UTF8.GetString(rijndaelManaged.CreateDecryptor().TransformFinalBlock(inputBuffer, 0, inputBuffer.Length));
    }
    catch
    {
      return string.Empty;
    }
  }

  public static string PublicKeyEncrypt(string value, string publicKey = "<RSAKeyValue><Modulus>36nIT5kA8A1N84POjl6T/sz+kRU8kDbUO0VKzpBl5dSVhoemlMa1YXa8X6gEXx6hicqazNbtSSLjqHvo4FEPRm8L8QS1U7bQ/DydMWcj96FirKbeFLJZGIAhBCZFDODWN9TAF/kHmyL6logvmuyvINDPv6voLN6YzXmt6FgleBk=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>")
  {
    RSACryptoServiceProvider cryptoServiceProvider = new RSACryptoServiceProvider();
    cryptoServiceProvider.FromXmlString(publicKey);
    return Convert.ToBase64String(cryptoServiceProvider.Encrypt(EncryptHelper.StringToBytes(value), false));
  }

  public static string PrivateKeyDecrypt(string encryptedValue, string privateKey = "<RSAKeyValue><Modulus>36nIT5kA8A1N84POjl6T/sz+kRU8kDbUO0VKzpBl5dSVhoemlMa1YXa8X6gEXx6hicqazNbtSSLjqHvo4FEPRm8L8QS1U7bQ/DydMWcj96FirKbeFLJZGIAhBCZFDODWN9TAF/kHmyL6logvmuyvINDPv6voLN6YzXmt6FgleBk=</Modulus><Exponent>AQAB</Exponent><P>5z0V7GaoJZ/BZMZ755ynqe1wu/Bj7nB3Nq7eE56eYvcjRPHIQBUoy8C0dsuX+SOE0O72Xa64Z4hnAw9G2167Ww==</P><Q>950IhQUK0hyyLXIlEB1T1O1P5aapEScPq8cFpAbsawPefklgFiK4S87XWL9K/b1JISfo7XexJ0nlq2j29WSYmw==</Q><DP>fa4x0D8rfOeLkV5f0c7PQgiPkVZiuiHeaZY5lahMpbV1Me/Hyyy086lVbIvTmdG4SmbW+KwSBhOZCYywEmM2qQ==</DP><DQ>uVwQmKNhqlBZAbRFEn8h1m+gM+ZDAdgf3xOpoVSdfq7yy87Z4zgyhm1cv87TsIcWS3+42quTLjofd+WnmaOoqQ==</DQ><InverseQ>4VGAgecowM/Ub+4kF3fWVN/k1+kepTdwDXkMept9XKbar08lmtNGyvMZU/9awwCYZZfs6l1gFyWLLKVm3g1UFQ==</InverseQ><D>W9Ie6xacPPCpVNSCww3u4gcUZ0l5oJbx0BdlW6IKQy1f6WfdKmzdX9LYCMk4ajhwBtqHbJq7tW++WJfuBdEhXHt6CMHw0y4+5tBVErq6nwkqFeogu/dV4ksBSwI/N644El1k3uBPOUigvDkLXn5qIrlO2sa+JUml65ABSy3jS4U=</D></RSAKeyValue>")
  {
    RSACryptoServiceProvider cryptoServiceProvider = new RSACryptoServiceProvider();
    cryptoServiceProvider.FromXmlString(privateKey);
    return EncryptHelper.BytesToString(cryptoServiceProvider.Decrypt(Convert.FromBase64String(encryptedValue), false));
  }

  public static string Base64Encode(string value)
  {
    return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
  }

  public static string Base64Decode(string value)
  {
    return Encoding.UTF8.GetString(Convert.FromBase64String(value));
  }

  public static byte[] StringToBytes(string txt)
  {
    return Encoding.UTF8.GetBytes(txt);
  }

  public static string BytesToString(byte[] bitBytes)
  {
    return Encoding.UTF8.GetString(bitBytes);
  }
}
