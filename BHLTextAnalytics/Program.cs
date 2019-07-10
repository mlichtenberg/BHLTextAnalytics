using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using System;
using System.IO;
using System.Net;
using System.Xml.Linq;

namespace BHLTextAnalytics
{
    class Program
    {
        private static int _itemID = 0;

        public static void Main(string[] args)
        {
            try
            {
                if (ReadCommandLineArguments())
                {
                    // Download from BHL the text of the specified item
                    DownloadText();

                    // Set up the Azure Text Analytics client
                    var credentials = new ApiKeyServiceClientCredentials(Config.SubscriptionKey);
                    var client = new TextAnalyticsClient(credentials)
                    {
                        Endpoint = Config.Endpoint
                    };

                    // Get the list of files to be analyzed from the input directory.
                    var dir = new DirectoryInfo(Config.InputFolder);
                    FileInfo[] inputFiles = dir.GetFiles();

                    // Perform the analysis
                    TextAnalytics.RecognizeEntities(client, inputFiles, _itemID.ToString()).Wait();

                    Console.Write($"Analysis of item {_itemID} is complete.");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
            }
        }

        /// <summary>
        /// Download the text to be analyzed.
        /// </summary>
        private static void DownloadText()
        {
            // Create the input directory if it does not exist, or clear it if it does
            if (Directory.Exists(Config.InputFolder))
            {
                string[] files = Directory.GetFiles(Config.InputFolder);
                foreach (string file in files) File.Delete(file);
            }
            else
            {
                Directory.CreateDirectory(Config.InputFolder);
            }

            // Get the item text from the BHL API
            string itemMetadataResponse =
                BHLApi3.GetItemMetadata(_itemID, true, true, false, BHLApi3.ResponseFormat.Xml, Config.BhlApiKey);

            // Extract the text from the API response
            XDocument xml = XDocument.Parse(itemMetadataResponse);
            foreach(XElement page in xml.Root
                .Elements("Result")
                .Elements("Item")
                .Elements("Pages")
                .Elements("Page"))
            {
                string pageID = page.Element("PageID").Value;
                string pageText = page.Element("OcrText").Value;

                // Write the text of each page to a file
                File.WriteAllText(string.Format("{0}\\{1}.txt", Config.InputFolder, pageID), 
                    pageText, System.Text.Encoding.UTF8);
            }
        }

        /// <summary>
        /// Read the Item ID from the command line.
        /// </summary>
        /// <returns></returns>
        private static bool ReadCommandLineArguments()
        {
            bool validArgs = false;

            string[] args = System.Environment.GetCommandLineArgs();
            switch (args.Length)
            {
                case 1:
                    Console.WriteLine("BHL Item ID is required.  Format is \"BHLTextAnalytics <ITEMID>\".");
                    break;
                case 2:
                    if (!int.TryParse(args[1], out _itemID))
                    {
                        Console.Write("Invalid Item ID.  Item ID must be a numeric integer value.  Example:  BHLTextAnalytics 1234");
                    }
                    else
                    {
                        validArgs = true;
                    }
                    break;
                default:
                    Console.WriteLine("Too many command line arguments.  Format is \"BHLTextAnalytics <ITEMID>\".");
                    break;
            }

            return validArgs;
        }
    }
}
