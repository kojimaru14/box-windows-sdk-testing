using Box.V2.Config;
using Box.V2.JWTAuth;
using Box.V2.Models;
using Box.V2;
using System;
using System.Net;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace JWTtest
{
  class Program
  {
    // modify the app.config file to reflect your Box app config values
    static readonly string CLIENT_ID = ConfigurationManager.AppSettings["boxClientId"];
    static readonly string CLIENT_SECRET = ConfigurationManager.AppSettings["boxClientSecret"];
    static readonly string ENTERPRISE_ID = ConfigurationManager.AppSettings["boxEnterpriseId"];
    static readonly string JWT_PRIVATE_KEY_PASSWORD = ConfigurationManager.AppSettings["boxPrivateKeyPassword"];
    static readonly string JWT_PUBLIC_KEY_ID = ConfigurationManager.AppSettings["boxPublicKeyId"];
    static bool isProxyEnabled = false; // set isProxyEnabled = true if using Fiddler

    static void Main(string[] args)
    {
      Task t = MainAsync();
      t.Wait();

      Console.WriteLine();
      Console.Write("Press return to exit...");
      Console.ReadLine();
    }

    static async Task MainAsync()
    {
      // rename the private_key.pem.example to private_key.pem and put your JWT private key in the file
      var privateKey = File.ReadAllText("private_key.pem");

      var boxConfig = new BoxConfig(CLIENT_ID, CLIENT_SECRET, ENTERPRISE_ID, privateKey, JWT_PRIVATE_KEY_PASSWORD, JWT_PUBLIC_KEY_ID);

      // Proxy configuration - set isProxyEnabled = true if using Fiddler!!
      if (isProxyEnabled != false)
      {
        System.Net.WebProxy webProxy = new System.Net.WebProxy("http://127.0.0.1:8888");
        NetworkCredential credential = new NetworkCredential("testUser", "testPass");
        webProxy.Credentials = credential;
        boxConfig.WebProxy = webProxy;
      }

      var boxJWT = new BoxJWTAuth(boxConfig);

      var adminToken = boxJWT.AdminToken();
      Console.WriteLine("Admin Token: " + adminToken);
      Console.WriteLine();

      var adminClient = boxJWT.AdminClient(adminToken);

      var adminFunc = new Func(adminClient);
      adminFunc.GetFolderItems();

      var userId = "3768478578";
      var userToken = boxJWT.UserToken(userId); // valid for 60 minutes so should be cached and re-used
      BoxClient userClient = boxJWT.UserClient(userToken, userId);

      var userFunc = new Func(userClient);
      userFunc.GetFolderItems();
      
      // Stream fileContents = await userClient.FilesManager.DownloadStreamAsync(id: "675996854920"); // Download the file 675996854920

      // var userRequest = new BoxUserRequest() { Name = "test appuser", IsPlatformAccessOnly = true };
      // var appUser = await adminClient.UsersManager.CreateEnterpriseUserAsync(userRequest);
      // Console.WriteLine("Created App User");

      // var userToken = boxJWT.UserToken(appUser.Id);
      // var userClient = boxJWT.UserClient(userToken, appUser.Id);

      // var userDetails = await userClient.UsersManager.GetCurrentUserInformationAsync();
      // Console.WriteLine("\nApp User Details:");
      // Console.WriteLine("\tId: {0}", userDetails.Id);
      // Console.WriteLine("\tName: {0}", userDetails.Name);
      // Console.WriteLine("\tStatus: {0}", userDetails.Status);
      // Console.WriteLine();

      // await adminClient.UsersManager.DeleteEnterpriseUserAsync(appUser.Id, false, true);
      // Console.WriteLine("Deleted App User");
    }
  }
}
