using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;

namespace Trained_WPF.Classes
{
    public class Authorization
    {
        public static bool CheckGroups(string domainName, string grouptoCheck)
        {
            //Проверяем, что пользователь в группе
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain, domainName);
            UserPrincipal user = UserPrincipal.FindByIdentity(ctx, Environment.UserDomainName + "\\" + Environment.UserName);            

            List<GroupPrincipal> result = new List<GroupPrincipal>();

            if (user != null)
            {
                PrincipalSearchResult<Principal> groups = user.GetGroups();

                foreach (Principal p in groups)
                {
                    var item = p as GroupPrincipal;
                    if (item != null)
                    {
                        result.Add(item);
                        string results = String.Join(", ", result);

                        if (results.Contains(grouptoCheck))
                        {                            
                            return true;
                        }
                    }
                }
                return false;
            }
            return false;
        }

        public static void UserLogged()
        {
                NLog.AuthToLog("User logged");
        }
    }
}
