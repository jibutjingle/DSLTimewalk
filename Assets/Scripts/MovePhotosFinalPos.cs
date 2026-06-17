using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePhotosFinalPos : MonoBehaviour
{
    [SerializeField]
    private GameObject photo1;
    [SerializeField] private GameObject photo2;
    [SerializeField] private GameObject photo3;
    [SerializeField] private GameObject photo4;

    public Transform position1;
    public Transform position2;
    public Transform position3;
    public Transform position4;

    public float speed = 1.0f;
    private float t = 0f;

    private Vector3 startPosition1;
    private Vector3 startPosition2;
    private Vector3 startPosition3;
    private Vector3 startPosition4;

    private bool timetomove = false;

    public void MovePhotosToFinalPos()
    {
        timetomove = true;
    }

    private void Update()
    {
        if (timetomove) {
            t += Time.deltaTime * speed;

            startPosition1 = photo1.transform.position;
            startPosition2 = photo2.transform.position;
            startPosition3 = photo3.transform.position;
            startPosition4 = photo4.transform.position;

            photo1.transform.position = Vector3.Slerp(startPosition1, position1.position, t);
            photo2.transform.position = Vector3.Slerp(startPosition2, position2.position, t);
            photo3.transform.position = Vector3.Slerp(startPosition3, position3.position, t);
            photo4.transform.position = Vector3.Slerp(startPosition4, position4.position, t);

        }
        
    }
}
