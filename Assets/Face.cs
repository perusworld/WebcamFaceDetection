using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace CognitiveServices
{

    [Serializable]
    public class Detect
    {
        public FaceResponse[] faces;
    }

    [Serializable]
    public class Face
    {
        public string faceId;
        public string emotion;
        public string personId;
        public string who;
    }


    [Serializable]
    public class FaceResponse
    {
        public string faceId;
        public FaceRectangle faceRectangle;
        public FaceAttributes faceAttributes;
    }

    [Serializable]
    public class FaceRectangle
    {
        public int height;
        public int left;
        public int top;
        public int width;
    }

    [Serializable]
    public class FaceAttributes
    {
        public Emotion emotion;
    }

    [Serializable]
    public class Emotion
    {
        public double anger;
        public double contempt;
        public double disgust;
        public double fear;
        public double happiness;
        public double neutral;
        public double sadness;
        public double surprise;

        public Dictionary<string, double> AsDictionary()
        {
            Dictionary<string, double> ret = new Dictionary<string, double>();
            ret.Add("anger", anger);
            ret.Add("contempt", contempt);
            ret.Add("disgust", disgust);
            ret.Add("fear", fear);
            ret.Add("happiness", happiness);
            ret.Add("neutral", neutral);
            ret.Add("sadness", sadness);
            ret.Add("surprise", surprise);
            return ret;
        }
        public string GetLikely()
        {
            var res = from entry in AsDictionary() orderby entry.Value descending select entry.Key;
            return res.First<string>();
        }
    }

    [Serializable]
    public class FaceIdentifyRequest
    {
        public string personGroupId;
        public string[] faceIds;
    }

    [Serializable]
    public class FaceIdentifyResponseWrapper
    {
        public FaceIdentifyResponse[] responses;
    }

    [Serializable]
    public class FaceIdentifyResponse
    {
        public string faceId;
        public FaceCandidate[] candidates;
    }

    [Serializable]
    public class FaceCandidate
    {
        public string personId;
        public float confidence;
    }

    [Serializable]
    public class Person
    {
        public string personId;
        public string[] persistedFaceIds;
        public string name;
        public string userData;
    }
}