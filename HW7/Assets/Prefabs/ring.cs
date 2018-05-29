using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ring : MonoBehaviour {

    public ParticleSystem particleSystem;
    private int count;

	// Use this for initialization
	void Start () {
        particleSystem = gameObject.GetComponent<ParticleSystem>();
        count = 0;
	}
	
	// Update is called once per frame
	void Update () {
        if (particleSystem.isPlaying)
        {
            return;
        }
        switch(count)
        {
            case 0:
                particleSystem.startColor = Color.red;
                break;
            case 1:
                particleSystem.startColor = Color.blue;
                break;
            case 2:
                particleSystem.startColor = Color.yellow;
                break;
            case 3:
                particleSystem.startColor = Color.white;
                break;
            default:
                break;
        }
        particleSystem.Play();
        count++;
        if (count == 4)
            count = 0;
	}
}
