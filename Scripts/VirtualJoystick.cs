using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [System.Serializable]
    public class EventV2 : UnityEvent<Vector2> { }

    public EventV2 outputEventVector;

    [SerializeField]
    private RectTransform interior, exterior;

    private RectTransform baseRect = null;

    private bool pointerHold = false;

    private Vector2 direccionJoystick;

    private void Start()
    {
        baseRect = GetComponent<RectTransform>();
        direccionJoystick = Vector2.zero;
    }

    private void LateUpdate()
    {
        if (pointerHold)
        {
            outputEventVector.Invoke(direccionJoystick);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pointerHold = true;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(baseRect, eventData.position, eventData.pressEventCamera, out Vector2 posicion);
        UpdateRectPosition(exterior, posicion);
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(exterior, eventData.position, eventData.pressEventCamera, out Vector2 posicion);
        posicion = Vector2.ClampMagnitude(posicion, exterior.sizeDelta.magnitude);
        UpdateRectPosition(interior, posicion);
        float inverseLerp = InverseLerp(Vector2.zero, posicion.normalized * exterior.sizeDelta.magnitude, posicion);
        //print(inverseLerp);
        //print(posicion.normalized);
        direccionJoystick = Vector2.Lerp(Vector2.zero, posicion.normalized, inverseLerp);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pointerHold = false;
        UpdateRectPosition(interior, Vector2.zero);
        outputEventVector.Invoke(Vector2.zero);
    }

    private void UpdateRectPosition(RectTransform rect, Vector2 newPosition)
    {
        rect.anchoredPosition = newPosition;
    }

    public static float InverseLerp(Vector3 a, Vector3 b, Vector3 value)
    {
        Vector3 AB = b - a;
        Vector3 AV = value - a;
        float c = Vector3.Dot(AB, AB);
        if (c==0)
        {
            return 0;
        }
        return Vector3.Dot(AV, AB) / c;
    }
}
