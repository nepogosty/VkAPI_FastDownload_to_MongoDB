using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using System.Threading;

namespace VK.net_form
{
    public partial class Form2 : Form
    {
        public string []accesstoken= new string[2];
       
        public string nameid;
        static string connectionString = "mongodb://localhost";
        List<Group> groups = new List<Group>();
        static int kolvo = 0;
        int idgroup ;
        string namegroup;

        MongoClient client = new MongoClient(connectionString);
        public Form2(string[] Accesstoken, string Nameid)
        {
            
            accesstoken[0] = Accesstoken[0];
            accesstoken[1] = Accesstoken[1];
            nameid = Nameid;
            InitializeComponent();
           
        }

        private string GET(string Url, string Method, string Parameters, string Token)
        {
            WebRequest req = WebRequest.Create(String.Format(Url, Method, Parameters, Token));
            WebResponse resp = req.GetResponse();
            Stream stream = resp.GetResponseStream();
            StreamReader sr = new StreamReader(stream);
            string Out = sr.ReadToEnd();
            return Out;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
               
                listBox1.Items.Clear();
                listBox1.DisplayMember = "Name";
                listBox1.ValueMember = "Id";
                string reqStrTemplate = "https://api.vk.com/method/{0}?{1}access_token={2}&extended=1&fields=members_count&v=5.52";
                string method = "groups.get";
                string parameters = "";
                var f = GET(reqStrTemplate, method,parameters, accesstoken[0]);
                var user = JsonConvert.DeserializeObject(f) as JObject;
                StringBuilder stroka = new StringBuilder();
                int kolvo = Convert.ToInt32(user["response"]["count"]);
                for (int i = 0; i < kolvo; i++)
                {
                    groups.Add(new Group
                    {
                        Id = Convert.ToInt32(user["response"]["items"][i]["id"]),
                        Name = Convert.ToString(user["response"]["items"][i]["name"])
                    });

                    listBox1.DataSource = groups.OrderBy(p => p.Name).ToList();
                    listBox1.DisplayMember = "Name";
                    listBox1.ValueMember = "Id";
                    //stroka.Append(user["response"]["items"][i]["name"] + " " + user["response"]["items"][i]["id"]);
                    //listBox1.Items.Add(stroka.ToString());
                    //stroka.Clear();

                }
              
            }
            catch
            {
                MessageBox.Show("Что-то пошло не так");

            }

        
        }
        private void button2_Click(object sender, EventArgs e)
        {   
            try
            {
                
                 
                DateTime date1 = DateTime.Now;
                
                string reqStrTemplate = "https://api.vk.com/method/{0}?{1}&access_token={2}&order=name&fields=name&v=5.21";
                string method = "groups.getMembers";
                //string friendsnameid = listBox1.SelectedItem.ToString();

                int idgroup = Convert.ToInt32(listBox1.SelectedValue);
                string namegroup = listBox1.Text;
                var database = client.GetDatabase("VK_database");
                database.DropCollection(namegroup);
                var collection = database.GetCollection<BsonDocument>(namegroup);

                string parameters = "group_id=" + idgroup + "&count=1000";
                var f = GET(reqStrTemplate, method, parameters, accesstoken[0]);
                var followers = JsonConvert.DeserializeObject(f) as JObject;
                StringBuilder stroka = new StringBuilder();
                int kolvofollowers = Convert.ToInt32(followers["response"]["count"]);
                var streams = Math.Ceiling((double)kolvofollowers / 1000);

                int a = 0;
                Parallel.For(0, Convert.ToInt32(streams), new ParallelOptions { MaxDegreeOfParallelism = 2}, async i =>
                {
                    {
                        DateTime date10 = DateTime.Now;
                        if (i % 2 == 0)
                        {
                            a = 0;
                        }
                        else
                        {
                            a = 1;
                        }
                        if (i % 3 == 0)
                            System.Threading.Thread.Sleep(750);


                        parameters = "group_id=" + idgroup + "&count=1000" + "&offset=" + i * 1000;
                        f = GET(reqStrTemplate, method, parameters, accesstoken[a]);
                        followers = JsonConvert.DeserializeObject(f) as JObject;

                        string grps = followers["response"]["items"].ToString();
                        var document = BsonSerializer.Deserialize<IList<BsonDocument>>(grps);
                        await collection.InsertManyAsync(document);
                        DateTime date11 = DateTime.Now;
                        if ((date11 - date10).Seconds < 1)
                            Thread.Sleep(1000 - (date11 - date10).Milliseconds);
                       
                    }
                });
                DateTime date2 = DateTime.Now;

                string c = (date2 - date1).ToString();
                textBox5.Text = c;
            }
            catch
            {
                MessageBox.Show("Пожалуйста, выберите друга");

            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            textBox1.Text = accesstoken[0];
            textBox2.Text = accesstoken[1];

        }
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime date1 = DateTime.Now;
               
                string reqStrTemplate = "https://api.vk.com/method/{0}?{1}&access_token={2}&order=name&fields=name&v=5.21";
                string method = "groups.getMembers";

                int idgroup = Convert.ToInt32(listBox1.SelectedValue);
                string namegroup = listBox1.Text;

                var database = client.GetDatabase("VK_database");
                database.DropCollection(namegroup);
                var collection = database.GetCollection<BsonDocument>(namegroup);

                string parameters = "group_id=" + idgroup + "&count=1000";
                //string f;/*= GET(reqStrTemplate, method, parameters, accesstoken[1]);*/
                //var followers = JsonConvert.DeserializeObject(f) as JObject;
                StringBuilder stroka = new StringBuilder();
                //int kolvofollowers = Convert.ToInt32(followers["response"]["count"]);
                var streams = Math.Ceiling((double)kolvo / 25000);
                DateTime date3 = DateTime.Now;
                int a = 0;
                method = "execute";
                reqStrTemplate = "https://api.vk.com/method/{0}?code={1}&access_token={2}&v=5.21";
                Parallel.For(0, Convert.ToInt32(streams), async i =>
                {
                
                        if (i % 2 == 0)
                        {
                            a = 0;
                        }
                        else
                        {
                            a = 1;
                        }
                        //if (i % 6 == 0)
                        //    System.Threading.Thread.Sleep(1000);
                        parameters = "int%20i%3D" + i * 25 + "%3B%0Aint%20y%3Di%2B25%3B%0Aint%20x%3D1000%3B%0Avar%20res%3D%7B%7D%3B%0Awhile%20(i<y)%0A%7B%20res%3Dres%2BAPI.groups.getMembers(%7B\"group_id\"%3A" + idgroup + "%2C\"offset\"%3Ai*x%2C\"count\"%3Ax%2C\"fields\"%3A\"first_name%2Clast_name\"%7D).items%3B%0Ai%20%3D%20i%2B1%3B%0A%7D%3B%0Areturn%20res%3B";

                        string f = GET(reqStrTemplate, method, parameters, accesstoken[a]);
                        var followers = JsonConvert.DeserializeObject(f) as JObject;

                        string grps = followers["response"].ToString();
                        var document = BsonSerializer.Deserialize<IList<BsonDocument>>(grps);
                        await collection.InsertManyAsync(document);
                });
                DateTime date2 = DateTime.Now;

                string c = (date2 - date1).ToString();
                string c1 = (date3 - date1).ToString();
                textBox3.Text = c;
                textBox6.Text = c1;
            }
            catch
            {
                MessageBox.Show("Пожалуйста, выберите друга");

            }
        }
        

     
     


        private void listBox1_Click(object sender, EventArgs e)
        {
            textBox7.Text = "";
            string reqStrTemplate = "https://api.vk.com/method/{0}?{1}&access_token={2}&order=name&fields=name&v=5.21";
            string method = "groups.getMembers";
            int idgroup = Convert.ToInt32(listBox1.SelectedValue);
            
            string parameters = "group_id=" + idgroup + "&count=1000";
            var f = GET(reqStrTemplate, method, parameters, accesstoken[0]);
            var followers = JsonConvert.DeserializeObject(f) as JObject;
            
            kolvo = Convert.ToInt32(followers["response"]["count"]);
            textBox7.Text =Convert.ToString(kolvo);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime date1 = DateTime.Now;

                string reqStrTemplate = "https://api.vk.com/method/{0}?{1}&access_token={2}&order=name&fields=name&v=5.21";
                string method = "groups.getMembers";  
                int idgroup = Convert.ToInt32(listBox1.SelectedValue);
                string namegroup = listBox1.Text;
                var database = client.GetDatabase("VK_database");
                database.DropCollection(namegroup);
                var collection = database.GetCollection<BsonDocument>(namegroup);
                string parameters = "group_id=" + idgroup + "&count=1000";
                var streams = Math.Ceiling((double)kolvo / 1000);
                
                int a = 0;
                Parallel.For(0, Convert.ToInt32(streams),new ParallelOptions { MaxDegreeOfParallelism = 3 }, async i =>
                {
                    {
                        DateTime date10 = DateTime.Now;
                        if (i % 3 == 0)
                            System.Threading.Thread.Sleep(2000);
                        parameters = "group_id=" + idgroup + "&count=1000" + "&offset=" + i * 1000;
                        var f = GET(reqStrTemplate, method, parameters, accesstoken[a]);
                        var followers = JsonConvert.DeserializeObject(f) as JObject;
                        string grps = followers["response"]["items"].ToString();
                        var document = BsonSerializer.Deserialize<IList<BsonDocument>>(grps);
                        await collection.InsertManyAsync(document);
                        DateTime date11 = DateTime.Now;
                        if ((date11 - date10).Seconds < 1)
                            Thread.Sleep(1000 - (date11 - date10).Milliseconds);
                    }
                });
                DateTime date2 = DateTime.Now;

                string c = (date2 - date1).ToString();
                textBox8.Text = c;
            }
            catch
            {
                MessageBox.Show("Пожалуйста, выберите друга");

            }
        }

        
    }
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
