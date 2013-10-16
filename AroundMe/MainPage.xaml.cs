using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using AroundMe.Resources;
using System.Device.Location;
using Windows.Devices.Geolocation;
using System.Net.Http;
using Newtonsoft.Json;
using System.Windows.Media.Imaging;
using System.Threading;

namespace AroundMe
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            Loaded += MainPage_Loaded;
            // Sample code to localize the ApplicationBar
            BuildLocalizedApplicationBar();
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            SystemTray.ProgressIndicator = new ProgressIndicator();

            UpdateMap();
        }

        public static void SetProgressIndicator(bool isVisible)
        {
            SystemTray.ProgressIndicator.IsIndeterminate = isVisible;
            SystemTray.ProgressIndicator.IsVisible = isVisible;
        }

        private async void UpdateMap()
        {
            Geolocator geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 50;


            SetProgressIndicator(true);
            SystemTray.ProgressIndicator.Text = "Getting GPS Position.";

            try
            {
                Geoposition position =
               await geolocator.GetGeopositionAsync(
               TimeSpan.FromMinutes(1),
               TimeSpan.FromSeconds(30));

                SystemTray.ProgressIndicator.Text = "Acquired.";

                var gpsCoorCenter =
                    new GeoCoordinate(
                        position.Coordinate.Latitude,
                        position.Coordinate.Longitude);

                AroundMeMap.Center = gpsCoorCenter;
                AroundMeMap.ZoomLevel = 15;

                SetProgressIndicator(false);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Location is disabled.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }




            //SystemTray.ProgressIndicator = new ProgressIndicator();
            SystemTray.ProgressIndicator.Text = "Acquired";

            //Thread.Sleep(10000);



            SystemTray.ProgressIndicator.IsVisible = true;
            SystemTray.IsVisible = false;
        }

        // Sample code for building a localized ApplicationBar
        private void BuildLocalizedApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();

            // Create a new button and set the text value to the localized string from AppResources.
            ApplicationBarIconButton appBarButton =
                new ApplicationBarIconButton(
                    new Uri("/Assets/feature.search.png", UriKind.Relative));

            appBarButton.Text = "Search";
            appBarButton.Click += SearchClick;
            ApplicationBar.Buttons.Add(appBarButton);

            // Create a new menu item with the localized string from AppResources.
            ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
            ApplicationBar.MenuItems.Add(appBarMenuItem);
        }

        private void SearchClick(object sender, EventArgs e)
        {
            var topic = HttpUtility.UrlEncode(SearchTopic.Text);

            string navTo = string.Format(
                "/SearchResults.xaml?latitude={0}&longitude={1}&topic={2}&radius={3}",
                AroundMeMap.Center.Latitude,
                AroundMeMap.Center.Longitude,
                topic,
                5);

            NavigationService.Navigate(new Uri(navTo, UriKind.RelativeOrAbsolute));
        }
    }
}