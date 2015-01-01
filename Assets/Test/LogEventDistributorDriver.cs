using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LogPon;

namespace Test
{
    public class LogEventDistributorDriver : MonoBehaviour
    {
        public List<LogEntry> entries1;
        public List<LogEntry> entries2;

        IEnumerator Start ()
        {
            LogEventDistributor.Init (false);
            LogEventDistributor.AddLogCallback (OnLog);
            LogEventDistributor.AddLogCallback (OnLog2);
//            LogEventDistributor.AddLogCallback (entries.Add);
            LogEventDistributor.AddLogCallback ((condition, stackTrace, type) => {
            });
            var logger = gameObject.AddComponent<DummyLogger> ();
            LogEventDistributor.AddLogCallback (logger.OnLog);
            LogEventDistributor.AddLogCallback (logger.OnLog2);
            Destroy (logger);
            Debug.Log ("hoge");
            Debug.LogWarning ("moge");
            Debug.LogError ("piyo");
            yield break;
        }

        public void OnLog (string condition, string stackTrace, LogType logType)
        {
            entries1.Add (new LogEntry (condition, stackTrace, logType));
        }

        public void OnLog2 (LogPon.LogEntry entry)
        {
            entries2.Add (entry);
        }
    }

    class DummyLogger : MonoBehaviour
    {
        public int count = 0;
        public List<LogEntry> entries3 = new List<LogEntry> ();
        public List<LogEntry> entries4 = new List<LogEntry> ();

        public void OnLog (string condition, string stackTrace, LogType logType)
        {
            entries3.Add (new LogEntry (condition, stackTrace, logType));
        }

        public void OnLog2 (LogPon.LogEntry entry)
        {
            count++;
            this.entries4.Add (entry);
        }
    }
}
