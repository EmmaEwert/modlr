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
                { "textures", this.textures },
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
                                    { "north", new JSON { { "texture", "#north" }, { "cullface", "north" } } },
                                    { "south", new JSON { { "texture", "#south" }, { "cullface", "south" } } },
                                    { "west", new JSON { { "texture", "#west" }, { "cullface", "west" } } },
                                    { "east", new JSON { { "texture", "#east" }, { "cullface", "east" } } }
                                }
                            }
                        }
                    )
                }
            }.ToString();
        }
    }



	void Start() {
		block.Add(new Box(16));

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
