using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Utils;

namespace Assets
{
    public static class CustomTimer
    {
        public static IEnumerator Timer(float seconds, Action callBackAction, bool runOnce = false)
        {
           

            var shouldContinue = true;

            while (shouldContinue)
            {
                shouldContinue = !runOnce;

                Task awaitTask = AsyncTask.Await(TimeSpan.FromSeconds(seconds).TotalMilliseconds);
                yield return new WaitUntil(() => awaitTask.IsCompleted);

                callBackAction();
            }

            yield break;
            
        }
    }
}
