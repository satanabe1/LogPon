using UnityEngine;
using System.Collections;
using LogPon;

public class LogPonDriver : MonoBehaviour
{
    IEnumerator Start ()
    {
        LogEventDistributor.Init (false);
        var isValid = LogEventDistributor.AddLogCallback (LogPonAdapter.AddLog);
        Debug.Log (isValid);

        while (true) {
            Debug.Log (Time.frameCount);
            yield return new WaitForSeconds (2f);
        }
    }
}
