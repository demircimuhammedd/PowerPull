using Dapper;
using MySql.Data.MySqlClient;
using PowerPull.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using WordPressSharp;
using WordPressSharp.Models;

namespace PowerPull.Helper
{
  public class ApiHelper
  {
    private static int connectionTimeOut = 30;

    public static async System.Threading.Tasks.Task SaveBlogAsync(List<Content> listContent)
    {
      Console.WriteLine(
          "Makale Seo uyumlu olsun mu?: \r\n " +
             "1- Evet \r\n " +
             "2- Hayır");

      bool isSeoArticle = Convert.ToInt32(Console.ReadLine()) == 1 ? true : false;

      using (MySqlConnection mySqlConnection = new MySqlConnection("Server = 94.73.147.204; Database = u7614282_mzills; Uid = u7614282_mzills; Pwd = FEax10F8; Convert Zero Datetime = true; Allow User Variables=True; "))
      {
        using (var client = new WordPressClient())
        {
          int index = listContent.Count;
          foreach (var content in listContent)
          {
            try
            {

              if (!IsExistRowDb(mySqlConnection, content.Title))
              {
                string contentBody = string.Empty;
                if (isSeoArticle)
                {
                  contentBody = ArticleReWrite(WebUtility.HtmlDecode(content.Body.Replace("'", "")).Replace("'", "").Replace("“", "").Replace("”", "")).Replace("?", "");
                }
                string seoBody = WebUtility.HtmlDecode(isSeoArticle ? (contentBody.Length > 50) ? contentBody : content.Body : content.Body);
                if (string.IsNullOrEmpty(seoBody)) continue;

                content.SeoBody = seoBody;

                content.Title = WebUtility.HtmlDecode(content.Title);
                var post = new Post
                {
                  PostType = "post",
                  Title = content.Title,
                  Content = content.SeoBody,
                  PublishDateTime = DateTime.Now,
                  Status = "publish"
                };

                if (!content.Media.Contains("iframe") && !string.IsNullOrEmpty(content.Media))
                {
                  string imgExtension = string.Empty;
                  if (content.Media.Contains(".jpg"))
                    imgExtension = ".jpg";

                  if (content.Media.Contains(".png"))
                    imgExtension = ".png";

                  var featureImage = Data.CreateFromUrl(content.Media.Substring(0, content.Media.IndexOf(imgExtension) + imgExtension.Length));
                  UploadResult uploadResult = await client.UploadFileAsync(featureImage);
                  post.FeaturedImageId = uploadResult.Id;
                }
                else
                {
                  post.Content = string.Concat(post.Content, content.Media);
                  post.FeaturedImageId = "48196";
                }

                await client.NewPostAsync(post);

                Console.WriteLine("({0} - {1}) Eklenen makale: {2}", listContent.Count, index, content.Title);
              }
              else
              {
                Console.WriteLine("({0} - {1}) Zaten kayıtlı makale: {2}", listContent.Count, index, content.Title);
              }

              index--;
            }
            catch (Exception ex)
            {
              Console.WriteLine(ex.Message);
              continue;
            }
          }
        }
      }
    }

    private static bool IsExistRowDb(MySqlConnection mySqlConnection, string post_title)
    {
      var total = mySqlConnection.Query<long>("SELECT COUNT(*) FROM wp_posts where post_title = @post_title", new { post_title }).Single();
      if (total > 0) return true;
      else return false;
    }

    private static string ArticleReWrite(string article)
    {
      string api_key = "e1fa82649659a96614ea87b790cac4b1";
      string serviceUri = "http://www.re-writer.net/api.php";
      string lang = "en";

      string post_data = "api_key=" + api_key + "&article=" + article + "&lang=" + lang;

      HttpWebRequest request = (HttpWebRequest)
      WebRequest.Create(serviceUri);
      request.Method = "POST";

      byte[] postBytes = System.Text.Encoding.ASCII.GetBytes(post_data);
      request.ContentType = "application/x-www-form-urlencoded";
      request.ContentLength = postBytes.Length;
      Stream requestStream = request.GetRequestStream();

      requestStream.Write(postBytes, 0, postBytes.Length);
      requestStream.Close();

      HttpWebResponse response = (HttpWebResponse)request.GetResponse();
      return new StreamReader(response.GetResponseStream()).ReadToEnd();
    }
  }
}
