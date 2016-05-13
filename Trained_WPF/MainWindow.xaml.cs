using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using Trained_WPF.Classes;

namespace Trained_WPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 
    /// 
    public partial class MainWindow : Window
    {
        public static string SearchName;

        public static ObservableCollection<UserGroup> NamesGroup = new ObservableCollection<UserGroup>();
        public static ObservableCollection<UserAd> NamesAd = new ObservableCollection<UserAd>();

        private const string FijiAdGroup = "UK.Access.Fiji.TeachApproved";

        readonly AdClient _adClient;

        public MainWindow()
        {
            InitializeComponent();
            Classes.Authorization.CheckGroups();

            if (FlagClass.FlagAccess.Equals(true))
            {

                _adClient = new AdClient("2GIS");

                Credentials.Content = "|  " + FlagClass.Permiss.ToString();
                Classes.Authorization.UserLogged();
                
                _adClient.LoadUsersGroup(NamesGroup, FijiAdGroup);
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


        public class FlagClass
        {
            public static bool FlagAccess;
            public static string Permiss;
        }


        


        private async void BtnSearchAd_Click(object sender, RoutedEventArgs e)
        {
            SearchName = "*" + SearchBoxAd.Text + "*";

            //var slowTask = Task<ObservableCollection<UserAd>>.Factory.StartNew(() => Classes.LoadUsersInAd.LoadUsersInAdMethod(SearchName, NamesAd));
            var slowTask = Task<ObservableCollection<UserAd>>.Factory.StartNew(() => _adClient.LoadUsersInAd(SearchName, NamesAd));

            BtnSearchAd.IsEnabled = false;
            Loader.Visibility=Visibility.Visible;
            Loader.IsIndeterminate = true;
            status_text.Content = string.Empty;


            await slowTask;

            var DT = slowTask.Result;

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
                _adClient.AddUser2Group(status_text, userId, FijiAdGroup);
                _adClient.LoadUsersGroup(NamesGroup, FijiAdGroup);

                CollectionViewSource.GetDefaultView(NamesGroup).Refresh();
            }

        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (ListGroup.SelectedValue != null)
            {
                string userId = ListGroup.SelectedValue.ToString();            

            _adClient.RevomeUser(status_text, userId, FijiAdGroup);
            _adClient.LoadUsersGroup(NamesGroup, FijiAdGroup);
            
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
                this.DragMove();
        }



        //фильтруем ListGroup
        private bool UserFilter(object item)
        {
            if (String.IsNullOrEmpty(GroupFilter.Text))
                return true;
            else
                return ((item as UserGroup).NameGroup.IndexOf(GroupFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }
        private void GroupFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(ListGroup.ItemsSource).Refresh();
        }
    }




}