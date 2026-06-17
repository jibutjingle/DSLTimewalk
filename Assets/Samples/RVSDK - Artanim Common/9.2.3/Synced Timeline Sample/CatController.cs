using UnityEngine;

/// <summary>
/// Used to trigger jumps on obstacles. This is not network synced to be able to verify the synchronization
/// of the timlines between the components.
/// </summary>

namespace RvSdk.Samples
{
    [RequireComponent(typeof(Animator))]
    public class CatController : MonoBehaviour
    {
        private const string TRIGGER_JUMP = "Jump";

        private Animator Animator;

        void Start()
        {
            Animator = GetComponent<Animator>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Obstacle>() != null)
                Animator.SetTrigger(TRIGGER_JUMP);
        }
    }
}
