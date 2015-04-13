using UnityEngine;
using System.Collections;
using System.IO;

public class Editor : MonoBehaviour {
    public enum EditorState
    {
        Sculpting,
        Painting
    }



	public Transform focus;

    public Model model;

	private float zoom = 32.0f;

    private EditorState currentState = EditorState.Sculpting;



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

        
        // Switch states on tab
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            this.ChangeState();
        }

        // Editing of Model
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            switch (this.currentState)
            {
                case EditorState.Sculpting:
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
                    break;
                case EditorState.Painting:
                    if (Input.GetMouseButtonDown(0))
                    {

                    }

                    if (Input.GetMouseButtonDown(1))
                    {

                    }
                    break;
                default:
                    break;
            }
        }

        // Debug
        if (Input.GetKeyDown (KeyCode.Return)) this.Save("bin", "crafting_table");
	}



    void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 100, 22), this.currentState == EditorState.Sculpting ? "Sculpting" : "Painting");
    }



    public void ChangeState()
    {
        if (this.currentState == EditorState.Sculpting)
        {
            this.currentState = EditorState.Painting;
        }
        else if (this.currentState == EditorState.Painting)
        {
            this.currentState = EditorState.Sculpting;
        }
    }



    /// <summary>
    /// Loads a model from a JSON string and loads it into the editor
    /// </summary>
    /// <param name="filePathAndName">The File Path and File Name to read from</param>
    public void Load(string filePathAndName)
    {
        // TODO - Write up importing
        //this.model.json = File.ReadAllText(filePathAndName);
    }



    /// <summary>
    /// Saves the model into a JSON string and writes to the given file
    /// </summary>
    /// <param name="filePath">The File Path to write the file to</param>
    /// <param name="fileName">The File Name to write to </param>
    public void Save(string filePath, string fileName)
    {
        Directory.CreateDirectory(filePath);
        File.WriteAllText(filePath + "/" + fileName + ".json", this.model.json);
    }
}
