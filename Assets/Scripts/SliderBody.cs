using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SliderBody : MonoBehaviourSingleton<SliderBody>
{
    public Transform MaxPosition;
    public Transform MinPosition;

    public float minValue = 0f;
    public float maxValue = 100f;
    
    public SliderHandle handlePrefab; // Prefab for the handle
    
    public List<SliderHandle> handles = new List<SliderHandle>(); //

    public Transform handleParent;
    
    int currentHandleIndex = 0; // Index to keep track of the current handle
    
    public TMP_InputField minValueInput;
    public TMP_InputField maxValueInput;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public int GetCurrentIndex(int value)
    {
        int index = 0;
        for(int i = 0; i < handles.Count; i++)
        {
            if (value >= handles[i].StateValue)
            {
                index = i;
            }
        }

        return index;
    }
    
    public void SortHandlesByValue()
    {
        handles.Sort((a, b) => a.StateValue.CompareTo(b.StateValue));
        Debug.Log("Handles sorted by value");
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
    
    public void CreateHandle()
    {
        Vector3 position = (GetMinPosition() + GetMaxPosition())/2;
        SliderHandle newHandle = Instantiate(handlePrefab, position, Quaternion.identity, handleParent);
        handles.Add(newHandle);
        newHandle.transform.localPosition = new Vector3(0, -50, 0);
        //.transform.SetParent(transform); // Set the parent to this handler for organization
    }

    public void SetMinValueInput(float value)
    {
        minValueInput.SetTextWithoutNotify(value.ToString());
    }

    public void SetMaxValueInput(float value)
    {
        maxValueInput.SetTextWithoutNotify(value.ToString());
    }

    public void DeleteHandle(SliderHandle handle)
    {
        if (handles.Contains(handle))
        {
            handles.Remove(handle);
            Destroy(handle.gameObject);

            SortHandlesByValue();
            VaedoInstance.Instance.AssessState(true);
        }
    }


}
