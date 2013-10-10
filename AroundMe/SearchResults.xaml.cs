using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace AroundMe
{
    public partial class SearchResults : PhoneApplicationPage
    {
        private double _latitude;
        private double _longitude;
        private string _topic;
        private double _radius;
        private const string flickrApiKey = "e59ab439401d6bd1772fc748d857f4c6";

        public SearchResults()
        {
            InitializeComponent();

            Loaded += SearchResults_Loaded;
        }

        private async void SearchResults_Loaded(object sender, RoutedEventArgs e)
        {
            var images = await FlickrImage.GetFlickrImages(
                flickrApiKey,
                _topic,
                _latitude, 
                _longitude,
                _radius);
            DataContext = images;
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            _latitude = Convert.ToDouble(NavigationContext.QueryString["latitude"]);
            _longitude = Convert.ToDouble(NavigationContext.QueryString["longitude"]);
            _radius = Convert.ToDouble(NavigationContext.QueryString["radius"]);
            _topic = NavigationContext.QueryString["topic"].ToString();
        }
    }
}