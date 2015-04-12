using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Model : MonoBehaviour {
	public GameObject boxPrefab;

	private Block block = new Block();
	private float time;

	private JSON json {
		get {
			return new JSON{
				from = { 0, 0, 0 }
			};
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
			Debug.Log(this);
		}
	}



	public override string ToString() {
		return this.json.Stringify();
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
