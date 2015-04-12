using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Model : MonoBehaviour {
	public GameObject boxPrefab;

	private Block block = new Block();
	private float time;

	private JSON json {
		get {
			return new JSON { {
				"elements", new[] {
					new JSON {
						{ "from",  new[] {  0,  0,  0 } },
						{ "to",    new[] { 15.5f, 15, .5e-5 } },
						{ "faces", new JSON {
							{ "down",  new JSON { { "texture", "#down"  }, { "cullface", "down"  } } },
							{ "up",    new JSON { { "texture", "#up"    }, { "cullface", "up"    } } },
							{ "north", new JSON { { "texture", "#north" }, { "cullface", "north" } } },
							{ "south", new JSON { { "texture", "#south" }, { "cullface", "south" } } },
							{ "west",  new JSON { { "texture", "#west"  }, { "cullface", "west"  } } },
							{ "east",  new JSON { { "texture", "#east"  }, { "cullface", "east"  } } }
						} }
					}
				}
			} };
		}
	}



	void Start() {
		block.Add(new Box(16));

		this.Rebuild();
	}



	void Update() {
		var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit)) {
			if (Input.GetMouseButtonDown(0)) {
				hit.point -= hit.normal * 0.5f;
				this.block.Remove(new Box(new Vector(Mathf.FloorToInt(hit.point.x), Mathf.FloorToInt(hit.point.y), Mathf.FloorToInt(hit.point.z))));
				
				this.Rebuild();
			}
			
			if (Input.GetMouseButtonDown(1)) {
				hit.point += hit.normal * 0.5f;
				this.block.Add(new Box(new Vector(Mathf.FloorToInt(hit.point.x), Mathf.FloorToInt(hit.point.y), Mathf.FloorToInt(hit.point.z))));

				this.Rebuild();
			}
		}

		// Temporary JSON test
		if (Input.GetKeyDown(KeyCode.Return)) {
			Debug.Log(this.json);
		}
	}



	void Rebuild() {
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
