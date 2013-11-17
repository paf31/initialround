///
///

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InitialRound.BusinessLogic.Properties;

namespace InitialRound.BusinessLogic.Helpers
{
    public static class EncryptionHelper
    {
        public static byte[] EncryptToken(byte[] plainText)
        {
            return Common.Helpers.EncryptionHelper.EncryptBytes(plainText, Keys.Default.TokenKey, Keys.Default.TokenVector);
        }

        public static byte[] DecryptToken(byte[] encryptedToken)
        {
            return Common.Helpers.EncryptionHelper.DecryptBytes(encryptedToken, Keys.Default.TokenKey, Keys.Default.TokenVector);
        }

        public static byte[] EncryptAntiForgeryToken(byte[] plainText)
        {
            return Common.Helpers.EncryptionHelper.EncryptBytes(plainText, Keys.Default.AntiForgeryKey, Keys.Default.AntiForgeryVector);
        }

        public static byte[] DecryptAntiForgeryToken(byte[] encryptedToken)
        {
            return Common.Helpers.EncryptionHelper.DecryptBytes(encryptedToken, Keys.Default.AntiForgeryKey, Keys.Default.AntiForgeryVector);
        }

        public static byte[] EncryptURL(byte[] plainText)
        {
            return Common.Helpers.EncryptionHelper.EncryptBytes(plainText, Keys.Default.URLKey, Keys.Default.URLVector);
        }

        public static byte[] DecryptURL(byte[] encryptedToken)
        {
            return Common.Helpers.EncryptionHelper.DecryptBytes(encryptedToken, Keys.Default.URLKey, Keys.Default.URLVector);
        }
    }
}
