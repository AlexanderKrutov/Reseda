# Reseda

## What is it?

Reseda (abbreviation from **Res**ource **ed**itor for **A**ndroid) is tool for handy management of Android localization resources.
The main idea of the tool is to convert original resource sets (*strings.xml* and *arrays.xml*) to a single CSV file which can be imported to any spreadsheet application for convenient side-by side translation.
Later, the translated CSV can be exported from spreadsheet editor and converted back to Android resource files. 

## What Reseda is not
* Reseda is not a translation tool
* Reseda is not a spreadsheet editor

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
| `-in-res <dirpath>`      | path to input folder with XML resource files that will be converted to CSV. |
| `-out-csv <filepath>`   | path to output CSV file with localization resources. |
| `-locales <locales>`    | list of comma-separated locales. Use it to filter unused localizations.<br>*Example:*<br>`-locales "ru, en, uk"`<br>means use default (always ON), Russian, English and Ukrainian locales. |
| `-separator <char>`     | CSV separator symbol. Usually `","` or `";"`.<br>Default is `","`. |
| `-indent <string>`      | XML tree indent string. Only white-space chars allowed.<br>Default is `"  "` (2 spaces). |
| `-dontexit`             | flag indicating that application will not exit until any key will be pressed. |
| `-keep-empty-rows`      | keep empty rows on converting resources from XML to CSV and vise versa. |
| `-force-untranslated`   | force untranslated strings, i.e. if the flag is ON, missing translations to be replaced with values from default locale. |
| `-h` or `-help`         | prints the help. |

## Samples
TODO

## CSV File Format
Reseda uses CSV ([Comma Separated Values](https://en.wikipedia.org/wiki/Comma-separated_values)) file format for interchanging between Android localization resources and spreadsheet editors.
The file should match special agreements or rules in order to be correctly processed by Reseda tool.

### File Header
First line of the CSV file is a special header that contains information about columns.
Header columns are following:

| Column # | Title        | Description |
|----------|--------------|-------------|
| 1        | `META`       | Meta column. Has special markers to instruct Reseda how the resource item should be processed. See below. |
| 2        | `NAME`       | Resource item name. It can be name of Android string resource item name, Android array resource name or comment text. |
| 3        |              | Value of resource item for default localization. |
| 4        | `<locale_1>` | Value of resource item for localization `<locale_1>`. | 
| ...      | ...          | ... |
| N        | `<locale_N>` | Value of resource item for localization `<locale_N>`. `N` is a number of application localizations. For example if an app has default localization (English) and additional localizations `ru`, `de`, `N` is 2. |
| N+1      | `DOCS`      | Optional documentation column, content of `documentation` attribute of Android resource item.

Lines 2 contain values of resource items that can be processed by Reseda.

### Meta Column
Contains information how the resource item should be processed.
There are several markers that the column can contain:

| Marker | Meaning |
|--------|---------|
| `-`    | The row should be excluded from the resource items list while processing CSV file. Use it for temporarily excluding strings or for internal CSV comments that should not get into output resource files. |
| `#`    | The comment that should get into output resource file. |
| `f`    | The *un**F**ormatted* marker. Attribute `formatted="false"` will be added to such resource items. |
| `t`    | The *un**T**ranslatable* marker. Resource items marked with this meta symbol should be present in default localization resource set, but will not get into other localizations. Attribute `translatable="false"` will be added to such resource items. |
