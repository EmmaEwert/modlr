using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json;

public abstract class Asset<T> {
  [NonSerialized]
  public static Dictionary<string, T> cache;
  
  private static JsonSerializerSettings settings = new JsonSerializerSettings {
    DefaultValueHandling = DefaultValueHandling.Ignore,
    NullValueHandling = NullValueHandling.Ignore
  };
  
  private static string folder {
    get {
      if (Asset<T>._folder == null) {
        Asset<T>._folder = Environment.CurrentDirectory;
        var separator = Path.DirectorySeparatorChar;
        
        switch (Environment.OSVersion.Platform) {
          case PlatformID.Unix:
            Asset<T>._folder = string.Format("{0}{1}.minecraft{1}", Environment.GetFolderPath(Environment.SpecialFolder.Personal), separator);
            break;
          case PlatformID.MacOSX:
            Asset<T>._folder = string.Format("{0}{1}Library{1}Application Support{1}minecraft{1}", Environment.GetFolderPath(Environment.SpecialFolder.Personal), separator);
            break;
          case PlatformID.Win32NT:
            Asset<T>._folder = string.Format("{0}{1}.minecraft{1}", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), separator);
            break;
        }
      }
      return Asset<T>._folder;
    }
  }
  private static string _folder;
  
  
  
  public static void Preload() {
    Block.Preload("blockstates");
  }
  
  
  
  // TODO: Preload all models before calling Load() for submodels (how?)
  // TODO: Load asynchronously
  // TODO: Load resourcepacks, mod assets and modlr projects
  protected static void Preload(string path) {
    Asset<T>.cache = new Dictionary<string, T>();
    
    ZipFile jar = null;
    var folders = new List<DirectoryInfo>(new System.IO.DirectoryInfo(string.Format("{0}versions", Asset<T>.folder)).GetDirectories()); folders.Reverse();
    
    foreach (DirectoryInfo folder in folders) {
      string version = folder.Name;
      if (version.StartsWith("1.")) {
        string source = string.Format("{0}{1}{2}.jar", folder.FullName, Path.DirectorySeparatorChar, version);
        jar = new ZipFile(File.OpenRead(source));
        break;
      }
    }
    
    if (jar == null) {
      return;
    }
    
    string prefix = string.Format("assets/minecraft/{0}/", path);
    
    foreach (ZipEntry entry in jar) {
      if (entry.Name.StartsWith(prefix)) {
        string filename = entry.Name.Remove(0, prefix.Length);
        if (filename.EndsWith(".json")) {
          string name = filename.Remove(filename.Length - ".json".Length, ".json".Length);
          if (!Asset<T>.cache.ContainsKey(name)) {
            Stream stream = jar.GetInputStream(entry);
            Asset<T>.cache[name] = JsonConvert.DeserializeObject<T>(new StreamReader(stream).ReadToEnd(), Asset<T>.settings);
          }
        }
      }
    }
    
    jar.Close();
  }
  
  
  
  protected static T Load(string path, string name) {
    if (Asset<T>.cache == null) {
      Asset<T>.Preload(path);
    }
    
    T t;
    
    if (name.StartsWith("builtin/") || Asset<T>.cache.TryGetValue(name, out t)) {
      return t;
    }
    
    ZipFile jar = null;
    var folders = new List<DirectoryInfo>(new System.IO.DirectoryInfo(string.Format("{0}versions", Asset<T>.folder)).GetDirectories());
    
    folders.Reverse();
    
    foreach (DirectoryInfo folder in folders) {
      string version = folder.Name;
      if (version.StartsWith("1.")) {
        string source = string.Format("{0}{1}{2}.jar", folder.FullName, Path.DirectorySeparatorChar, version);
        jar = new ZipFile(File.OpenRead(source));
        break;
      }
    }
    
    if (jar == null) {
      return default (T);
    }
    
    string prefix = string.Format("assets/minecraft/{0}/{1}", path, name);
    
    foreach (ZipEntry entry in jar) {
      if (entry.Name.StartsWith(prefix)) {
        string extension = entry.Name.Remove(0, prefix.Length);
        Stream stream = jar.GetInputStream(entry);
        switch (extension.ToLower()) {
          case ".json":
            t = JsonConvert.DeserializeObject<T>(new StreamReader(stream).ReadToEnd(), Asset<T>.settings);
            Asset<T>.cache[name] = t;
            jar.Close();
            return t;
        }
        break;
      }
    }
    
    jar.Close();
    
    return default (T);
  }
  
  
  
  public override string ToString() {
    return JsonConvert.SerializeObject(this, Formatting.Indented, Asset<T>.settings);
  }
}
