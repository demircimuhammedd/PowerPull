using HtmlAgilityPack;
using PowerPull.Helper;
using PowerPull.Models;
using System;
using System.Collections.Generic;

namespace PowerPull.Scraping
{
  public static class Elle
  {
    public static void Run()
    {
      HtmlWeb web = new HtmlWeb();

      List<Content> listContent = new List<Content>();
      Console.WriteLine("Start: {0}", DateTime.Now);

      Console.WriteLine("Elle Kaçıncı sayfadan başlasın? ");
      int page = NumberHelper.InputControl(Console.ReadLine());

      Console.WriteLine("Elle Kaç sayfa olsun? ");
      int count = NumberHelper.InputControl(Console.ReadLine());

      List<string> listDetailUrl = DetailUrlCrawler(page, count, web);
      Console.WriteLine("Bulunan makale sayısı: {0}", listDetailUrl.Count);

      Scrap(listDetailUrl, web, listContent);
      Console.WriteLine("Kaydedilebilen makale sayısı: {0}", listContent.Count);
      ApiHelper.SaveBlogAsync(listContent).GetAwaiter().GetResult();
      //FileHelper.SaveFile(listContent, "Elle");
    }

    private static List<string> DetailUrlCrawler(int page, int count, HtmlWeb web)
    {
      List<string> listDetailUrl = new List<string>();
      string baseUrl = "https://www.elle.com/";
      for (int i = (page - 1); i < ((page - 1) + count); i++)
      {
        try
        {
          string url = string.Format("{0}ajax/infiniteload/?id=ba469bb1-15a6-4d9e-85d6-98b4cae84000&class=CoreModels%5Csections%5CSectionModel&viewset=section&page={1}", baseUrl, i + 1);
          Console.WriteLine("Url: {0}", url);
          HtmlNode node = web.Load(url).DocumentNode;
          var postBox = node.QuerySelectorAll(".full-item-title");
          foreach (var item in postBox)
          {
            string detailUrl = string.Format("{0}{1}", baseUrl, item.Attributes["href"].Value);
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

    private static void Scrap(List<string> listDetailUrl, HtmlWeb web, List<Content> listContent)
    {
      foreach (var detailUrl in listDetailUrl)
      {
        try
        {
          HtmlNode node = web.Load(string.Format("{0}", detailUrl)).DocumentNode;
          var bodyElement = node.QuerySelector(".article-body-content");

          if (bodyElement != null)
          {
            var urlSegments = new List<string>(new UriBuilder(detailUrl).Uri.Segments);
            string caption = urlSegments[urlSegments.Count - 1].Replace("/", "");
            Console.WriteLine("Data toplanıyor: {0}", caption);

            string body = string.Empty;
            var paragraphs = bodyElement.QuerySelectorAll("p");
            foreach (var paragraph in paragraphs)
              body += paragraph.InnerText;

            string title = string.Empty;
            string media = string.Empty;

            var elementTitleType1 = node.QuerySelector("h1 .content-hed");
            var elementImageType1 = node.QuerySelector(".content-lede-image-wrap img");

            if (elementTitleType1 != null)
              title = elementTitleType1.InnerText;

            if (elementImageType1 != null)
              media = elementImageType1.Attributes["data-src"].Value;

            listContent.Add(new Content
            {
              Title = title,
              Media = media,
              Body = body,
              DetailUrl = detailUrl,
              Caption = caption
            });
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
  }
}
