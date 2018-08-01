using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.StringCipher
{
  public interface IMyEncrypt
  {
    string Encrypt(string plainText, string passPhrase);
    string Decrypt(string cipherText, string passPhrase);
  }
}
