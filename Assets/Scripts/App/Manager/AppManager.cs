using UnityEngine;
using System.Collections;

public class AppManager : Singleton<AppManager>
{
    public OPUser user;
	public OPGameSetup gameSetup { set; get;}
   
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
    }

    private void onInitDBComplete()
    {
        user = OPUserDAO.Instance.getCurrentUser();
    }
}
