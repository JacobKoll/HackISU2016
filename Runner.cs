using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public class Runner : MonoBehaviour {

    [System.Serializable]
    public class Animal{
        public GameObject activeBody = null;
        public Brain activeBrain = null;
        public float score = -10f;
    }
    public GameObject ancestor;

    public string bestPrefabName = "Doggooo";
    public int bestIndex = 0;

    public int currentGen = 0;
    public int maxGeneration = 1002;
    public Animal[] flock;
    //public float genBestScore = float.MinValue;

    public int N = 3; // number of animals per best
    //public int K = 3; // keep best k animals

    public float spacing = 5f;

    public float simulationTime = 10f;

    public float wDistance = 1f;
    public float wHeight = 1f;
    public float wDot = 1f;

    public Camera_Controller cameraController;

    IEnumerator Start(){
        

        flock = new Animal[N];

        string[] headers = new string[N+1];
        headers[0] = "Generation";
        for(int h = 1; h < headers.Length; h++){
            headers[h] = "A"+h;
        }
        SaveCSV.ClearAndSetHeaders(headers);

        fill();

        while(currentGen < maxGeneration){

            run();

            Transform[] flockTransforms = new Transform[flock.Length];
            for(int f = 0; f < flock.Length; f++){
                flockTransforms[f] = flock[f].activeBody.transform;
            }
            cameraController.setTargets(flockTransforms);

            yield return new WaitForSeconds(simulationTime);

            grade();

            for(int ff = 0; ff < flock.Length; ff++){
                Destroy(flock[ff].activeBody);
            }

            currentGen++;
        }
	}

    void fill(){
        int i = 0;
        //for(int k = 0; k < K; k++){
            GameObject temp = (GameObject)Instantiate(ancestor);
            for(int n = 0; n < N; n++){
                //flock[i].prefabRef = (GameObject)
                PrefabUtility.CreatePrefab("Assets/Resources/run_temp/"+i+".prefab", temp);
                i++;
            }
            Destroy(temp);
        //}
    }

    void grade(){
        string[] data = new string[flock.Length+1];
        data[0] = ""+currentGen;
        Animal tempBest = flock[0];
        tempBest.score = flock[0].activeBrain.getScore(wDistance, wHeight, wDot);
        for(int f = 0; f < flock.Length; f++){
            flock[f].score = flock[f].activeBrain.getScore(wDistance, wHeight, wDot);
            data[f+1] = ""+flock[f].score;
            if(flock[f].score > tempBest.score){
                tempBest = flock[f];
                bestIndex = f;
            }
        }
        Debug.Log("Gen " + currentGen + " = " + tempBest.score);
        SaveCSV.AddRow(data);
        SaveCSV.Save();
        if(currentGen == 1 || currentGen == 10 || currentGen == 50 || currentGen == 100 || currentGen == 200 || currentGen == 500 || currentGen == 800 || currentGen%500 == 0)
            save(bestIndex);
    }

    void run(){
        int i = 0;
        //for(int k = 0; k < K; k++){
            for(int n = 0; n < N; n++){
                flock[i] = new Animal();
            flock[i].activeBody = (GameObject)Instantiate((GameObject)Resources.Load("run_temp/"+bestIndex+"", typeof(GameObject)));
                flock[i].activeBody.name = ""+i;
                Vector3 pos = flock[i].activeBody.transform.position;
                pos.x = i*spacing;
                flock[i].activeBody.transform.position = pos;
                flock[i].activeBrain = flock[i].activeBody.GetComponent<Brain>();
                flock[i].activeBrain.generation = currentGen;
                flock[i].activeBrain.initialize();
              
                if(bestIndex != i){
                flock[i].activeBrain.mutate(Random.Range(n/4f, n/2f));
                    PrefabUtility.CreatePrefab("Assets/Resources/run_temp/"+i+".prefab", flock[i].activeBody);
                }
                i++;
            }
        //}
    }

    void save(int index){
        GameObject obj = (GameObject)Instantiate((GameObject)Resources.Load("run_temp/"+index+"", typeof(GameObject)));
        PrefabUtility.CreatePrefab("Assets/Resources/run_saved/"+ ancestor.name + "@Gen_" + currentGen + ".prefab", obj);
        Destroy(obj);
    }
}