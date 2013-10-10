using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace AroundMe
{
    public class FlickrImage
    {
        public Uri Image320 { get; set; }
        public Uri Image1024 { get; set; }

        public async static Task<List<FlickrImage>> GetFlickrImages(
            string flickrApiKey,
            string topic,
            double latitude = double.NaN,
            double longitude = double.NaN,
            double radius = double.NaN) 
        {
            HttpClient client = new HttpClient();
            var baseUrl = GetBaseUrl(flickrApiKey, topic, latitude, longitude, radius);

            string flickrResult = await client.GetStringAsync(baseUrl);

            FlickrData apiData = JsonConvert.DeserializeObject<FlickrData>(flickrResult);

            List<FlickrImage> images = new List<FlickrImage>();

            if (apiData.stat.Equals("ok"))
            {
                foreach (Photo data in apiData.photos.photo)
                {
                    FlickrImage img = new FlickrImage();
                    string photoUrl = "http://farm{0}.staticflickr.com/{1}/{2}_{3}";
                    string baseFlickrUrl = string.Format(photoUrl,
                        data.farm,
                        data.server,
                        data.id,
                        data.secret);

                    img.Image320 = new Uri(baseFlickrUrl + "_n.jpg");
                    img.Image1024 = new Uri(baseFlickrUrl + "_n.jpg");

                    images.Add(img);
                    

                }
            }

            return images;
        }

        private static string GetBaseUrl(
            string flickrApiKey, 
            string topic,
            double latitude = double.NaN, 
            double longitude = double.NaN,
            double radius = double.NaN)
        {
            string[] licenses = { "4", "5", "6", "7" };
            string license = string.Join(",", licenses);
            //string flickrApiKey = "e59ab439401d6bd1772fc748d857f4c6";

            if (!double.IsNaN(latitude))
                latitude = Math.Round(latitude, 5);

            if (!double.IsNaN(longitude))
                longitude = Math.Round(longitude, 5);

            string url = "http://api.flickr.com/services/rest/?method=flickr.photos.search" +
                "&api_key={0}" +
                "&license={1}" +
                "&format=json" +
                "&nojsoncallback=1";

            var baseUrl = string.Format(url,
                flickrApiKey,
                license);

            if((!string.IsNullOrWhiteSpace(topic)))
                baseUrl += string.Format("&text=%22{0}%22", topic);

            if(!double.IsNaN(latitude) && !double.IsNaN(longitude))
                baseUrl += string.Format("&lat={0}&lon={1}",latitude, longitude);

            if(!double.IsNaN(radius))
                baseUrl += string.Format("&radius={0}", radius);

            //var baseUrl = string.Format(url,
            //    flickrApiKey,
            //    license,
            //    latitude,
            //    longitude,
            //    topic);

            return baseUrl;
        }
    }

   
}
