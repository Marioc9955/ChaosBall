using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MoveCamera : MonoBehaviour
{
    public CinemachineVirtualCamera cam;
    CinemachineTransposer ct;

    [SerializeField]
    private float speed;

    private void Start()
    {
        ct = cam.GetCinemachineComponent<CinemachineTransposer>();
    }

    public void MoverCamera(Vector2 direccion)
    {
        direccion *= speed * Time.deltaTime;
        Vector2 fo = ct.m_FollowOffset;
        if ((direccion.y > 0 && fo.y < 21) || (direccion.y < 0 && fo.y > 3))
        {
            ct.m_FollowOffset += new Vector3(0, direccion.y);
        }
        if ((direccion.x > 0 && fo.x > -21) || (direccion.x < 0 && fo.x < 21 ))
        {
            ct.m_FollowOffset -= new Vector3(direccion.x, 0);
        }
        //ct.m_FollowOffset += new Vector3(direccion.x, direccion.y);
    }
}
