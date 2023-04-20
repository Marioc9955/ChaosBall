using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set;}

    [SerializeField] private CinemachineVirtualCamera camCannon, camFollowProjectile;
    private CinemachineTransposer ctCamC, ctCamP;

    private bool followCam = false;

    [SerializeField] private float speedMove;

    private bool followingProjectile = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        followCam = false;
        followingProjectile= false;

        ctCamC = camCannon.GetCinemachineComponent<CinemachineTransposer>();
        ctCamP = camFollowProjectile.GetCinemachineComponent<CinemachineTransposer>();
    }

    public void ToggleFollowCam()
    {
        followCam = !followCam;
    }

    public void ActivateCamFollowProyectil(Transform proyectil)
    {
        camFollowProjectile.Follow = proyectil;
        camFollowProjectile.LookAt = proyectil;
        followingProjectile= true;
        camFollowProjectile.Priority += 10;
        CinemachineTransposer ct = camFollowProjectile.GetCinemachineComponent<CinemachineTransposer>();
        ct.m_FollowOffset = camCannon.transform.position - PlayerLifeController.Instance.transform.position;
    }

    public void BackToCannon()
    {
        camFollowProjectile.Priority = 9;
        followingProjectile= false;
    }

    public bool IsFollowCamActive()
    {
        return followCam;
    }

    public void MoverCamera(Vector2 direccion)
    {
        if (followingProjectile)
        {
            MoveTransposer(ctCamP, direccion);
        }
        else
        {
            MoveTransposer(ctCamC, direccion);
        }
    }

    private void MoveTransposer(CinemachineTransposer ct, Vector2 direccion)
    {
        direccion *= speedMove * Time.deltaTime;
        Vector2 fo = ct.m_FollowOffset;
        if ((direccion.y > 0 && fo.y < 27) || (direccion.y < 0 && fo.y > 5))
        {
            ct.m_FollowOffset += new Vector3(0, direccion.y);
        }
        if ((direccion.x > 0 && fo.x > -21) || (direccion.x < 0 && fo.x < 21))
        {
            ct.m_FollowOffset -= new Vector3(direccion.x, 0);
        }
    }
}
