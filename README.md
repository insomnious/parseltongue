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

`Parseltongue.exe language.bin` will output `language.json`

`Parseltongue.exe language.json` will output `language.bin`

`Parseltongue.exe json2bin language_json.txt` will output `language_json.bin`

## Credits

- Snake Icon by [Vectors Market]("https://iconscout.com/icons/snake-head")  
- Testers from the [Hogwarts Legacy Modding](https://discord.gg/Bmmtv3sYAa) discord server:
  - [tucker](https://hamstersquad.github.io/)
