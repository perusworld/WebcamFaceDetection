using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CognitiveServices;

public class FaceDetecter : RateLimit
{

    public string faceAPIKey = "--face-api-key-here--";
    public string faceAPIEndPoint = "https://westcentralus.api.cognitive.microsoft.com/face/v1.0";
    public string personGroupId = "--person-group-id-here--";

    byte[] bytes;
    private FaceAPI faceAPI;

    // Use this for initialization
    void Start () {
        faceAPI = new FaceAPI();
        faceAPI.parent = this;
        faceAPI.faceAPIEndPoint = faceAPIEndPoint;
        faceAPI.faceAPIKey = faceAPIKey;
        faceAPI.personGroupId = personGroupId;
        faceAPI.rateLimit = Limit;
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void ProcessImage(Texture2D image)
    {
        if (null != image)
        {
            bytes = image.EncodeToJPG();
            DoRateLimitedOperation();
        }
    }

    public override void DoOperation()
    {
        faceAPI.IdentifyFaces(bytes, (done) =>
        {
            OnOperationDone();
        });
    }

}
