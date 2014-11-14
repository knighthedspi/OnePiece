using UnityEngine;
using System.Collections;

public class AppManager : Singleton<AppManager>
{
    public static OPUser user;    

    void Awake()
    {
        user = OPUserDAO.Instance.getCurrentUser();
    }

    void OnDestroy()
    {

    }
}
