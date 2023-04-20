using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CannonRotation : MonoBehaviour
{
    public float RotationFactor { set => rotationFactor = value; get => rotationFactor; }

    [SerializeField] private float rotationFactor;
    [SerializeField] private Transform objectToRotate;
    [SerializeField] private bool rotationVertical;

    private Vector3 mouseCurrentPosition, mouseLastPosition, mouseDeltaPosition;

    private bool isMobile;
    private bool clickedOnUI;

    private MeshRenderer meshRenderer;
    [SerializeField] private Material materialWood, materialWoodRot;

    private void Start()
    {
        clickedOnUI = false;
        isMobile = Touchscreen.current != null && Application.isMobilePlatform;
        meshRenderer= GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        if (!GameManager.Instance.IsGamePlaying()) return;

        mouseCurrentPosition = Input.mousePosition;
        if (rotationVertical)
        {
            var verticalMove = Input.GetAxis("Vertical");

            RotacionVertical(verticalMove * 2);
        }
        else
        {
            var horizontalMove = Input.GetAxis("Horizontal");
            RotacionHorizontal(horizontalMove * 2);
        }
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject()) { clickedOnUI = true; }
        else { 
            clickedOnUI= false;
            meshRenderer.material= materialWoodRot;
        }
    }
    private void OnMouseUp()
    {
        meshRenderer.material = materialWood;
    }

    private void OnMouseDrag()
    {
        if (!GameManager.Instance.IsGamePlaying()) return;

        if (EventSystem.current.IsPointerOverGameObject() ||clickedOnUI)
        {
            return;
        }

        mouseLastPosition = Input.mousePosition;
        float deltaPosition;

        if (rotationVertical)
        {
            //deltaPosition = Input.GetTouch(0).deltaPosition.y;
            if (isMobile)
            {
                deltaPosition = UnityEngine.InputSystem.Touchscreen.current.delta.ReadValue().y;
            }
            else
            {
                //deltaPosition = Mouse.current.delta.ReadValue().y;
                mouseDeltaPosition = mouseCurrentPosition - mouseLastPosition;
                deltaPosition = mouseDeltaPosition.y;
            }

            RotacionVertical(deltaPosition);

        }
        else
        {
            if (isMobile)
            {
                deltaPosition = Touchscreen.current.delta.ReadValue().x;
            }
            else
            {
                //deltaPosition = Mouse.current.delta.ReadValue().x;
                mouseDeltaPosition = mouseCurrentPosition - mouseLastPosition;
                deltaPosition = mouseDeltaPosition.x;
            }

            //objectToRotate.Rotate(0, deltaPosition * rotationFactor, 0);
            RotacionHorizontal(deltaPosition);
        }
    }

    public void RotacionHorizontal(float deltaPosition)
    {
        objectToRotate.Rotate(0, deltaPosition * rotationFactor, 0);
    }

    public void RotacionVertical(float deltaPosition)
    {
        float ang = objectToRotate.transform.rotation.eulerAngles.z + 180;
        ang = ang > 360 ? ang - 360 : ang; //deltaPosition > 0 aumenta ang, 100<ang<175
        float angResultante = ang + deltaPosition * rotationFactor;
        angResultante = angResultante > 360 ? angResultante - 360 : angResultante < 0 ? angResultante + 360 : angResultante;

        if ((deltaPosition > 0 && ang < 180) ||
            (deltaPosition < 0 && ang > 99))
        {
            if (angResultante < 180 && angResultante > 99)
            {
                objectToRotate.Rotate(0, 0, deltaPosition * rotationFactor);
            }
            else
            {
                angResultante = Mathf.Clamp(angResultante, 99, 180) - ang;
                objectToRotate.Rotate(0, 0, angResultante);
            }
        }
    }
}
