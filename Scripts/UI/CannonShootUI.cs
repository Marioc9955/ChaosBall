using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CannonShootUI : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private Image cannonImage;
    [SerializeField] private Image ballImage;

    [SerializeField] private Transform puntaCannonUI, bottomCannonUI;

    private CannonShoot cannonShoot;

    private void Start()
    {
        cannonShoot=CannonShoot.Instance;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (cannonShoot.IsCannonCargado())
        {
            float fireInputUIValue = VirtualJoystick.InverseLerp(puntaCannonUI.position, bottomCannonUI.position, eventData.position);
            if (fireInputUIValue > 0 && fireInputUIValue <= 1)
            {
                UpdateCannonUI(fireInputUIValue);
                cannonShoot.UpdateTransparencyMaterialTrans(fireInputUIValue);
                cannonShoot.UpdateProjectilePositionAndDirShoot(cannonShoot.LerpCannonTipToBottom(fireInputUIValue));
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (cannonShoot.CanShoot())
        {
            float fireInputUIValue = VirtualJoystick.InverseLerp(puntaCannonUI.position, bottomCannonUI.position, eventData.position);

            CargarCannonUI(fireInputUIValue);

            cannonShoot.CargarCannon(cannonShoot.LerpCannonTipToBottom(fireInputUIValue));
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        cannonShoot.ShootCannon();
    }

    public void CargarCannonUI(float fireInputUIValue)
    {
        ballImage.enabled = true;
        cannonImage.color = Color.HSVToRGB(0, 0, 1);

        float yBall = Mathf.Lerp(111, 0, fireInputUIValue);
        ballImage.transform.localPosition = new Vector3(ballImage.transform.localPosition.x, yBall);
    }

    public void UpdateCannonUI(float fireInputUIValue)
    {
        cannonImage.color = Color.HSVToRGB(0, fireInputUIValue, 1);
        float yBall = Mathf.Lerp(111, 0, fireInputUIValue);
        ballImage.transform.localPosition = new Vector3(ballImage.transform.localPosition.x, yBall);
    }
}
