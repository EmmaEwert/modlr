using UnityEngine;
using System.Collections;
using System.IO;

public class Editor : MonoBehaviour {
	public Transform focus;

    public Model model;

	private float zoom = 32.0f;



	void Start() {
		this.transform.LookAt(focus.position, Vector3.up);
		this.transform.position = this.focus.position - this.transform.forward * this.zoom;
	}



	void Update() {
		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");
		float normal = Input.GetAxis("Normal"); // Zoom

		if (normal != 0.0f) {
			this.zoom -= (1.0f + 1.0f / 256.0f) * normal;
			this.transform.LookAt(focus.position, Vector3.up);
			this.transform.position = this.focus.position - this.transform.forward * this.zoom;
		}

		if (horizontal != 0.0f || vertical != 0.0f) {
			this.transform.Translate(Vector3.right * horizontal * this.zoom / 32.0f + Vector3.up * vertical * this.zoom / 32.0f );
			this.transform.LookAt(focus.position, Vector3.up);
			this.transform.position = this.focus.position - this.transform.forward * this.zoom;
		}

        
        // Editing of Model
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (Input.GetMouseButtonDown(0))
            {
                hit.point -= hit.normal * 0.5f;
                this.model.block.Remove(new Box(new Vector(Mathf.FloorToInt(hit.point.x), Mathf.FloorToInt(hit.point.y), Mathf.FloorToInt(hit.point.z))));

                this.model.Rebuild();
            }

            if (Input.GetMouseButtonDown(1))
            {
                hit.point += hit.normal * 0.5f;
                this.model.block.Add(new Box(new Vector(Mathf.FloorToInt(hit.point.x), Mathf.FloorToInt(hit.point.y), Mathf.FloorToInt(hit.point.z))));

                this.model.Rebuild();
            }
        }
	}



    /// <summary>
    /// Imports a model from a JSON string and loads it into the editor
    /// </summary>
    /// <param name="filePathAndName">The File Path and File Name to read from</param>
    public void ImportModelFromJSON(string filePathAndName)
    {
        // TODO - Write up importing
        //this.model.json = File.ReadAllText(filePathAndName);
    }



    /// <summary>
    /// Exports the model into a JSON string and writes to the given file
    /// </summary>
    /// <param name="filePathAndName">The File Path and File Name to write to</param>
    public void ExportModelToJSON(string filePathAndName)
    {
        Directory.CreateDirectory(filePathAndName);
        File.WriteAllText(this.model.json, filePathAndName);
    }
}
