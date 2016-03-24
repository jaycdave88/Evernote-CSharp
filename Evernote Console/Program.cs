using System;
using System.Collections.Generic;
using EvernoteSDK;
using EvernoteSDK.Advanced;
using System.Text.RegularExpressions;

namespace Evernote_Console
{
    class Program
    {

        static void Main(string[] args)
        {

            // Start session with SessionDeveloperToken and SessionNoteStoreUrl
            ENSessionAdvanced.SetSharedSessionDeveloperToken("[OAUTH_TOKEN]", "[NOTEBOOK_URL]");


            // check to make your your session tokens are valid
            if (ENSession.SharedSession.IsAuthenticated == false)
            {
                Console.WriteLine("FAIL");
                ENSession.SharedSession.AuthenticateToEvernote();
            }
            Console.WriteLine("SUCCESS" + "\n");

            // using regex to capature everything within the search
            string textToFindPattern = "^\\.*\\$";

            // creating a list of the results with the regex 
            List<ENSessionFindNotesResult> myResultsList = ENSession.SharedSession.FindNotes(ENNoteSearch.NoteSearch(textToFindPattern), null, ENSession.SearchScope.All, ENSession.SortOrder.RecentlyUpdated, 500);


            // creating a check to see if the list is empty 
            if (myResultsList.Count > 0)
            {
                // regex to grab and search and group together results
                var contentPattern = @"(<body\b[^>]*>)(.*?)(<\/body>)";

                // looping through the contents of the list
                for (int i = 0; i <= myResultsList.Count - 1; i++)
                {
                    // creating a noteRef witin my loop
                    var noteRef = myResultsList[i].NoteRef;

                    ENNote myDownloadedNote = ENSession.SharedSession.DownloadNote(noteRef);

                    // Find all tags associated with a note by the noteRef
                    ENNoteStoreClient noteStore = ENSessionAdvanced.SharedSession.NoteStoreForNoteRef(noteRef);
                    // create a list of strings with each tagname that is associated to a note by its GUID
                    List<string> tagNames = noteStore.GetNoteTagNames(noteRef.Guid);

                    var title = myDownloadedNote.Title;

                    // a varible to hold the html for each note and converts it to a string
                    var contentXML = myDownloadedNote.HtmlContent.ToString();

                    // creates a collection of match result from the given regex and the string xml
                    MatchCollection matches = Regex.Matches(contentXML, contentPattern);
                   
                    // creating a loop and parsing through the each match to return the 2nd group of each match ( this was done because it also removes the <body> </body> tags 
                    foreach (Match match in matches)
                    {
                       Console.WriteLine("NOTE: " + match.Groups[2].Value.ToString() + "\n" );
                    }

                    // UI
                    Console.WriteLine("TITLE: " + title + "\n");
                    Console.WriteLine("TAGS: ");

                    // Loop through all the tagNames and print them to screen
                    foreach (string tagName in tagNames)
                    {

                        Console.WriteLine(tagName +"\n");
                    }

                }
            }
            // final check if nothing is found.
            if (myResultsList == null)
            {
                Console.WriteLine("No Results Found");
            }

        }

    }
}
