using UnityEngine;
using UnityEditor;
using System.Collections;

namespace LogPon
{
    /// <summary>
    /// Unity起動時にLogEventDistributorと、LogPonAdapterとLogPonWindowを繋ぐ
    /// </summary>
    [InitializeOnLoad]
    class LogPonRunner
    {
        static LogPonRunner ()
        {
            LogPonAdapter.OnRequireRepaint = LogPonWindow.RequireRepaint;
            LogEventDistributor.AddLogCallback (LogPonAdapter.AddLog);
            LogEventDistributor.Init (false);
        }
    }
}
