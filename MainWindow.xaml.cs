using RequestLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AllION_Base
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SendRequest();
        }

        private void SendRequest()
        {
            AniVostGetInfoTitle(); 

        }

        private void AniVostGetInfoTitle()
        {
            List<string> UrlAnime = AniVostGetLastUpd();

            if (UrlAnime == null)
            {
                MessageBox.Show("Метод получения списка изменений вернул пустоту","Error",MessageBoxButton.OK,MessageBoxImage.Error);
                return;
            }

            foreach (var Url in UrlAnime)
            {
                string address = Url;
                var request = new GetRequest(address);
                request.Accept = "*/*";

            }


        }

        private List<string> AniVostGetLastUpd() 
        {
            string address = "https://v2.vost.pw/";

            var request = new GetRequest(address);
            request.Accept = "*/*";
            request.Host = "v2.vost.pw";
            request.Proxy = new WebProxy("127.0.0.1:8888");

            CookieContainer container = new CookieContainer();

            request.Run(ref container);

            var response = request.Response != null ? request.Response : null;

            if (response == null)
                return null;

            try
            {
                response = Regex.Replace(response, @"[\r\n\t]", "");

                Regex regex = new Regex(@"<ul class=""raspis raspis_fixed"">.*?</ul>");

                MatchCollection lastupdstrings = regex.Matches(response.ToString());

                string lastupdstring = "";

                foreach (var li in lastupdstrings)
                {
                    lastupdstring += li;
                }

                regex = new Regex(@"href="".*?""");

                MatchCollection URLLastUpdMC = regex.Matches(lastupdstring);

                List<string> URLLastUpdStrs = new List<string>();
                string Urllastupdstr = "";

                //for(var url in URLLastUpdMC)
                //{
                //    URLLastUpdStr.Append(Regex.Replace(url.ToString(), @"", ""));
                //}

                for (int i = 0; i < URLLastUpdMC.Count / 3; i++)
                {
                    var clearurlstr = Regex.Replace(URLLastUpdMC[i].ToString(), @"href=""|""", "");

                    URLLastUpdStrs.Add(clearurlstr);
                    Urllastupdstr += $"{i + 1}). " + clearurlstr + "\n";
                }

                TBResponse.Text = Urllastupdstr;

                return URLLastUpdStrs;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка в стаке получения списка последних изменений \nПодробнее: \n{ex.Message}\n{ex.InnerException}","Error",MessageBoxButton.OK,MessageBoxImage.Error);

                return null;
            }
        }
    }
}
