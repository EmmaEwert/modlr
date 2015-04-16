using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class Model : Asset<Model> {
  [JsonConverter(typeof(Converter))]
  public Model parent;
  [NonSerializedAttribute]
  public string name;
  [DefaultValue(true)]
  public bool ambientocclusion = true;
  public List<Element> elements;
  
  
  
  public static Model Load(string name) {
    return Model.Load("models", name);
  }
  
  
  
  public class Element : Asset<Element> {
    [JsonConverter(typeof(Vector.Converter))]
    public Vector from;
    [JsonConverter(typeof(Vector.Converter))]
    public Vector to;
    public Rotation rotation;
    [DefaultValue(true)]
    public bool shade = true;
    public Dictionary<string, Face> faces;
    
    
    
    public class Vector {
      public int x;
      public int y;
      public int z;
      
      public Vector(int x, int y, int z) {
        this.x = x;
        this.y = y;
        this.z = z;
      }
      
      public Vector(float x, float y, float z) {
        this.x = (int)x;
        this.y = (int)y;
        this.z = (int)z;
      }
      
      public class Converter : JsonConverter {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
          Vector vector = value as Vector;
          float[] array = new float[] { vector.x, vector.y, vector.z };
          JToken.FromObject(array).WriteTo(writer);
        }
        
        public override object ReadJson(JsonReader reader, Type type, object value, JsonSerializer serializer) {
          List<float> array = new List<float>(JToken.ReadFrom(reader).Values<float>());
          Vector vector = new Vector(array[0], array[1], array[2]);
          return vector;
        }
        
        public override bool CanConvert(Type type) {
          return type == typeof(Vector);
        }
      }
    }
    
    
    
    public class Rotation {
      [JsonConverter(typeof(Vector.Converter))]
      public Vector origin;
      public string axis;
      public float angle;
      public bool rescale;
    }
    
    
    
    public class Face {
      public int[] uv;
      public string cullface;
      public int rotation;
      public int tintindex;
    }
  }
  
  
  
  public class Converter : JsonConverter {
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
      JToken.FromObject((value as Model).name).WriteTo(writer);
    }
    
    public override object ReadJson(JsonReader reader, Type type, object value, JsonSerializer serializer) {
      string name = JToken.ReadFrom(reader).ToString();
      Model model = Model.Load("models", name);
      model.name = name;
      return model;
    }
    
    public override bool CanConvert(Type type) {
      return type == typeof(Model);
    }
  }
}
