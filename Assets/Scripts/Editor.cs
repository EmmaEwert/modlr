using UnityEngine;
using System.Collections;

public class Editor : MonoBehaviour {
	public Transform focus;

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
	}
}
