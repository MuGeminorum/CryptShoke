using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExecutionOrderBehaviour : MonoBehaviour, System.IComparable<ExecutionOrderBehaviour> {
	public int priority;
	
	void OnEnable () {
		ExecutionOrderController controller = GetComponent<ExecutionOrderController>();
		//if (controller == null)
		//	controller = gameObject.AddComponent<ExecutionOrderController>();
		
		controller.Register(this);
	}
	
	public int CompareTo (ExecutionOrderBehaviour other) {
		return priority.CompareTo(other.priority);
	}
	
	public virtual void UpdateCustom () {}
	public virtual void LateUpdateCustom () {}
}

public class ExecutionOrderController : MonoBehaviour {
	
	public List<ExecutionOrderBehaviour> behaviours = new List<ExecutionOrderBehaviour>();
	
	public void Register (ExecutionOrderBehaviour behaviour) {
		if (!behaviours.Contains (behaviour)) {
			behaviours.Add(behaviour);
			behaviours.Sort();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.deltaTime == 0 || Time.timeScale == 0)
			return;
		foreach (ExecutionOrderBehaviour behaviour in behaviours)
			behaviour.UpdateCustom();
	}
	
	// LateUpdate is called once per frame
	void LateUpdate () {
		if (Time.deltaTime == 0 || Time.timeScale == 0)
			return;
		foreach (ExecutionOrderBehaviour behaviour in behaviours)
			behaviour.LateUpdateCustom();
	}
}
