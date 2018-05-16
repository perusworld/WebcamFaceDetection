using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WebCamOn : MonoBehaviour {

    public RawImage rawimage;

    private FaceDetecter faceDetecter;

    WebCamTexture webcam;

    // Use this for initialization
    void Start () {

        faceDetecter = gameObject.GetComponent<FaceDetecter>();

        webcam = new WebCamTexture();
        rawimage.texture = webcam;
        rawimage.material.mainTexture = webcam;
        webcam.Play();
    }
	
	// Update is called once per frame
	void Update () {
        Texture2D webcamImage = new Texture2D(webcam.width, webcam.height);
        webcamImage.SetPixels(webcam.GetPixels());
        webcamImage.Apply();
        faceDetecter.ProcessImage(webcamImage);
    }
}
