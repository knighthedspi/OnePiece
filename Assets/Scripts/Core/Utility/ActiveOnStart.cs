using UnityEngine;
using System.Collections;

public class ActiveOnStart : MonoBehaviour {

    public GameObject[] targets;

    void Start() {
        foreach (GameObject o in targets) o.SetActive(true);
    }
}
