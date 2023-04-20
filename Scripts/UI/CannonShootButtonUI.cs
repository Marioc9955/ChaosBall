using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CannonShootButtonUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    [SerializeField] private CannonShootUI shootUI;

    private CannonShoot cannonShoot;

    private float fireInputUIValue;

    private bool disparando;

    private void Start()
    {
        disparando= false;
        cannonShoot = CannonShoot.Instance;
        fireInputUIValue= 0f;
    }

    private void Update()
    {
        if (!disparando) { return; }

        if (fireInputUIValue<1 && cannonShoot.IsCannonCargado()) {
            shootUI.UpdateCannonUI(fireInputUIValue);
            fireInputUIValue += Time.deltaTime;
            cannonShoot.UpdateTransparencyMaterialTrans(fireInputUIValue);
            cannonShoot.UpdateProjectilePositionAndDirShoot(cannonShoot.LerpCannonTipToBottom(fireInputUIValue));
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (cannonShoot.CanShoot())
        {
            fireInputUIValue = 0f;
            disparando = true;
            shootUI.CargarCannonUI(fireInputUIValue);
            cannonShoot.CargarCannon(cannonShoot.LerpCannonTipToBottom(fireInputUIValue));
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        cannonShoot.ShootCannon();
        disparando= false;
    }
}
