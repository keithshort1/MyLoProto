using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Dynamic;
using Facebook;
using System.IO;
using MyLoStoreNS;
using Newtonsoft;

namespace MyLO
{
    public partial class MyLOForm1 : Form
    {
        private const string AppId = "396995136986637";

        private MyLoStore _ms;

        private string ExtendedPermissions = 
            "user_about_me" + 
            ",friends_about_me" + 
            ",user_activities" + 
            ",friends_activities" +
            ",user_likes" +
            ",friends_likes" + 
            ",user_events" +
            ",friends_events" + 
            ",user_location" +
            ",friends_location" +
            ",user_checkins" +
            ",friends_checkins" +
            ",user_status" +
            ",user_relationships" +
            ",email" +
            ",friends_status" +
            ",read_stream" +
            ",user_status" ;

        private string _accessToken;

        public MyLOForm1()
        {
            InitializeComponent();
        }

        private void MyLOForm1_Load(object sender, EventArgs e)
        {
            try
            {
                _ms = new MyLoStore("MyLo01");

            }
            catch (Exception ex)
            {
                Console.WriteLine("RDFTest Error");
                Console.WriteLine(ex.Message);
            }
        }

        private void buttonFBlogin_Click(object sender, EventArgs e)
        {
            var fbLoginDialog = new FacebookLoginDialog(AppId, ExtendedPermissions);
            fbLoginDialog.ShowDialog();

            GetItemsFromFacebook(fbLoginDialog.FacebookOAuthResult);


            // ********** Temporary for debugging when not calling Facebook Login Dialog using lines above
            //_ms.MyLoFacebookAlignment();

            //string outFile = @"../../../Output1.txt";
            //using (StreamWriter outStream = new StreamWriter(outFile, false))
            //{
            //    _ms.WriteStore(outStream);
            //}
            // ***********************
        }

        private void GetItemsFromFacebook(FacebookOAuthResult facebookOAuthResult)
        {
            if (facebookOAuthResult != null)
            {
                if (facebookOAuthResult.IsSuccess)
                {
                    _accessToken = facebookOAuthResult.AccessToken;
                    var fb = new FacebookClient(facebookOAuthResult.AccessToken);

                    try
                    {
                        //dynamic parameters = new ExpandoObject();
                        //parameters.fields = "locations";
                        //dynamic result2 = fb.Get("me", parameters);

                        //Console.WriteLine(result2);

                        //foreach (var loc in result2.locations.data)
                        //{
                        //    string lName = loc.place.name;
                        //    Console.WriteLine(lName);
                        //}


                        _ms.AddFacebookLocationsToGraph(facebookOAuthResult.AccessToken);

                        _ms.AddFacebookEventsToGraph(facebookOAuthResult.AccessToken);

                        _ms.MyLoFacebookAlignment();

                        _ms.MyLoSchemaReasoner();

                        string outFile = @"../../../Output1.txt";
                        using (StreamWriter outStream = new StreamWriter(outFile, false))
                        {
                            _ms.WriteGraph(outStream);
                        }

                        textBox1.Text = "Finished Finding Items on Facebook";

                        buttonFBlogout.Visible = true;
                    }
                    catch (FacebookOAuthException fbEx)
                    {
                        // oauth exception occurred
                        textBox1.Text = String.Format("FacebookAPI Oauth Error {0}", fbEx.Message);
                    }
                    catch (FacebookApiLimitException fbEx)
                    {
                        // api limit exception occurred.
                        textBox1.Text = String.Format("FacebookAPI App Limit Error {0}", fbEx.Message);
                    }
                    catch (FacebookApiException fbEx)
                    {
                        // other general facebook api exception
                        textBox1.Text = String.Format("FacebookAPI App Error {0}", fbEx.Message);
                    }
                    catch (Exception fbEx)
                    {
                        // non-facebook exception such as no internet connection.
                        textBox1.Text = String.Format("Non Facebook API App Error {0}", fbEx.Message);
                    }
                }
                else
                {
                    textBox1.Text = String.Format(facebookOAuthResult.ErrorDescription);
                }
            }
        }

        private void buttonFBlogout_Click(object sender, EventArgs e)
        {
            var webBrowser = new WebBrowser();
            var fb = new FacebookClient();
            var logouUrl = fb.GetLogoutUrl(new { access_token = _accessToken, next = "https://www.facebook.com/connect/login_success.html" });
            webBrowser.Navigate(logouUrl);
            buttonFBlogout.Visible = false;
            textBox1.Text = String.Format("Logged Out from Faceboook");
        }

        private void btnSaveStore_Click(object sender, EventArgs e)
        {
            _ms.SaveGraphToFile();
        }

        private void query1result_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnRunQuery1_Click(object sender, EventArgs e)
        {
            if (this.query1result.Text != String.Empty)
            {
                string results = _ms.RunSPARQLQuery(this.query1result.Text);
                this.queryOutBox.Text = results;
            }

        }

        private void queryOutBox_TextChanged(object sender, EventArgs e)
        {

        }

    }
}
