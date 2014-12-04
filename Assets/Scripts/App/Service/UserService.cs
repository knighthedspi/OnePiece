using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UserService
{

    public readonly static UserService Instance = new UserService();

    public void increaseBelly(OPUser user,int belly)
    {
        user.Belly += belly;
    }

    public void decreaseBelly(OPUser user,int belly)
    {
        user.Belly -= belly;
        OPUserDAO.Instance.Update(user);   
    }

    public void increaseExp(OPUser user,int exp)
    {
        user.Exp += exp;
        OPUserDAO.Instance.Update(user);
    }

	//#TODO : check level up 
    public bool isLevelUp()
    {
        return false;
    }
	
    public bool isHighScore(OPUser user,int score)
    {
        if (score > user.HighScore)
		{
			user.HighScore = score;
			return true;
		}
		return false;
    }

	public void updateState(OPUser user, MonsterController currentMonster)
	{
		OPUserDAO.Instance.Update(user);
		if(currentMonster != null)
		{
			PlayerPrefs.SetString( Config.CURRENT_MONSTER_NAME, currentMonster.monsterModel.CharacterName);
			PlayerPrefs.SetFloat( Config.CURRENT_MONSTER_INITIAL_HP_KEY, currentMonster.initialHP);
			PlayerPrefs.SetFloat( Config.CURRENT_MONSTER_CURRENT_HP_KEY, currentMonster.currentHP);
		}
		else
		{
			PlayerPrefs.DeleteKey( Config.CURRENT_MONSTER_NAME);
			PlayerPrefs.DeleteKey( Config.CURRENT_MONSTER_INITIAL_HP_KEY);
			PlayerPrefs.DeleteKey( Config.CURRENT_MONSTER_CURRENT_HP_KEY);
		}
		PlayerPrefs.Save();
	}
}