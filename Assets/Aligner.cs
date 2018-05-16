using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aligner : MonoBehaviour {

    public GameObject WebcamFeed;
    public GameObject UserInfo;
    public GameObject StatusHolder;

	// Use this for initialization
	void Start () {
        Debug.Log(string.Format("{0},{1}", Screen.width, Screen.height));
        Camera camera = Camera.main;
        Vector3 pos = camera.WorldToScreenPoint(WebcamFeed.transform.position);
        LogPos(WebcamFeed.transform.position);
        LogPos(pos);
        Vector3 newPos = camera.ScreenToWorldPoint(new Vector3(0,0, pos.z));
        Debug.Log(string.Format("{0},{1},{2}", newPos.x, newPos.y, newPos.z));
    }

    private void LogPos(Vector3 pos)
    {
        Debug.Log(string.Format("{0},{1},{2}", pos.x, pos.y, pos.z));
    }

    // Update is called once per frame
    void Update () {
		
	}
}
