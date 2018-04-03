using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour {

    public Transform origin;
    public float re_speed;  // revolution speed
    public float ro_speed;  // rotation speed
    float rz, ry;

	// Use this for initialization
	void Start () {
        rz = Random.Range(10, 60);
        ry = Random.Range(10, 60);
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 axis = new Vector3(0, ry, rz);
        this.transform.RotateAround(origin.position, axis, re_speed * Time.deltaTime);
        this.transform.Rotate(Vector3.up * ro_speed * Time.deltaTime);
    }
}
