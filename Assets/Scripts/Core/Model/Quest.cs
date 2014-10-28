using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Quest{

	public string QuestId{get; private set;}
	public string questName{get; private set;}
	public bool unitWaitMode{get; private set;}
	public string bossAdventMode{get; private set;}
	public bool bossAdventTime{get; private set;}
	public List<Character>[] waveMonsters{get; private set;}
	public List<Character> monsters{get; private set;}
	public List<Character> units{get; private set;}
}