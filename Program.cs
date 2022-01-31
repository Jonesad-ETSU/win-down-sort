namespace windows_download_sorter;
using System;
using System.Text.RegularExpressions;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        Application.Run(new FileManager());
    }    
}

class FileManager : ApplicationContext {
    public static string? configDirectory, downloadsDirectory, userDirectory;
    public static Dictionary<string,string> shortcuts, extensions;
    public static FileSystemWatcher? downloadsWatcher, configWatcher;
    public static Func<String, bool> isAbsolutePath = System.IO.Path.IsPathFullyQualified;


    public FileManager () {
        // Check configuration directory and read in config.
        userDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        downloadsDirectory = $"{userDirectory}\\Downloads";
        configDirectory = $"{downloadsDirectory}\\config";
        
        useDirectory(configDirectory);
        parseConfig(configDirectory);

        if (File.Exists($"{configDirectory}\\EvaluateDirectoryOnStartup")) {
            Console.WriteLine($"Run for each file in {downloadsDirectory} - EXCEPT NOT YET- FEATURE NOT READY YET");
            // THIS IS POTENTIALLY DANGEROUS - NEEDS TESTING FIRST
            // var files = Directory.GetFiles(downloadsDirectory);
            // foreach(var file in files) {
            //     string name = file.Split('\\').Last();
            //     checkFile(name, file);
            // }
        }

        // Handle Download Folder Changes
        downloadsWatcher = new FileSystemWatcher(downloadsDirectory);
        downloadsWatcher.NotifyFilter = NotifyFilters.Attributes |
                               NotifyFilters.FileName | 0;

        downloadsWatcher.Created += changeHandler;
        downloadsWatcher.Renamed += changeHandler;
        downloadsWatcher.IncludeSubdirectories = false;
        downloadsWatcher.EnableRaisingEvents = true;

        //Handle changes in configuration
        configWatcher = new FileSystemWatcher(configDirectory);
        configWatcher.NotifyFilter = NotifyFilters.LastWrite;

        configWatcher.Changed += changeConfigHandler;
        configWatcher.IncludeSubdirectories = false;
        configWatcher.EnableRaisingEvents = true;
    }

    public static void parseConfig(string directory) {
        shortcuts = new Dictionary<string, string>();
        extensions = new Dictionary<string, string>();
        
        var shortsFile = useFile($"{directory}\\shorts.cfg");
        var extensionsFile = useFile($"{directory}\\extensions.cfg");
        
        readFile (
            shortsFile,
            "shorts",
            shortcuts,
            checkForShorts
        );

        readFile (
            extensionsFile,
            "extensions",
            extensions,
            checkForShorts  //Handle Shortcuts in extensions declaration.
        );        
    }

    public static void changeHandler (object sender, FileSystemEventArgs e) {
        if (e.Name != null && e.FullPath != null)
            checkFile(e.Name, e.FullPath);
    }

    public static void changeConfigHandler (object sender, FileSystemEventArgs e) {
        if(configDirectory != null)
            parseConfig(configDirectory);
    }

    public static void checkFile (string name = "", string fullPath = "") {        
        Regex pullBetweenBraces = new Regex("^(?<=\\[).*(?=\\])");   //Thank you Google.
        Match match = pullBetweenBraces.Match(name);
        string? moveToDirectory;
        if (match.Success) {
            // var directoryStack = match.Value.Split('\\');
            var directoryStack = match.Value.Split(',');
            string parent = directoryStack[0];
            string children = String.Join('\\', directoryStack.Skip(1));
            
            if (shortcuts.ContainsKey(parent))
                parent = shortcuts[match.Value];

            moveToDirectory = parent;
            if (children != "")
                moveToDirectory += $"\\{children}";

        
            moveToDirectory = useDirectory(moveToDirectory);

            var fileNameNoInstruction = name.Split(']')[1]; //Remove Instruction.
            moveFile(fullPath, $"{moveToDirectory}\\{fileNameNoInstruction}");
        } else {
            var fileExtension = name.Substring(name.LastIndexOf('.')+1);
            if (extensions.ContainsKey(fileExtension)) {
                moveToDirectory = useDirectory(extensions[fileExtension]);
                moveFile(fullPath, $"{moveToDirectory}\\{name}");
            }
        }
    }


    public static string useFile(string file) {
        if (!File.Exists(file)) {
            File.Create(file).Close();
            // Closes current stream(will be used later)
        }
        return file;
    }

    public static string useDirectory(string dir) {
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        if (!isAbsolutePath(dir))
            return $"{userDirectory}\\{dir}";
        return dir;
    }

    public static void moveFile (string from, string to) {
        if (File.Exists(to)) {
            string[] toStack = to.Split('.');
            string name = String.Join('.',toStack.Take(toStack.Length - 1));
            string extension = toStack.Last();
            string counter = "(1)"; // We already know there is an inital match.
            for ( 
                int i = 2; // Avoids an unnecessary loop iteration where counter = (1) again.
                File.Exists($"{name}{counter}.{extension}");
                counter = $"({i++})"
            ) {}
            to = $"{name}{counter}.{extension}";
        }

        while(!attemptMove(from, to)) { Thread.Sleep(1000); }
        bool attemptMove(string from, string to) {
            try {
                File.Move(from, to);
                Console.WriteLine($"File moved to: {to}");
                return true;
            } catch (Exception e) {
                Console.WriteLine(e.GetBaseException());
                return false;
            }
        }
    }

    public static string[] checkForShorts (string[] keyVal) {
        var elements = keyVal[1].Split('\\');
        if (shortcuts.ContainsKey(elements[0]))
            elements[0] = shortcuts[elements[0]];

        return new String[] {
            keyVal[0], String.Join('\\',elements)
        };
    }

    public static void readFile (
        string file,
        string name,
        Dictionary<string, string> dict,
        Func<string[],string[]>? keyFilter = null
    ) {
        using (var sr = new StreamReader(file)) {
            while(!sr.EndOfStream) {
                string? line = sr.ReadLine();
                
                if(line == null)
                    throw new Exception($"Config Line for {name} is null");
                else
                    line = line.Trim();

                if (line.StartsWith("//")) continue; //Allow comments

                var keyVal = line.Split('=');
                if (keyFilter != null)
                    keyVal = keyFilter(keyVal);

                if (dict.ContainsKey(keyVal[0]))
                    dict[keyVal[0]] = keyVal[1];
                else
                    dict.Add(keyVal[0], keyVal[1]);
            }
        }
    }
}