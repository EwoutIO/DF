# Design Pattern Finder

## About

This solution contains the code for a plugin that scans a solution for design patterns. It parses all files and matches them to the given configuration (found in the ConfigurationFiles-folder). Currently there are four design patterns defined, but it is possible to add more, based on the XSD (Found in ".\DesignFinder\DesignFinder.Vsix\Resources\ConfigurationFile_V2.xsd").

## Configuration files

The configuration files are plain XML-files. A template to create your own files is included: ".\ConfigurationFiles\Template\PatternTemplate.xml".

## Run

Compile the DF.CLI-project.
Run the exe: There are two parameters: /solutionPath= and /configurationFolderPath=, both should be appended with the corresponding locations.