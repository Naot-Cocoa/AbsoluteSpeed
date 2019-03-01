using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Linq;

public class CarRotate : MonoBehaviour
{
	
	// Update is called once per frame
	void Update () {
        transform.eulerAngles += new Vector3(0f,0.1f,0f);
        if (Input.GetKeyDown(KeyCode.S))
        {
            foreach(var x in gameObject.Descendants())
            {
                x.isStatic = true;
            }
            gameObject.isStatic = true;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            foreach (var x in gameObject.Descendants())
            {
                x.isStatic = false;
            }
            gameObject.isStatic = false;
        }
    }
}
