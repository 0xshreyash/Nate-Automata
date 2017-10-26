using UnityEngine;
using System.Collections;

public class PlayerHealthIndicator : MonoBehaviour {

    // Use this for initialization
    public Color HealthTwoLeft;
    public Color HealthOneLeft;

    public Renderer RenderComponent;
	
	void Update () {

        int h = GetComponent<DestroyOnTriggerEnter>().Health;
	    Debug.Log(h);

	    if (h == 2) {
		   //RenderComponent.material.SetColor(HealthTwoLeft);
		   Debug.Log("Should be setting color for when health is two");
           RenderComponent.material.color = HealthTwoLeft;    
	    }

		if (h == 1) {
			RenderComponent.material.color = HealthOneLeft;
		}
            

    }
}
