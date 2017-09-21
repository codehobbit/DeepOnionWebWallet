using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OnionWallet.Framework.Extensions;

namespace OnionWallet.Data
{
    public static class Extensions
    {
        public static bool CheckPassword(this OnionUser user, string password)
        {
            string stringToHash = password + user.PasswordSaltGUID.ToString();

            if (user.Password == stringToHash.CreateMD5())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void SetPassword(this OnionUser user, string password)
        {
            string stringToHash = password + user.PasswordSaltGUID.ToString();
            user.Password = stringToHash.CreateMD5();
        }

        public static void InitGUIDs(this OnionUser user)
        {
            user.GUID = Guid.NewGuid();
            user.EmailConfirmationGUID = Guid.NewGuid();
            user.PasswordSaltGUID = Guid.NewGuid();
            user.RecoveryGUID = Guid.NewGuid();
        }
    }
}
