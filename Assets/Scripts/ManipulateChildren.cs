using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManipulateChildren : MonoBehaviour
{
    public float offsetX, offsetY, offsetZ;
    public int numX, numY, numZ;
    public Vector3 posInicial;

    public float radioParaPosCircular, angInicial;
    
    public Vector3 scale;

    [ContextMenu("Colocar en cuadricula")]
    void ColocarEnCuadricula()
    {
        Vector3 offset = Vector3.zero;

        int x = numX - 1, y = numY - 1, z = numZ - 1;
        for (int i = 0; i < transform.childCount; i++)
        {
            Vector3 pos = posInicial + offset;
            transform.GetChild(i).position = pos;
            if (x > 0)
            {
                offset += new Vector3(offsetX, 0);
                print(x + " " + pos);
                x--;
            }
            else if (y > 0)
            {
                x = numX - 1;
                offset += new Vector3(0, offsetY);
                offset -= new Vector3(offsetX, 0) * x;
                y--;

            }
            else //if (z > 0) 
            {
                y = numY - 1;
                x = numX - 1;
                offset += new Vector3(0, 0, offsetZ);
                offset -= new Vector3(0, offsetY) * y;
                offset -= new Vector3(offsetX, 0) * x;
                z--;
            }
            //else
            //{
            //    offset += new Vector3(offsetX, offsetY, offsetZ);
            //}
        }
        
    }

    [ContextMenu("Pos inicial primer hijo")]
    void PosInicialFirstSib()
    {
        posInicial = transform.GetChild(0).position;
    }

    [ContextMenu("Posicion circular de hijos")]
    public void PosCircularHijos()
    {
        float ang = Mathf.Deg2Rad * angInicial;
        float angS = Mathf.Deg2Rad * (360f / transform.childCount);
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).position = transform.position + radioParaPosCircular * new Vector3(Mathf.Cos(ang), 0, Mathf.Sin(ang));
            ang += angS;
        }
    }

    [ContextMenu("Posicion circular de hijos XY")]
    public void PosCircularHijosXY()
    {
        float ang = Mathf.Deg2Rad * angInicial;
        float angS = Mathf.Deg2Rad * (360f / transform.childCount);
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).position = transform.position + radioParaPosCircular * new Vector3(Mathf.Cos(ang), Mathf.Sin(ang));
            ang += angS;
        }
    }

    [ContextMenu("Establecer scale para todos")]
    void SetScale()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).localScale = scale;
        }
    }

    [ContextMenu("Convertir en convexos todos los coliders de los hijos")]
    void ConvertMeshColliderToConvex()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            MeshCollider mc = transform.GetChild(i).GetComponent<MeshCollider>();
            if (!mc.convex)
            {
                mc.convex = true;
            }
        }
    }

    [ContextMenu("Mesh de mesh filter el mismo de mesh renderer")]
    void MeshFilterMeshOriginal()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Mesh m = transform.GetChild(i).GetComponent<MeshCollider>().sharedMesh;
            MeshFilter mf = transform.GetChild(i).GetComponent<MeshFilter>();
            mf.mesh = m;
        }
    }
}
