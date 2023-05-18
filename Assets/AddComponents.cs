using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AddComponents : MonoBehaviour
{

    public int rayCount = 8;
    public float rayLength = 10.0f;

    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < rayCount; i++)
        {
            float angle = 360.0f / rayCount * i;
            Vector3 direction = Quaternion.AngleAxis(angle, Vector3.up) * transform.forward;
            Ray ray = new Ray(transform.position, direction);
            Debug.DrawRay(transform.position, direction * rayLength, Color.red);
        }
    }


    [ContextMenu("Add rigidbody to childs mass is mesh volume")]
    // Start is called before the first frame update
    void AddRigidBodyWithMassChilds()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            AddRigidBodyWithMassToAnObject(child);
        }
    }


    void AddRigidBodyWithMassToAnObject(GameObject obj)
    {
        float volume;
        VolumeCalculator vc;
        if (obj.TryGetComponent<VolumeCalculator>(out vc))
        {
            volume = vc.GetVolume();
        }
        else
        {
            vc = obj.AddComponent<VolumeCalculator>();
            volume = vc.GetVolume();
        }
        DestroyImmediate(vc);
        Rigidbody rb;
        if (!obj.TryGetComponent<Rigidbody>(out rb))
        {
            rb = obj.AddComponent<Rigidbody>();
        }
        rb.mass = volume;
    }

    [ContextMenu("Agregar rigidbody con masa como el volumen del mesh a este objecto")]
    public void AddRigidBodyWithMass()
    {
        AddRigidBodyWithMassToAnObject(gameObject);
    }
}
