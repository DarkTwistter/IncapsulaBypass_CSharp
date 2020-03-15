using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jurassic;
using System.IO;
using System.Text.RegularExpressions;
using xNetStandard;

namespace IncapsulaBypass
{
    class Program
    {
        static void Main(string[] args)
        {
         Regex incapsula_regex_code = new Regex("(?<=var b=\")(.*?)(?=\")");
         Regex incapsula_regex_link = new Regex("(?<=xhr\\.open\\(\"GET\",\")(.*?)(?=\")");
         string js;
         string site;
         string fullSite;
         fullSite = args[0];


            getSite();

            void getSite()
            {
                site = fullSite.Replace("http://", "").Replace("https://", "").TrimEnd('/');

            }


            js = File.ReadAllText("incapsula.js");





                HttpRequest httpRequest = new HttpRequest();
                httpRequest.Cookies = new CookieDictionary(false);
                httpRequest.ConnectTimeout = 10000;
                httpRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML like Gecko) Chrome/29.0.1547.76 Safari/537.36";
                httpRequest.IgnoreProtocolErrors = true;

                string input = httpRequest.Get(site, (RequestParams)null).ToString();

                ScriptEngine scriptEngine = new ScriptEngine();
                scriptEngine.Execute(js);

                string b = incapsula_regex_code.Match(input).Value;
                string link = incapsula_regex_link.Match(scriptEngine.Evaluate<string>("String.fromCharCode(" + scriptEngine.Evaluate<string>("getCode(\"" + b + "\")") + ")")).Value;

                httpRequest.Get(site + link, (RequestParams)null);

                var output = httpRequest.Get(site, (RequestParams)null).ToString();

                Uri myUri = new Uri(fullSite);
                Directory.CreateDirectory(myUri.Host);


                using (StreamWriter sw = File.CreateText($"HTML_{myUri.Host}.txt"))
                {
                //Console.WriteLine("created HTML");
                    sw.WriteLine(output);
                }
                using (StreamWriter sw = File.CreateText($"Cookie_{myUri.Host}.txt"))
                {
                //Console.WriteLine("created Cookie");
                sw.WriteLine(httpRequest.Cookies);
                }
            

                string currentPath = Directory.GetCurrentDirectory();
                //Console.WriteLine(currentPath);
                //string moveTo = System.IO.Path.Combine(currentPath, $"/{myUri.Host}", $"HTML_{myUri.Host}.txt");

            //Console.WriteLine(currentPath + $"/{myUri.Host}/" + $"HTML_{myUri.Host}.txt");
              if (File.Exists(currentPath + $"/{myUri.Host}/" + $"HTML_{myUri.Host}.txt"))
                File.Delete(currentPath + $"/{myUri.Host}/" + $"HTML_{myUri.Host}.txt");

            if (File.Exists(currentPath + $"/{myUri.Host}/" + $"Cookie_{myUri.Host}.txt"))
                File.Delete(currentPath + $"/{myUri.Host}/" + $"Cookie_{myUri.Host}.txt");


            
            File.Move($"HTML_{myUri.Host}.txt", currentPath + $"/{myUri.Host}/" + $"HTML_{myUri.Host}.txt");
            File.Move($"Cookie_{myUri.Host}.txt", currentPath + $"/{myUri.Host}/" + $"Cookie_{myUri.Host}.txt");



        }
    }
}
