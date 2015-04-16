using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using UnityEngine;
using UnityEngine.UI;

public class Editor : MonoBehaviour {
  public enum EditorState {
    Sculpting,
    Painting
  }

  public enum Direction {
    West,
    East,
    Down,
    Up,
    South,
    North
  }

  public Transform focus;

  public Model model;

  private float zoom = 32.0f;

  private EditorState currentState = EditorState.Sculpting;

  public Text mode;

  // Painting variables
  private Color paintColor;

  public GameObject inventory;
  public RectTransform texturesInventory;
  public RawImage inventoryTexturePrefab;
  public List<Toggle> textureToggles;


  public Direction side {
    get {
      var forward = this.transform.forward;
      var absolute = new Vector3(Mathf.Abs(forward.x), Mathf.Abs(forward.y), Mathf.Abs(forward.z));
      if (absolute.x > absolute.y && absolute.x > absolute.z) {
        return forward.x > 0.0f ? Direction.West : Direction.East;
      } else if (absolute.y > absolute.x && absolute.y > absolute.z) {
        return forward.y > 0.0f ? Direction.Down : Direction.Up;
      } else {
        return forward.z > 0.0f ? Direction.South : Direction.North;
      }
    }
  }


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


  private Dictionary<string, Texture2D> textures {
    get {
      if (this._textures == null) {
        this._textures = new Dictionary<string, Texture2D>();
        var jar = OpenJAR();
        var folder = "assets/minecraft/textures/";
        var extension = ".png";
        foreach (ZipEntry entry in jar) {
          var name = entry.Name;
          if (name.StartsWith(folder + "blocks") && name.EndsWith(extension, true, null)) {
            var stream = jar.GetInputStream(entry);
            var memory = new MemoryStream(4096);
            var buffer = new byte[4096];
            StreamUtils.Copy(stream, memory, buffer);
            var texture = new Texture2D(16, 16, TextureFormat.ARGB32, false, true);
            if (texture.LoadImage(memory.ToArray())) {
              texture.filterMode = FilterMode.Point;
              texture.wrapMode = TextureWrapMode.Clamp;
              name = name.Substring(folder.Length, name.Length - folder.Length - extension.Length);
              this._textures[name] = texture;
            }
          }
        }
      }
      return this._textures;
    }
  }


  private Texture texture {
    set {
      this.textureToggles[(int)this.side].GetComponentInChildren<RawImage>().texture = value;
      this.model.materials[this.side].mainTexture = value;
    }
  }


  private string _folder;
  private Dictionary<string, Texture2D> _textures;



  void Start() {
    this.transform.LookAt(this.focus.position, Vector3.up);
    this.transform.position = this.focus.position - this.transform.forward * this.zoom;

    this.textureToggles[(int)this.side].isOn = true;

    this.mode.text = this.currentState.ToString();

    // Insert textures into inventory
    foreach (var texture in this.textures) {
      var image = Instantiate<RawImage>(this.inventoryTexturePrefab);
      image.transform.SetParent(this.texturesInventory);
      image.transform.localScale = Vector3.one;
      image.name = texture.Key;
      image.texture = texture.Value;
      image.uvRect = Rect.MinMaxRect(0, 0, 1.0f / (texture.Value.width / 16), 1.0f / (texture.Value.height / 16));
      var button = image.GetComponent<Button>();
      button.onClick.AddListener(() => this.texture = image.texture);
      // TODO: Animated textures
    }
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
      this.transform.Translate(Vector3.right * horizontal * this.zoom / 32.0f + Vector3.up * vertical * this.zoom / 32.0f);
      this.transform.LookAt(focus.position, Vector3.up);
      this.transform.position = this.focus.position - this.transform.forward * this.zoom;
    }


    // Switch states on tab
    if (Input.GetKeyDown(KeyCode.Tab)) {
      this.ChangeState();
    }

    // Editing of Model
    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    RaycastHit hit;

    if (Physics.Raycast(ray, out hit)) {
      switch (this.currentState) {
        case EditorState.Sculpting:
          if (Input.GetMouseButtonDown(0)) {
            hit.point -= hit.normal * 0.5f;
            this.model.block.Remove(new Box(new Vector(Mathf.FloorToInt(hit.point.x), Mathf.FloorToInt(hit.point.y), Mathf.FloorToInt(hit.point.z))));

            this.model.Rebuild();
          }

          if (Input.GetMouseButtonDown(1)) {
            hit.point += hit.normal * 0.5f;
            this.model.block.Add(new Box(new Vector(Mathf.FloorToInt(hit.point.x), Mathf.FloorToInt(hit.point.y), Mathf.FloorToInt(hit.point.z))));

            this.model.Rebuild();
          }
          break;
        case EditorState.Painting:
          if (Input.GetMouseButtonDown(0)) {
            // Get the color on the boxS
            Texture2D texture = hit.transform.GetComponent<MeshRenderer>().sharedMaterial.mainTexture as Texture2D;

            Vector2 uv = hit.textureCoord * 16.0F;
            Debug.Log("Get Pixel Color from coords: " + uv);

            this.paintColor = texture.GetPixel(Mathf.RoundToInt(uv.x), Mathf.RoundToInt(uv.y));
          }
          
          if (Input.GetMouseButtonDown(1)) {
            // Paint the selected color on the box
            Texture2D texture = hit.transform.GetComponent<MeshRenderer>().sharedMaterial.mainTexture as Texture2D;

            Vector2 uv = hit.textureCoord * 16.0F;
            Debug.Log("Set Pixel Color at coords: " + uv);

            texture.SetPixel(Mathf.FloorToInt(uv.x), Mathf.FloorToInt(uv.y), this.paintColor);
            texture.Apply();
          }
          break;
        default:
          break;
      }
    }


    // UI
    if (Input.GetButtonDown("Inventory")) {
      this.inventory.SetActive(!this.inventory.activeSelf);
    }


    // Debug
    if (Input.GetKeyDown(KeyCode.Return)) {
      this.Save("Export", "crafting_table");
    }
  }



  public void ChangeState() {
    if (this.currentState == EditorState.Sculpting) {
      this.currentState = EditorState.Painting;
    } else if (this.currentState == EditorState.Painting) {
      this.currentState = EditorState.Sculpting;
    }
    this.mode.text = this.currentState.ToString();
  }



  /// <summary>
  /// Saves the model into a JSON string and writes to the given file
  /// </summary>
  /// <param name="filePath">The File Path to write the file to</param>
  /// <param name="fileName">The File Name to write to </param>
  public void Save(string filePath, string fileName) {
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
