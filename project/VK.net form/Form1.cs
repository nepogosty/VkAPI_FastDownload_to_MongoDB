using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Model.RequestParams;
using System.Web;
using MongoDB.Driver;

namespace VK.net_form
{
    public partial class Form1 : Form
    {
        public string[] accesstoken = new string[2];
        public string userid;
        public Form1()
        {
            InitializeComponent();
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            //string connectionString = "mongodb://localhost:27017";
            //MongoClient client = new MongoClient(connectionString);
            //IMongoDatabase database = client.GetDatabase("test");
            string[] appid = new string [2] { "7722235", "7722061" };
            
           
            var uriStr = @"https://oauth.vk.com/authorize?client_id=" + appid[0] +
            @"&scope=offline&redirect_uri=https://oauth.vk.com/blank.html&display=page&v=5.6&response_type=token";
            var uriStr1 = @"https://oauth.vk.com/authorize?client_id=" + appid[1] +
           @"&scope=offline&redirect_uri=https://oauth.vk.com/blank.html&display=page&v=5.6&response_type=token";
            webBrowser1.Navigate(new Uri(uriStr));
            System.Threading.Thread.Sleep(5000);
            webBrowser2.Navigate(new Uri(uriStr1));
          
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
 
        

        private void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            if (e.Url.AbsoluteUri.Contains(@"oauth.vk.com/blank.html"))
            {
                string url = e.Url.Fragment;
                url = url.Trim('#');
                string Access_token = HttpUtility.ParseQueryString(url).Get("access_token");
                userid = HttpUtility.ParseQueryString(url).Get("user_id");
                //this.Hide();
                accesstoken[0] = Access_token;





            }
        }
        

        private void webBrowser2_Navigated_1(object sender, WebBrowserNavigatedEventArgs e)
        {
            if (e.Url.AbsoluteUri.Contains(@"oauth.vk.com/blank.html"))
            {
                string url = e.Url.Fragment;
                url = url.Trim('#');
                string Access_token = HttpUtility.ParseQueryString(url).Get("access_token");
                userid = HttpUtility.ParseQueryString(url).Get("user_id");
                //this.Hide();
                accesstoken[1] = Access_token;
                Form2 f = new Form2(accesstoken, userid);
                f.Show();




            }
        }
    }
   
}
