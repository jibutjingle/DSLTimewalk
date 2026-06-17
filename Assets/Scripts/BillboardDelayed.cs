using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardDelayed : MonoBehaviour
{
    [SerializeField] bool m_WorldUp;
    [SerializeField] bool m_FlipForward;

    private Transform m_Camera;

    private void Start()
    {
        StartCoroutine(FindCameraWithDelay());
    }
    private IEnumerator FindCameraWithDelay()
    {
        // Keep trying until Camera.main exists (XR Rig has spawned)
        while (Camera.main == null)
        {
            yield return null; // wait a frame
        }

       m_Camera = Camera.main.transform;
    }

    void LateUpdate()
    {
        if (m_Camera != null)
        {
            //transform.LookAt(m_Camera);
            Quaternion lookRot = Quaternion.LookRotation(m_Camera.transform.position - transform.position);

            if (m_WorldUp)
            {
                Vector3 offset = lookRot.eulerAngles;
                offset.x = 0;
                offset.z = 0;

                if (m_FlipForward)
                    offset.y += 180;

                lookRot = Quaternion.Euler(offset);
            }

            transform.rotation = lookRot;
        }
    }
}
