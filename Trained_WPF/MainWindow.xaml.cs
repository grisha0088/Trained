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
    public partial class MainWindow
    {
        public static string SearchName;

        public static ObservableCollection<User> NamesGroup = new ObservableCollection<User>();
        public static ObservableCollection<User> NamesAd = new ObservableCollection<User>();

        readonly string _grouptoCheck = ConfigurationManager.AppSettings["GrouptoCheck"];
        readonly string _workAdGroup = ConfigurationManager.AppSettings["FijiGroupName"];
        readonly string _domainName = ConfigurationManager.AppSettings["DomainName"];

        readonly AdClient _adClient;

        public MainWindow()
        {
            InitializeComponent();

            if (Authorization.CheckGroups(_domainName, _grouptoCheck))
            {
                _adClient = new AdClient(_domainName);

                Credentials.Content = "|  " + Environment.UserDomainName + "\\" + Environment.UserName;
                Classes.NLog.AuthToLog("User logged");

                NamesGroup = _adClient.LoadUsersGroup(_workAdGroup);
                ListGroup.ItemsSource = NamesGroup;

                //для фильтрации
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListGroup.ItemsSource);
                view.Filter = UserFilter;

                //селектим GroupCombo
                GroupCombo.SelectedIndex = 0;
            }
            else
            {
                Classes.NLog.AuthToLog("Access denied");
                MessageBox.Show("Похоже, у вас нет прав." + System.Environment.NewLine +"Обратитесь в техническую поддержку support@2gis.ru.");
                Application.Current.Shutdown();
            }
        }
        
        private async void BtnSearchAd_Click(object sender, RoutedEventArgs e)
        {
            SearchName = "*" + SearchBoxAd.Text + "*";
            
            //обнуляем коллекцию пользователей AD и заново заполняем
            NamesAd.Clear();
            var asyncAdListFill = Task<ObservableCollection<User>>.Factory.StartNew(() => NamesAd = _adClient.LoadUsersInAd(SearchName));

            BtnSearchAd.IsEnabled = false;
            Loader.Visibility=Visibility.Visible;
            Loader.IsIndeterminate = true;
            status_text.Content = String.Empty;

            await asyncAdListFill;

            BtnSearchAd.IsEnabled = true;
            Loader.IsIndeterminate = false;
            Loader.Visibility = Visibility.Hidden;

            ListAd.ItemsSource = NamesAd.ToList();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            //надо бы прикрутить сюда async и progressbar

            if (ListAd.SelectedValue != null)
            {
                string userId = ListAd.SelectedValue.ToString();
                _adClient.AddUser2Group(status_text, userId, _workAdGroup);
      
                //обнуляем коллекцию пользователей группы и заново заполняем
                NamesGroup.Clear();
                NamesGroup = _adClient.LoadUsersGroup(_workAdGroup);

                CollectionViewSource.GetDefaultView(NamesGroup).Refresh();
            }
        }
        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (ListGroup.SelectedValue != null)
            {
                string userId = ListGroup.SelectedValue.ToString();            

            _adClient.RevomeUser(status_text, userId, _workAdGroup);
             
             //обнуляем коллекцию пользователей группы и заново заполняем
             NamesGroup.Clear();
             NamesGroup = _adClient.LoadUsersGroup(_workAdGroup);
            
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
                ListAd.FontSize = 15;
                ListGroup.FontSize = 15;
                GroupCombo.FontSize = 15;

                BtnAdd.Width = 50;
                BtnAdd.Height = 50;
                BtnRemove.Width = 50;
                BtnRemove.Height = 50;

                //костыль ресайза ListView
                foreach (var c in GridAd.Columns)
                {
                    c.Width = 220;
                }

                foreach (var c in GridGroup.Columns)
                {
                    c.Width = 220;
                }
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

                //костыль ресайза ListView
                foreach (var c in GridAd.Columns)
                {
                    c.Width = ListAd.Width / 2;
                }

                foreach (var c in GridGroup.Columns)
                {
                    c.Width = ListAd.Width / 2;
                }
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
                var userGroup = item as User;
                return userGroup != null && (userGroup.Name.IndexOf(GroupFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0);
            }
        }
        private void GroupFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(ListGroup.ItemsSource).Refresh();
        }
    }




}