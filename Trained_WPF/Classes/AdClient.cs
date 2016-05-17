using System;
using System.Collections.ObjectModel;
using System.DirectoryServices.AccountManagement;
using System.Windows.Controls;

namespace Trained_WPF.Classes
{
   public class AdClient
   {
        private readonly string _domainName;
        
        public AdClient(string domainName)
       {
            _domainName = domainName;                        
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
                        if (@group != null)
                        {
                            var users = @group.GetMembers(true);
                            //users.ToList().ForEach(x => Debug.WriteLine(x.UserPrincipalName));
                            //Debug.WriteLine("");


                            foreach (var principal in users)
                            {
                                var user = (UserPrincipal) principal;
                                namesGroup.Add(new UserGroup() { UpnGroup = user.UserPrincipalName, NameGroup = user.Name });
                            }
                        }
                    }
                }


            }
            catch (Exception e)
            {
                NLog.ExceptionToLog("Error loading users in group: " + e + "");
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
            catch (Exception e)
            {
                NLog.ExceptionToLog("Error loading users in AD: " + e + "");
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
                    if (group != null)
                    { 
                    group.Members.Add(ctx, IdentityType.UserPrincipalName, userId);
                    group.Save();
                    }
                }

                status.Content = "Пользователь " + userId + " добавлен в группу";

                //логируем добавление
                NLog.OperationToLog("AddUser: ", userId);
            }
            catch (PrincipalExistsException e)
            {
                status.Content = "Пользователь " + userId + " уже есть в группе";

                NLog.ExceptionToLog("Error adding user: " + e + "");
            }

            catch (Exception e)
            {                
                NLog.ExceptionToLog("Error adding user: " + e + "");
            }
        }

        public void RevomeUser(Label status, string userId, string groupName)
        {
            try
            {
                using (var ctx = new PrincipalContext(ContextType.Domain, _domainName))
                {
                    GroupPrincipal group = GroupPrincipal.FindByIdentity(ctx, groupName);
                    if (group != null)
                    { 
                    group.Members.Remove(ctx, IdentityType.UserPrincipalName, userId);
                    group.Save();
                    }
                }

                status.Content = "Пользователь " + userId + " удалён из группы";

                //логируем удаление               
                NLog.OperationToLog("RemoveUser: ", userId);

            }
            catch (Exception e)
            {
                NLog.ExceptionToLog("Error removing user: " + e + "");
            }
        }
    }
}
