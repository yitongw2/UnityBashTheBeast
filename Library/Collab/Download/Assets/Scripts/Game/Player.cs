using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

	private GameController gc;
	public int ID;
	public string Name;
	public bool HasFinished;
	public int CurrentFieldID;
	public Sprite[] cars;
	public Sprite[] angles;
	public Vector2 track;
	public ChooseCharacters cc;
	public int beast; // each player need to have a beast
	public int delay = 5;
	public bool message = false;

	// Use this for initialization
	void Start () {
		GetComponent<SpriteRenderer>().sprite = cars[ID];
		GetComponent<SpriteRenderer>().sortingLayerName="fore";
		gc = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
		track = new Vector2 (0, 25);

	}

	// Update is called once per frame
	void Update () {
		
	}
		
	public void Action(){

		if (gc.inAction) {
			if (gc.field [CurrentFieldID].Type == FieldType.Action && gc.field [CurrentFieldID].Action == ActionType.GoToField) {
				cc = GameObject.FindGameObjectWithTag ("ChoosePanel").GetComponent<ChooseCharacters> ();
				cc.setActive (true);
				if (cc.isChoosen && cc.choice > 0) {
					track = gc.tracks [cc.choice - 1];
					CurrentFieldID = (int)track.x;
					transform.position = gc.field [CurrentFieldID].transform.position + gc.playerSlots [ID];
					cc.reset ();
					gc.dice.isRolled = false;
					gc.inAction = false;
					gc.NextPlayer ();
					cc.setActive (false);
				}
					
			} else if (gc.field [CurrentFieldID].Type == FieldType.Action && gc.field [CurrentFieldID].Action == ActionType.GoBack
				&& gc.field [CurrentFieldID].GoBackNumSteps != 0) {
					CurrentFieldID = (int)(CurrentFieldID - gc.field [CurrentFieldID].GoBackNumSteps);
					transform.position = gc.field [CurrentFieldID].transform.position + gc.playerSlots [ID];
					gc.dice.isRolled = false;
					gc.inAction = false;
					gc.NextPlayer ();
			} else if (gc.field [CurrentFieldID].Type == FieldType.Action && gc.field [CurrentFieldID].Action == ActionType.GoAhead 
				&& gc.field [CurrentFieldID].GoAheadNumSteps != 0) {
					CurrentFieldID = (int)(CurrentFieldID + gc.field [CurrentFieldID].GoAheadNumSteps);
					transform.position = gc.field [CurrentFieldID].transform.position + gc.playerSlots [ID];
					gc.dice.isRolled = false;
					gc.inAction = false;
					gc.NextPlayer ();
			}
			else if (gc.field [CurrentFieldID].Type == FieldType.Action && gc.field [CurrentFieldID].Action == ActionType.Action)
			{
				// if the action cell is between home to hospital: appear purple card track:(0,25) ;beast card(0,13)
				if (0 <= CurrentFieldID && CurrentFieldID <= 26) {
					GameObject.FindGameObjectWithTag ("beastCard").GetComponent<cards> ().randomIndex = Random.Range (0, 13);
					GameObject.FindGameObjectWithTag ("beastCard").GetComponent<cards> ().setActive (true);
				}
				// if the action cell is in the first track. This is Stan's track:(26,52); beast card(44,58)
				else if (26 <=CurrentFieldID && CurrentFieldID<= 51) {
					GameObject.FindGameObjectWithTag ("beastCard").GetComponent<cards> ().randomIndex = Random.Range (44, 58);
					GameObject.FindGameObjectWithTag ("beastCard").GetComponent<cards> ().setActive (true);

				}
				// if the action cell is in the second track. This is Sugar Ray's track(53,78); beast card(59,73)
				else if (53 <= CurrentFieldID && CurrentFieldID <= 77) {
					GameObject.FindGameObjectWithTag ("beastCard").GetComponent<cards> ().randomIndex = Random.Range (59, 73);
					GameObject.FindGameObjectWithTag ("beastCard").GetComponent<cards> ().setActive (true);

				}
				// if the action cell is in the third track. This is Dora's track(79,104); beast card(14,28)
				else if (79 <= CurrentFieldID && CurrentFieldID <= 103) {
					GameObject.FindGameObjectWithTag ("beastCard").GetComponent<cards> ().randomIndex = Random.Range (14, 28);
					GameObject.FindGameObjectWithTag ("beastCard").GetComponent<cards> ().setActive (true);

				}
				// if the action cell is in the fourth track. This is Sid's track(105,130); beast card(29,43)
				else if (105 <= CurrentFieldID && CurrentFieldID <= 129) {
					GameObject.FindGameObjectWithTag ("beastCard").GetComponent<cards> ().randomIndex = Random.Range (29, 43);
					GameObject.FindGameObjectWithTag ("beastCard").GetComponent<cards> ().setActive (true);

				}
				//GameObject.FindGameObjectWithTag ("beastCard").GetComponent<cards> ().randomIndex = Random.Range (0, GameObject.FindGameObjectWithTag ("beastCard").GetComponent<cards> ().bcards.Length - 1);
				//GameObject.FindGameObjectWithTag ("beastCard").GetComponent<cards> ().setActive (true);
				int new_beast = beast;
				new_beast += GameObject.FindGameObjectWithTag("beastCard").GetComponent<cards>().points[GameObject.FindGameObjectWithTag ("beastCard").GetComponent<cards> ().randomIndex];

				if (0 <= new_beast && new_beast < 7) {
					beast = new_beast;
				} else if (new_beast < 0)
					beast = 0;
				else if (new_beast > 6)
					beast = 6;
				gc.dice.isRolled = false;
				gc.inAction = false;
				gc.NextPlayer ();


			}
			else {
				// temporary action for nothing 
				gc.dice.isRolled = false;
				gc.inAction = false;
				gc.NextPlayer ();
			}
		}
	}

	public void Move(int dicedNumber){
		if (HasFinished)
			gc.NextPlayer ();
		else
			StartCoroutine(moveForwards(dicedNumber));
	}

	// Move the current Player to the target Position field by field
	IEnumerator moveForwards(int dicedNumber)
	{
		gc.isGamerMoving = true;



		// get the field the player is currently on
		int currentField = CurrentFieldID;

		if (currentField + dicedNumber > track.y) {
			dicedNumber = (int)track.y - currentField;	
		}

		for (int i = 0; i < dicedNumber; i++)
		{ 
				//if(moveSound != null)
				//	audio.PlayOneShot(moveSound);

				float t = 0f;

				// increase the start- and endposition each iteration
				// startposition represents the field the player is on
				// endposition is always one field ahead of the players field
				// to make it look like the player moves one field at the time
				Vector3 startPosition = gc.field[(currentField + i)].transform.position+ gc.playerSlots[ID];
				Vector3 endPosition = gc.field[(currentField + i + 1)].transform.position+gc.playerSlots[ID];
				//Debug.Log("Player: "+ID+" FieldID: "+ CurrentFieldID+" StartPosition="+startPosition+" endPosition="+endPosition+" PlayerSlots: "+gc.playerSlots[ID].x+" "+gc.playerSlots[ID].y);
				float startZ = gc.field [(currentField + i)].transform.rotation.z;
				float endZ = gc.field [(currentField + i + 1)].transform.rotation.z;

				// lerp to the next field
				while(t < 1f)
				{
					t += Time.deltaTime * 4f;

					transform.position = Vector3.Lerp (startPosition, endPosition, t);
					//Debug.Log ("Rotate angle z="+(endZ-startZ));
					transform.Rotate (0,0,(endZ-startZ));
					yield return null;
				}
				yield return null;
			}
			// update the players current field
			CurrentFieldID = currentField + dicedNumber;
			//move to the hospital and need to pop up menu

			if (gc.field[CurrentFieldID].Type == FieldType.Finish)
			{
				HasFinished = true;
				gc.winner.Add (this);

				if(gc.stopWhenFirstPlayerHasFinished)
				{
					gc.isGameOver = false;
				}
				else
				{
					gc.isGameOver = gc.IsGameOver();
				}
				yield return new WaitForSeconds(0.15f);
			}

			// this player has finished moving

		// wait a little
		yield return new WaitForSeconds(0.1f);
		gc.isGamerMoving = false;
		gc.waitForYourTurn = false;

		yield return 0;
	}
}
