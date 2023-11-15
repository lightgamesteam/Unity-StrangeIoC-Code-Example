using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.Download
{
    public class CheckDownloadDateBooksCommand : BaseCommand
    {
        public override void Execute()
        {
            Retain();

            DirectoryInfo booksDirectory = new DirectoryInfo(Application.persistentDataPath + "/BooksPlayer/");

            if (booksDirectory.Exists)
            {
                Debug.Log("<color=green> booksDirectory: " + booksDirectory + " </color>");
                List<DirectoryInfo> allBooksFolders = new List<DirectoryInfo>(booksDirectory.GetDirectories());

                foreach (var item in allBooksFolders)
                {
                    // 30 - the number of days after which the book is automatically deleted
                    if (((DateTime.Now - item.CreationTime).TotalDays) >= 30)
                    {
                        Directory.Delete(Application.persistentDataPath + "/BooksPlayer/" + item.Name, true);
                        Debug.Log("<color=green>book: " + item.Name + " </color> has been deleted since it was downloaded more than 30 days ago");
                    }
                }
            }

            Release();
        }
    }

}

