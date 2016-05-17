using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using Trained_WPF.Classes;

namespace Trained_WPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 
    /// 
    public partial class MainWindow
    {
        public static string SearchName;

        public static ObservableCollection<UserGroup> NamesGroup = new ObservableCollection<UserGroup>();
        public static ObservableCollection<UserAd> NamesAd = new ObservableCollection<UserAd>();

        readonly string _grouptoCheck = ConfigurationManager.AppSettings["GrouptoCheck"];
        readonly string _workAdGroup = ConfigurationManager.AppSettings["FijiGroupName"];
        readonly string _domainName = ConfigurationManager.AppSettings["DomainName"];

        readonly AdClient _adClient;

        public MainWindow()
        {
            InitializeComponent();
            Authorization.CheckGroups(_domainName, _grouptoCheck);
            
            if (Authorization.CheckGroups(_domainName, _grouptoCheck))
            {
                _adClient = new AdClient(_domainName);

                Credentials.Content = "|  " + Environment.UserDomainName + "\\" + Environment.UserName;
                Authorization.UserLogged();
                
                _adClient.LoadUsersGroup(NamesGroup, _workAdGroup);
                ListGroup.ItemsSource = NamesGroup;

                //для фильтрации
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListGroup.ItemsSource);
                view.Filter = UserFilter;

                //селектим GroupCombo
                GroupCombo.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("Похоже, у вас нет прав");
                Application.Current.Shutdown();
            }
        }
        
        private async void BtnSearchAd_Click(object sender, RoutedEventArgs e)
        {
            SearchName = "*" + SearchBoxAd.Text + "*";

            //var slowTask = Task<ObservableCollection<UserAd>>.Factory.StartNew(() => Classes.LoadUsersInAd.LoadUsersInAdMethod(SearchName, NamesAd));
            var slowTask = Task<ObservableCollection<UserAd>>.Factory.StartNew(() => _adClient.LoadUsersInAd(SearchName, NamesAd));

            BtnSearchAd.IsEnabled = false;
            Loader.Visibility=Visibility.Visible;
            Loader.IsIndeterminate = true;
            status_text.Content = String.Empty;

            await slowTask;

            //var dt = slowTask.Result;

            BtnSearchAd.IsEnabled = true;
            Loader.IsIndeterminate = false;
            Loader.Visibility = Visibility.Hidden;

            ListAd.ItemsSource = NamesAd.ToList();
        }


        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (ListAd.SelectedValue != null)
            {

                string userId = ListAd.SelectedValue.ToString();

                //Classes.AddUserGroup.AddUser2Group(status_text, userId);                        
                //Classes.LoadUsersGroup.LoadUsersGroupMethod(NamesGroup);
                _adClient.AddUser2Group(status_text, userId, _workAdGroup);
                _adClient.LoadUsersGroup(NamesGroup, _workAdGroup);

                CollectionViewSource.GetDefaultView(NamesGroup).Refresh();
            }
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (ListGroup.SelectedValue != null)
            {
                string userId = ListGroup.SelectedValue.ToString();            

            _adClient.RevomeUser(status_text, userId, _workAdGroup);
            _adClient.LoadUsersGroup(NamesGroup, _workAdGroup);
            
            CollectionViewSource.GetDefaultView(NamesGroup).Refresh();
            }
        }


        

        private void SearchBoxAd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnSearchAd.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }

        private void GridAd_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

                BtnAdd.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

        }

        private void ListGroup_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

                BtnRemove.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

        }



        
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow.WindowState == WindowState.Maximized)
            {
                Application.Current.MainWindow.WindowState = WindowState.Normal;
                //Application.Current.MainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;     
                ListAd.FontSize = 15;
                ListGroup.FontSize = 15;
                GroupCombo.FontSize = 15;

                BtnAdd.Width = 50;
                BtnAdd.Height = 50;
                BtnRemove.Width = 50;
                BtnRemove.Height = 50;
            }

            else
            {
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
                ListAd.FontSize = 20;
                ListGroup.FontSize = 20;
                GroupCombo.FontSize = 17;

                BtnAdd.Width = 70;
                BtnAdd.Height = 70;
                BtnRemove.Width = 70;
                BtnRemove.Height = 70;
            }

        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void WindowHeader_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }



        //фильтруем ListGroup
        private bool UserFilter(object item)
        {
            if (String.IsNullOrEmpty(GroupFilter.Text))
            { 
                return true;
            }
            else
            {
                var userGroup = item as UserGroup;
                return userGroup != null && (userGroup.NameGroup.IndexOf(GroupFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0);
            }
        }
        private void GroupFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(ListGroup.ItemsSource).Refresh();
        }
    }




}