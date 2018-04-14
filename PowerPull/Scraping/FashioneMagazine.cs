using HtmlAgilityPack;
using PowerPull.Helper;
using PowerPull.Models;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;

namespace PowerPull.Scraping
{
  public static class FashioneMagazine
  {
    public static void Run()
    {
      Console.WriteLine("FashioneMagazine işleniyor bekleyiniz..");

      ChromeOptions option = new ChromeOptions();
      option.AddArgument("--headless");
      ChromeDriver chromeDriver = new ChromeDriver(option)
      {
        Url = "https://fashionmagazine.com/fashion/"
      };

      Scrool(chromeDriver);
      ApiHelper.SaveBlogAsync(Scrap(Crawler(chromeDriver))).GetAwaiter().GetResult();
    }

    private static List<string> Crawler(ChromeDriver chromeDriver)
    {
      HtmlDocument htmlDoc = new HtmlDocument();
      List<string> listDetailUrl = new List<string>();
      htmlDoc.LoadHtml(chromeDriver.PageSource);
      IList<HtmlNode> postBox = htmlDoc.QuerySelectorAll(".sjmlm_bucket");
      foreach (var item in postBox)
      {
        listDetailUrl.Add(item.Attributes["data-scroll_url"].Value);
      }
      chromeDriver.Quit();
      Console.Clear();
      return listDetailUrl;
    }

    private static List<Content> Scrap(List<string> listDetailUrl)
    {
      List<Content> listContent = new List<Content>();
      HtmlWeb web = new HtmlWeb();
      Console.WriteLine("Bulunan makale sayısı " + listDetailUrl.Count);
      foreach (var item in listDetailUrl)
      {
        try
        {
          string url = string.Format("{0}", item);
          Console.WriteLine("Url: {0}", url);
          HtmlNode node = web.Load(url).DocumentNode;
          string title = Regex.Replace(HtmlHelper.RemoveHtml(node.QuerySelector(".article-headline").InnerText), "[^0-9a-zA-Z- ]+", "");
          string media = node.QuerySelector(".article-hero-image--horizontal__inner").QuerySelector("img").Attributes["src"].Value;
          string body = string.Empty;

          string caption = Regex.Replace(Regex.Replace(new UriBuilder(url).Uri.Segments[2].ToString().ToLowerInvariant(), "[^0-9a-zA-Z- ]+", ""), " ", "-");
          var paragraphs = node.QuerySelector(".article-main").QuerySelectorAll("p");
          foreach (var paragraph in paragraphs)
          {
            try
            {
              body += paragraph.InnerText;
            }
            catch (Exception ex)
            {
              Console.WriteLine("Hata oluştu: {0}", ex.Message);
              Console.WriteLine("Devam ediliyor..");
              continue;
            }

          }

          listContent.Add(new Content
          {
            Title = title,
            Media = media,
            Body = Regex.Replace(HtmlHelper.RemoveHtml(body), "[^0-9a-zA-Z- ]+", ""),
            DetailUrl = url,
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
      return listContent;
    }

    private static void Scrool(ChromeDriver chromeDriver)
    {
      Console.Clear();
      Console.WriteLine("FashioneMagazine sistem kaç kere scrool yapsın?");

      int count = NumberHelper.InputControl(Console.ReadLine());

      for (int i = 0; i < count; i++)
      {
        try
        {
          Thread.Sleep(1000);
          chromeDriver.ExecuteScript("window.scrollTo(0, document.body.scrollHeight)");
          Console.WriteLine("Scrool: {0}", i + 1);
          Console.Clear();
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
