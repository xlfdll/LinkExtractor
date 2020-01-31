# Link Extractor
A utility that can extract specific links from web pages

<p align="center">
  <img src="https://github.com/xlfdll/xlfdll.github.io/raw/master/images/projects/LinkExtractor.png"
       alt="Link Extractor">
</p>

## System Requirements
* .NET Framework 4.8

[Runtime configuration](https://docs.microsoft.com/en-us/dotnet/framework/migration-guide/how-to-configure-an-app-to-support-net-framework-4-or-4-5) is needed for running on other versions of .NET Framework.

## Usage
Just run the program and copy matched links from supported sites to the clipboard. The program will parse them and extract all designated links into the list for use.

## Development Prerequisites
* Visual Studio 2015+

Before the build, generate-build-number.sh needs to be executed in a Git / Bash shell to generate build information code file (BuildInfo.cs).