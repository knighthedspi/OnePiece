
using UnityEngine;
using System.Collections;

public class OPUser
{
	
    private int? id;
    private string userName; 
	private string lastName;
	private string firstName;
	private string fbId;
    private string facebookToken;
    private int exp;
    private int belly;
    private int levelId;
    private string lastedHealthRes;
    private int health;
    private int score;
    private int highScore;
    private int currentMonsterID;
    private int updatedAt;
    private int createdAt;

    public int? Id {
        get {
            return this.id;
        }
        set {
            id = value;
        }
    }

	public string LastName {
		get {
			return this.lastName;
		}
		set {
			lastName = value;
		}
	}

	public string FirstName {
		get {
			return this.firstName;
		}
		set {
			firstName = value;
		}
	}

	public string FbId {
		get {
			return this.fbId;
		}
		set {
			fbId = value;
		}
	}

    public string UserName {
        get {
            return this.userName;
        }
        set {
            userName = value;
        }
    }

    public string FacebookToken {
        get {
            return this.facebookToken;
        }
        set {
            facebookToken = value;
        }
    }

    public int Exp {
        get {
            return this.exp;
        }
        set {
            exp = value;
        }
    }

    public int Belly {
        get {
            return this.belly;
        }
        set {
            belly = value;
        }
    }

    public int LevelId {
        get {
            return this.levelId;
        }
        set {
            levelId = value;
        }
    }

    public string LastedHealthRes {
        get {
            return this.lastedHealthRes;
        }
        set {
            lastedHealthRes = value;
        }
    }

    public int Health {
        get {
            return this.health;
        }
        set {
            health = value;
        }
    }

    public int Score {
        get {
            return this.score;
        }
        set {
            score = value;
        }
    }

    public int HighScore {
        get {
            return this.highScore;
        }
        set {
            highScore = value;
        }
    }

    public int CurrentMonsterID {
        get {
            return this.currentMonsterID;
        }
        set {
            currentMonsterID = value;
        }
    }

    public int UpdatedAt {
        get {
            return this.updatedAt;
        }
        set {
            updatedAt = value;
        }
    }

    public int CreatedAt {
        get {
            return this.createdAt;
        }
        set {
            createdAt = value;
        }
    }

}
