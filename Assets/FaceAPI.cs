using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace CognitiveServices
{

    public class FaceAPI
    {

        public MonoBehaviour parent;
        public string faceAPIKey;
        public string faceAPIEndPoint = "https://westcentralus.api.cognitive.microsoft.com/face/v1.0";
        public string faceDetectParams = "returnFaceId=true&returnFaceLandmarks=false&returnFaceAttributes=age,gender,headPose,smile,facialHair,glasses,emotion,hair,makeup,occlusion,accessories,blur,exposure,noise";
        public string personGroupId;
        public float rateLimit;

        private void Log(string status)
        {
            Debug.Log(status);
            parent.BroadcastMessage("OnUpdateStatus", status);
        }
        public void DetectFaces(byte[] image)
        {
            parent.StartCoroutine(ProcessImageDetection(image));
        }
        public void IdentifyFaces(byte[] image, Action<bool> callback = null)
        {
            parent.StartCoroutine(ProcessImageDetection(image, (faces, ex) =>
            {
                if (null == ex)
                {
                    if (0 < faces.Count)
                    {
                        parent.StartCoroutine(ProcessImageIdentification(faces, (resps, exi) =>
                        {
                            if (null == exi)
                            {
                                if (0 < resps.Length)
                                {
                                    parent.StartCoroutine(ProcessIdentificationMappings(resps, faces, (done) =>
                                    {
                                        if (null != callback)
                                        {
                                            callback(done);
                                        }
                                    }));
                                }
                                else
                                {
                                    Log("No Face IDs");
                                    if (null != callback)
                                    {
                                        callback(false);
                                    }
                                }
                            }
                            else
                            {
                                Log("Error");
                                Debug.Log(exi.Message);
                                if (null != callback)
                                {
                                    callback(false);
                                }
                            }
                        }));
                    }
                    else
                    {
                        Log("No Faces");
                        if (null != callback)
                        {
                            callback(false);
                        }
                    }
                }
                else
                {
                    Log("Error");
                    Debug.Log(ex.Message);
                    if (null != callback)
                    {
                        callback(false);
                    }
                }
            }));
        }

        private string getFaceDetectURL()
        {
            string url = faceAPIEndPoint;
            if (!url.EndsWith("/")) url += "/";
            url += "detect?";
            url += faceDetectParams;
            return url;
        }

        private string getFaceIdentifyURL()
        {
            string url = faceAPIEndPoint;
            if (!url.EndsWith("/")) url += "/";
            url += "identify";
            return url;
        }

        private string getFaceURL(string faceId, string personGroupId)
        {
            string url = faceAPIEndPoint;
            if (!url.EndsWith("/")) url += "/";
            url += string.Format("persongroups/{1}/persons/{0}", faceId, personGroupId);
            return url;
        }

        IEnumerator ProcessImageDetection(byte[] image, Action<List<Face>, Exception> callback = null)
        {
            Log("Analyzing");
            var headers = new Dictionary<string, string>()
        {
            { "Ocp-Apim-Subscription-Key", faceAPIKey},
            { "Content-Type", "application/octet-stream" }
        };

            WWW www = new WWW(getFaceDetectURL(), image, headers);
            yield return www;

            List<Face> faces = null;
            Exception ex = null;
            if (www.error != null && www.error != "")
            {
                Log("Error");
                Debug.Log(www.text);
                ex = new Exception(www.text);
            }
            else
            {
                try
                {
                    var wrapper = JsonUtility.FromJson<Detect>("{\"faces\":" + www.text + "}");
                    faces = new List<Face>();
                    foreach (var face in wrapper.faces)
                    {
                        faces.Add(new Face { faceId = face.faceId, emotion = GetEmotion(face) });
                    }
                    Log(string.Format("Found {0}", faces.Count));
                    if (null == callback)
                    {
                        if (0 < faces.Count)
                        {
                            parent.BroadcastMessage("OnEmotions", faces);
                        }
                    }
                }
                catch (Exception exi)
                {
                    ex = exi;
                    faces = null;
                    Log("Error");
                    Debug.Log(ex.Message);
                }
            }

            if (null != callback)
            {
                yield return new WaitForSeconds(rateLimit);
                callback(faces, ex);
            }
        }

        IEnumerator ProcessImageIdentification(List<Face> faces, Action<FaceIdentifyResponse[], Exception> callback = null)
        {
            Log("Identifying");
            var headers = new Dictionary<string, string>()
        {
            { "Ocp-Apim-Subscription-Key", faceAPIKey},
            { "Content-Type", "application/json" }
        };

            FaceIdentifyRequest req = new FaceIdentifyRequest { personGroupId = personGroupId, faceIds = faces.Select(face => face.faceId).ToArray() };
            byte[] data = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(req));
            WWW www = new WWW(getFaceIdentifyURL(), data, headers);
            yield return www;

            FaceIdentifyResponse[] responses = null;
            Exception ex = null;
            if (www.error != null && www.error != "")
            {
                Log("Error");
                Debug.Log(www.text);
                ex = new Exception(www.text);
            }
            else
            {
                try
                {
                    var wrapper = JsonUtility.FromJson<FaceIdentifyResponseWrapper>("{\"responses\":" + www.text + "}");
                    responses = wrapper.responses;
                }
                catch (Exception exi)
                {
                    ex = exi;
                    responses = null;
                    Log("Error");
                    Debug.Log(ex.Message);
                }
            }

            if (null != callback)
            {
                yield return new WaitForSeconds(rateLimit);
                callback(responses, ex);
            }
        }

        IEnumerator ProcessIdentificationMappings(FaceIdentifyResponse[] idResps, List<Face> faces, Action<bool> callback = null)
        {
            Log("Mappings");
            if (null == idResps || 0 == idResps.Length)
            {
                yield return new WaitForSeconds(rateLimit);
            }
            else
            {
                foreach (FaceIdentifyResponse resp in idResps)
                {
                    Face face = faces.Find(obj => obj.faceId == resp.faceId);
                    if (null == face)
                    {
                        Debug.Log(string.Format("Skipping {0}", resp.faceId));
                    }
                    else
                    {
                        Debug.Log(string.Format("Mapping {0}", resp.faceId));
                        yield return ProcessIdentificationMapping(resp, face);
                    }
                }
            }
            Log("Done Mappings");
            parent.BroadcastMessage("OnIdentificationMappings", faces);
            if (null != callback)
            {
                callback(true);
            }
        }

        IEnumerator ProcessIdentificationMapping(FaceIdentifyResponse idResp, Face face)
        {
            Log("Mapping");
            if (null == idResp.candidates || 0 == idResp.candidates.Length)
            {
                Log("Empty");
            }
            else
            {
                var headers = new Dictionary<string, string>()
        {
            { "Ocp-Apim-Subscription-Key", faceAPIKey},
            { "Content-Type", "application/json" }
        };
                face.personId = idResp.candidates[0].personId;
                WWW www = new WWW(getFaceURL(face.personId, personGroupId), null, headers);
                yield return www;

                if (www.error != null && www.error != "")
                {
                    Log("Error");
                    Debug.Log(www.text);
                }
                else
                {
                    try
                    {
                        var wrapper = JsonUtility.FromJson<Person>(www.text);
                        face.who = wrapper.name;
                    }
                    catch (Exception ex)
                    {
                        Log("Error");
                        Debug.Log(ex.Message);
                    }
                }
            }
            yield return new WaitForSeconds(rateLimit);
        }

        public string GetEmotion(FaceResponse face)
        {
            string ret = "Unknown";
            if (null != face && null != face.faceAttributes && null != face.faceAttributes.emotion)
            {
                ret = face.faceAttributes.emotion.GetLikely();
            }
            return ret;
        }

    }
}