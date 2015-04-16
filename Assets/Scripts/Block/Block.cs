using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class Block : Asset<Block> {
  public Dictionary<string, Variant> variants = new Dictionary<string, Variant>();
  
  
  
  public static Block Load(string name) {
    return Block.Load("blockstates", name);
  }
  
  
  
  public class Variant {
    [JsonConverter(typeof(Block.ModelConverter))]
    public Model model;
    public int x;
    public int y;
    public bool uvlock;
    [DefaultValue(1)]
    public int weight = 1;
  }
  
  
  
  public class ModelConverter : Model.Converter {
    public override object ReadJson(JsonReader reader, Type type, object value, JsonSerializer serializer) {
      string name = JToken.ReadFrom(reader).ToString();
      Model model = Model.Load("models", "block/" + name);
      model.name = name;
      return model;
    }
  }
}
