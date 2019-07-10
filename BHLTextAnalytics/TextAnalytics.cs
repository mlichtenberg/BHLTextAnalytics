using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BHLTextAnalytics
{
    static class TextAnalytics
    {
        /// <summary>
        /// Submit the text of the input files to the Azure service, and 
        /// output the results to tab-separated files.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="files"></param>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public static async Task RecognizeEntities(TextAnalyticsClient client, FileInfo[] files, string itemID)
        {
            // Read the text of the input files and build the input list to be submitted for entity recognition
            List<MultiLanguageInput> inputs = new List<MultiLanguageInput>();
            foreach (FileInfo file in files)
            {
                string fileText = File.ReadAllText(file.FullName);
                // The ID can be any value; we use the filename (which is the BHL Page ID)
                inputs.Add(new MultiLanguageInput(null, file.Name.Replace(file.Extension, ""), fileText));
            }

            // Call the Azure service to analyze the text
            var inputDocuments = new MultiLanguageBatchInput(inputs);
            var entitiesResult = await client.EntitiesAsync(false, inputDocuments);

            // Output the recognized entities from the Azure response
            WriteOutput(entitiesResult, itemID);
        }

        /// <summary>
        /// Write one file for each analyzed document, as well as a file that contains
        /// the data for all documents.
        /// </summary>
        /// <param name="entitiesResult"></param>
        /// <param name="itemID"></param>
        private static void WriteOutput(EntitiesBatchResult entitiesResult, string itemID)
        {
            List<string> outputLines = new List<string>();

            // Add the header for the output file
            outputLines.Add("Seq\tPageID\tName\tType\tSubType\tWikipediaID\tWikipediaLanguage\tWikipediaUrl\tOffset\tLength\tScore\tWikipediaScore\tIsScientificName");
            int docCount = 0;
            int sequence = 1;
            foreach (var document in entitiesResult.Documents)
            {
                foreach (var entity in document.Entities)
                {
                    // Read the data to be included in the output file
                    string pageID = document.Id;
                    string eName = entity.Name.Replace('\n', ' ').Replace('\r', ' ');
                    string eType = entity.Type ?? "N/A";
                    string eSubType = entity.SubType ?? "N/A";
                    string eWikipediaId = entity.WikipediaId ?? "N/A";
                    string eWikipediaLanguage = entity.WikipediaLanguage ?? "N/A";
                    string eWikipediaUrl = entity.WikipediaUrl ?? "N/A";

                    foreach (var match in entity.Matches)
                    {
                        // Make sure this entity meets our criteria for inclusion in the output
                        if (IncludeEntity(entity, match))
                        {
                            // Determine if the entity is a scientific name
                            string isName = IsSciName(entity) ? "True" : "False";

                            // Build the data to be output
                            string outputLine = string.Format($"{sequence}\t{pageID}\t{eName}\t{eType}\t{eSubType}\t{eWikipediaId}\t{eWikipediaLanguage}\t{eWikipediaUrl}\t{match.Offset}\t{match.Length}\t{match.EntityTypeScore:F3}\t{match.WikipediaScore:F3}\t{isName}");
                            outputLines.Add(outputLine);
                            sequence++;
                        }
                    }
                }

                docCount++;
                Console.WriteLine($"{docCount} documents processed");
            }

            // Write the accumulated output file with the data from all documents
            if (!Directory.Exists(Config.OutputFolder)) Directory.CreateDirectory(Config.OutputFolder);
            File.WriteAllLines(string.Format("{0}\\Item{1}.tsv", Config.OutputFolder, itemID), 
                outputLines.ToArray(), Encoding.UTF8);
        }

        /// <summary>
        /// Apply some filters that are specific to this input data set.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="match"></param>
        /// <returns>True to include this entity in the output, False otherwise.</returns>
        private static bool IncludeEntity(EntityRecord entity, MatchRecord match)
        {
            // Skip any entities that appear in the first 50 characters of the document.
            // The goal is to skip data extracted from page headers.
            // This parameter should be modified... perhaps set to 0... for books that do 
            // not include page headers.
            if (match.Offset <= 50) return false;

            if (entity.Type.ToLower() == "location")
            {
                // Skip any locations that are equal or less than 6 characters in length.
                // In limited observations, such locations have proven to be mostly invalid.
                // Future tests might consider modifying this parameter.
                if (match.Length <= 6) return false;

                // Skip any locations that start with a lowercase letter, as they are likely
                // to be invalid.
                if (Char.IsLower(entity.Name[0])) return false;
            }

            return true;
        }

        /// <summary>
        /// Invoke the BHL "NameSearch" API to determine if the given entity is a scientific name 
        /// recognized by BHL.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>True if the entity is a scientific name, False otherwise.</returns>
        private static bool IsSciName(EntityRecord entity)
        {
            bool found = false;

            // In the Azure output, Scientific Names are categorized as "Other".
            if (entity.Type.ToLower() == "other")
            {
                // Use the BHL Api to search for the name in BHL
                string nameSearchResponse = BHLApi3.NameSearch(entity.Name, BHLApi3.ResponseFormat.Xml, Config.BhlApiKey);

                // The response will contain a NameConfirmed element that starts with the search string 
                // if the name was found.  The following is an ugly brute-force evaluation of the XML
                // API response.
                found = nameSearchResponse.Contains("<NameConfirmed>" + entity.Name);
            }

            return found;
        }
    }
}
