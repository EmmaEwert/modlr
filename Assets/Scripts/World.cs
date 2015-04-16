using UnityEngine;
using System.Collections.Generic;

public class World : MonoBehaviour {
  void Start() {
    // Preload blocks
    float time = Time.realtimeSinceStartup;
    Block.Preload();
    Debug.Log(string.Format("Preload: {0} s", Time.realtimeSinceStartup - time));
    
    
    // Build blocks
    Vector3 position = (Vector3.left + Vector3.back) * Mathf.Sqrt(Block.cache.Values.Count) * 96.0f;
    foreach (Block block in Block.cache.Values) {
      float angle = 0.0f;
      foreach (Block.VariantCollection variants in block.variants.Values) {
        Vector3 offset = position + Quaternion.AngleAxis(angle, Vector3.up) * Vector3.right * 64.0f;
        foreach (Block.Variant variant in variants) {
          Model model = variant.model;
          while (model.parent != null) {
            model = model.parent;
          }
          foreach (Model.Element element in model.elements) {
            Transform cube = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
            cube.localScale = (element.to - element.from).vector3;
            cube.localPosition = offset + element.from.vector3 + (element.to - element.from).vector3 / 2.0f;
            if (element.rotation != null) {
              cube.RotateAround(offset + element.rotation.origin.vector3, Vector3.up, element.rotation.angle);
            }
          }
          offset += Vector3.up * 24.0f;
        }
        angle += 360.0f / block.variants.Count;
      }
      if (position.x < Mathf.Sqrt(Block.cache.Values.Count) * 96.0f) {
        position.x += 192.0f;
      } else {
        position.x = -Mathf.Sqrt(Block.cache.Values.Count) * 96.0f;
        position.z += 192.0f;
      }
    }
  }
}
