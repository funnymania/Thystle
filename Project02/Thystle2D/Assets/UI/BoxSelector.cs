using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

//TODO: Enable masking for structures in game, and for types/tags of MOs in the Map Editor

//Maybe do this from Command Menu Tick
public class BoxSelector : MonoBehaviour
{
	public static bool isEnabled = true;
	public static bool isOverUI = false;
	public static void EnableNextFrame()
	{
		enabledNextFrame = true;
	}

	private static bool enabledNextFrame;

	[SerializeField] private MapObjectRuntimeSet selectedMapObjects;

	private Image boxSelectorImage;
	private Camera mainCamera;

	private bool isDragging;
	private Vector2 boxStartScreenPosition;
	private Vector2 boxEndScreenPosition;

	public enum SelectionType { None, OwnUnits, EnemyUnits}
	public static SelectionType selectionType;

	private void Awake()
	{
		boxSelectorImage = GetComponent<Image>();
		mainCamera = Camera.main;
	}

	private void Update()
	{
		UpdateBoxSelector();

		if(enabledNextFrame)
		{
			isEnabled = true;
			enabledNextFrame = false;
		}
	}

	private void UpdateBoxSelector()
	{
		if (Mouse.current.leftButton.wasPressedThisFrame && isEnabled && !isOverUI)
		{
			//Start drag
			isDragging = true;
			boxSelectorImage.enabled = true;

			boxStartScreenPosition = Mouse.current.position.ReadValue();
		}

		if (isDragging)
		{

			boxEndScreenPosition = Mouse.current.position.ReadValue();

			//Visual

			//Get distance
			Vector2 screenDragDistance = boxEndScreenPosition - boxStartScreenPosition;

			//Position Image at midpoint
			Vector2 screenMidPoint = boxStartScreenPosition + new Vector2(screenDragDistance.x / 2, screenDragDistance.y / 2);
			boxSelectorImage.rectTransform.anchoredPosition = screenMidPoint;

			//Convert to abs value
			screenDragDistance = new Vector2(Mathf.Abs(screenDragDistance.x), Mathf.Abs(screenDragDistance.y));

			//Set size to abs value
			boxSelectorImage.rectTransform.sizeDelta = screenDragDistance;

			//Detect Map Objects
			//TODO: Add hover units set

			//Get Contacts
			Collider2D[] contacts = Physics2D.OverlapAreaAll(mainCamera.ScreenToWorldPoint(boxStartScreenPosition), mainCamera.ScreenToWorldPoint(boxEndScreenPosition), 1<<14);


			if (Mouse.current.leftButton.wasReleasedThisFrame)
			{
				//End drag
				isDragging = false;
				boxSelectorImage.enabled = false;

				//TODO: Add modifiers for shift to "Add to selection", etc.

				//Clear selection
				selectedMapObjects.Clear();

				//Reset
				selectionType = SelectionType.None;

				if (contacts.Length > 0)
				{
					//Find Map Objects from contacts
					List<MapObject> contactedMapObjects = new List<MapObject>();
					foreach (Collider2D contact in contacts)
					{
						MapObject mapObject = contact.GetComponentInParent<MapObject>();

						if (mapObject)
							contactedMapObjects.Add(mapObject);
					}

					//Sort selection

					//Figure out selection type
					foreach(MapObject mapObject in contactedMapObjects)
					{
						/*if(mapObject.ownerID == NetworkManager.singleton.GetClient().ID)
						{
							selectionType = SelectionType.OwnUnits;
							break;
						}*/
					}

					if (selectionType == SelectionType.None)
						selectionType = SelectionType.EnemyUnits;


					//Remove units according to selection type
					if (selectionType == SelectionType.OwnUnits)
					{
						//Remove anything that's not a unit you own (including own structures)
						/*contactedMapObjects.RemoveAll(
							mo => mo.ownerID != NetworkManager.singleton.GetClient().ID
							);*/

						//Check if our selection has any units
						bool hasUnits = false;
						foreach(MapObject mapObject in contactedMapObjects)
							if(!mapObject.tags.Contains(MapObject.Tag.Structure))
							{
								hasUnits = true;
								break;
							}

						//remove structures if we have at least one unit
						if(hasUnits)
							contactedMapObjects.RemoveAll(
							mo => mo.tags.Contains(MapObject.Tag.Structure)
							);
					}

					if(selectionType == SelectionType.EnemyUnits)
					{
						//Remove all but one
						contactedMapObjects = new List<MapObject>() { contactedMapObjects[0] };
					}

					//Write to RuntimeSet
					selectedMapObjects.AddRange(contactedMapObjects);

				}
			}
		}
	}
}
