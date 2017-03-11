using System;
using System.Windows.Forms;
using System.IO;
using System.Net;

namespace TC
{
    public partial class Form1 : Form
    {


      

        static string filterUri = "_DisplayType_LF";
        static string filtermUri = "_PciaId_";

        public static string Generate(string marca, string modelo, string estado, int code)
        {

            if (code == 0) return "http://listado." + "tucarro.com.ve" + "/carros/" + marca + "/" + modelo + "/" + estado +   filterUri;
            else return "http://listado." + "tumoto.com.ve" + "/motos/" + modelo + "/" + marca + "/" + estado + filtermUri;
        }


        public static string Query(string uri)
        {



     

            string completo = string.Empty;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";


            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();

            StreamReader reader = new StreamReader(responseStream);

            completo = reader.ReadToEnd();


            reader.Close();
            reader.Dispose();
            reader = null;
            responseStream.Close();
            responseStream.Dispose();
            responseStream = null;

            response = null;
            request = null;


            return completo;
        }


        public Form1()
        {
            InitializeComponent();

            this.webbx.SelectedIndex = 0;

        }

        private void listBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.ListBS.EndEdit();
            this.TAM.UpdateAll(this.tC);

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'tC.Carros' table. You can move, or remove it, as needed.
            this.carrosTableAdapter.Fill(this.tC.Carros);
            // TODO: This line of code loads data into the 'tC.List' table. You can move, or remove it, as needed.
            this.ListTA.Fill(this.tC.List);

        }

        private void querybtn_Click(object sender, EventArgs e)
        {


            this.tC.List.Clear();


            //string estadobx = "miranda";

            string uri = Generate(marcabx.Text, modelobx.Text, estadobx.Text, this.webbx.SelectedIndex);
            string res = Query(uri);
            string[] result=   res.Split('\n');
            int i = 0;
            for (i = 250; i < result.Length; i++)
            {
                res = result[i];
                if (res.Contains("meta name")) break;

            }
            res = result[i];
            result = res.Split('<');
     

            for (i = 500; i < result.Length;i++)
            {
                string r = result[i];
                if (r.Contains("li class="))
                {
                    string id = string.Empty;
                    string link = string.Empty;
                    string ano = string.Empty;
                    string trans = string.Empty;

                    if (r.Contains("pagination") || !r.Contains("list-view-item rowItem"))
                    {
                        continue;
                    }
                    try
                    {

                   
                  
                    id = r.Remove(0,38);
                    id = id.Split('"')[0];
                    i=i+2;
                    link = result[i].Remove(0,8);
                    link = link.Split('"')[0];
                    i = i + 4;
                     ano = result[i].Remove(0, 41);
                  
                    i = i + 2;
                     trans = result[i].Remove(0, 35);
                    i = i + 10;

                    }
                    catch (Exception)
                    {

                       // throw;
                    }
                    this.tC.List.AddListRow(estadobx.Text, marcabx.Text, modelobx.Text, ano, trans,id,link);

                }
            }

         
            

        }

        private void databtn_Click(object sender, EventArgs e)
        {

            this.tC.Carros.Clear();

            string res;
            string[] result;
            int i;
            foreach (TCdb.ListRow r in this.tC.List)
            {
                string content = string.Empty;
                string telf = string.Empty;

                try
                {
                    res = Query(r.Link + "?noIndex=true&showPhones=true");
                    result = res.Split('\n')[1].Split('<');
                    i = 200;
                    content = string.Empty;
                    telf= string.Empty;
                    for (i = 270; i < result.Length; i++)
                    {
                        res = result[i];
                        if (res.Contains("strong>") && !res.Contains("/strong>"))
                        {
                            content += "*" + res.Replace("strong>", null);
                        }
                        else if (res.Contains("ch-icon-phone"))
                        {
                            telf = result[i + 2].Remove(0, 43);
                        }

                    }

               

              
                   
                }
                catch(Exception ex)
                {

                }

                string[] data = content.Split('*');

                Application.DoEvents();

                this.tC.Carros.AddCarrosRow(r, data[3], data[4], data[5], data[10], data[12], data[13], data[14], data[16], "", data[17], r.Estado, telf, data[15]);

            }


        }
    }
}
