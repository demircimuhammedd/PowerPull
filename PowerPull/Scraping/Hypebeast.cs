using HtmlAgilityPack;
using PowerPull.Helper;
using PowerPull.Models;
using System;
using System.Collections.Generic;
using WordPressSharp;
using WordPressSharp.Models;

namespace PowerPull.Scraping
{
    internal static class Hypebeast
    {
        public static void Run()
        {
            HtmlWeb web = new HtmlWeb();

            List<Content> listContent = new List<Content>();
            Console.WriteLine("Start: {0}", DateTime.Now);

            Console.WriteLine("Hypebeast Kaçıncı sayfadan başlasın? ");
            int page = NumberHelper.InputControl(Console.ReadLine());

            Console.WriteLine("Hypebeast Kaç sayfa olsun? ");
            int count = NumberHelper.InputControl(Console.ReadLine());

            List<string> listDetailUrl = DetailUrlCrawler(page, count, web);
            Console.WriteLine("Bulunan makale sayısı: {0}", listDetailUrl.Count);

            Scrap(listDetailUrl, web, listContent); 

            ApiHelper.SaveBlogAsync(listContent).GetAwaiter().GetResult(); 
        }

        private static void Scrap(List<string> listDetailUrl, HtmlWeb web, List<Content> listContent)
        {
            foreach (var detailUrl in listDetailUrl)
            {
                try
                {
                    HtmlNode node = web.Load(string.Format("{0}", detailUrl)).DocumentNode;

                    string title = string.Empty;
                    string media = string.Empty;
                    string body = string.Empty;
                    string caption = new UriBuilder(detailUrl).Uri.Segments[3].ToString();

                    Console.WriteLine("Data toplanıyor: {0}", caption);

                    var elementTitleType1 = node.QuerySelector(".post-gallery-info-title");
                    var elementTitleType2 = node.QuerySelector(".post-body-title");
                    var elementImageType1 = node.QuerySelector(".carousel-cell-image");
                    var elementImageType2 = node.QuerySelectorAll("iframe");
                    var paragraphs = node.QuerySelector(".post-body-content").ChildNodes.QuerySelectorAll("p");

                    foreach (var paragraph in paragraphs)
                        body += paragraph.InnerText;

                    if (elementTitleType1 != null)
                        title = elementTitleType1.InnerText;

                    if (elementTitleType2 != null)
                        title = elementTitleType2.InnerText;

                    if (elementImageType1 != null)
                        media = elementImageType1.Attributes["src"].Value;

                    if (elementImageType2 != null && (elementTitleType1 != null || elementTitleType2 != null))
                    {
                        foreach (var imageType2 in elementImageType2)
                        {
                            if (imageType2.ParentNode.QuerySelector("div") != null)
                                media = imageType2.OuterHtml;
                        }
                    }

                    listContent.Add(new Content
                    {
                        Title = title,
                        Media = media,
                        Body = body,
                        DetailUrl = detailUrl,
                        Caption = caption
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Hata oluştu: {0}", ex.Message);
                    Console.WriteLine("Devam ediliyor..");
                    continue;
                }
            }
        }

        private static List<string> DetailUrlCrawler(int page, int count, HtmlWeb web)
        {
            List<string> listDetailUrl = new List<string>();
            string baseUrl = "https://hypebeast.com/fashion/";
            for (int i = (page - 1); i < ((page - 1) + count); i++)
            {
                try
                {
                    string url = string.Format("{0}page/{1}", baseUrl, i + 1);
                    Console.WriteLine("Url: {0}", url);
                    HtmlNode node = web.Load(url).DocumentNode;
                    var postBox = node.QuerySelectorAll(".post-box");
                    foreach (var item in postBox)
                    {
                        string detailUrl = item.Attributes["data-permalink"].Value;
                        listDetailUrl.Add(detailUrl);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Hata oluştu: {0}", ex.Message);
                    Console.WriteLine("Devam ediliyor..");
                    continue;
                }

            }
            return listDetailUrl;
        }
    }
}
