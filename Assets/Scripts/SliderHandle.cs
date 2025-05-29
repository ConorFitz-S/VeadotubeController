using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SliderHandle : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    //public Transform HandleTransform;
    public TextMeshProUGUI ValueText;
    public int StateValue;
    
    

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Logic for when dragging starts
        //Debug.Log("Dragging started");
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Logic for dragging the handle
        Vector3 delta = new Vector3(eventData.delta.x, 0,0);
        Vector3 newPosition = transform.position + delta;
        transform.position = newPosition;
        
        // Clamp position within slider bounds
        Vector3 clampedPosition = new Vector3(
            Mathf.Clamp(transform.position.x, SliderBody.Instance.GetMinPosition().x, SliderBody.Instance.GetMaxPosition().x),
            transform.position.y,
            transform.position.z);
        
        transform.position = clampedPosition;
        ValueText.text = GetValue().ToString();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Logic for when dragging ends
        //Debug.Log("Dragging ended");
    }

    public void SetValue(string value)
    {
        StateValue = int.Parse(value);
    }

    public int GetValue()
    {
        //Get value from the slider handle position based on slider body min and max values
        float minValue = SliderBody.Instance.minValue;
        float maxValue = SliderBody.Instance.maxValue;
        float range = maxValue - minValue;
        float normalizedValue = (transform.position.x - SliderBody.Instance.GetMinPosition().x) / (SliderBody.Instance.GetMaxPosition().x - SliderBody.Instance.GetMinPosition().x);
        return Mathf.RoundToInt(minValue + normalizedValue * range);
        
    }
}