using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Model : MonoBehaviour {
  public GameObject boxPrefab;
  public Block block = new Block();
  
  private bool ambientocclusion = true;
  private string parent = null;
  private Dictionary<string, string> textures = new Dictionary<string,string>();



  public string json {
    get {
      return new JSON {
        { "parent", this.parent, null },
        { "ambientocclusion", this.ambientocclusion, true },
        { "textures", this.textures, null },
        { "elements", this.block.boxes.ConvertAll(box => new JSON {
            { "from", box.min },
            { "to", (box.max + 1) },
            //{ "shade", box.shade, true },
            { "faces", new JSON
              {
                { "up", new JSON { { "texture", "#up" }, { "cullface", "up" } } },
                { "down", new JSON { { "texture", "#down" }, { "cullface", "down" } } },
                { "north", new JSON { { "texture", "#side" }, { "cullface", "north" } } },
                { "south", new JSON { { "texture", "#side" }, { "cullface", "south" } } },
                { "west", new JSON { { "texture", "#side" }, { "cullface", "west" } } },
                { "east", new JSON { { "texture", "#side" }, { "cullface", "east" } } }
              }
            }
          })
        }
      }.ToString();
    }
  }



  void Start() {
    this.block.Add(new Box(16));
    
    this.textures = new Dictionary<string, string>();
    this.textures["particle"] = "blocks/crafting_table_front";
    this.textures["up"] = "blocks/crafting_table_top";
    this.textures["down"] = "blocks/planks_oak";
    this.textures["north"] = "blocks/crafting_table_front";
    this.textures["west"] = "blocks/crafting_table_front";
    this.textures["south"] = "blocks/crafting_table_side";
    this.textures["east"] = "blocks/crafting_table_side";

    this.Rebuild();
  }



  public void Rebuild() {
    foreach (Transform transform in this.transform) {
      Destroy(transform.gameObject);
    }

    foreach (Box box in this.block.boxes) {
      var transform = Instantiate<GameObject>(this.boxPrefab).transform;
      transform.parent = this.transform;
      transform.position = box.position;
      transform.localScale = box.scale;
    }
  }
}
