using System;
using System.DirectoryServices.AccountManagement;
using System.Linq;

namespace Trained_WPF.Classes
{
    public class Authorization
    {
        //Проверяем, что пользователь в группе
        public static bool CheckGroups(string domainName, string grouptoCheck)
        {
            try
            {
                PrincipalContext ctx = new PrincipalContext(ContextType.Domain, domainName);
                UserPrincipal user = UserPrincipal.FindByIdentity(ctx,
                    Environment.UserDomainName + "\\" + Environment.UserName); //получаем пользователя            

                if (user != null)
                {
                    PrincipalSearchResult<Principal> groups = user.GetGroups(); //полкчаем группы пользователя
                    return groups.Any(p => p.Name == grouptoCheck); //проверяем, есть ли там искомая группа
                }
            }
            catch (Exception e)
            {
                NLog.ExceptionToLog("Error starting app: " + e + "");
            }
            return false;
        }
    }
}
