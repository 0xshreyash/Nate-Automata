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
	


	/*void Start()
	{
		h = GetComponent<DestroyOnTriggerEnter>().Health;
		Debug.Log("Player health is : " + h);

		if (HealthObject != null)
		{

			const float LEFT_EDGE_BORDER_OFFSET = 0.9f;
			const float BOTTOM_EDGE_BORDER_OFFSET = 2f;
			const float CIRCLES_X_OFFSET = 0.6f;
			
			playerHealthCircles = new ArrayList();
			GameObject addCircle;
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			Vector3 playerPosition = player.transform.position;

			const float Y_OFFSET = 0.6f;
			float currentY = 0f;

			for (int x = 0; x < h; x++)
			{
				float positionX = playerPosition.x - LEFT_EDGE_BORDER_OFFSET + x * CIRCLES_X_OFFSET;
				float positionY = playerPosition.y;
				float positionZ = playerPosition.z - BOTTOM_EDGE_BORDER_OFFSET;

				addCircle = Utils.InstantiateSafe(HealthObject, new Vector3(positionX, positionY, positionZ));
				addCircle.transform.Rotate(0,90,90);
				addCircle.transform.parent = player.transform;
				playerHealthCircles.Add(addCircle);
			}
		}
		
	}*/

	void Update () {

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

		if (HealthObject != null)
		{
			/*
			if (playerHealthCircles.Count != h)
			{
				GameObject removedHealth = (GameObject)playerHealthCircles[h];
				playerHealthCircles.RemoveAt(h);
				
				Destroy(removedHealth);
				
			}*/
		}
            
    }
	
	
	
}
