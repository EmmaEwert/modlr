using System.Collections.Generic;

public class Box {
	public Vector min;
	public Vector max;
	
	public UnityEngine.Vector3 position { get { return this.scale / 2.0f + this.min.vector3; } }
	public UnityEngine.Vector3 scale    { get { return this.size.vector3; } }

	public Vector size { get { return this.max - this.min + 1; } }
	public int volume { get { return this.size.x * this.size.y * this.size.z; } }



	public Box() : this(8) { }
	
	public Box(int size) : this(new Vector(8 - size / 2), new Vector(8 - size / 2 + size - 1)) { }
	
	public Box(Vector position) : this(position, position) { }
	
	public Box(Vector min, Vector max) {
		this.min = new Vector(min.x, min.y, min.z);
		this.max = new Vector(max.x, max.y, max.z);
	}



	public bool Intersects(Box box) {
		return
			this.min.x <= box.max.x &&
			this.min.y <= box.max.y &&
			this.min.z <= box.max.z &&
			this.max.x >= box.min.x &&
			this.max.y >= box.min.y &&
			this.max.z >= box.min.z;
	}

















}
