using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices.AccountManagement;
using System.Windows.Controls;

namespace Trained_WPF.Classes
{
   public class AdClient
   {
       private readonly string _domainName;
       readonly string _author = Environment.UserDomainName.ToString();
        
        public AdClient(string domainName)
       {
            this._domainName = domainName;                        
        }

        public void LoadUsersGroup(ObservableCollection<UserGroup> namesGroup, string groupName)
        {
            namesGroup.Clear();

            try
            {


                using (var ctx = new PrincipalContext(ContextType.Domain, _domainName))
                {
                    using (var group = GroupPrincipal.FindByIdentity(ctx, groupName))
                    {

                        var users = group.GetMembers(true);
                        //users.ToList().ForEach(x => Debug.WriteLine(x.UserPrincipalName));
                        //Debug.WriteLine("");


                        foreach (UserPrincipal user in users)
                        {

                            namesGroup.Add(new UserGroup() { UpnGroup = user.UserPrincipalName, NameGroup = user.Name });


                        }

                    }
                }





            }
            catch (Exception e)
            {
                Classes.NLog.ExceptionToLog("Error loading users in group: " + e.ToString() + "");
            }


        }

        public ObservableCollection<UserAd> LoadUsersInAd(string searchName, ObservableCollection<UserAd> namesAd)
        {
            namesAd.Clear();

            try
            {
                using (var ctx = new PrincipalContext(ContextType.Domain, _domainName))
                {
                    UserPrincipal userFind = new UserPrincipal(ctx);
                    userFind.Enabled = true;
                    userFind.Name = searchName;

                    using (PrincipalSearcher srch = new PrincipalSearcher(userFind))
                    {
                        // find all matches

                        foreach (var found in srch.FindAll())
                        {
                            // DT.Rows.Add(found.DisplayName.ToString(), found.UserPrincipalName);
                            namesAd.Add(new UserAd() { UpnAd = found.UserPrincipalName, NameAd = found.Name });
                        }

                    }
                }

            }
            catch (System.Exception e)
            {
                Classes.NLog.ExceptionToLog("Error loading users in AD: " + e.ToString() + "");
            }


            return namesAd;

        }

        public void AddUser2Group(Label status, String userId, string groupName)
        {

            try
            {
                using (var ctx = new PrincipalContext(ContextType.Domain, _domainName))
                {
                    GroupPrincipal group = GroupPrincipal.FindByIdentity(ctx, groupName);
                    group.Members.Add(ctx, IdentityType.UserPrincipalName, userId);
                    group.Save();
                }

                status.Content = "Пользователь " + userId.ToString() + " добавлен в группу";


                //логируем добавление
                Classes.NLog.OperationToLog("AddUser: ", userId, _author);
            }
            catch (System.DirectoryServices.AccountManagement.PrincipalExistsException e)
            {
                status.Content = "Пользователь " + userId.ToString() + " уже есть в группе";

                Classes.NLog.ExceptionToLog("Error adding user: " + e.ToString() + "");
            }

            catch (System.Exception e)
            {                
                Classes.NLog.ExceptionToLog("Error adding user: " + e.ToString() + "");
            }

        }

        public void RevomeUser(Label status, string userId, string groupName)
        {

            try
            {
                using (var ctx = new PrincipalContext(ContextType.Domain, _domainName))
                {
                    GroupPrincipal group = GroupPrincipal.FindByIdentity(ctx, groupName);
                    group.Members.Remove(ctx, IdentityType.UserPrincipalName, userId);
                    group.Save();
                }

                status.Content = "Пользователь " + userId.ToString() + " удалён из группы";

                //логируем удаление               
                Classes.NLog.OperationToLog("RemoveUser: ", userId, _author);

            }
            catch (System.Exception e)
            {
                Classes.NLog.ExceptionToLog("Error removing user: " + e.ToString() + "");
            }

        }


    }
}
