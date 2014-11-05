using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GamePlayService{

	public readonly static GamePlayService Instance = new GamePlayService();

	/// <summary>
	/// The visited boolean. check block is visited or bot
	/// </summary>
	private bool [,] visited ;

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
	public void initBlock(Vector2 _tilesNum, Block[,] _tiles){
		this._tilesNum = _tilesNum;
		this._tiles = _tiles;
		visited = new bool[(int) _tilesNum.x, (int)_tilesNum.y];
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
	private bool checkIsSameTypeOfBlock(Block b1, Block b2){
		return (b1._type == b2._type);
	}

	/// <summary>
	/// Search the specified way for block[x, y], result will be kept in matchingSet.
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	/// <param name="type">Type of block</param>
	/// <param name="matchingSet">Matching set.</param>
	private void Search(int x, int y, Block type, List<Block> matchingSet ){
		
		if(checkIsSameTypeOfBlock(_tiles[x,y], type)){
			matchingSet.Add(_tiles[x,y]);
			visited[x,y] = true;
			if( x < _tilesNum.x - 1 && !visited[x + 1, y]){
				Search(x + 1, y, type, matchingSet);
			}
			
			if( y < _tilesNum.y - 1 && !visited[x, y + 1]){
				Search(x , y + 1, type, matchingSet);
			}
			
			if( x > 0 && !visited[x - 1, y]){
				Search(x - 1, y, type, matchingSet);
			}
			
			if( y > 0 && !visited[x, y - 1]){
				Search(x , y - 1, type, matchingSet);
			}
			
			if( x % 2 != 0) {
				if( x < _tilesNum.x - 1 && y > 0  && !visited[x + 1, y - 1]){
					Search(x + 1, y - 1, type, matchingSet);
				}
				
				if( x > 0 && y > 0  && !visited[x - 1, y - 1]){
					Search(x - 1, y - 1, type, matchingSet);
				}
			}else{
				if( x < _tilesNum.x - 1 && y < _tilesNum.y - 1  && !visited[x + 1, y + 1]){
					Search(x + 1, y + 1, type, matchingSet);
				}
				
				if( x > 0 && y < _tilesNum.y - 1  && !visited[x - 1, y + 1]){
					Search(x - 1, y + 1, type, matchingSet);
				}
			}
		}
	}

	/// <summary>
	/// Clears the visited match.
	/// </summary>
	private void ClearVisitedMatch(){
		for(int x = 0; x < (int) _tilesNum.x; x ++){
			for(int y = 0; y < (int) _tilesNum.y ; y++){
				visited[x, y] = false;
			}
		}
	}
	
	/// <summary>
	/// Finds all the matches.
	/// </summary>
	private void FindMatches(){
		ClearVisitedMatch();
		best_way.Clear();
		
		for(int x = 0; x < (int) _tilesNum.x; x++){
			for(int y = 0; y <  (int) _tilesNum.y; y++){
				if(!visited[x,y]){
					visited[x, y] = true;
					List<Block> matchingSet = new List<Block>();
					Search(x, y, _tiles[x,y], matchingSet);
					if(matchingSet.Count > best_way.Count){
						best_way = matchingSet;
					}
				}
			}
		}
		
	}

	/// <summary>
	/// Finds the hint that has maximum same type of blocks
	/// </summary>
	public List<Block> FindHint ()
	{
		FindMatches();
		return best_way;
	}

}