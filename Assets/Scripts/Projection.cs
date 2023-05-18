using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projection : MonoBehaviour {

    [SerializeField] private LineRenderer _line;

    private Rigidbody projectileRigidbody;

    int numPuntos = 0;

    public void DrawTrajectoryOnShoot(ProyectilPlayer ball)
    {
        StopAllCoroutines();
        _line.positionCount = 0;
        numPuntos = 0;
        projectileRigidbody = ball.GetComponent<Rigidbody>();
        StartCoroutine(DrawingTrajectory(ball, .05f));
    }

    IEnumerator DrawingTrajectory(ProyectilPlayer ball, float timeBetweenPoints)
    {
        while (ball != null)
        {
            float sqrVel = projectileRigidbody.velocity.sqrMagnitude;
            yield return new WaitForSeconds(timeBetweenPoints);
            if (sqrVel>5 && ball.GetNumCol()<=0)
            {
                numPuntos++;
                
                _line.positionCount = numPuntos;
                if (ball == null)
                {
                    break;
                }
                _line.SetPosition(numPuntos - 1, ball.transform.position);
            }
            
        }
        
    }
}