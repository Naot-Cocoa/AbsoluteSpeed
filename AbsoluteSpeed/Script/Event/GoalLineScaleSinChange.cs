using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalLineScaleSinChange : MonoBehaviour {

	void Update () {
        var scale = Mathf.Sin(1 * (Time.time * 2));
        this.transform.localScale = new Vector3(scale, scale, scale);
    }
}
