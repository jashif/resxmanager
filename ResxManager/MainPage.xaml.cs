using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ResxManager
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }
        Xml2CSharp.Root wrroot;
        List<Xml2CSharp.Data> andrdatas = new List<Xml2CSharp.Data>();
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Xml2CSharp.Root merged = new Xml2CSharp.Root();
            merged.Data = new List<Xml2CSharp.Data>();

            StorageFile wfile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Resx.xml"));
            var stream = await wfile.OpenStreamForReadAsync();
            wrroot = Xml2CSharp.XMLUtil.DeSerialize(stream) as Xml2CSharp.Root;

            andrdatas = new List<Xml2CSharp.Data>();
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///values/strings.xml"));
            var data = await FileIO.ReadTextAsync(file);


            var eles = XDocument.Parse(data).Descendants("string");
            foreach (var el in eles)
            {
                Xml2CSharp.Data dt = new Xml2CSharp.Data();
                dt.Name = el.Attribute("name").Value.ToString();
                dt.AId = el.Attribute("name").Value.ToString();
                dt.Space = "reserve";
                dt.Value = el.Value;
                dt.Comment = el.Value;
                if (wrroot.Data.Where(x => x.Name.ToLower() == el.Attribute("name").Value.ToLower()).Count() != 0)
                {
                    if (wrroot.Data.Where(x => x.Value.ToLower() == el.Value.ToLower()).Count() == 0)
                    {
                        var item = wrroot.Data.FirstOrDefault(x => x.Name.ToLower() == el.Attribute("name").Value.ToLower());
                        wrroot.Data[wrroot.Data.IndexOf(item)].AId = dt.AId;
                        item.Value = dt.Value;
                        item.AId = dt.AId;

                    }
                    else
                    {
                        var item = wrroot.Data.FirstOrDefault(x => x.Name.ToLower() == el.Attribute("name").Value.ToLower());
                        wrroot.Data[wrroot.Data.IndexOf(item)].AId = dt.AId;
                        item.AId = dt.AId;

                    }

                }
                else
                {
                    if (wrroot.Data.Where(x => x.Value.ToLower() == el.Value.ToLower()).Count() == 0)
                    {
                        wrroot.Data.Add(dt);
                    }
                    else
                    {
                        var item = wrroot.Data.FirstOrDefault(x => x.Value.ToLower() == el.Value.ToLower());
                        wrroot.Data[wrroot.Data.IndexOf(item)].AId = dt.AId;
                        item.AId = dt.AId;

                    }
                }

                andrdatas.Add(dt);
            }

        }
        private async void LoadAn()
        {
            StorageFile fullrestfile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///resxfile.txt"));

            string fulltext = await FileIO.ReadTextAsync(fullrestfile);
            Xml2CSharp.Root rt = new Xml2CSharp.Root();
            rt.replace = "hi";
            rt.Data = new List<Xml2CSharp.Data>();

            StorageFile restfile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///values-" + lancode.Text + "/strings.xml"));
            string text = await FileIO.ReadTextAsync(restfile);
            var eles = XDocument.Parse(text).Descendants("string");
            prg.Maximum = wrroot.Data.Count;
            int i = 0;
            foreach (var vl in wrroot.Data)
            {
                Xml2CSharp.Data dt = new Xml2CSharp.Data();
                dt = vl;
                dt.Space = "preserve";
                var val = eles.Where(x => x.Attribute("name").Value == vl.AId).FirstOrDefault()?.Value;
                if (val != null)
                    dt.Value = val;
                else
                {
                    dt.Value = await Translate(vl.Value);
                    await Task.Delay(100);
                }
                prg.Value = i;
                t1.Text = dt.Name;
                t2.Text = dt.Value;
                i++;
            
                rt.Data.Add(dt);
            }
            var serialisedxml = Xml2CSharp.XMLUtil.Serialize(rt);

            var xml = XElement.Parse(serialisedxml).Elements("data").Select(x => x.ToString());
            string dtxml = "";
            foreach (var x in xml)
            {
                dtxml += x;
            }
            fulltext = fulltext.Replace("<replace>", dtxml);
            var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(lancode.Text + "-IN",CreationCollisionOption.ReplaceExisting);
            StorageFile reswfile = await folder.CreateFileAsync("Resources.resw",CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(reswfile, fulltext);
        }
        private async void Load()
        {
            try
            {
                StorageFile restfile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///resxfile.txt"));

                string text = await FileIO.ReadTextAsync(restfile);

                Xml2CSharp.Root root2 = new Xml2CSharp.Root();
                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Resx.xml"));
                root2.Data = new List<Xml2CSharp.Data>();
                var stream = await file.OpenStreamForReadAsync();

                Xml2CSharp.Root root = Xml2CSharp.XMLUtil.DeSerialize(stream) as Xml2CSharp.Root;
                prg.Maximum = root.Data.Count;
                int i = 0;
                foreach (var rt in root.Data)
                {
                    Xml2CSharp.Data dt = new Xml2CSharp.Data();
                    dt.Name = rt.Name;

                    root2.Data.Add(dt);
                    dt.Value = await Translate(rt.Value);
                    dt.Space = "preserve";
                    dt.Comment = rt.Value;
                    prg.Value = i;
                    t1.Text = rt.Value;
                    t2.Text = dt.Value;
                    i++;
                    await Task.Delay(100);
                }
                root2.replace = "hi";
                //<replace>hi</replace>
                //   root2.Resheader = root.Resheader;
                //root2.Schema = root.Schema;
                var serialisedxml = Xml2CSharp.XMLUtil.Serialize(root2);

                var xml = XElement.Parse(serialisedxml).Elements("data").Select(x => x.ToString());
                string dtxml = "";
                foreach (var x in xml)
                {
                    dtxml += x;
                }
                text = text.Replace("<replace>", dtxml);
                var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(lancode.Text + "-IN");
                StorageFile reswfile = await folder.CreateFileAsync("Resources.resw");
                await FileIO.WriteTextAsync(reswfile, text);

            }
            catch (Exception ex)
            {

                // throw;
            }
        }

        private async Task<string> Translate(string text)
        {
            if (text == "") return "";
            string encode = WebUtility.UrlEncode(text.ToLower());
            string url = "";// $"https://translate.google.co.in/translate_a/single?client=t&sl=en&tl={lancode.Text}&hl=en&dt=at&dt=bd&dt=ex&dt=ld&dt=md&dt=qca&dt=rw&dt=rm&dt=ss&dt=t&ie=UTF-8&oe=UTF-8&source=btn&rom=1&ssel=3&tsel=4&kc=0&tk=180409.323042&q={encode}";
            url = $"https://translate.google.com/translate_a/single?client=at&dt=t&dt=ld&dt=qc&dt=rm&dt=bd&dj=1&sl=en&tl={lancode.Text}&hl=en_GB&ie=UTF-8&oe=UTF-8&iid=85a8c6a4-027c-47a7-80e7-a85a9633dac4&q={encode}&inputm=2&itid=pk&otf=1";
            //https://content.googleapis.com/language/translate/v2?q={encode}&target={lancode.Text}&source=en&key=AIzaSyD-a9IF8KKYgoC3cpgS-Al7hLQDbugrDcw";
            //$"http://api.microsofttranslator.com/v2/ajax.svc/TranslateArray2?appId=%22TxCDkAWsS7O77LG7lop_IB0Us6-0OpHz3QCAenI35_DE*%22&texts=%5B%22{text}%22%5D&from=%22en%22&to=%22{destlang}%22";
            try
            {
                var htpclient = new HttpClient();

                Dictionary<string, string> ict = new Dictionary<string, string>
                {

{"Host" ,"translate.google.com"},
{"Connection", "Keep-Alive"},
{"User-Agent", "AndroidTranslate/4.4.0.RC01.104701208-44000162 4.4.4 phone GTR_TRANS_WLOPV1_ANDROID GTR_TRANS_WLOPV1_DE_EN_AR"}
                };
                foreach (var item in ict.Keys)
                {
                    htpclient.DefaultRequestHeaders.Add(item, ict[item]);
                }
                var response = await htpclient.GetStringAsync(url);
                if (response != null)
                {
                    var data = Windows.Data.Json.JsonObject.Parse(response)["sentences"].GetArray();
                    string dt = "";
                    foreach (var item in data)
                    {
                        var obj = item.GetObject();
                        if (obj.ContainsKey("trans"))
                        {
                            dt += obj["trans"].GetString();
                        }
                    }
                    //[0].GetObject()["trans"].GetString();
                    // var replc = response.Replace(@"[[[""", "");
                    // string data = replc.Split(',')[0];
                    return dt;
                    // Windows.Data.Json.JsonObject arry = ((Windows.Data.Json.JsonValue)(Windows.Data.Json.JsonArray.Parse(response).FirstOrDefault())).GetObject();
                    // return arry["TranslatedText"].GetString();
                }
            }
            catch (Exception ex)
            {

            }
            return "";
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {

            LoadAn();
        }
    }
}

