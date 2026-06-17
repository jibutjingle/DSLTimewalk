using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using Microsoft.Win32;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

[InitializeOnLoad]
public class DependencyChecker
{
    static string dreamscapeSettingsPath = "Assets/Editor/DreamscapeProjectSettings.asset";
    static string PLAYERPREF_UPDATE_SKIP_KEY = "SDK_update_skip_time";
    static string current_install_path;
    static bool dissonance_installed;
    static string latest_major_version;
    static string latest_major_version_install;
    static string latest_version;
    static string latest_version_install;
    static DependencyChecker()
    {

        UnityEditor.PackageManager.Client.Resolve();

        bool found = false;
        var list = UnityEditor.PackageManager.Client.List(true, false);
        while (!list.IsCompleted)
        {

        }
        dissonance_installed = false;
        string currentArtanimVersionString = "";
        Version currentArtanimVersion = new Version();
        foreach (var item in list.Result)
        {
            if (item.name == "com.artanim.common")
            {
                found = true;
                currentArtanimVersionString = item.version;
                currentArtanimVersion = Version.Parse(currentArtanimVersionString.Replace("-exp", ""));
            }
            else if (item.name == "com.dreamscape.voip.dissonance")
            {
                dissonance_installed = true;
            }
        }
        bool uptodate = false;
        bool foundOneSdk = false;
        string desiredVersion = string.Empty;
        if (!found)
        {
            //on first install if template-info.json is present use it to select the desired version instead of the latest
            var templateInfoPath = Application.dataPath + "/template-info.json";

            if (File.Exists(templateInfoPath))
            {
                var jsonString = File.ReadAllText(templateInfoPath);
                TemplateInfo templateInfo = JsonUtility.FromJson<TemplateInfo>(jsonString);
                desiredVersion = templateInfo.version;
                Debug.Log($"Using Template Info version {desiredVersion}");
            }
            else
            {
                Debug.LogError($"No Template Info version {templateInfoPath}");
            }
        }

        List<RegistryKey> dreamscapeLearnKeys = new List<RegistryKey>();

        RegistryKey softwareLMKey = Registry.CurrentUser.OpenSubKey("Software");
        if (softwareLMKey != null)
        {
            RegistryKey dreamscapeKey = softwareLMKey.OpenSubKey("Dreamscape");
            if (dreamscapeKey != null)
            {
                RegistryKey dreamscapeLearnKey = dreamscapeKey.OpenSubKey("DreamscapeLearn");
                if (dreamscapeLearnKey != null)
                {
                    dreamscapeLearnKeys.Add(dreamscapeLearnKey);
                }
            }
        }
        RegistryKey softwareKey = Registry.LocalMachine.OpenSubKey("Software");
        if (softwareKey != null)
        {
            RegistryKey dreamscapeKey = softwareKey.OpenSubKey("Dreamscape");
            if (dreamscapeKey != null)
            {
                RegistryKey dreamscapeLearnKey = dreamscapeKey.OpenSubKey("DreamscapeLearn");
                if (dreamscapeLearnKey != null)
                {
                    dreamscapeLearnKeys.Add(dreamscapeLearnKey);
                }
            }
        }
        Dictionary<string, RegistryKey> artanimVersions = new Dictionary<string, RegistryKey>();
        latest_version = "";
        Version selectedArtanimVersion = null;
        foreach (var dreamscapeLearnKey in dreamscapeLearnKeys)
        {
            foreach (string sub in dreamscapeLearnKey.GetSubKeyNames())
            {
                RegistryKey key = dreamscapeLearnKey.OpenSubKey(sub);
                string artanim = (string)key.GetValue("artanim_version");

                latest_major_version = "";
                Version latestMajor = new Version();
                if (artanim == null)
                {
                    Debug.LogError($"Artanim Common SDK version not found");
                }
                else
                {
                    artanimVersions.Add(artanim, key);
                    var version = Version.Parse(artanim.Replace("-exp", ""));
                    if (desiredVersion != string.Empty)
                    {
                        if (artanim == desiredVersion)
                        {
                            latest_version = artanim;
                            selectedArtanimVersion = version;
                        }
                    }
                    else if (latest_version == "" || (CompareVersions(version, selectedArtanimVersion) > 0 && (currentArtanimVersionString == "" || version.Major == currentArtanimVersion.Major)))
                    {
                        latest_version = artanim;
                        selectedArtanimVersion = version;
                    }

                    if (latest_major_version == "" || (CompareVersions(version, selectedArtanimVersion) > 0))
                    {
                        latest_major_version = artanim;
                        latestMajor = version;
                        latest_major_version_install = (string)key.GetValue("install");
                    }
                }
            }
            if(!foundOneSdk)
                foundOneSdk = dreamscapeLearnKey.GetSubKeyNames().Count() > 0;
        }


        
        desiredVersion = latest_version;


        if (CompareVersions(selectedArtanimVersion, currentArtanimVersion) > 0 && currentArtanimVersionString != "" && selectedArtanimVersion.Major == currentArtanimVersion.Major)
        {
            uptodate = false;
            Debug.Log($"ARTANIM {currentArtanimVersionString} -> {latest_version}");
        }
        else
        {
            uptodate = true;
        }

        if (artanimVersions.ContainsKey(currentArtanimVersionString))
        {
            current_install_path = (string)artanimVersions[currentArtanimVersionString].GetValue("install");

        }

        if (artanimVersions.ContainsKey(latest_version))
        {
            latest_version_install = (string)artanimVersions[latest_version].GetValue("install");

        }
        if (latest_version_install == null)
        {
            Debug.LogError($"SDK install path not found");
        }




        //Debug.Log($"DependencyChecker foundOneSdk {foundOneSdk} uptodate {uptodate} found {found} install_path {install_path}");
        if (!foundOneSdk)
        {
            Debug.LogError("SDK NOT INSTALLED");
        }
        else
        {

            var skip_date = PlayerPrefs.GetString(PLAYERPREF_UPDATE_SKIP_KEY, "");
            bool skip = false;
            if (skip_date != "")
            {
                var date = DateTime.Parse(skip_date);
                if (date != null && date.AddMonths(1).CompareTo(DateTime.Now) > 0)
                {
                    skip = true;
                }
            }

            if (!skip)
            {
                try
                {
                    // Read the file content
                    string fileContent = File.ReadAllText(dreamscapeSettingsPath);

                    // Regular expression to match m_check_update
                    Regex regex = new Regex(@"m_check_update:\s*(\d+)");
                    Match match = regex.Match(fileContent);
                    if (match.Success)
                    {
                        string mCheckUpdateValue = match.Groups[1].Value;
                        if (mCheckUpdateValue == "0")
                        {
                            skip = true;
                        }
                    }

                }
                catch (Exception ex)
                {
                    Debug.Log($"{ex.Message}");
                }
            }

            //   PlayerPrefs.GetString()
            if (!uptodate && !skip && found)
            {
                if (EditorUtility.DisplayDialog($"Dreamscape SDK version {currentArtanimVersionString} is outdated",
            $"A more recent SDK is installed, do you wish to upgrade your project to version {desiredVersion} ? If you upgrade Unity will restart once to reload the packages.", "Upgrade", "Skip"))
                {
                    Clean();

                    found = false;
                }
                else
                {
                    PlayerPrefs.SetString(PLAYERPREF_UPDATE_SKIP_KEY, DateTime.Now.ToString());
                }
            }


            if (latest_version_install == null || latest_version_install == "")
            {
                Debug.LogError($"SDK install path undefined");
            }
            else if (!found)
            {
                Update(latest_version_install);
            }
        }
    }

    private static void Clean()
    {
        Debug.Log("CLEANING DREAMSCAPE SDK");
        //clean manifest.json
        string path = Path.GetDirectoryName(Application.dataPath) + "\\Packages\\dependencies\\";
        Debug.Log(path);
        System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo("ds-unity-package-importer.exe");
        info.WorkingDirectory = path;
        info.Arguments = $" --clean";
        var process = System.Diagnostics.Process.Start(info);
        process.WaitForExit();

        //clean dependencies folder
        FileUtil.DeleteFileOrDirectory("Packages/dependencies");
    }

    private static void Update(string install_path)
    {
        string projectPath = Path.GetDirectoryName(Application.dataPath);

        string path = $"{install_path}dependencies\\pack-import-without-dsl.bat";


        System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo("\"" + path + "\"");
        info.Arguments = $"\"{projectPath}\"";
        var process = System.Diagnostics.Process.Start(info);
        process.WaitForExit();
        Debug.Log("DEPENDENCIES IMPORT DONE");

        //update this file
        var destFilePath = "Assets/Editor/DependencyChecker.cs";
        FileUtil.DeleteFileOrDirectory(destFilePath);
        FileUtil.CopyFileOrDirectory(install_path.Replace("\\", "/") + "unityScripts/DependencyChecker.cs", destFilePath);

        //Reimport addressable groups to update them use reflexion for compilation if package not present
        //Type AddressableSchemaFix = Type.GetType("Dreamscape.Rvsdk.RVSDKPackageReimportTool, com.artanim.common.editor");
        //if (AddressableSchemaFix != null)
        //{
        //    MethodInfo ReimportAddressableAssetGroups = AddressableSchemaFix.GetMethod("ReimportAddressableAssetGroups", BindingFlags.Static | BindingFlags.Public);
        //    if (ReimportAddressableAssetGroups != null)
        //    {
        //        Debug.LogError("ReimportAddressableAssetGroups");
        //        ReimportAddressableAssetGroups.Invoke(null, null);
        //    }
        //    else
        //    {
        //        Debug.LogError("No method ReimportAddressableAssetGroups");
        //    }

        //}

        ForceAddressableGroupReimport();
    }

    private static int CompareVersions(Version a, Version b)
    {
        if (a == b) return 0;
        if (a.Revision == -1 || b.Revision == -1)
        {
            Version a_ = new Version(a.Major, a.Minor, Mathf.Max(a.Build, 0));
            Version b_ = new Version(b.Major, b.Minor, Mathf.Max(b.Build, 0));
            int res = a_.CompareTo(b_);
            if (res == 0)
            {
                if (a.Revision == -1)
                {
                    res = 1;
                }
                else if (b.Revision == -1)
                {
                    res = -1;
                }
            }
            return res;
        }
        else
        {
            return a.CompareTo(b);
        }
    }

    [MenuItem("RVSDK/ForceAddressableGroupReimport")]
    public static void ForceAddressableGroupReimport()
    {
        string projectPath = Path.GetDirectoryName(Application.dataPath);
        var packageCachePath = projectPath.Replace("\\", "/") + "/Library/PackageCache";
        if (Directory.Exists(packageCachePath))
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(packageCachePath);

            foreach (var dir in directoryInfo.GetDirectories())
            {
                if (dir.Name.Contains("artanim") || dir.Name.Contains("dreamscape"))
                {
                    Debug.Log(dir.FullName);
                    Directory.Delete(dir.FullName, true);
                }
            }
        }
        Debug.Log("cleaned packageCache");

        PlayerPrefs.SetInt("ReimportOnRestart", 1);
        PlayerPrefs.Save();

        EditorApplication.OpenProject(Directory.GetCurrentDirectory());
    }
    private class TemplateInfo
    {
        public string version;
    }

    const string MENU_IMPORT_DISSONANCE = "RVSDK/Import Dissonance";

    [MenuItem(MENU_IMPORT_DISSONANCE)]
    public static void ImportDissonance()
    {

        //clean manifest.json
        string path = Path.GetDirectoryName(Application.dataPath) + "\\Packages\\dependencies\\";
        Debug.Log(path);
        System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo("ds-unity-package-importer.exe");
        info.WorkingDirectory = path;
        info.Arguments = $" --clean";
        var process = System.Diagnostics.Process.Start(info);
        process.WaitForExit();

        string projectPath = Path.GetDirectoryName(Application.dataPath);

        path = $"{current_install_path}dependencies\\pack-import-without-dsl-dissonance.bat";
        Debug.Log(path);
        info = new System.Diagnostics.ProcessStartInfo("\"" + path + "\"");
        info.Arguments = $"\"{projectPath}\"";
        process = System.Diagnostics.Process.Start(info);
        process.WaitForExit();

    }

    [MenuItem(MENU_IMPORT_DISSONANCE, true)]
    private static bool ValidateImportDissonance()
    {
        return !dissonance_installed;
    }

    const string MENU_UPDATE_MAJOR = "RVSDK/Tools/Update to latest installed Major version";

    [MenuItem(MENU_UPDATE_MAJOR)]
    public static void UpdateToLatestMajor()
    {
        if (latest_major_version != string.Empty && latest_major_version_install != string.Empty)
        {
            if (current_install_path == latest_major_version_install)
            {
                Debug.LogWarning($"SDK already using version {latest_major_version}");
            }
            else if (EditorUtility.DisplayDialog($"Do you want to update SDK to version {latest_major_version}?",
             $"If you upgrade Unity will restart once to reload the packages.", "Update", "Cancel"))
            {
                Clean();

                Update(latest_major_version_install);
            }
        }
    }

    [MenuItem(MENU_UPDATE_MAJOR, true)]
    private static bool ValidateUpdateToLatestMajor()
    {
        return latest_major_version != string.Empty && latest_major_version_install != string.Empty && current_install_path != latest_major_version_install;
    }

    const string MENU_UPDATE_MINOR = "RVSDK/Tools/Update to latest installed Minor version";

    [MenuItem(MENU_UPDATE_MINOR)]
    public static void UpdateToLatestMinor()
    {
        if (latest_version != string.Empty && latest_version_install != string.Empty)
        {

            if (current_install_path == latest_version_install)
            {
                Debug.LogWarning($"SDK already using version {latest_version}");
            }
            else if (EditorUtility.DisplayDialog($"Do you want to update SDK to version {latest_version}?",
             $"If you upgrade Unity will restart once to reload the packages.", "Update", "Cancel"))
            {
                Clean();

                Update(latest_version_install);
            }
        }
    }

    [MenuItem(MENU_UPDATE_MINOR, true)]
    private static bool ValidateUpdateToLatestMinor()
    {
        return latest_version != string.Empty && latest_version_install != string.Empty && current_install_path != latest_version_install;
    }
}

