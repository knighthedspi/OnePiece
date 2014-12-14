using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelService
{
    public readonly static LevelService Instance = new LevelService();

    public OPLevel GetUserLevel(OPUser user)
    {
        return OPLevelDAO.Instance.GetLevelByID(user.LevelId);
    }

    public bool IsLevelUp(OPLevel level, int exp)
    {
        return (exp > (level.Exp + level.Diff));
    }

}