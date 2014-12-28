using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public enum DialogType
{
	DialogOneButton,
	DialogBonus,
	DialogTwoButton,
    DialogResult,
    DialogHighScore,
	DialogPause,
    DialogLevelUp,
}

public enum DialogTransition
{
	ZoomIn,
	ZoomOut,

}

public class DialogData {

	public DialogType dialogType;
	public Dictionary<string, string> textData;
	public Dictionary<string, EventDelegate.Callback> eventData;
	public Dictionary<string, SoundVolume> soundData;

	public DialogData()
	{
		textData 	= new Dictionary<string,string > ();
		eventData 	= new Dictionary<string,EventDelegate.Callback> ();
		soundData 	= new Dictionary<string,SoundVolume > ();
	}
	
}
