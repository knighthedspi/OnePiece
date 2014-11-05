
using UnityEngine;
using System.Collections;

[System.Serializable]
public class OPCharacter{
	[SerializeField]
	public int id;
	[SerializeField]
	public string characterName;
	[SerializeField]
	public int levelID;
	[SerializeField]
	public Vector3 direction;
	[SerializeField]
	public Vector3 position;
	[SerializeField]
	public int soundId;
	[SerializeField]
	public Animation animation;

	public OPCharacter()
	{

	}

	public OPCharacter (int id, string characterName, int levelID, Vector3 direction, Vector3 position, int soundId)
	{
		this.id = id;
		this.characterName = characterName;
		this.levelID = levelID;
		this.direction = direction;
		this.position = position;
		this.soundId = soundId;
	}
	

	public int Id {
		get {
			return this.id;
		}
		set {
			id = value;
		}
	}

	public string CharacterName {
		get {
			return this.characterName;
		}
		set {
			characterName = value;
		}
	}

	public int LevelID {
		get {
			return this.levelID;
		}
		set {
			levelID = value;
		}
	}

	public Vector3 Direction {
		get {
			return this.direction;
		}
		set {
			direction = value;
		}
	}

	public Vector3 Position {
		get {
			return this.position;
		}
		set {
			position = value;
		}
	}

	public int SoundId {
		get {
			return this.soundId;
		}
		set {
			soundId = value;
		}
	}

	public Animation Animation {
		get {
			return this.animation;
		}
		set {
			animation = value;
		}
	}
}