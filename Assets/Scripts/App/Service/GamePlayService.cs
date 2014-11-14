using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GamePlayService
{

    public readonly static GamePlayService Instance = new GamePlayService();
	
    /// <summary>
    /// The visited boolean. check block is visited or bot
    /// </summary>
    private bool[,] visited ;

    /// <summary>
    /// The best_way , use 2 hint 
    /// </summary>
    private List<Block> best_way;

    /// <summary>
    /// The number of block
    /// </summary>
    private Vector2 _tilesNum ;

    /// <summary>
    /// The array of blocks
    /// </summary>
    private Block[,] _tiles;

    /// <summary>
    /// Inits the block data
    /// </summary>
    /// <param name="_tilesNum">number of block</param>
    public void initBlock(Vector2 _tilesNum,Block[,] _tiles)
    {
        this._tilesNum = _tilesNum;
        this._tiles = _tiles;
        visited = new bool[(int)_tilesNum.x, (int)_tilesNum.y];
        best_way = new List<Block>();
    }

    /// <summary>
    /// Loads the character.
    /// </summary>
    /// <returns>The character.</returns>
//	public GameObject loadCharacter()
//	{
//
//
//	}

    /// <summary>
    /// Checks the is same type of block.
    /// </summary>
    /// <returns><c>true</c>, if is same type of block was checked, <c>false</c> otherwise.</returns>
    /// <param name="b1">B1.</param>
    /// <param name="b2">B2.</param>
    private bool checkIsSameTypeOfBlock(Block b1,Block b2)
    {
        return (b1._type == b2._type);
    }

    /// <summary>
    /// Search the specified way for block[x, y], result will be kept in matchingSet.
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <param name="type">Type of block</param>
    /// <param name="matchingSet">Matching set.</param>
    private void Search(int x,int y,Block type,List<Block> matchingSet)
    {
		
        if(checkIsSameTypeOfBlock(_tiles[x, y], type)) {
            matchingSet.Add(_tiles[x, y]);
            visited[x, y] = true;
            if(x < _tilesNum.x - 1 && !visited[x + 1, y]) {
                Search(x + 1, y, type, matchingSet);
            }
			
            if(y < _tilesNum.y - 1 && !visited[x, y + 1]) {
                Search(x, y + 1, type, matchingSet);
            }
			
            if(x > 0 && !visited[x - 1, y]) {
                Search(x - 1, y, type, matchingSet);
            }
			
            if(y > 0 && !visited[x, y - 1]) {
                Search(x, y - 1, type, matchingSet);
            }
			
            if(x % 2 != 0) {
                if(x < _tilesNum.x - 1 && y > 0 && !visited[x + 1, y - 1]) {
                    Search(x + 1, y - 1, type, matchingSet);
                }
				
                if(x > 0 && y > 0 && !visited[x - 1, y - 1]) {
                    Search(x - 1, y - 1, type, matchingSet);
                }
            } else {
                if(x < _tilesNum.x - 1 && y < _tilesNum.y - 1 && !visited[x + 1, y + 1]) {
                    Search(x + 1, y + 1, type, matchingSet);
                }
				
                if(x > 0 && y < _tilesNum.y - 1 && !visited[x - 1, y + 1]) {
                    Search(x - 1, y + 1, type, matchingSet);
                }
            }
        }
    }

    /// <summary>
    /// Clears the visited match.
    /// </summary>
    private void ClearVisitedMatch()
    {
        for(int x = 0;x < (int) _tilesNum.x;x ++) {
            for(int y = 0;y < (int) _tilesNum.y;y++) {
                visited[x, y] = false;
            }
        }
    }
	
    /// <summary>
    /// Finds all the matches.
    /// </summary>
    private void FindMatches()
    {
        ClearVisitedMatch();
        best_way.Clear();
		
        for(int x = 0;x < (int) _tilesNum.x;x++) {
            for(int y = 0;y <  (int) _tilesNum.y;y++) {
                if(!visited[x, y]) {
                    visited[x, y] = true;
                    List<Block> matchingSet = new List<Block>();
                    Search(x, y, _tiles[x, y], matchingSet);
                    if(matchingSet.Count > best_way.Count && matchingSet.Count > 2) {
                        best_way = matchingSet;
                    }
                }
            }
        }
		
    }

    /// <summary>
    /// Finds the hint that has maximum same type of blocks
    /// </summary>
    public List<Block> FindHint()
    {
        FindMatches();
        return best_way;
    }

    /// <summary>
    /// Adds the neighbor block2 stack.
    /// </summary>
    /// <param name="b">The block component.</param>
    /// <param name="_neighbors">_neighbors stack list</param>
    public void addNeighborBlock2Stack(Block b,List<Block> _neighbors)
    {
        Vector2 posInBoard = b._posInBoard;
        int posX = (int)posInBoard.x;
        int posY = (int)posInBoard.y;
        if(posX < _tilesNum.x - 1 && !_neighbors.Contains(_tiles[posX + 1, posY]))
            _neighbors.Add(_tiles[posX + 1, posY]);
        if(posY < _tilesNum.y - 1 && !_neighbors.Contains(_tiles[posX, posY + 1]))
            _neighbors.Add(_tiles[posX, posY + 1]);
        if(posX > 0 && !_neighbors.Contains(_tiles[posX - 1, posY]))
            _neighbors.Add(_tiles[posX - 1, posY]);
        if(posY > 0 && !_neighbors.Contains(_tiles[posX, posY - 1]))
            _neighbors.Add(_tiles[posX, posY - 1]);
        if(posX % 2 != 0) {
            if(posX < _tilesNum.x - 1 && posY > 0 && !_neighbors.Contains(_tiles[posX + 1, posY - 1]))
                _neighbors.Add(_tiles[posX + 1, posY - 1]);
            if(posX > 0 && posY > 0 && !_neighbors.Contains(_tiles[posX - 1, posY - 1]))
                _neighbors.Add(_tiles[posX - 1, posY - 1]);
        } else {
            if(posX < _tilesNum.x - 1 && posY < _tilesNum.y - 1 && !_neighbors.Contains(_tiles[posX + 1, posY + 1]))
                _neighbors.Add(_tiles[posX + 1, posY + 1]);
            if(posX > 0 && posY < _tilesNum.y - 1 && !_neighbors.Contains(_tiles[posX - 1, posY + 1]))
                _neighbors.Add(_tiles[posX - 1, posY + 1]);
        }
    }

    /// <summary>
    /// Loads the character
    /// </summary>
    /// <returns>The Monster Component of this character.</returns>
    /// <param name="parent">Parent game object</param>
    /// <param name="pos">Position of character</param>
    /// <param name="direction">Direction of character</param>
    public Monster loadCharacter(GameObject parent,Vector3 pos,Vector3 direction)
    {
        //#TODO get by user level
        OPCharacter characterObj = CharacterService.Instance.getCharacterByLevel(1);
        return MonsterService.Instance.createMonster(characterObj, Config.TAG_CHARACTER, parent, pos, direction);
    }

    /// <summary>
    /// Loads list of monster per game level
    /// </summary>
    /// <returns>The list of Monster Components</returns>
    /// <param name="parent">Parent.</param>
    /// <param name="pos">Position.</param>
    /// <param name="direction">Direction.</param>
    public List<Monster> loadMonsterList(GameObject parent,List<Vector3> pos,Vector3 direction)
    {
        // TODO get by user 's current monster id
        int cmID = 0;
        List<OPCharacter> listMonsterModel = OPCharacterDAO.Instance.getListMonster(cmID);
        List<Monster> listMonster = MonsterService.Instance.createListMonster(listMonsterModel, parent, pos, direction); 
        return listMonster;	
    }

    #region calculate_scores
    ///start caculate exp
    /// <summary>
    /// Caculates the exp.

    /// Cách tính belly.
    ///	1. Ăn 3 block k được tính 
    ///	2. Ăn 4 block: 1
    ///	3. Ắn 5 block: 2
    ///	4. Ăn 6 block: 4
    ///	belly = 2^(num_block - 4)
    /// </summary>
    /// <returns>The exp.</returns>
    /// <param name="numBlocks">Number blocks.</param>
    public int calculateExp(int numBlocks)
    {
        if(numBlocks < 4) {
            return 0;
        }
        return 2 << (numBlocks - 4);
    }

    
    /// <summary>
    /// Cách tính score.
    //    Score 
    //        1. Có phụ thuộc vào level nhưng mà ít. Level up -> score up
    //            2. Skill: up -> up
    //            3. Combo up -> up
    //            4. Item up -> up
    //            5. Fever time
    //            Score 1 lần ăn
    //            
    //            score = a * 2^(block -3) + (combo + b)
    //            trong đó
    //            Block: số block ăn được trong 1 lần vuốt
    //            combo: số lượng combo hiện tại
    //            a: const = 20
    //            b: const = 50
    /// </summary>
    /// <returns>The score.</returns>
    /// <param name="numBlocks">Number blocks.</param>
    /// <param name="combo">Combo.</param>
    public int calculateScore(int numBlocks,int combo,int a = 20,int b = 50)
    {
        if(numBlocks < 3)
            return (combo + b);
        return (a * (2 << (numBlocks - 3)) + (combo + b));
    }

    /// <summary>
    /// Cách tính belly.
    //    1. Ăn 3 block k được tính 
    //        2. Ăn 4 block: 1
    //            3. Ắn 5 block: 2
    //            4. Ăn 6 block: 4
    //            
    //            belly = 2^(num_block - 4)
    /// </summary>
    /// <returns>The belly.</returns>
    /// <param name="numBlocks">Number blocks.</param>
    public int calculateBelly(int numBlocks)
    {
        if(numBlocks < 4) {
            return 0;
        }
        return 2 << (numBlocks - 4);
    }

    #endregion calculate_scores
	
    /// <summary>
    /// check Intersects the node to node.
    /// </summary>
    /// <returns><c>true</c>, if node 1 was intersected in node 2, <c>false</c> otherwise.</returns>
    /// <param name="node1">Node1.</param>
    /// <param name="node2">Node2.</param>
    public bool intersectNodeToNode(GameObject node1,GameObject node2)
    {
        Vector2 node1Pos = getNodePos(node1);
        Vector2 node2Pos = getNodePos(node2);

        UISprite spr1 = node1.GetComponent<UISprite>();
        UISprite spr2 = node2.GetComponent<UISprite>();

        float width1 = (float)spr1.width;
        float height1 = (float)spr1.height;
        float width2 = (float)spr2.width;
        float height2 = (float)spr2.height;

        if(node2Pos.x > node1Pos.x && node2Pos.y > node1Pos.y &&
            node2Pos.x + width2 < node1Pos.x + width1 &&
            node2Pos.y + height2 < node1Pos.y + height1) {
            return true;
        }
//		if( node1Pos.x + width1 < node2Pos.x || node1Pos.y + height1 < node2Pos.y 
//		   || node2Pos.x + width2 < node1Pos.x || node2Pos.y + height2 < node1Pos.y ){
//			OPDebug.Log("failed with " + node1Pos + ";" + width1 + "; " + node2Pos + ";" + width2 + ";" + height1 + ";" + height2);
//			return false;
//		}
        return true;
    }

    /// <summary>
    /// Gets the node position.
    /// </summary>
    /// <returns>The node position.</returns>
    /// <param name="node">Node.</param>
    private Vector2 getNodePos(GameObject node)
    {
        float ax = 0;
        float ay = 0;
        UISprite spr = node.GetComponent<UISprite>();
        switch(spr.pivot) {
        case UIWidget.Pivot.TopLeft:
            ax = 0;
            ay = 1.0f;
            break;
        case UIWidget.Pivot.Top:
            ax = 0.5f;
            ay = 1.0f;
            break;
        case UIWidget.Pivot.TopRight:
            ax = 1.0f;
            ay = 1.0f;
            break;
        case UIWidget.Pivot.Left:
            ax = 0;
            ay = 0.5f;
            break;
        case UIWidget.Pivot.Center:
            ax = 0.5f;
            ay = 0.5f;
            break;
        case UIWidget.Pivot.Right:
            ax = 1.0f;
            ay = 0.5f;
            break;
        case UIWidget.Pivot.BottomLeft:
            ax = 0;
            ay = 0;
            break;
        case UIWidget.Pivot.Bottom:
            ax = 0.5f;
            ay = 0;
            break;
        case UIWidget.Pivot.BottomRight:
            ax = 1.0f;
            ay = 0;
            break;
        }

        float width = (float)spr.width;
        float height = (float)spr.height;

        return new Vector2(
			node.transform.localPosition.x - width * ax,
			node.transform.localPosition.y - height * ay);
    }

}