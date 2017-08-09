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


	// Use this for initialization
	void Start () {
		GetComponent<SpriteRenderer>().sprite = cars[ID];
		GetComponent<SpriteRenderer>().sortingLayerName="fore";
		gc = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
		track = new Vector2 (0, 22);

	}

	// Update is called once per frame
	void Update () {

	}

	public void Action(){

		if (gc.inAction) {
			if (gc.field [CurrentFieldID].Type == FieldType.Action && gc.field [CurrentFieldID].Action == ActionType.GoToField) 
			{
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
					
			} 
			else if (gc.field [CurrentFieldID].Type == FieldType.Action && gc.field [CurrentFieldID].Action == ActionType.Action)
			{
				GameObject.FindGameObjectWithTag ("beastCard").GetComponent<cards> ().setActive (true);
				beast += 1;

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
