# Reseda

## What is it?

Reseda (abbreviation from **Res**ource **ed**itor for **A**ndroid) is tool for handy management of Android localization resources.
The main idea of the tool is to convert original resource sets (*strings.xml* and *arrays.xml*) to a single CSV file which can be imported to any spreadsheet application for convenient side-by side translation.
Later, the translated CSV can be exported from spreadsheet editor and converted back to Android resource files. 

## What it is not
* not a translation tool
* not a spreadsheet editor

## General Usage
  
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
| `-locales <locales>`    | list of comma-separated locales. Use it to filter unused localizations.<br>*Example:*<br>`-locales ", ru, en, uk"`<br>means use Default, Russian, English and Ukrainian locales.<br>Default is `""`. |
| `-separator <char>`     | CSV separator symbol. Usually `","` or `";"`.<br>Default is `","`. |
| `-indent <string>`      | XML tree indent string. Only white-space chars allowed.<br>Default is `"  "` (2 spaces). |
| `-dontexit`             | flag indicating that application will not exit until any key will be pressed. |
| `-h` or `-help`         | prints the help. |

## Samples
TODO

## CSV File Format
Reseda uses CSV ([Comma Separated Values](https://en.wikipedia.org/wiki/Comma-separated_values)) file format for interchanging between Android localization resources and spreadsheet editors.
The file should matche special agreements or rules in order to be correctly procecced by Reseda tool.

### File Header
First line of the CSV file is a special header which contains information about columns.
Header columns are following:
* 1<sup>st</sup> `META` - Meta column;
* 2<sup>nd</sup> `NAME` - Resource Item Name;
* 3<sup>rd</sup> `Default` - Value of resource item for default localization;
* 4..N<sup>th</sup> `Locales` - Each column corresponds to a specific localization;
* N+1<sup>th</sup> `COMMENTS` - Contains documentation for the resource item

Other lines contains information about resource items that can be processed by Reseda.

### Meta Column
Contains information how the resource item should be processed.
There are several markers that the column can contain:
* `-`
* `#`
* `a`
* `f`
* `t`
