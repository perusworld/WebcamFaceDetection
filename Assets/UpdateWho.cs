using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CognitiveServices;

public class UpdateWho : TextMeshUpdate {

    public void OnIdentificationMappings(List<Face> faces)
    {
        Debug.Log(string.Format("Got face id mappings {0}", faces.Count));
        if (0 < faces.Count)
        {
            txt.text = faces[0].who;
        }
    }

}
