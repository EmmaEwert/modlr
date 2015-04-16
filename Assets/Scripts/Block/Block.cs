using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class Block : Asset<Block> {
  public Dictionary<string, VariantCollection> variants = new Dictionary<string, VariantCollection>();
  
  
  
  public static Block Load(string name) {
    return Block.Load("blockstates", name);
  }
  
  
  
  public class Variant {
    [JsonConverter(typeof(ModelConverter))]
    public Model model;
    public int x;
    public int y;
    public bool uvlock;
    [DefaultValue(1)]
    public int weight = 1;
    
    public class ModelConverter : Model.Converter {
      public override object ReadJson(JsonReader reader, Type type, object value, JsonSerializer serializer) {
        string name = JToken.ReadFrom(reader).ToString();
        Model model = Model.Load("models", "block/" + name);
        model.name = name;
        return model;
      }
    }
  }
  
  
  
  [JsonConverter(typeof(VariantCollection.Converter))]
  public class VariantCollection : IEnumerable<Variant> {
    private List<Variant> variants;
    
    public VariantCollection(Variant variant) {
      this.variants = new List<Variant> { variant };
    }
    
    public VariantCollection(List<Variant> variants) {
      this.variants = variants;
    }

    public IEnumerator<Variant> GetEnumerator() {
      return this.variants.GetEnumerator();
    }
    
    IEnumerator IEnumerable.GetEnumerator() {
      return this.variants.GetEnumerator() as IEnumerator;
    }
    
    public class Converter : JsonConverter {
      public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
        List<Variant> variants = (value as VariantCollection).variants;
        if (variants.Count == 0) {
          JToken.FromObject(null).WriteTo(writer);
        } else if (variants.Count == 1) {
          JToken.FromObject(variants[0]).WriteTo(writer);
        } else {
          JToken.FromObject(variants).WriteTo(writer);
        }
      }
      
      public override object ReadJson(JsonReader reader, Type type, object value, JsonSerializer serializer) {
        JToken token = JToken.ReadFrom(reader);
        if (token.Type == JTokenType.Object) {
          return new VariantCollection(token.ToObject<Variant>());
        } else {
          return new VariantCollection(token.ToObject<List<Variant>>());
        }
      }
      
      public override bool CanConvert(Type type) {
        return type == typeof(VariantCollection) || type == typeof(Variant);
      }
    }
  }
}
