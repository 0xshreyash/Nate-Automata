using UnityEngine;
using System.Collections;

public class PlayerHealthIndicator : MonoBehaviour {

    // Use this for initialization
    public Color HealthTwoLeft;
    public Color HealthOneLeft;

	public Renderer RenderComponent;


	public GameObject HealthObject;


	private int h;
	private ArrayList playerHealthCircles;
	
	
	
	const float LEFT_EDGE_BORDER_OFFSET = 0.9f;
	const float BOTTOM_EDGE_BORDER_OFFSET = 2f;
	const float CIRCLES_X_OFFSET = 0.6f;


	private GameObject player;
	


	void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player");
		h = GetComponent<DestroyOnTriggerEnter>().Health;
		Debug.Log("Player health is : " + h);

		if (HealthObject != null && h <= 4)
		{

			
			playerHealthCircles = new ArrayList();
			GameObject addCircle;
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			Vector3 playerPosition = player.transform.position;

			const float Y_OFFSET = 0.6f;
			float currentY = 0f;

			
			// Display the prefab object to be rendered as a "health bar" 
			for (int a = 0; a < h; a++)
			{
				float positionX = playerPosition.x - LEFT_EDGE_BORDER_OFFSET + a * CIRCLES_X_OFFSET;
				float positionY = playerPosition.y;
				float positionZ = playerPosition.z - BOTTOM_EDGE_BORDER_OFFSET;
				
				addCircle = Utils.InstantiateSafe(HealthObject, new Vector3(positionX, positionY, positionZ));
				addCircle.transform.Rotate(0,90,90);
				playerHealthCircles.Add(addCircle);
				
				addCircle.transform.parent = player.transform;
			}
		}
		
	}

	void Update () {

        h = GetComponent<DestroyOnTriggerEnter>().Health;
	    Debug.Log("Update: h is " + h);

	    if (h == 2) {
		   //RenderComponent.material.SetColor(HealthTwoLeft);
		   Debug.Log("Should be setting color for when health is two");
		    RenderComponent.material.color = HealthTwoLeft;
	    }

		if (h == 1) {
			RenderComponent.material.color = HealthOneLeft ;
		}

		if (HealthObject != null && h <= 4)
		{
			
			if (playerHealthCircles.Count != h)
			{
				GameObject removedHealth = (GameObject)playerHealthCircles[h];
				playerHealthCircles.RemoveAt(h);

				Destroy(removedHealth);

			}
		}
            
    }
	
	
	
}