using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileManagerTaskTray
{
    
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new FileManager());
        }
    }


    public class FileManager : ApplicationContext
    {
        private NotifyIcon                _tray;
        private Thread                    _scanner;
        private static StreamReader       _fileIn;
        private static StreamWriter       _log;
        private static List<List<String>> _fileExtensions = new List<List<String>>();
        private static List<String>       _ext = new List<String>(),
                                          _paths = new List<String>();
        private static String             _userPath = Environment.GetEnvironmentVariable("userprofile"),
                                          _cfgPath = _userPath + @"\Downloads\DL_MGR\cfg\";
        private static String             _newPath = _userPath, line;
        private static String[]           _dlFolder = Directory.GetFiles(_userPath + @"\Downloads\"),
                                          _cfgFolder;
        private static char[]             _pathCmdDelims = new char[] { '.', '_', '/', '\\' };


        public static void Loop() {
            downloadManager_init();
            while (true) {
                Thread.Sleep(5000);
                downloadManager();
            }
        }

        public FileManager() {
            _tray = new NotifyIcon() {
                Icon = new System.Drawing.Icon("..\\..\\Treetog-I-Documents.ico"),
                ContextMenu = new ContextMenu(new MenuItem[] {
                    new MenuItem("Config", Config),
                    new MenuItem("About", About),
                    new MenuItem("Exit", Exit)
                }),
                Visible = true
            };
            ThreadStart scanLoop = new ThreadStart(Loop);
            _scanner = new Thread(scanLoop);
            _scanner.Start();
        }

        void Config(object sender, EventArgs e) {
            _scanner.Suspend();
            DialogResult ans = MessageBox.Show
                ("Current Config Directory: " + _cfgPath + "\n\t\tChange Directory?",
                 "Config",
                 MessageBoxButtons.YesNo);
            if (ans == DialogResult.Yes) {
                var fd = new FolderBrowserDialog();
                if (fd.ShowDialog() == DialogResult.OK)
                {
                    _cfgPath = fd.SelectedPath;
                    _scanner = new Thread(new ThreadStart(Loop));   //Restarts the Scanner with new path.
                    _scanner.Start();
                }
                fd.Dispose();
            }
        }

        void About(object sender, EventArgs e) {
           Wndw_Abt about = new Wndw_Abt();
           about.ShowDialog();
        }

        void Exit(object sender, EventArgs e) {
            _tray.Visible = false;
            Application.Exit();
        }

        static void downloadManager_init()
        {
            _log = new StreamWriter(_cfgPath + "\\..\\log.txt");
            if (Directory.Exists(_cfgPath))
            {
                _cfgFolder = Directory.GetFiles(_cfgPath);
                foreach (var f in _cfgFolder.Where(n => n.Contains(".txt")))
                {
                    _fileIn = new StreamReader(f);
                    while ((line = _fileIn.ReadLine()) != null)
                    {
                        if (line.Contains("Path"))
                        {
                            line = line.Split(':')[1].Trim();  //eg) Path:   -> C:\Users\test\Downloads\Images <-
                            if (!line.Contains(':'))           //If relative path,
                                line = _userPath + line;        //make abs.
                            _paths.Add(line);
                        }
                        _ext.Add(line);
                    }
                    _fileIn.Close();
                    _fileExtensions.Add(_ext);
                }
                string temp;
                System.Text.StringBuilder pathsRead = new System.Text.StringBuilder("\nFile Types Read: ");
                foreach (var f in _paths)
                {
                    pathsRead.Append("\n\t\t" + f.Substring((temp = f.Substring(0, f.Length - 1)).LastIndexOf("\\")));
                }
                logIt(pathsRead.ToString());
            }
            else
                throw new DirectoryNotFoundException("Config Folder (" + _cfgPath + ") not found.");

            void logIt(String input)
            {
                _log.WriteLine(DateTime.Now);
                _log.Write(input + "\n");
            }
        }

        static void downloadManager()
        {
             foreach (var f in _dlFolder.Where(p => !p.Contains("EXCLUDE")))
             {
                 String fileName = f.Substring(f.LastIndexOfAny(new char[] { '/', '\\' }) + 1);
                 string PathInstruction;
                 if ((fileName.StartsWith("[") && fileName.Contains("]")))
                 {
                     string file = fileName.Split(']')[1];
                     PathInstruction = fileName.Split(']')[0].Split('[')[1]; //Everything between [ and ]. 
                     List<String> path = PathInstruction.Split(_pathCmdDelims).ToList();
                     foreach (string folder in path)
                     {
                         _newPath += @"\" + folder;
                         if (!Directory.Exists(_newPath))
                             Directory.CreateDirectory(_newPath);
                     }
                     File.Move(_userPath + @"\Downloads\" + fileName, _newPath + @"\" + fileName.Substring(fileName.IndexOf(']') + 1));
                     logIt("\n" + fileName + " moved from " + _userPath + "\\Downloads => " + _newPath+"via Instruction\n");
                 } else {
                     String Extension = f.Substring(f.LastIndexOf('.'), f.Length - f.LastIndexOf('.'));
                     Console.WriteLine("Extension: {0}", Extension);

                     int path_num;
                     for (path_num = 0; path_num < _fileExtensions.ToArray().GetLength(0); path_num++)
                         for (int j = 0; j < _fileExtensions[path_num].Count(); j++)
                             if (Extension.ToUpper() == _fileExtensions[path_num][j])
                                 break;

                     _paths.Add(@"\Other\"); //Accounts for going throughout the entire array.
                     PathInstruction = _paths[path_num];

                     _newPath = String.Empty;
                     String[] folders = PathInstruction.Split('\\');
                     if (folders[0] == String.Empty)
                         _newPath += _userPath;
                     foreach (string folder in folders) {
                         _newPath += ((_newPath.Length!=0) ? @"\" : "") + folder;
                         if (!Directory.Exists(_newPath))
                             Directory.CreateDirectory(_newPath);
                     }
                 }
                 try {
                     File.Move(_userPath + @"\Downloads\" + fileName, _newPath + "_" + fileName);
                     logIt("\n" + fileName + " moved from " + _userPath + "\\Downloads => " + _newPath + "automatically");
                 }
                 catch (IOException e) {
                     Console.WriteLine(e.Message);
                 }
             }//end file Moving
             _log.Dispose();
             void logIt(String input) {
                 _log.WriteLine(DateTime.Now);
                 _log.Write(input+"\n");
             }
        }

        //https://code.4noobz.net/c-copy-a-folder-its-content-and-the-subfolders/
        static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
             Directory.CreateDirectory(target.FullName);

             // Copy each file into the new directory.
             foreach (FileInfo fi in source.GetFiles())
             {
                 Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                 fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
             }

             // Copy each subdirectory using recursion.
             foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
             {
                 DirectoryInfo nextTargetSubDir =
                     target.CreateSubdirectory(diSourceSubDir.Name);
                 CopyAll(diSourceSubDir, nextTargetSubDir);
             }
        }
    }//end Class
}
