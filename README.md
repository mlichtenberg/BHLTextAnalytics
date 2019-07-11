# BHLTextAnalytics
## Overview
This is a proof of concept intended to show how Microsoft Azure Cognitive Services, specifically the Text Analytics service, can be used to extract Named Entities from OCR text.  Name Entities include such things as Locations, Persons, and Dates.

The metadata returned by the Text Analytics service includes the location of each Named Entity within the text.  This makes it possible to identify which Entities appear in close proximity to one another, perhaps providing clues about relationships between the Entities.  For example, a Location and a Person, or a Scientific Name and a Date.

More information about the Text Analytics service is available at https://azure.microsoft.com/en-us/services/cognitive-services/text-analytics/.  As of July 2019, the free tier of this Azure service allows it to be called 5000 times per month.

The BHLTextAnalytics tool is written in C# and utilizes .NET Core 2.1 (free, open-source, and cross-platform).  It can be compiled to run on Windows, OSX, or Linux.

## Using the tool

The compiled tool is executed from the command line as follows:

    BHLTextAnalytics BHL-ITEM-ID

where BHL-ITEM-ID is the identifier of a BHL Item (a book in BHL).

When executed, the tool performs the following actions:

1. Invokes the BHL API to download the text for each page of the book.
2. Submits the text to the Azure Text Analytics service.
3. Parses the Azure service response, calls the BHL API to identify scientific names included in the identified Name Entities, and outputs the results to a tab-separated file. 
   
More information about the BHL API is available at https://www.biodiversitylibrary.org/docs/api3.html.

## Setting Up The Tool
To set up your environment to run this tool, do the following:

1. Install .NET Core 2.1 or later.
2. Download the code from this repository.
3. Get a BHL API key at https://www.biodiversitylibrary.org/getapikey.aspx.  
4. Create an Azure Cognitive Services subscription and get the associated Azure key and endpoint.  See https://docs.microsoft.com/en-us/azure/cognitive-services/cognitive-services-apis-create-account for more information.
5. Update the appsettings.json file with the BHL API key, Azure subscription key, and Azure service endpoint.
6. On the command line, navigate to the folder that contains the project file (BHLTextAnalytics.csproj).
7. Compile the tool for your environment with one of the following commands:
   1. dotnet publish -r win-x86 BHLTextAnalytics.csproj
   2. dotnet publish -r win-x64 BHLTextAnalytics.csproj
   3. dotnet publish -r osx-x64 BHLTextAnalytics.csproj
   4. dotnet publish -r linux-x64 BHLTextAnalytics.csproj

## Analysis of the Tool Output

The "Examples" folder contains the following: 
1. Example input (OCR) files in the "Input" folder.  These files contain the OCR text for BHL Item 20995
2. An example output file (Item20995.tsv) in the "Output folder.
3. An example SQL query (TextAnalyticsQueries.sql) that can be used to analyze data from the output file.
4. The output of the SQL query (TextAnalyticsQueryOutput.tsv), showing terms that appear in close proximity to Locations in the text.

Here is an example of the output of the tool, taken from the Item20995.tsv sample file:

ItemID | Seq | PageID | Name | Type | SubType | WikipediaID | WikipediaLanguage | WikipediaUrl | Offset | Length | Score | WikipediaScore | IsScientificName
--- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | ---
20995 | 93 | 2774078 | Rudyerd Boulton | Person | N/A | Rudyerd Boulton | en | https://en.wikipedia.org/wiki/Rudyerd_Boulton | 73 | 15 | 1.000 | 0.807 | False
20995 | 94 | 2774078 | Curator | Other | N/A | Curator | en | https://en.wikipedia.org/wiki/Curator | 90 | 7 | 0.800 | 0.569 | False
20995 | 95 | 2774078 | Bird | Other | N/A | Bird | en | https://en.wikipedia.org/wiki/Bird | 101 | 5 | 0.800 | 0.140 | False
20995 | 96 | 2774078 | Bird | Other | N/A | Bird | en | https://en.wikipedia.org/wiki/Bird | 1644 | 5 | 0.800 | 0.140 | False
20995 | 97 | 2774078 | Sea captain | Other | N/A | Sea captain | en | https://en.wikipedia.org/wiki/Sea_captain | 172 | 7 | 0.800 | 0.219 | False
20995 | 98 | 2774078 | Robert Bartlett (explorer) | Person | N/A | Robert Bartlett (explorer) | en | https://en.wikipedia.org/wiki/Robert_Bartlett_(explorer) | 180 | 8 | 0.971 | 0.042 | False
20995 | 99 | 2774078 | Sooty shearwater | Other | N/A | Sooty shearwater | en | https://en.wikipedia.org/wiki/Sooty_shearwater | 298 | 16 | 0.800 | 0.821 | False
20995 | 100 | 2774078 | Sooty shearwater | Other | N/A | Sooty shearwater | en | https://en.wikipedia.org/wiki/Sooty_shearwater | 325 | 16 | 0.800 | 0.821 | False
20995 | 101 | 2774078 | Sooty shearwater | Other | N/A | Sooty shearwater | en | https://en.wikipedia.org/wiki/Sooty_shearwater | 355 | 17 | 0.800 | 0.821 | False
20995 | 102 | 2774078 | Johann Friedrich Gmelin | Person | N/A | Johann Friedrich Gmelin | en | https://en.wikipedia.org/wiki/Johann_Friedrich_Gmelin | 316 | 6 | 0.800 | 0.231 | False
20995 | 103 | 2774078 | 1 | Quantity | Number | N/A | N/A | N/A | 343 | 1 | 0.800 |  | False
20995 | 104 | 2774078 | 1 | Quantity | Number | N/A | N/A | N/A | 2334 | 1 | 0.800 |  | False
20995 | 105 | 2774078 | Three | Quantity | Number | N/A | N/A | N/A | 349 | 5 | 0.800 |  | False
20995 | 106 | 2774078 | Three Sooty Shearwaters | Organization | N/A | N/A | N/A | N/A | 349 | 23 | 0.642 |  | False
20995 | 107 | 2774078 | June 25 | DateTime | Date | June 25 | en | https://en.wikipedia.org/wiki/June_25 | 390 | 7 | 0.800 | 0.171 | False
20995 | 108 | 2774078 | miles | Quantity | Dimension | N/A | N/A | N/A | 405 | 5 | 0.800 |  | False
20995 | 109 | 2774078 | Bay of Fundy | Location | N/A | Bay of Fundy | en | https://en.wikipedia.org/wiki/Bay_of_Fundy | 427 | 12 | 0.903 | 0.813 | False
20995 | 110 | 2774078 | Bay of Fundy | Location | N/A | Bay of Fundy | en | https://en.wikipedia.org/wiki/Bay_of_Fundy | 865 | 12 | 0.824 | 0.813 | False
20995 | 111 | 2774078 | Newfoundland and Labrador | Location | N/A | Newfoundland and Labrador | en | https://en.wikipedia.org/wiki/Newfoundland_and_Labrador | 469 | 8 | 0.921 | 0.185 | False
20995 | 112 | 2774078 | Newfoundland and Labrador | Location | N/A | Newfoundland and Labrador | en | https://en.wikipedia.org/wiki/Newfoundland_and_Labrador | 1390 | 8 | 0.457 | 0.185 | False
20995 | 113 | 2774078 | Greenland | Location | N/A | Greenland | en | https://en.wikipedia.org/wiki/Greenland | 485 | 9 | 0.869 | 0.282 | False
20995 | 114 | 2774078 | Greenland | Location | N/A | Greenland | en | https://en.wikipedia.org/wiki/Greenland | 607 | 9 | 0.994 | 0.282 | False
20995 | 115 | 2774078 | Greenland | Location | N/A | Greenland | en | https://en.wikipedia.org/wiki/Greenland | 1421 | 9 | 0.729 | 0.282 | False
20995 | 116 | 2774078 | Greenland | Location | N/A | Greenland | en | https://en.wikipedia.org/wiki/Greenland | 2313 | 9 | 0.809 | 0.282 | False

And here is an example of the SQL query output, taken from the TextAnalyticsQueryOutput.tsv file.  It presents a different visualization of the tool output by pairing Named Entities that appear within 100 characters of a Location in the text:

ItemID | PageID | Type | Name | Offset | Type | Name | Offset | IsScientificName
--- | --- | --- | --- | --- | --- | --- | --- | ---
20995 | 2774078 | Location | Bay of Fundy | 427 | Location | Newfoundland and Labrador | 469 | 0
20995 | 2774078 | Location | Bay of Fundy | 427 | Location | Greenland | 485 | 0
20995 | 2774078 | Location | Bay of Fundy | 427 | Other | Sooty shearwater | 355 | 0
20995 | 2774078 | Location | Bay of Fundy | 427 | Quantity | 1 | 343 | 0
20995 | 2774078 | Location | Bay of Fundy | 427 | Quantity | Three | 349 | 0
20995 | 2774078 | Location | Bay of Fundy | 427 | Organization | Three Sooty Shearwaters | 349 | 0
20995 | 2774078 | Location | Bay of Fundy | 427 | DateTime | June 25 | 390 | 0
20995 | 2774078 | Location | Bay of Fundy | 427 | Quantity | miles | 405 | 0
20995 | 2774078 | Location | Bay of Fundy | 865 | Other | Great shearwater | 766 | 0
20995 | 2774078 | Location | Bay of Fundy | 865 | Quantity | first | 815 | 0
20995 | 2774078 | Location | Bay of Fundy | 865 | Location | Cape Race | 924 | 0
20995 | 2774078 | Location | Bay of Fundy | 865 | DateTime | June 30 | 964 | 0
20995 | 2774078 | Location | Greenland | 485 | Quantity | miles | 405 | 0
20995 | 2774078 | Location | Greenland | 485 | DateTime | June 25 | 390 | 0
20995 | 2774078 | Location | Greenland | 485 | Location | Bay of Fundy | 427 | 0
20995 | 2774078 | Location | Greenland | 485 | Location | Newfoundland and Labrador | 469 | 0
20995 | 2774078 | Location | Greenland | 607 | DateTime | July 20 | 644 | 0
20995 | 2774078 | Location | Greenland | 607 | Other | Great shearwater | 704 | 0
20995 | 2774078 | Location | Greenland | 607 | Person | N. Lat | 635 | 0
20995 | 2774078 | Location | Greenland | 1421 | Person | N. Lat | 1455 | 0
20995 | 2774078 | Location | Greenland | 1421 | Location | Newfoundland and Labrador | 1390 | 0
20995 | 2774078 | Location | Greenland | 1421 | Location | Cape Farewell, Greenland | 1403 | 0
20995 | 2774078 | Location | Greenland | 1421 | Location | Greenwich, Connecticut | 1375 | 0
20995 | 2774078 | Location | Greenland | 1421 | Quantity | 62 | 1452 | 0
20995 | 2774078 | Location | Greenland | 2313 | Other | Unless | 2336 | 0
20995 | 2774078 | Location | Greenland | 2313 | Quantity | 1 | 2334 | 0
20995 | 2774078 | Location | Newfoundland and Labrador | 469 | Location | Greenland | 485 | 0
20995 | 2774078 | Location | Newfoundland and Labrador | 469 | Location | Bay of Fundy | 427 | 0
20995 | 2774078 | Location | Newfoundland and Labrador | 469 | Quantity | miles | 405 | 0
20995 | 2774078 | Location | Newfoundland and Labrador | 469 | DateTime | June 25 | 390 | 0
20995 | 2774078 | Location | Newfoundland and Labrador | 1390 | Location | Greenland | 1421 | 0
20995 | 2774078 | Location | Newfoundland and Labrador | 1390 | Person | N. Lat | 1455 | 0
20995 | 2774078 | Location | Newfoundland and Labrador | 1390 | Quantity | 62 | 1452 | 0
20995 | 2774078 | Location | Newfoundland and Labrador | 1390 | Location | Cape Farewell, Greenland | 1403 | 0
20995 | 2774078 | Location | Newfoundland and Labrador | 1390 | Location | Greenwich, Connecticut | 1375 | 0
