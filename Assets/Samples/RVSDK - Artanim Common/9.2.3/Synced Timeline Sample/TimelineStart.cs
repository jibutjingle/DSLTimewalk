using Artanim;
using RvSdk.Component;
using RvSdk.Controller;
using System.Threading;
using UnityEngine.Playables;

namespace RvSdk.Samples
{
    public class TimelineStart : ServerSideBehaviour
    {
        public PlayableDirector ControlledTimeline;

        private void OnEnable()
        {
            if (GameController.Instance)
            {
                GameController.Instance.OnSceneLoadedInSession += Instance_OnSceneLoadedInSession;
            }
        }

        private void OnDisable()
        {
            if (GameController.Instance)
            {
                GameController.Instance.OnSceneLoadedInSession -= Instance_OnSceneLoadedInSession;
            }
        }

        private void Instance_OnSceneLoadedInSession(string[] sceneNames, bool sceneLoadTimedOut)
        {
            if (ControlledTimeline)
            {
                ControlledTimeline.Play();
            }
        }

        public void DoSleep()
        {
            Thread.Sleep(1000);
        }

    }
}
