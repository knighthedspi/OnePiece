using UnityEngine;
using System.Collections;

public class AppManager : Singleton<AppManager>
{
    public OPUser user;
   
    public OPUser User {
        get {
            return this.user;
        }
        set {
            user = value;
        }
    }
}
