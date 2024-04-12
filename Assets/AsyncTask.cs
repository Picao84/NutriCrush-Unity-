using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Utils
{
    public static class AsyncTask
    {
        public static async Task Await(double miliseconds, Action action = null)
        {
            float startTime = Time.realtimeSinceStartup;
            double delay = miliseconds / 1000f;

            while (Time.realtimeSinceStartup - startTime < delay)
            {
                await Task.Yield();
            }
        }
    }
}
