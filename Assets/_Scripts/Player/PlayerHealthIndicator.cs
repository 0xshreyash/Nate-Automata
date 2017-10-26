using UnityEngine;
using System.Collections;

public class PlayerHealthIndicator : MonoBehaviour {

    // Use this for initialization
    public Color HealthTwoLeft;
    public Color HealthOneLeft;

    public Renderer RenderComponent;
	public Texture healthTexture;


	private int h;
	private ArrayList playerHealthCircles;


	void Awake()
	{
		h = GetComponent<DestroyOnTriggerEnter>().Health;
		playerHealthCircles = new ArrayList();

		for (int x = 0; x < h; x++)
		{
			playerHealthCircles.Add(Utils.InstantiateSafe(ParticleSystemPrefab, position);)		}
		
	}

	void Update () {
		
	//	if(healthTexture == null)
			//healthTexture = 

        h = GetComponent<DestroyOnTriggerEnter>().Health;
	    Debug.Log(h);

	    if (h == 2) {
		   //RenderComponent.material.SetColor(HealthTwoLeft);
		   Debug.Log("Should be setting color for when health is two");
		    RenderComponent.material.color = HealthTwoLeft;
	    }

		if (h == 1) {
			RenderComponent.material.color = HealthOneLeft ;
		}
            
    }
	
	
	
}
