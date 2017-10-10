# Reseda
Reseda (abbreviation from **Res**ource **ed**itor for **A**ndroid) is tool for handy management of Android localziation resources.
The main idea of the tool is to convert original resource sets (*strings.xml* and *arrays.xml*) to a single CSV file which can be imported to any spreadsheet application for convenient side-by side translation.
Later, the translated CSV can be exported from spreadsheet editor and conveted back to Android resource files. 

## General Workflow
  
1. Convert Android localization resources to CSV file:

  ```bat
  Reseda -in-res "<PathToMyApp>\app\src\main\res" -out-csv "localization.csv"
  ```

2. Import the CSV file to any spreadsheet editor application, translate resource items in columns side-by-side.

3. Export the sheet to CSV file.

4. Convert translated CSV back to resources:

  ```bat
  Reseda -in-csv "translated.csv" -out-res "<PathToMyApp>\app\src\main\res"
  ```

## Command Line Reference

| Parameter               | Description |
|-------------------------|-------------|
| `-in-csv <filepath>`    | path to input CSV file with localization resources. |
| `-out-res <dirpath>`    | path to output folder where the generated XML resource files will be placed. |
| `in-res <dirpath>`      | path to input folder with XML resource files that will be converted to CSV. |
| `-out-csv <filepath>`   | path to output CSV file with localization resources. |
| `-locales <locales>`    | list of comma-separated locales. Use it to filter unused localizations.<br>*Example:*<br>`-locales ", ru, en, uk"`<br>means use Default, Russian and Ukrainian locales.<br>Default is `""`. |
| `-separator <char>`     | CSV separator symbol. Usually `","` or `";"`.<br>Default is `","`. |
| `-indent <string>`      | XML tree indent string. Only white-space chars allowed.<br>Default is `"  "` (2 spaces). |
| `-dontexit`             | flag indicating that application will not exit until any key will be pressed. |
| `-h` or `-help`         | prints the help. |

## Samples
TODO

## CSV File Format
### Header
### Meta Column
TODO
