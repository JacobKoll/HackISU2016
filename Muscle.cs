using UnityEngine;
using System.Collections;
using UnityEditor;

[System.Serializable]
public class Muscle : MonoBehaviour {

    public float amplitude = 2f;
    public float offset = 0f;
    public float frequency = 1f;
    public float center = 0f;

    float time = 0f;

    HingeJoint joint;
    public Muscle pairedMuscle;
    public enum Pair{
        none,
        copy,
        mirror,
        follow
    }
    public Pair pair;

	public void initialize(){
        joint = GetComponent<HingeJoint>();
        if(pairedMuscle && !pairedMuscle.pairedMuscle){
            pairedMuscle.pairedMuscle = this;
            pairedMuscle.pair = pair;
        }
        mutate(0f);
	}

    public void mutate(float power = 1f){
        offset+= power * Random.Range(-0.1f, 0.1f);
        frequency+= power * Random.Range(-0.1f, 0.1f);
        amplitude+= power * Random.Range(-0.1f, 0.1f);
        center+= power * Random.Range(-0.01f, 0.01f);

        if(pairedMuscle){
            pairedMuscle.frequency = frequency;
            pairedMuscle.center = center;
            pairedMuscle.amplitude = amplitude;
            if(pair == Pair.copy){
                pairedMuscle.offset = offset;
            }
            else if(pair == Pair.mirror){
                pairedMuscle.offset = offset + frequency/2f;
            }
            else if(pair == Pair.follow){
                pairedMuscle.offset = offset + frequency/8f;
            }
        }
    }
	
	void FixedUpdate(){
        time+= Time.fixedDeltaTime;
        JointMotor m = joint.motor;
        m.targetVelocity = amplitude * 100f * sinusoid(time + offset, frequency, amplitude, center);
        joint.motor = m;
	}

    float sinusoid(float t, float f, float a, float c){
        return a*(Mathf.Sin(t * Mathf.PI*2f*f)+c);
    }
}
