using PowerPull.Helper;
using PowerPull.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace PowerPull.Scraping
{
    public class VanityFair
    {
        public static void Run()
        {
            Console.WriteLine("Start: {0}", DateTime.Now);
            Console.WriteLine("VanityFair Kaçıncı sayfadan başlasın? ");
            int page = NumberHelper.InputControl(Console.ReadLine());

            Console.WriteLine("VanityFair Kaç sayfa olsun? ");
            int count = NumberHelper.InputControl(Console.ReadLine());
            List<Content> listContent = Scrap(page, count);
            Console.WriteLine("Bulunan Makale Sayısı: {0}", listContent.Count);
            FileHelper.SaveFile(listContent, "VanityFair");
        }

        private static List<Content> Scrap(int page, int count)
        {
            List<Content> listContent = new List<Content>();
            for (int i = (page - 1); i < ((page - 1) + count); i++)
            {
                try
                {
                    string url = string.Format("https://www.vanityfair.com/api/search?page={0}&size={1}&sort=publishDate%20desc&types=article&q=&tags=&channel=Style&subchannel=Fashion&contributor=&nottags=_noriver", i + 1, 10);

                    Console.WriteLine("Url: {0}", url);
                    using (WebClient webClient = new WebClient())
                    {
                        var data = webClient.DownloadString(url);
                        var listVanityFair = JsonConvert.DeserializeObject<dynamic>(data)["hits"]["hits"];
                        foreach (var item in listVanityFair)
                        {
                            var source = item["_source"];
                            string title = Regex.Replace(source["hed"].Value, "[^0-9a-zA-Z- ]+", "");
                            string body = Regex.Replace(HtmlHelper.RemoveHtml(source["body"].Value), "[^0-9a-zA-Z- ]+", "");

                            var image = JsonConvert.DeserializeObject<dynamic>(source["_embedded"]["photosTout"].ToString()).Last;

                            string imageId = image["fields"]["id"].Value;
                            string imageTitle = image["fields"]["filename"].Value;
                            string imageUrl = string.Format("https://media.vanityfair.com/photos/{0}/master/w_768,c_limit/{1}", imageId, imageTitle);
                             
                            string caption = Regex.Replace(Regex.Replace(source["seoTitle"].Value.ToLowerInvariant(), "[^0-9a-zA-Z- ]+", ""), " ", "-");

                            Console.WriteLine("Data toplanıyor: {0}", caption);
                            listContent.Add(new Content
                            {
                                Title = title,
                                Media = imageUrl,
                                Body = body,
                                Caption = caption
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Hata oluştu: {0}", ex.Message);
                    Console.WriteLine("Devam ediliyor..");
                    continue;
                }
            }
            return listContent;
        }
    }
}
