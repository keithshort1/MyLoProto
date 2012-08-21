/*

Copyright Keith Short 2012
keith_short@hotmail.com

This file is part of the MyLo application

------------------------------------------------------------------------
*/

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
using Newtonsoft.Json;
using MyLoFacebookContextReaderNS;
using System.Diagnostics;
using GPSlookupNS;

namespace MyLoFacebookContextApp
{
    public partial class MyLoForm1 : Form
    {
        private const string AppId = "396995136986637";

        private MyLoFacebookContextReader _fbContext;
        private BingMapsGPSlookup _bingGpsLookup;

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

        public MyLoForm1()
        {
            InitializeComponent();
        }

        private void MyLOForm1_Load(object sender, EventArgs e)
        {
            try
            {
                _bingGpsLookup = new BingMapsGPSlookup();
                _fbContext = new MyLoFacebookContextReader(_bingGpsLookup);
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


                        _fbContext.AddFacebookLocationsToContext(facebookOAuthResult.AccessToken);

                        _fbContext.AddFacebookEventsToContext(facebookOAuthResult.AccessToken);

                        _fbContext.MyLoFacebookAlignment01();

                        _fbContext.MyLoSchemaReasoner();

                        _fbContext.SaveContextToFile();

                        string outFile = @"../../../Output1.txt";
                        using (StreamWriter outStream = new StreamWriter(outFile, false))
                        {
                            _fbContext.WriteGraph(outStream);
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
            try
            {
                MyLoFacebookContextReader _fbContext = new MyLoFacebookContextReader(_bingGpsLookup);
                _fbContext.InitializeContextFromFile();

                _fbContext.MyLoFacebookAlignment02();
                _fbContext.MyLoSchemaReasoner();
                _fbContext.SaveContextToFile();

                // Not sure if this is a bug in the dotNetRDF library, but it seems to require a fresh store and read from file
                // to make the second inference rules work ... investigating ...
                MyLoFacebookContextReader _fbContext2 = new MyLoFacebookContextReader(_bingGpsLookup);
                _fbContext2.InitializeContextFromFile();
                _fbContext2.SaveContextToDB("Keith");
                this.queryOutBox.Text = "Context saved to database";
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Store Error");
                Debug.WriteLine(ex.Message);
            }
        }

        private void query1result_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnRunQuery1_Click(object sender, EventArgs e)
        {
            if (this.query1result.Text != String.Empty)
            {
                MyLoFacebookContextReader _fbContext = new MyLoFacebookContextReader(_bingGpsLookup);
                _fbContext.InitializeContextFromFile();
                string results = _fbContext.RunSPARQLQuery(this.query1result.Text);
                this.queryOutBox.Text = results;
            }

        }

        private void queryOutBox_TextChanged(object sender, EventArgs e)
        {

        }

    }
}
