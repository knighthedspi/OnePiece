using UnityEngine;
using System;

public static class Config {

	public const int              TARGET_FRAMERATE      = 60;
	public const int              VSYNC_COUNT           = 0;
	public const bool             ASSETBUNDLE_AUTO_SAVE = true;

	public const string           UNITY_VERSION         = "4.5.5f1";

	public const string           SCENE_COMMON          = "common_scene";
	public const string           SCENE_MAIN            = "main_scene";

	public const string           TAG_COMMON            = "CommonScene";
	public const string           TAG_MAIN_CAMERA       = "MainCamera";

	public const string			  START_VIEW 			= "Start";
	public const string			  FB_VIEW 				= "Facebook";
	public const string			  GAME_PLAY_VIEW 		= "GamePlay";
	public const string			  LOADING_VIEW			= "Loading"; 	

	// TODO : define tag 
	public const string           TAG_CHARACTER         = "CHARACTER";
	public const string           TAG_UNIT              = "Unit";
	public const string           TAG_UNTAGGED          = "Untagged";
	public const string           TAG_DECK              = "Deck";

	// TODO : define layer

//	public static readonly int    LAYER_NONE            = LayerMask.NameToLayer("None");
//	public static readonly int    LAYER_DEFAULT         = LayerMask.NameToLayer("Default");
//	public static readonly int    LAYER_UI              = LayerMask.NameToLayer("UI");
//	public static readonly int    LAYER_ATTACK          = LayerMask.NameToLayer("Attack");
//	public static readonly int    LAYER_DEFENSE         = LayerMask.NameToLayer("Defense");
//	public static readonly int    LAYER_ATTACK_MONSTER  = LayerMask.NameToLayer("AttackMonster");
//	public static readonly int    LAYER_DEFENSE_MONSTER = LayerMask.NameToLayer("DefenseMonster");
//	public static readonly int    LAYER_LABEL           = LayerMask.NameToLayer("Label");
	
	public const int			  COUNT_OF_MONSTERS     = 5;	
	public const string			  RESOURCE_PREFIX		= "Prefab/Onepiece/Monster/";	
	public static readonly Vector3 CHARACTER_POSITION	= new Vector3(-100, 290, 0);
	public static readonly Vector3 CHARACTER_DIRECTION	= Vector3.zero;
	public static readonly Vector3 MONSTER_POSITION		= new Vector3(100, 290, 0);		 
	public const float			  TWEEN_DURATION		= 0.5f;
}