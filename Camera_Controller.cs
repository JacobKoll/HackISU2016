using UnityEngine;
using System.Collections;

public class Camera_Controller : MonoBehaviour 
{
	
	public float interpVelocity;
	public float minDistance;
	public float followDistance;
    public Transform tmpTarg;
    public Transform target;
	Vector3 offset;
	Vector3 toffset;

    public Transform[] targets;
	
    void Start(){
        offset = transform.position;
	}

	void Update(){
        if(targets.Length == 0)
            return;
		tmpTarg = targets[0];
		for(int i = 0; i < targets.Length; i++){
            if(targets[i] && tmpTarg && targets[i].position.z >= tmpTarg.position.z){
				tmpTarg = targets[i];
			}
		}
        target = tmpTarg;

		if(target){
			Vector3 posNoZ = transform.position;
			posNoZ.z = target.position.z;

			Vector3 targetDirection = (target.position - posNoZ);

			interpVelocity = targetDirection.magnitude * 5f;

			toffset = transform.position + (targetDirection.normalized * interpVelocity * Time.deltaTime); 

            transform.position = Vector3.Lerp(transform.position, target.position + offset, 2f * Time.deltaTime);

		}
	}

    public void setTargets(Transform[] t){
        targets = t;
    }
}
