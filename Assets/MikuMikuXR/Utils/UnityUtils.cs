using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MikuMikuXR.Utils
{
    public static class UnityUtils
    {
        public static void DelayCall(MonoBehaviour behaviour, float time, Delegates.Runnable runnable)
        {
            behaviour.StartCoroutine(DoDelayCall(time, runnable));
        }

        private static IEnumerator DoDelayCall(float time, Delegates.Runnable runnable)
        {
            yield return new WaitForSeconds(time);
            runnable();
        }

        public static void ResetTransform(Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        public static List<float> Vector3ToList(Vector3 v)
        {
            return new List<float> {v.x, v.y, v.z};
        }

        public static List<float> QuaternionToList(Quaternion q)
        {
            return new List<float> {q.x, q.y, q.z, q.w};
        }

        public static Vector3 ListToVector3(IList<float> list)
        {
            return new Vector3(list[0], list[1], list[2]);
        }

        public static Quaternion ListToQuaternion(IList<float> list)
        {
            return new Quaternion(list[0], list[1], list[2], list[3]);
        }
    }
}