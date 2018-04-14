using PowerPull.Helper;
using PowerPull.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace PowerPull.Scraping
{
    public static class Qz
    {
        public static void Run()
        {
            Console.WriteLine("Start: {0}", DateTime.Now);
            Console.WriteLine("Qz Kaçıncı sayfadan başlasın? ");
            int page = NumberHelper.InputControl(Console.ReadLine());

            Console.WriteLine("Qz Kaç sayfa olsun? ");
            int count = NumberHelper.InputControl(Console.ReadLine());
            List<Content> listContent = Scrap(page, count);
            Console.WriteLine("Bulunan Makale Sayısı: {0}", listContent.Count);
            FileHelper.SaveFile(listContent, "Qz");
        }

        private static List<Content> Scrap(int page, int count)
        {
            List<Content> listContent = new List<Content>();
            for (int i = (page - 1); i < ((page - 1) + count); i++)
            {
                try
                {
                    string url = string.Format("https://qz.com/api/obsession/fashion/page/{0}/", i + 1);

                    Console.WriteLine("Url: {0}", url);
                    using (WebClient webClient = new WebClient())
                    {
                        var data = webClient.DownloadString(url);
                        var listPost = JsonConvert.DeserializeObject<dynamic>(data)["items"];
                        foreach (var item in listPost)
                        {
                            string caption = Regex.Replace(Regex.Replace(item["seoTitle"].Value.ToLowerInvariant(), "[^0-9a-zA-Z- ]+", ""), " ", "-");
                            Console.WriteLine("Data toplanıyor: {0}", caption);
                            string title = Regex.Replace(item["title"].Value, "[^0-9a-zA-Z- ]+", "");
                            string body = Regex.Replace(HtmlHelper.RemoveHtml(item["content"].Value), "[^0-9a-zA-Z- ]+", "");
                            string imageUrl = item["hero"]["url"].Value;
                            string detailUrl = item["permalink"].Value;

                            listContent.Add(new Content
                            {
                                Title = title,
                                Media = imageUrl,
                                Body = body,
                                Caption = caption,
                                DetailUrl = detailUrl
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
