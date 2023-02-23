# parseltongue

Tool to convert Hogwarts Legacy language .bin files to and from various formats, primarily JSON. It can be used by dragging and dropping a file in windows onto the executable or via the command line.

## Usage

### Drag-and-drop

Drop a `.bin`, `.json` or a `.csv` file onto the tool and it will use the below conversions. This is the same as the command line usage of `Parseltongue.exe <file>`

- `.bin` > `.json`
- `.json` > `.bin`
- `.csv` > `.bin`

If anything more specific than this is needed, then the command line will need to be used

### Command Line

`Parseltongue.exe <file> [command] [options]`

#### Arguments

`<file>`: File to convert (tool will use file extension to determine what to do. See above.).

#### Options
```shell
--version       Show version information
-?, -h, --help  Show help and usage information
```

#### Commands

For specify a certain convertor

`bin2json <file>`  Convert .bin to .json

`json2bin <file>`  Convert .json to .bin

`csv2bin <file>`  Convert .csv to .bin

### Examples

`Parseltongue.exe language.bin` will output `language-modified.json`

`Parseltongue.exe language.json` will output `language-modified.bin`

`Parseltongue.exe json2bin language_json.txt` will output `language_json-modified.bin`

## Notes

CSV conversion has only been tested with a `.csv` exported from Google Sheets

## Credits

- Snake Icon by [Vectors Market](https://iconscout.com/icons/snake-head)  
- Testers from the [Hogwarts Legacy Modding](https://discord.gg/Bmmtv3sYAa) discord server:
  - [tucker](https://hamstersquad.github.io/)

## Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/) and this project adheres to [Semantic Versioning](http://semver.org/).

### [v0.2.3] - 2023-02-23

Fixed bug where output file wasn't changing extension

### [v0.2.2] - 2023-02-23

Few small quality of life tweaks

#### Changed

- Changed output to include '-modified' suffix to the filename
- Output window stays open if an error has occured

#### Fixed

- Fixed uncaught error when duplicate keys appeared in the CSV

### [v0.2.1] - 2023-02-12

Fixed bug where quotes in CSV files were being double escaped when converting to BIN

### [v0.2.0] - 2023-02-10

Added support to convert .csv to .bin

### [v0.1.0] - 2023-02-09

Initial release that supports BIN to JSON and JSON to BIN