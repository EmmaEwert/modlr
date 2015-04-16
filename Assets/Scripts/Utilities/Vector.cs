using System.Collections.Generic;

public class Vector {
  public static Vector west { get { return new Vector(-1, 0, 0); } }
  public static Vector east { get { return new Vector(1, 0, 0); } }
  public static Vector down { get { return new Vector(0, -1, 0); } }
  public static Vector up { get { return new Vector(0, 1, 0); } }
  public static Vector south { get { return new Vector(0, 0, -1); } }
  public static Vector north { get { return new Vector(0, 0, 1); } }
  public UnityEngine.Vector3 vector3 { get { return new UnityEngine.Vector3(this.x, this.y, this.z); } }

  public int x;
  public int y;
  public int z;



  public Vector(int a) : this(a, a, a) { }



  public Vector(int x, int y, int z) {
    this.x = x;
    this.y = y;
    this.z = z;
  }



  public static Vector Min(Vector a, Vector b) {
    return new Vector(a.x < b.x ? a.x : b.x, a.y < b.y ? a.y : b.y, a.z < b.z ? a.z : b.z);
  }



  public static Vector Max(Vector a, Vector b) {
    return new Vector(a.x > b.x ? a.x : b.x, a.y > b.y ? a.y : b.y, a.z > b.z ? a.z : b.z);
  }



  public static bool operator ==(Vector a, Vector b) {
    return a.Equals(b);
  }



  public static bool operator !=(Vector a, Vector b) {
    return !a.Equals(b);
  }



  public static Vector operator -(Vector a, Vector b) {
    return new Vector(a.x - b.x, a.y - b.y, a.z - b.z);
  }



  public static Vector operator +(Vector a, Vector b) {
    return new Vector(a.x + b.x, a.y + b.y, a.z + b.z);
  }



  public static Vector operator +(Vector a, int b) {
    return new Vector(a.x + b, a.y + b, a.z + b);
  }



  public override bool Equals(object o) {
    if (o == null) {
      return false;
    }

    return this == (Vector)o;
  }



  public bool Equals(Vector v) {
    return this.x == v.x && this.y == v.y && this.z == v.z;
  }



  public override string ToString() {
    return string.Format("[{0},{1},{2}]", this.x, this.y, this.z);
  }



  public override int GetHashCode() {
    return x ^ y ^ z;
  }
}