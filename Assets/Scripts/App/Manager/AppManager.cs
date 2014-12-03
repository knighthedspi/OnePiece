using UnityEngine;
using System.Collections;

public class AppManager : Singleton<AppManager>
{
    public OPUser user;
	public OPGameSetup gameSetup { private set; get;}
   
    public OPUser User {
        get {
            return this.user;
        }
        set {
            user = value;
        }
    }

    void Start()
    {
        DBManager.Instance.onInitComplete = onInitDBComplete;
		InitializeGameSetup();
    }

    private void onInitDBComplete()
    {
		FBManager.Instance.CallFBInit();
    }

	// TODO : load from file
	private void InitializeGameSetup()
	{
		this.gameSetup = new OPGameSetup();
		this.gameSetup.deltaStartX 			= 24;
		this.gameSetup.deltaStartY			= -38;
		this.gameSetup.blockNum				= new Vector2(7,  6)  ;
		this.gameSetup.blockSize 			= new Vector2(80, 80) ;
		this.gameSetup.boardPadding			= new Vector2(0, 0) ;
		this.gameSetup.blockMargin			= new Vector2(-4, -6);
		this.gameSetup.stage_time 			= 60;
		this.gameSetup.hintTime 			= 5;
		this.gameSetup.scoreRatio1 			= 20;
		this.gameSetup.scoreRatio2 			= 50;
		this.gameSetup.comboStepTime 		= 1.5f;
		this.gameSetup.feverLimit 			= 5;
		this.gameSetup.feverStepTime 		= 1.5f;
		this.gameSetup.deltaMonsterPos 		= 300;
		this.gameSetup.listBlockTypes 		= new BlockType[5]
												{
													BlockType.luffy,
													BlockType.sanji,
													BlockType.nami,
													BlockType.zoro,
													BlockType.usop
												};
	}

}
