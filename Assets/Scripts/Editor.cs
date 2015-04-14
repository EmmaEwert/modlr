using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

public class Editor : MonoBehaviour {
	public Transform focus;
	public Model model;
	public RectTransform texturePanel;
	public Image texturePrefab;
	
	public string folder {
		get {
			if (this._folder == null) {
				this._folder = Environment.CurrentDirectory;
				var separator = Path.DirectorySeparatorChar;

				switch (Environment.OSVersion.Platform) {
					case PlatformID.Unix:
						this._folder = string.Format("{0}{1}.minecraft{1}", Environment.GetFolderPath(Environment.SpecialFolder.Personal), separator);
						break;
					case PlatformID.MacOSX:
						this._folder = string.Format("{0}{1}Library{1}Application Support{1}minecraft{1}", Environment.GetFolderPath(Environment.SpecialFolder.Personal), separator);
						break;
					case PlatformID.Win32NT:
						this._folder = string.Format("{0}{1}.minecraft{1}", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), separator);
						break;
				}
			}
			return this._folder;
		}
	}

	private float zoom = 32.0f;

	private string _folder;



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

        // Debug
        if (Input.GetKeyDown (KeyCode.Return)) {
			this.Save("Export", "crafting_table");
		}

		if (Input.GetKeyDown(KeyCode.Backspace)) {
			this.Load(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "/.minecraft/versions/1.8.3/1.8.3.jar");
		}
	}



    /// <summary>
    /// Loads a model from a JSON string and loads it into the editor
    /// </summary>
    /// <param name="filePathAndName">The File Path and File Name to read from</param>
	// TODO: Store unpacked files locally as atlas for quick relaunch
	// TODO: Asynchronous unpacking
    public void Load(string filename)
	{
		var jar = OpenJAR();
		foreach (ZipEntry entry in jar) {
			var name = entry.Name;
			if (name.EndsWith(".png", true, null)) {
				var stream = jar.GetInputStream(entry);
				var memory = new MemoryStream(4096);
				var buffer = new byte[4096];
				StreamUtils.Copy(stream, memory, buffer);
				var texture = new Texture2D(256, 256, TextureFormat.ARGB32, false, true);
				if (texture.LoadImage(memory.ToArray())) {
					texture.filterMode = FilterMode.Point;
					texture.wrapMode = TextureWrapMode.Clamp;
					var textureImage = Instantiate<Image>(this.texturePrefab);
					textureImage.GetComponent<RectTransform>().SetParent(this.texturePanel);
					textureImage.sprite = Sprite.Create(texture, new Rect(0, 0, 16, 16), Vector2.one * 0.5f);
					// GameObject.Find("Front").GetComponent<MeshRenderer>().sharedMaterial.mainTexture = texture;
				}
			}
		}
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



	// Open most recent Minecraft JAR
	// TODO: Read <version>.json instead to determine most recent
	private ZipFile OpenJAR() {
		var folders = new List<DirectoryInfo>(new System.IO.DirectoryInfo(string.Format("{0}versions", this.folder)).GetDirectories());
		folders.Reverse();

		foreach (var folder in folders) {
			var version = folder.Name;
			if (version.StartsWith("1.")) {
				return OpenJAR(string.Format("{0}{1}{2}.jar", folder.FullName, Path.DirectorySeparatorChar, version));
			}
		}

		return null;
	}



	private ZipFile OpenJAR(string filename) {
		var stream = File.OpenRead(filename);
		return new ZipFile(stream);
	}
}
