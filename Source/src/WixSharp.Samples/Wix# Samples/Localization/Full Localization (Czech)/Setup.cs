/*
 Based on the code sample contributed by Wix# user Kosmak Martin from Czech Republic.
 */

using System;
using System.Xml;
using Microsoft.Win32;
using System.Windows.Forms;
using Microsoft.Deployment.WindowsInstaller;
using System.Resources;
using WixSharp;

class Script
{
    static public void Main(string[] args)
    {

        try
        {
            string lang = "cs-CZ"; // determine language localization

            //get access to CLR resource file
            var resPath = "Resource." + lang + ".resx"; // path to resource if buildинг from command prompt
            if (!System.IO.File.Exists(resPath))
                Environment.CurrentDirectory = @"..\..\"; // path to resource if buildинг from Visual Studio

            var rs = new ResXResourceSet(resPath);

            //Prepare features

            // binaries
            Feature binaries = new Feature
                                {
                                    Name = rs.GetString("FeatAppName"),
                                    ConfigurableDir = "APPLICATIONROOTDIRECTORY",    // this enables customization of installation folder
                                    Description = rs.GetString("FeatAppDesc")
                                };
            // application data
            Feature datas = new Feature
                             {
                                 Name = rs.GetString("FeatDataName"),
                                 Description = rs.GetString("FeatDataDesc")
                             };

            //documentation
            Feature docs = new Feature
                            {
                                Name = rs.GetString("FeatDocName"),
                                Description = rs.GetString("FeatDocDesc")
                            };
            //shortcuts
            Feature shortcuts = new Feature
                {
                    Name = rs.GetString("FeatShortcutName"),
                    Description = rs.GetString("FeatShortcutDesc")
                };

            // Prepare project
            Project project =
                new Project("LocalizationTest",

                    //Files and Shortcuts
                    new Dir(new Id("APPLICATIONROOTDIRECTORY"), @"%ProgramFiles%\LocalizationTest",

                        // application binaries
                        new File(binaries, @"AppFiles\Bin\MyApp.exe",
                            new WixSharp.Shortcut(shortcuts, @"APPLICATIONROOTDIRECTORY"),
                            new WixSharp.Shortcut(shortcuts, @"%Desktop%")),
                        new File(binaries, @"AppFiles\Bin\MyApp.dll"),
                        new WixSharp.Shortcut(binaries, "Uninstall Localization Test", "[System64Folder]msiexec.exe", "/x [ProductCode]"),

                        // directory with application data
                        new Dir("Data",
                            new File(datas, @"AppFiles\Data\app_data.dat")),

                        // directory with documentation
                        new Dir("Doc",
                            new File(docs, @"AppFiles\Docs\manual.txt"))),

                    //program menu uninstall shortcut
                    new Dir(@"%ProgramMenu%\Kosmii\KosmiiTest",
                        new WixSharp.Shortcut(shortcuts, "Uninstall Kosmii Test", "[System64Folder]msiexec.exe", "/x [ProductCode]")));

            // set project properties
            project.GUID = new Guid("6fe30b47-2577-43ad-9095-1861ba25889a");
            project.UpgradeCode = new Guid("6fe30b47-2577-43ad-9095-1861ba25889b");
            project.LicenceFile = @"AppFiles\License.rtf";
            project.Language = lang;
            project.Encoding = System.Text.Encoding.UTF8;
            project.Manufacturer = Environment.UserName;
            project.UI = WUI.WixUI_Mondo;
            project.SourceBaseDir = Environment.CurrentDirectory;
            project.MSIFileName = "LocalizationTest";

            Compiler.BuildMsi(project);
        }
        catch (System.Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}




