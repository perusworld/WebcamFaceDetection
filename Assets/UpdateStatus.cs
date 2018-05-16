using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateStatus : TextMeshUpdate {

    public void OnUpdateStatus(string msg)
    {
        if (null != msg)
        {
            txt.text = msg;
        }
    }

}
