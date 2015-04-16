﻿using System.Collections;
using System.Collections.Generic;

public class JSON : IEnumerable {
  private Dictionary<string, object> objects = new Dictionary<string, object>();



  public JSON() { }



  // Requires: Somewhat valid JSON object string
  public JSON(string text) {
    // this.objects = (this.Parse(text) as JSON).objects;
  }



  public static object Parse(string text) {
    return null;
  }



  public override string ToString() {
    var json = "{";
    foreach (var obj in this.objects) {
      string key = obj.Key;
      object value = obj.Value;
      json += string.Format("\"{0}\":", key);
      if (value is IEnumerable<JSON> || value is IEnumerable<object> || value is IEnumerable<IEnumerable<char>> || value is IEnumerable<int> || value is IEnumerable<float> || value is IEnumerable<double>) {
        json += "[";
        if (value is IEnumerable<object>) {
          new List<object>(value as object[]).ForEach(o => json += o + ",");
        } else if (value is IEnumerable<IEnumerable<char>>) {
          new List<IEnumerable<char>>(value as IEnumerable<IEnumerable<char>>).ForEach(o => json += string.Format("\"{0}\",", o));
        } else if (value is IEnumerable<int>) {
          new List<int>(value as IEnumerable<int>).ForEach(o => json += o + ",");
        } else if (value is IEnumerable<float>) {
          new List<float>(value as IEnumerable<float>).ForEach(o => json += o + ",");
        } else if (value is IEnumerable<double>) {
          new List<double>(value as IEnumerable<double>).ForEach(o => json += o + ",");
        } else if (value is IEnumerable<JSON>) {
          new List<JSON>(value as IEnumerable<JSON>).ForEach(o => json += o + ",");
        }
        json = json.TrimEnd(new[] { ',' }) + "]";
      } else if (value is IEnumerable<char>) {
        json += string.Format("\"{0}\"", value);
      } else if (value is Dictionary<string, string>) {
        json += "{";
        foreach (var keyValue in (value as Dictionary<string, string>)) {
          json += string.Format("\"{0}\":\"{1}\",", keyValue.Key, keyValue.Value);
        }
        json = json.TrimEnd(new[] { ',' }) + "}";
      } else {
        json += value;
      }
      json += ",";
    }
    return json.TrimEnd(new[] { ',' }) + "}";
  }



  public object this[string index] {
    get {
      object value;
      if (this.objects.TryGetValue(index, out value)) {
        return value;
      }
      return null;
    }
    set {
      if (this.objects.ContainsKey(index)) {
        this.objects.Remove(index);
      }
      this.objects.Add(index, value);
    }
  }



  public void Add(string key, object value) {
    if (value == null) return;

    this.objects.Add(key, value);
  }



  public void Add(string key, object value, object defaultValue) {
    if (value == null || value.Equals(defaultValue)) return;

    this.objects.Add(key, value);
  }



  IEnumerator IEnumerable.GetEnumerator() {
    throw new System.NotImplementedException();
  }
}
