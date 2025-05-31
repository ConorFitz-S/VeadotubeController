using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SliderHandle : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    //public Transform HandleTransform;
    public TextMeshProUGUI ValueText;
    public TMP_InputField NameInput;
    public int StateValue;
    
    public string StateName;

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
        int value = GetValue();
        ValueText.text = value.ToString();
        StateValue = value;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        SliderBody.Instance.SortHandlesByValue();
        VaedoInstance.Instance.AssessState(true);
        Debug.Log("Current index: " + SliderBody.Instance.GetCurrentIndex(StateValue));
        Debug.Log("VeadoInstance Index: " + VaedoInstance.Instance.currentStateIndex);
        
        // Logic for when dragging ends
        //Debug.Log("Dragging ended");
    }

    public void SetName(string value)
    {
        StateName = value;
        VaedoInstance.Instance.AssessState(true);
    }

    public void ShowName(string value)
    {
        NameInput.text = value;
        StateName = value;
        
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
    
    public void SetPosition(float value)
    {
        // Set the position of the handle based on the value
        float minX = SliderBody.Instance.GetMinPosition().x;
        float maxX = SliderBody.Instance.GetMaxPosition().x;
        float normalizedValue = (value - SliderBody.Instance.minValue) / (SliderBody.Instance.maxValue - SliderBody.Instance.minValue);
        float newX = Mathf.Lerp(minX, maxX, normalizedValue);
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        
        // Update the value text
        ValueText.text = value.ToString();
        StateValue = Mathf.RoundToInt(value);
    }

    public void Delete()
    {
        SliderBody.Instance.DeleteHandle(this);
    }
}