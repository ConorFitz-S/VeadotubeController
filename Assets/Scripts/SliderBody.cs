using UnityEngine;

public class SliderBody : MonoBehaviourSingleton<SliderBody>
{
    public Transform MaxPosition;
    public Transform MinPosition;

    public float minValue = 0f;
    public float maxValue = 100f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public Vector3 GetMaxPosition()
    {
        return MaxPosition.position;
    }

    public Vector3 GetMinPosition()
    {
        return MinPosition.position;
    }
    
    public void SetMinValue(string value)
    {
        minValue = float.Parse(value);
    }
    
    public void SetMaxValue(string value)
    {
        maxValue = float.Parse(value);
    }
}
