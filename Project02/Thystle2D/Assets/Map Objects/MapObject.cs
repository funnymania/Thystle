using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapObject : MonoBehaviour
{
	//What are the functions/responsibility of a Map Object?
	//-----
	//Act as an entity on the map
	//Holds current position, cliff level, and Z level
	//Can be selected (restricted in game-mode)
	//Placed by and edited by Map Editor
	//Can be pooled

	//Addon features / components

	//Can have sprites (single or multiple)
	//Can use a state machine
	//Can have a Team
	//Can use an animator, and velocity-based animation, and animation modifiers
	//Can use an agent and the nav mesh
	//Can have health and be attacked/killed
	//Can have a weapon and attack
	//Can use abilities
	//Can be issued commands by the command menu
	//Can be controlled by AI

	/*[SerializeField] private CommandData[] commandObjects;
	public CommandData[] GetCommandObjects() => commandObjects;

	private List<CommandBase> commands = new List<CommandBase>();

	public Action<CommandBase> OnActiveCommandChanged;
	public StateEvents onStateEnterEvent;

	private CommandBase currentCommand;

	public int ownerID;

	public bool usesPaletteSwapping;

	//TODO: solution for offline and placing objects in map editor (use game manager to centralize team data?)
	public int colorIndex
	{ //Shader looks at this for color swap

		get
		{
			return (int)NetworkManager.singleton.networkPlayers?[(ushort)ownerID].colorIndex;
		}
	}

	//Set from Map Object Manager
	public ushort ID = 0;

	private void Awake()
	{
		//Get commands from data
		foreach (CommandData commandData in commandObjects)
			if (commandData != null)
				commands.Add(commandData.GenerateCommand(this));

		SetCommand<IdleCommand>();

	}

	private void OnGameStart()
	{

	}

	public void UpdateMapObject()
	{
		currentCommand?.UpdateCommand();
	}

	public bool SetCommand<T>() where T : CommandBase
	{
		CommandBase command = GetCommand<T>();
		if (command != null)
		{
			foreach (CommandBase c in commands)
				c.enabled = false;

			command.enabled = true;
			OnActiveCommandChanged?.Invoke(command);

			currentCommand = command;
		}

		return command;
	}

	//Maybe set params this way (Send params from command menu / context or to CommandGiver)
	public CommandBase GetCommand<T>() where T : CommandBase
	{
		foreach (CommandBase command in commands)
			if (command.GetType() == typeof(T))
				return command;

		return null;
	}

	//Selection

	[SerializeField] private SpriteRenderer selectionCircle;

	public void EnableSelectionCircle()
	{
		selectionCircle.enabled = true;
	}

	public void DisableSelectionCircle()
	{
		selectionCircle.enabled = false;
	}*/


	//Temp, move to data obj
	public Sprite portrait;
	public Sprite selectionSprite_default, selectionSprite_hover, selectionSprite_pressed;

	public List<Tag> tags;

	public enum Tag { Structure, Biological, Construct, Magical, Light, Heavy }
}
