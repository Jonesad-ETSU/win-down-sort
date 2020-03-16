This program is designed as a download manager for windows 10. It parses text files, using new-line delimited strings as file extensions. Each text file contains a "Path:" line which directs the files in the "downloads" folder with an extension in the file to that folder.

SET-UP
------

By setting up a folder (Default: C:\Users\%Username%\Downloads\DL_MGR\cfg), you can put a text file into the folder (*Name doesn't matter*) with line delimited extensions and a final line containing "Path:" and a relative or absolute path after the ':' (The relative pathing starts from the user folder).

ROADMAP
-------

Custom String Path shortcuts:
	-Text file called "shorts":
		1. Each line is a new shortcut
		2. Format-> Shortcut String:path (relative or abs.)
		3. When doing path calculations, replaces shortcut with the path.
		
Config file:
	-Single file encapsulates all configuration
	-Choose a delimiter to seperate what would have been seperate files.
	