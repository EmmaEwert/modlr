using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Model : MonoBehaviour {
	public GameObject boxPrefab;
	
	// Add initial 48x48x48 volume
	public Block block = new Block();

    private float time;


    private bool ambientocclusion = true;

    private string parent;

    private Dictionary<string, string> textures = new Dictionary<string,string>();



    public string json
    {
        get
        {
            return new JSON
            {
                { "parent", this.parent, null },
                { "ambientocclusion", this.ambientocclusion, true },
                { "textures", this.textures, null },
                {
                    "elements", this.block.boxes.ConvertAll(box =>
                        new JSON
                        {
                            { "from", box.min },
                            { "to", (box.max + 1) },
                            //{ "shade", box.shade, true },
                            {
                                "faces", new JSON
                                {
                                    { "up", new JSON { { "texture", "#up" }, { "cullface", "up" } } },
                                    { "down", new JSON { { "texture", "#down" }, { "cullface", "down" } } },
                                    { "north", new JSON { { "texture", "#side" }, { "cullface", "north" } } },
                                    { "south", new JSON { { "texture", "#side" }, { "cullface", "south" } } },
                                    { "west", new JSON { { "texture", "#side" }, { "cullface", "west" } } },
                                    { "east", new JSON { { "texture", "#side" }, { "cullface", "east" } } }
                                }
                            }
                        }
                    )
                }
            }.ToString();
        }
    }



	void Start() {
		Debug.Log(new JSON("{\"elements\":[{\"from\":[0,0,0],\"to\":[15.5,15,5E-06],\"faces\":{\"down\":{\"texture\":\"#down\",\"cullface\":\"down\"},\"up\":{\"texture\":\"#up\",\"cullface\":\"up\"},\"north\":{\"texture\":\"#north\",\"cullface\":\"north\"},\"south\":{\"texture\":\"#south\",\"cullface\":\"south\"},\"west\":{\"texture\":\"#west\",\"cullface\":\"west\"},\"east\":{\"texture\":\"#east\",\"cullface\":\"east\"}}}]}"));

		block.Add(new Box(16));

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
		this.time = Time.realtimeSinceStartup;

		foreach (Transform transform in this.transform) {
			Destroy(transform.gameObject);
		}

		foreach (Box box in this.block.boxes) {
			var transform = Instantiate<GameObject>(this.boxPrefab).transform;
			transform.parent = this.transform;
			transform.position = box.position;
			transform.localScale = box.scale;
		}
		
		Debug.Log(string.Format("Rebuild ({0:0.000} ms)", (UnityEngine.Time.realtimeSinceStartup - time) * 1000.0f));
	}
}
