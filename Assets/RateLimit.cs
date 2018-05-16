using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RateLimit : MonoBehaviour {

    public float Limit = 3f;
    public bool EnableProcessing;

    float processedTimestamp;
    bool processing;

	public void DoRateLimitedOperation () {
        if (EnableProcessing)
        {
            if (processing || Limit > (Time.time - processedTimestamp))
            {
                //NOOP
            }
            else
            {
                Debug.Log("started processing");
                processing = true;
                try
                {
                    DoOperation();
                } catch (Exception ex)
                {
                    Debug.Log(ex.Message);
                    OnOperationDone();
                }
            }
        }

    }

    public virtual void DoOperation()
    {

    }

    public void OnOperationDone()
    {
        processing = false;
        processedTimestamp = Time.time;
        Debug.Log("on operation done");
    }

}
