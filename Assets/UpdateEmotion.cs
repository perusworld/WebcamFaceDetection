using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CognitiveServices;

public class UpdateEmotion : TextMeshUpdate {

    public void OnEmotions(List<Face> faces)
    {
        if (0 < faces.Count)
        {
            txt.text = faces[0].emotion;
        }
    }

    public void OnIdentificationMappings(List<Face> faces)
    {
        OnEmotions(faces);
    }
}
