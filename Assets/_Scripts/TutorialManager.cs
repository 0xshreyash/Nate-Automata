using System;
using System.Linq;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
	private enum STATE
	{
		WELCOME_TEXT,
		ADVICE_TEXT,
		MOVEMENT,
		SHOOTING,
		KILLENEMY,
		EXIT_FIRST,
		EXIT_SECOND,
		EXIT_THIRD
	}

	private Boolean actionPerformed;
	private float t;
	private float totalDisplayTime = 3;
	public GameObject enemyPrefab;
	public GameObject movementTrack;
	public GameObject bulletTrack;

	private STATE currentState;
	public TextMesh TextContent;
	private GameObject enemy;
	private GameObject circleTrack;
	private GameObject lineTrack;
	
	// Use this for initialization
	void Start ()
	{
		Cursor.visible = false;
		currentState = STATE.WELCOME_TEXT;
		actionPerformed = false;
		t = 0;
		TextContent.text = "Welcome to this Nate Automata tutorial!";
		/*PanelText.text = "Let\'s start with movement! You can use W to move up, S to move down, A to move left" +
		                 " and D to move right (You can also use the arrow keys to move about, but it might be" +
		                 "hard to use the mouse then";*/
	}
	
	// Update is called once per frame
	void Update () {

		if (currentState == STATE.WELCOME_TEXT)
		{
			t += Time.deltaTime;
			if (t > totalDisplayTime)
			{
				currentState = STATE.ADVICE_TEXT;
				TextContent.text = "We recommend using a mouse to play the game.";
				
				t = 0;
			}
		}
		else if (currentState == STATE.ADVICE_TEXT)
		{
			t += Time.deltaTime;
			if (t > totalDisplayTime)
			{
				currentState = STATE.MOVEMENT;
				TextContent.text = "You can use " +
				                   "\n-> W to move up" +
				                   "\n-> S to move down" +
				                   "\n-> A to move left," +
				                   "\n-> D to move right" +
								   "\n(You can alterantively use the arrow keys too)";
				totalDisplayTime = 5;
				t = 0;

			}
		}
		else if (currentState == STATE.MOVEMENT)
		{
			if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.A)
			    || Input.GetKeyDown(KeyCode.D) || actionPerformed || Input.GetKeyDown(KeyCode.UpArrow) ||
			    Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.LeftArrow) 
			    || Input.GetKeyDown(KeyCode.RightArrow))
			{
				actionPerformed = true; 
				t += Time.deltaTime;
				
			}
			if (t > totalDisplayTime)
			{
				actionPerformed = false;
				currentState = STATE.SHOOTING;
				TextContent.text = "Great! Now that you can move, let's use the mouse to orient Nate" +
				                   "\nand shoot by clicking the left mouse button";
				totalDisplayTime = 3;
				t = 0;
			}
		}
		else if (currentState == STATE.SHOOTING)
		{
			if (Input.GetMouseButtonDown(0) || actionPerformed)
			{
				actionPerformed = true;
				t += Time.deltaTime;
			}
			if (t > totalDisplayTime)
			{
				actionPerformed = false;
				currentState = STATE.KILLENEMY;
				GameObject player = GameObject.FindGameObjectWithTag("Player");
				TextContent.text = "Awesome! Now let's try that on an enemy sphere" +
				                   "\nYou should be able to see an enemy sphere in" +
				                   "\nthe middle of the play area." +
				                   "\nYou will need to shoot it 4 times to kill the enemy";
				float y = player.transform.position.y; 
				enemy = Instantiate(enemyPrefab, new Vector3(0.0f, y, 0.0f), Quaternion.identity);
				enemy.gameObject.GetComponent<BulletDistributor>().Player = player.transform;
				enemy.gameObject.GetComponent<BulletDistributor>().SpawnMode = BulletSpawnMode.TowardsPlayer;
				enemy.SetActive(true);
				//lineTrack = Instantiate(bulletTrack, new Vector3(0.0f, y, 0.0f), Quaternion.identity);
				
				//circleTrack = Instantiate(trackPrefab, new Vector3(0.0f, 100f, 0.0f), Quaternion.identity);
				//enemy.gameObject.GetComponent<PatrolEnemy>().Track = circleTrack.GetComponent<Track>();
				
				//lineTrack.GetComponent<LineTrack>().ShapePoints.Clear();
				//lineTrack.GetComponent<LineTrack>().setPoints(new Vector3(-10f, y, -10f), new Vector3(10f, y, 10f));
				
				
				//enemy.gameObject.GetComponent<BulletDistributor>().
				
				t = 0;
			}
		}
		else if (currentState == STATE.KILLENEMY)
		{
			
			if (enemy == null)
			{
				currentState = STATE.EXIT_FIRST;
				actionPerformed = true;
				//t += Time.deltaTime;
				TextContent.text = "You learn fast! Remember the enemies in the actual game" +
				                   "\nwon't be this harmless. (They will actually damage you)" +
				                   "\n and you only have 4 lives per stage. Use them wisely.";
				t = 0;


			}
			
		}
		else if (currentState == STATE.EXIT_FIRST)
		{
			t += Time.deltaTime;
			if (t > totalDisplayTime)
			{
				currentState = STATE.EXIT_SECOND;
				TextContent.text = "Hot tip: The purple enemy bullets are indescructible" + 
									"\nwhile the orange ones are not.";
				t = 0;
			}
		}
		else if (currentState == STATE.EXIT_SECOND)
		{
			t += Time.deltaTime;
			if (t > totalDisplayTime)
			{
				
				currentState = STATE.EXIT_THIRD;
				TextContent.text = "Now, go and help Nate escape this automatan by completing all the " +
				                   "\nlevels. Shoot the EXIT sphere to go back to the main menu";
			}
		}
	}
}
