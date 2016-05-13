using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;

namespace Trained_WPF.Classes
{
    public class Authorization
    {

        public static string GrouptoCheck = "Uk.Access.Dutty";

        public static void CheckGroups()
        {

            //Проверяем, что пользователь в группе

            PrincipalContext ctx = new PrincipalContext(ContextType.Domain, "2GIS");
            UserPrincipal user = UserPrincipal.FindByIdentity(ctx, Environment.UserDomainName.ToString() + "\\" + Environment.UserName.ToString());            



            List<GroupPrincipal> result = new List<GroupPrincipal>();

            if (user != null)
            {

                PrincipalSearchResult<Principal> groups = user.GetGroups();

                foreach (Principal p in groups)
                {
                    if (p is GroupPrincipal)
                    {
                        result.Add((GroupPrincipal)p);
                        string results = String.Join(", ", result);

                                                

                        if (results.Contains(GrouptoCheck))
                        {
                            MainWindow.FlagClass.FlagAccess = true;
                            MainWindow.FlagClass.Permiss = Environment.UserDomainName.ToString() + "\\" + Environment.UserName.ToString();


                        }
                        else
                        {

                            MainWindow.FlagClass.FlagAccess = false;                            

                        }
                    }
                }
            }




        }

        public static void UserLogged()
        {


                Classes.NLog.AuthToLog("User logged");


        }
    }
}
