using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DetachChildrenAndDestroy : MonoBehaviour {

	void Start () {
		if (transform.parent == null)
			transform.DetachChildren();
		else {
			List<Transform> children = new List<Transform>();
			foreach (Transform child in transform) {
				children.Add(child);
			}
			foreach (Transform child in children) {
				child.parent = transform.parent;
			}
		}
		
		Destroy(gameObject);
	}
}
