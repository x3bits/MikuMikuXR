using System.Collections;
using TinyTeam.UI;
using UnityEngine;

namespace MikuMikuXR.Utils
{
    public static class TtuiUtils
    {
        public static T GetPage<T>() where T : TTUIPage
        {
            var t = typeof(T);
            var pageName = t.ToString();
            TTUIPage val;
            TTUIPage.allPages.TryGetValue(pageName, out val);
            return (T) val;
        }

        public static void RunWithLoadingUI<T>(MonoBehaviour behaviour, Delegates.Runnable runnable)
            where T : TTUIPage, new()
        {
            behaviour.StartCoroutine(DoRunWithLoadingUI<T>(runnable));
        }

        public static IEnumerator DoRunWithLoadingUI<T>(Delegates.Runnable runnable) where T : TTUIPage, new()
        {
            TTUIPage.ShowPage<T>();
            yield return null;
            runnable();
            TTUIPage.ClosePage<T>();
            yield return null;
        }

        public static void ShowPageAfterLoadingUI<T>(MonoBehaviour behaviour, object pageData) where T : TTUIPage, new()
        {
            behaviour.StartCoroutine(DelayShowPage<T>(1, pageData));
        }

        public static IEnumerator DelayShowPage<T>(int delayFrames, object pageData) where T : TTUIPage, new()
        {
            for (var i = 0; i < delayFrames; i++)
            {
                yield return null;
            }
            TTUIPage.ShowPage<T>(pageData);
        }
    }
}