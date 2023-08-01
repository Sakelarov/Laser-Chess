using System;
using System.Collections;
using UnityEngine;

namespace Utils
{
    public static class DelaysExtensions 
    {
        public static Coroutine InvokeAfterFrames(this MonoBehaviour mono, int framesCount, Action action)
        {
            return mono.StartCoroutine(ExecuteAfterFrames(framesCount, action));
        }

        private static IEnumerator ExecuteAfterFrames(int framesCount, Action action)
        {
            if (framesCount <= 0) framesCount = 1;

            for (int i = 0; i < framesCount; i++)
            {
                yield return new WaitForEndOfFrame();
            }
            
            action?.Invoke();
        }
    }
}
