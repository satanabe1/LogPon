using UnityEngine;
using System.Collections;
using LogPon;

public class FrameCounter : MonoBehaviour
{
    IEnumerator Start ()
    {
        while (true) {
            Debug.Log (Time.frameCount);
            yield return new WaitForSeconds (2f);
        }
    }
}
