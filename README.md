# parseltongue
Converts Hogwarts Legacy language .bin file to and from JSON.

## Usage


`Parseltongue.exe <file> [command] [options]`


### Arguments
`file`: File to convert (will detect extension and use the necessary converter).

### Options
```shell
--version       Show version information
-?, -h, --help  Show help and usage information
```

### Commands

`bin2json <file>`  Convert .bin to .json

`json2bin <file>`  Convert .json to .bin

### Examples

```shell
Parseltongue.exe input.bin
Parseltongue.exe bin2json input.binary
Parseltongue.exe json2bin inputjson.txt
```


If no command is supplied then it'll use the extension to determine what convertor to use. 
For example, if a `.bin` is used then it'll attempt to convert to JSON and if a `.json` is used
then it'll attempt to convert to a bin.