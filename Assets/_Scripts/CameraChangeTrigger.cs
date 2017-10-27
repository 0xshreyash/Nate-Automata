using UnityEngine;
using System.Collections;

public class CameraChangeTrigger : MonoBehaviour {

    //unfinished
    public static int TriggerCount = 0;
    public static int ActiveTriggerIndex = 0;
    public int TriggerIndex = 0;

    public float NewYValue;

    public bool IsHideMeshRenderer = true;

	// Use this for initialization
	void Start () {
        TriggerCount++;
        TriggerIndex = TriggerCount;
    }
	
	// Update is called once per frame
	void Update () {
        if(IsHideMeshRenderer)
        {
            GetComponent<MeshRenderer>().enabled = false;
        }
	} 

    public void OnTriggerEnter(Collider colliderObject)
    {
        if (!colliderObject.CompareTag("Player"))
            return;

        if (CanActivate())
        {
            var c = Camera.main.GetComponent<TestCam>();
            c.SetZoomOffset(NewYValue);
            ActiveTriggerIndex = TriggerIndex;
        }
    }

    public bool CanActivate()
    {
        return ActiveTriggerIndex != TriggerIndex;
    }
}
