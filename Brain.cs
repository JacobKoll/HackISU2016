using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[System.Serializable]
public class Brain : MonoBehaviour {

    public string nickname = "Jake";

    public int generation = 0;

    public float scoreRecieved;

    public float startingHeight = 1f;

    public float coreStrength = 0f;
    public float headingStrength = 1f;
    public float pushStrenght = 0f;
    public float assistMultiplier = 1f;

    Muscle[] muscles;
    Rigidbody r;

    Color savedColor = Color.white;

    public string getName(){
        string n = nickname;
        n+= "@G" + generation + " #" + gameObject.name;
        return n;
    }
        
    public void Awake(){
        if(!r)
            initialize();
    }

    public void initialize(){
        r = GetComponent<Rigidbody>();
        muscles = GetComponentsInChildren<Muscle>();
        for(int m = 0; m < muscles.Length; m++){
            muscles[m].initialize();
        }
	}

    void FixedUpdate(){
        r.AddForceAtPosition(Vector3.up * coreStrength, transform.position + transform.up * coreStrength, ForceMode.Acceleration);
        r.AddForceAtPosition(-Vector3.up * coreStrength, transform.position - transform.up * coreStrength, ForceMode.Acceleration);
        r.AddForceAtPosition(Vector3.forward * headingStrength, transform.position + transform.forward * headingStrength, ForceMode.Acceleration);
        r.AddForceAtPosition(-Vector3.forward * headingStrength, transform.position - transform.forward * headingStrength, ForceMode.Acceleration);
        r.AddForce(Vector3.forward * pushStrenght, ForceMode.Acceleration);
    }

    void Start(){
        startingHeight = transform.position.y;
    }
	
    public void mutate(float power = 1f){
        for(int i = 0; i < muscles.Length; i++){
            muscles[i].mutate(power);
        }
        MaterialPropertyBlock props = new MaterialPropertyBlock();
        savedColor = savedColor * 0.5f + 0.5f * Random.ColorHSV(0f,1f,0.5f,1f,0.5f,1f);
        props.SetColor("_Color", savedColor);
        GetComponent<Renderer>().SetPropertyBlock(props);
        coreStrength = assistMultiplier*Mathf.Clamp(generation/100f, 0f, 1f);
        headingStrength = assistMultiplier*Mathf.Clamp(generation / 20f, 0f, 10f);
        pushStrenght = assistMultiplier*Mathf.Clamp(generation / 20f, 0f, 10f);
	}

    public float getScore(float wD, float wH, float wR){
        scoreRecieved = wD*transform.position.z/10f + wH*(transform.position.y - startingHeight)/startingHeight + wR*Vector3.Dot(transform.up, Vector3.up);
        return scoreRecieved;
    }
}
