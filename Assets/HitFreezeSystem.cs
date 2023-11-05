using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitFreezeSystem : MonoBehaviour
{
    // Start is called before the first frame update
    private const float HITFREEZE_DURATION_IN_SECONDS = 0.2f;
    private const float TIMESCALE_FACTOR = 0.1f;
    private float _pendingFreeze = 0f;
    
    
    private bool _isFrozen = false;

    private void Update()
    {
        if (_pendingFreeze > 0f && !_isFrozen)
        {
            StartCoroutine(DoFreeze());
        }
    }

    public void Freeze()
    {
        _pendingFreeze = HITFREEZE_DURATION_IN_SECONDS;
    }


    IEnumerator DoFreeze()
    {
        _isFrozen = true;
        var orginalTime = Time.timeScale;
        Time.timeScale = TIMESCALE_FACTOR;

        yield return new WaitForSecondsRealtime(HITFREEZE_DURATION_IN_SECONDS);

        Time.timeScale = orginalTime;
        _pendingFreeze = 0f;
        _isFrozen = false;
    }
}
