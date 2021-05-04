using Box.V2;
using System;

namespace JWTtest
{
  class Func
  {
    private BoxClient client;
    public Func(BoxClient BoxClient)
    {
      client = BoxClient;
    }
    public async void GetFolderItems(String folder_id = "0")
    {
      Console.WriteLine("Items inside the folder_id: {0}", folder_id);
      var items = await client.FoldersManager.GetFolderItemsAsync(folder_id, 500);
      items.Entries.ForEach(i =>
      {
        Console.WriteLine("\t{0}", i.Name);
        //if (i.Type == "file")
        //{
        //    var previewLink = adminClient.FilesManager.GetPreviewLinkAsync(i.Id).Result;
        //    Console.WriteLine("\tPreview Link: {0}", previewLink.ToString());
        //    Console.WriteLine();
        //}   
      });
      Console.WriteLine();
    }
  }
}
