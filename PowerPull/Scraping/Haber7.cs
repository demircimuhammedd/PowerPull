using HtmlAgilityPack;
using PowerPull.Helper;
using PowerPull.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PowerPull.Scraping
{
  public static class Haber7
  {
    public static void Run()
    {
      HtmlWeb web = new HtmlWeb();

      List<Content> listContent = new List<Content>();
      Console.WriteLine("Start: {0}", DateTime.Now);

      Console.WriteLine("Haber7 Kaçıncı sayfadan başlasın? ");
      int page = NumberHelper.InputControl(Console.ReadLine());

      Console.WriteLine("Haber7 Kaç sayfa olsun? ");
      int count = NumberHelper.InputControl(Console.ReadLine());

      List<string> listDetailUrl = DetailUrlCrawler(page, count, web);
      Console.WriteLine("Bulunan makale sayısı: {0}", listDetailUrl.Count);

      Scrap(listDetailUrl, web, listContent);
      Console.WriteLine("Kaydedilebilen makale sayısı: {0}", listContent.Count);
      ApiHelper.SaveBlogAsync(listContent).GetAwaiter().GetResult();
    }

    private static List<string> DetailUrlCrawler(int page, int count, HtmlWeb web)
    {
      List<string> listDetailUrl = new List<string>();

      string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
      string path = Directory.GetFiles(baseDirectory).SingleOrDefault(q => q.Contains("Haber7Categories.txt"));

      string[] readText = File.ReadAllLines(path);
      foreach (string itemUrl in readText)
      {
        Console.WriteLine(itemUrl);
        string baseUrl = itemUrl;
        for (int i = (page - 1); i < ((page - 1) + count); i++)
        {
          try
          {
            string url = string.Format("{0}p{1}?infinity-haber", baseUrl, i + 1);
            Console.WriteLine("Url: {0}", url);
            HtmlNode node = web.Load(url).DocumentNode;
            var postBox = node.QuerySelectorAll(".news-list ul li a");
            foreach (var item in postBox)
            {
              string detailUrl = string.Format("{0}", item.Attributes["href"].Value);
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
      }
      return listDetailUrl;
    }

    private static void Scrap(List<string> listDetailUrl, HtmlWeb web, List<Content> listContent)
    {
      foreach (var detailUrl in listDetailUrl)
      {
        try
        {
          HtmlNode node = web.Load(string.Format("{0}", detailUrl)).DocumentNode;
          var bodyElement = node.QuerySelectorAll(".news-content");

          var urlSegments = new List<string>(new UriBuilder(detailUrl).Uri.Segments);
          string caption = urlSegments[urlSegments.Count - 1].Replace("/", "");
          Console.WriteLine("Data toplanıyor: {0}", caption);

          string body = string.Empty;
          var paragraphs = bodyElement.QuerySelectorAll("p");
          foreach (var paragraph in paragraphs)
            body += paragraph.InnerText;

          string title = string.Empty;
          string media = string.Empty;

          var elementTitleType1 = node.QuerySelector(".head h1");
          var elementTitleType2 = node.QuerySelector(".news-header h1");

          var elementImageType1 = node.QuerySelector(".media img");
          var elementImageType2 = node.QuerySelector(".news-image img");

          if (elementTitleType1 != null)
            title = elementTitleType1.InnerText;
          else
            title = elementTitleType2.InnerText;

          if (elementImageType1 != null)
            media = elementImageType1.Attributes["src"].Value;
          else
            media = elementImageType2.Attributes["src"].Value;

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
  }
}
