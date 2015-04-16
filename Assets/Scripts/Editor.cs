using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class Editor : MonoBehaviour {
  public enum Direction {
    West,
    East,
    Down,
    Up,
    South,
    North
  }
  
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

  /*
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
  */
  
  private Texture texture {
    set {
      this.textureToggles[(int)this.side].GetComponentInChildren<RawImage>().texture = value;
    }
  }
  
  private float zoom = 32.0f;
  private string _folder;
  private Dictionary<string, Texture2D> _textures;



  void Start() {
    this.transform.LookAt(Vector3.zero, Vector3.up);
    this.transform.position = Vector3.zero - this.transform.forward * this.zoom;
    
    this.textureToggles[(int)this.side].isOn = true;
  }



  void Update() {
    // Camera movement
    float horizontal = Input.GetAxis("Horizontal");
    float vertical = Input.GetAxis("Vertical");
    float normal = Input.GetAxis("Normal"); // Zoom

    if (normal != 0.0f) {
      this.zoom -= (1.0f + 1.0f / 256.0f) * normal;
      this.transform.LookAt(Vector3.zero, Vector3.up);
      this.transform.position = Vector3.zero - this.transform.forward * this.zoom;
    }

    if (horizontal != 0.0f || vertical != 0.0f) {
      this.transform.Translate(Vector3.right * horizontal * this.zoom / 32.0f + Vector3.up * vertical * this.zoom / 32.0f);
      this.transform.LookAt(Vector3.zero, Vector3.up);
      this.transform.position = Vector3.zero - this.transform.forward * this.zoom;
      this.textureToggles[(int)this.side].isOn = true;
    }
    
    
    // Editing of Model
    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    RaycastHit hit;

    if (Physics.Raycast(ray, out hit)) {
      if (Input.GetButtonDown("Pick")) {
        hit.point -= hit.normal * 0.5f;
      }

      if (Input.GetButtonDown("Apply")) {
        hit.point += hit.normal * 0.5f;
      }
    }
    
    
    // UI
    if (Input.GetButtonDown("Inventory")) {
      this.inventory.SetActive(!this.inventory.activeSelf);
    }
  }
}
