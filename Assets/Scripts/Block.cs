using System.Collections.Generic;

public class Block {
  public List<Box> boxes { get; private set; }



  public Block() {
    this.boxes = new List<Box>();
  }



  public void Add(Box box) {
    this.boxes.Add(box);
  }



  public void Remove(Box a) {
    int index;

    // Split each intersected box into 0..6 other boxes
    while ((index = this.boxes.FindIndex(b => a.Intersects(b))) >= 0) {
      var b = this.boxes[index];
      var boxes = new List<Box>(6);
      boxes.Add(new Box(b.min, new Vector(a.min.x - 1, b.max.y, b.max.z))); // West
      boxes.Add(new Box(new Vector(a.max.x + 1, b.min.y, b.min.z), b.max)); // East
      boxes.Add(new Box(new Vector(a.min.x, b.min.y, b.min.z), new Vector(a.max.x, a.min.y - 1, b.max.z))); // Down
      boxes.Add(new Box(new Vector(a.min.x, a.max.y + 1, b.min.z), new Vector(a.max.x, b.max.y, b.max.z))); // Up
      boxes.Add(new Box(new Vector(a.min.x, a.min.y, b.min.z), new Vector(a.max.x, a.max.y, a.min.z - 1))); // South
      boxes.Add(new Box(new Vector(a.min.x, a.min.y, a.max.z + 1), new Vector(a.max.x, a.max.y, b.max.z))); // North
      foreach (var box in boxes) {
        if (box.size.x > 0 && box.size.y > 0 && box.size.z > 0) {
          this.boxes.Add(box);
        }
      }
      this.boxes.RemoveAt(index);
    }
  }
}
