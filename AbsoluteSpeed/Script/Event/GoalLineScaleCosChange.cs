using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalLineScaleCosChange : MonoBehaviour {

	void Update () {
        var scale = Mathf.Cos(1 * (Time.time * 2));
        this.transform.localScale = new Vector3(scale, scale, scale);
    }
}
