// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.1
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class GamePlayService{
	private 	bool[,] 						_visited ;
	private 	List<Block> 					_hints;
	private 	Vector2 						_blockNum ;
	private 	Block[,] 						_blocks;
	private 	float 							_deltaStartX;
	private 	float 							_deltaStartY;
	private 	Vector2 						_blockSize;
	private 	Vector2 						_boardPadding;
	private 	Vector2 						_blockMargin;
	private		GameObject 						_board;
	private		GameObject 						_panel;
	private		GameObject						_effectParticle;
	private		GameObject						_dotting;
	private		GameObject						_connectLine;
	private		Camera							_mainCamera;
	private 	Dictionary<Vector2,Vector2> 	_blockPositions;
	private 	Vector2 						_startBlockPosInBoard;
	private 	Vector2 						_endBlockPosInBoard;
	private 	List<Block> 					_stackBlock ;
	private 	List<GameObject> 				_stackDot 	;
	private 	List<GameObject> 				_stackLine 	;
	private 	GameObject 						_currentLine ;
	private 	float 							_blockDistanceLimit;
	private 	List<Block> 					_neighborBlocks;
	private 	OPGameSetup						_gameSetup;

    private     int                             _soundIndex;
    private     string[]                        _soundName;


	/// <summary>
	/// Initialize Game Play Service
	/// </summary>
	/// <param name="blockNum">Block number.</param>
	/// <param name="blocks">Array of blocks</param>
	/// <param name="deltaStartX">Delta start x .</param>
	/// <param name="deltaStartY">Delta start y.</param>
	/// <param name="blockSize">Block size.</param>
	/// <param name="boardPadding">Board padding.</param>
	/// <param name="blockMargin">Block margin.</param>
	/// <param name="board">Board.</param>
	/// <param name="mainCamera">Main camera.</param>
	public void initialize(GameObject board, GameObject panel, Camera mainCamera,  GameObject effectParticle, GameObject dotting, GameObject connectLine)
	{
		_gameSetup 			= AppManager.Instance.gameSetup;
		this._blockNum 		= _gameSetup.blockNum;
		this._deltaStartX 	= _gameSetup.deltaStartX;
		this._deltaStartY 	= _gameSetup.deltaStartY;
		this._blockSize 	= _gameSetup.blockSize;
		this._boardPadding 	= _gameSetup.boardPadding;
		this._blockMargin 	= _gameSetup.blockMargin;
		this._board = board;
		this._panel = panel;
		this._effectParticle = effectParticle;
		this._dotting = dotting;
		this._connectLine = connectLine;
		this._mainCamera = mainCamera;
		this._blockPositions = new Dictionary<Vector2,Vector2>();
		this._stackBlock = new List<Block>();
		this._stackDot 	= new List<GameObject>();
		this._stackLine = new List<GameObject>();
		this._currentLine 			= null;
		this._blockDistanceLimit = (float) Math.Sqrt(_blockSize.x * _blockSize.x / 4 + _blockSize.y * _blockSize.y); 
		this._neighborBlocks		= new List<Block>();

		this._hints = new List<Block>();
		this._blocks = new Block[(int)_blockNum.x, (int)_blockNum.y];
		this._visited = new bool[(int)_blockNum.x, (int)_blockNum.y];
		generateBlockPosition();

        _soundIndex = 2;
        _soundName = new string[]{"pianoA", "pianoB", "pianoC", "pianoD", "pianoE", "pianoF", "pianoG"};
	}

	#region touch_board
	private Vector2 getNodePos(GameObject node)
	{
		float ax = 0;
		float ay = 0;
		UISprite spr = node.GetComponentInChildren<UISprite>();
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

	/// <summary>
	/// check Intersects the node to point.
	/// </summary>
	/// <returns><c>true</c>, if node to point was intersected, <c>false</c> otherwise.</returns>
	/// <param name="node">Node.</param>
	/// <param name="pos">Position.</param>
	public bool intersectNodeToPoint(GameObject node,Vector3 pos)
	{
		UISprite spr = node.GetComponentInChildren<UISprite>();
		float width = (float)spr.width;
		float height = (float)spr.height;
		Vector2 nodePos = getNodePos(node);
		if(pos.x > nodePos.x && pos.y > nodePos.y &&
		   pos.x < nodePos.x + width &&
		   pos.y < nodePos.y + height) {
			return true;
		}
		return false;
	}

	/// <summary>
	/// Calculate 2D coordinates from screen coordinates
	/// </summary>
	/// <returns>The 2D coordinates </returns>
	/// <param name="pos">Screen position</param>
	/// <param name="camera">Main Camera</param>
	/// <param name="panel">Main Panel</param>
	public Vector3 screenTo2DPoint(Vector3 pos )
	{
		Vector3 worldPos = _mainCamera.ScreenToWorldPoint(pos);
		return _panel.transform.worldToLocalMatrix.MultiplyPoint3x4(worldPos);
	}

	/// <summary>
	/// Finds the intersect block with mouse position
	/// </summary>
	/// <returns>The intersect block position in board</returns>
	/// <param name="pos">Mouse position</param>
	public Vector2 findIntersectBlock(Vector3 pos)
	{
		for(int i=0;i < _blockNum.x;i++) {
			for(int j=0;j < _blockNum.y;j++) {
				if(_blocks[i, j] != null && intersectNodeToPoint(_blocks[i, j].gameObject, pos)) {
					return new Vector2(i, j);
				}
			}
		}
		return new Vector2(-1, -1);
	}

	/// <summary>
	/// Fills the board with block
	/// </summary>
	/// <param name="blockPrefabs">List of block prefabs</param>
	public void fillBoard(Block blockPrefabs, ref bool hintDirty, ref List<GameObject> hintObjs)
	{
		for(int i=0;i < _blockNum.x;i++) {
			for(int j=0;j < _blockNum.y;j++) {
				if(_blocks[i, j] == null) {
					Block block = null;
					if(j == 0) {
						block = pushNewItem(blockPrefabs, i, ref hintDirty, ref hintObjs);
					} else {
						block = _blocks[i, j - 1];
						_blocks[i, j - 1] = null;
					}
					if(block) {
						block.moveToY(getBlockPosition(i, j).y);
						_blocks[i, j] = block;
						_blocks[i, j].posInBoard = new Vector2(i, j);
					} else {
						OPDebug.Log("block (" + i + ";" + j + ") is null");
					}
				}
			}
		}
		
	}

	private Block pushNewItem(Block sample, int position, ref bool hintDirty, ref List<GameObject> hintObjs)
	{
		if(_blocks[position, 0] != null)
			return null;
	
		Block newBlock = sample.Spawn<Block>();
		newBlock.transform.parent = _panel.transform;
		newBlock.transform.localPosition = new Vector3(getBlockPosition(position, 0).x, 200);
		newBlock.InitRand ();				

		hintDirty = true;
		foreach(GameObject go in hintObjs) {
			go.Recycle();
		}
		hintObjs.Clear();
		
		return newBlock;
	}

	/// <summary>
	/// Clears all blocks in the board
	/// </summary>
	public void clearBlocks()
	{
		for(int i=0;i < _blockNum.x;i++)
		{
			for(int j=0;j < _blockNum.y;j++)
			{
				NGUITools.Destroy(_blocks[i,j].gameObject);
				_blocks[i, j] = null;
			}
		}
	}

	#endregion touch_board

	#region block_position_calculator

	private void generateBlockPosition()
	{
		for(int i = 0; i < _blockNum.x ; i++)
			for(int j = 0; j < _blockNum.y; j++)
				_blockPositions.Add(new Vector2(i,j) , blockPos(i , j));
	}

	private Vector2 blockPos(int x, int y )
	{
		if(x >= (int)_blockNum.x || x < 0)
			return new Vector2(0, 0);
		if(y >= (int)_blockNum.y || y < 0)
			return new Vector2(0, 0);
		
		float curve = x % 2 * _blockNum.x * _blockNum.y;
		
		Transform trans = _board.transform;
		
		float width = _board.GetComponent<UISprite>().width;
		float height = _board.GetComponent<UISprite>().height;
		float startX = trans.localPosition.x - width / 2 + _deltaStartX;
		float startY = trans.localPosition.y - height / 2 + _deltaStartY;
		float posX = (x * _blockSize.x);
		float posY = ((_blockNum.x - y - 1) * (_blockSize.y));
		
		posX = posX + startX + _blockSize.x / 2 + _boardPadding.x + _blockMargin.x * x;
		posY = posY + startY + _blockSize.y / 2 + _boardPadding.y + _blockMargin.y * y + curve;
		
		return new Vector2(posX, posY);
	}

	/// <summary>
	/// Gets the block position.
	/// </summary>
	/// <returns>The block position.</returns>
	/// <param name="x">The x coordinate in board</param>
	/// <param name="y">The y coordinate in board</param>
	public Vector2 getBlockPosition(int x, int y)
	{
		return _blockPositions[new Vector2(x, y)];
	}

	#endregion block_position_calculator

	#region effect
	private void addNeighborBlock2Stack(Block b)
	{
		Vector2 posInBoard = b.posInBoard;
		int posX = (int)posInBoard.x;
		int posY = (int)posInBoard.y;
		if(posX < _blockNum.x - 1 && !_neighborBlocks.Contains(_blocks[posX + 1, posY]))
			_neighborBlocks.Add(_blocks[posX + 1, posY]);
		if(posY < _blockNum.y - 1 && !_neighborBlocks.Contains(_blocks[posX, posY + 1]))
			_neighborBlocks.Add(_blocks[posX, posY + 1]);
		if(posX > 0 && !_neighborBlocks.Contains(_blocks[posX - 1, posY]))
			_neighborBlocks.Add(_blocks[posX - 1, posY]);
		if(posY > 0 && !_neighborBlocks.Contains(_blocks[posX, posY - 1]))
			_neighborBlocks.Add(_blocks[posX, posY - 1]);
		if(posX % 2 != 0) {
			if(posX < _blockNum.x - 1 && posY > 0 && !_neighborBlocks.Contains(_blocks[posX + 1, posY - 1]))
				_neighborBlocks.Add(_blocks[posX + 1, posY - 1]);
			if(posX > 0 && posY > 0 && !_neighborBlocks.Contains(_blocks[posX - 1, posY - 1]))
				_neighborBlocks.Add(_blocks[posX - 1, posY - 1]);
		} else {
			if(posX < _blockNum.x - 1 && posY < _blockNum.y - 1 && !_neighborBlocks.Contains(_blocks[posX + 1, posY + 1]))
				_neighborBlocks.Add(_blocks[posX + 1, posY + 1]);
			if(posX > 0 && posY < _blockNum.y - 1 && !_neighborBlocks.Contains(_blocks[posX - 1, posY + 1]))
				_neighborBlocks.Add(_blocks[posX - 1, posY + 1]);
		}
	}

	/// <summary>
	/// Clears the fever effects.
	/// </summary>
	public void clearFeverEffects()
	{
		_neighborBlocks.Clear();
	}
	

	/// <summary>
	/// Makes the effect particle.
	/// </summary>
	/// <returns>The effect particle.</returns>
	public GameObject MakeEffectParticle()
	{
		return _effectParticle.Spawn();
	}

	/// <summary>
	/// Makes the dot object when touch in block.
	/// </summary>
	/// <returns>The dot prefab.</returns>
	public GameObject MakeDotObject(){
		return _dotting.Spawn();
	}

	/// <summary>
	/// Makes the line object connect between two block
	/// </summary>
	/// <returns>The line prefab.</returns>
	public GameObject MakeLineObject(){
		return _connectLine.Spawn();
	}

	/// <summary>
	/// Check the blocks is moving or animating
	/// </summary>
	/// <returns><c>true</c>, if blocks is moving or animating, <c>false</c> otherwise.</returns>
	public bool isBlocksMoveToAnim()
	{
		for(int i=0;i < _blockNum.x;i++) {
			for(int j=0;j < _blockNum.y;j++) {
				if(_blocks[i, j] == null)
					return true;
				if(_blocks[i, j].isAnimation) {
					return true;
				}
			}
		}
		return false;
	}

	private int destroyBlocks(List<Block> v, AttackEffectController.OnFinished callback, Block except = null )
	{
		int count = 0;
		Vector2 endPos = _panel.transform.TransformPoint(Config.MONSTER_POSITION);
		foreach(Block _b in v) {
			if(except == _b)
				continue;
			
			_blocks[(int)_b.posInBoard.x, (int)_b.posInBoard.y] = null;
			
			AttackEffectController effectParticle = (MakeEffectParticle()).GetComponent<AttackEffectController>();
			Vector2 startPos = _panel.transform.TransformPoint(_b.transform.localPosition);

			effectParticle.generate(startPos, endPos);
			effectParticle.Finish = callback;

			// TODO : improve performance 
//			PlayerAttackParticle __ = (MakePlayerAttackParticle()).GetComponent<PlayerAttackParticle>();
//			__.generate(_panel, _b, _b.transform.localPosition, Config.MONSTER_POSITION);
//			__.Finish = callback;

			_b.Destroy();

			count++;
		}
		return count;
	}

	private void pushToStack(Block _b){
		_stackBlock.Add(_b);
		_b.TouchDown();
		
		GameObject dot = MakeDotObject();
		dot.transform.parent = _panel.transform;
		dot.transform.localScale = new Vector3(1,1,1);
		dot.GetComponent<UISprite>().width  = 24;
		dot.GetComponent<UISprite>().height = 24;
		dot.transform.localPosition = _b.transform.localPosition;
		
		_stackDot.Add(dot);

		if(_currentLine != null){
			_stackLine.Add(_currentLine);
			drawConnectLine(dot.transform.localPosition);
		}
	}

	private void popFromStack(){
		Block last = _stackBlock[_stackBlock.Count - 1];
		
		_stackBlock.Remove(last);
		last.TouchUp();
		
		GameObject _golast = _stackDot[_stackDot.Count - 1];
		_stackDot.Remove(_golast);
		_golast.Recycle();
		
		if(_stackLine.Count > 0){
			UnityEngine.GameObject.Destroy(_currentLine);
			_currentLine = _stackLine[_stackLine.Count - 1];
			_stackLine.Remove(_currentLine);
		}
	}
	
	/// <summary>
	/// Destroy all dots n lines
	/// </summary>
	public void dotLineDestroy(){
		foreach(GameObject go in _stackDot)
			go.Recycle();
		_stackDot.Clear();
		
		foreach(GameObject go in _stackLine)
			go.Recycle();
		_stackLine.Clear();
		
		UnityEngine.GameObject.Destroy(_currentLine);
		_currentLine = null;
	}

	public void clearStackBlock(){
		if(_stackBlock.Count > 0){
			foreach(Block b in _stackBlock)
				b.TouchUp();
			_stackBlock.Clear();
		}
	}

	private void drawConnectLine(Vector3 p)
	{
		Vector2 target = (p - _currentLine.transform.localPosition);
		target.Normalize();
		
		float dis = Vector2.Distance(_currentLine.transform.localPosition,p);
		float angle = Vector2.Angle(target,new Vector2(0,1));
		if(target.x < 0){
			angle = 180-angle+180;	
		}
		
		Quaternion _target = Quaternion.Euler(0, 0, -angle);
		_currentLine.transform.rotation = Quaternion.Slerp(_currentLine.transform.rotation, _target, 1);
		
		_currentLine.GetComponent<UISprite>().width = 18;
		_currentLine.GetComponent<UISprite>().height= (int)dis;
		_startBlockPosInBoard = findIntersectBlock(_currentLine.transform.localPosition);
		_endBlockPosInBoard	  = findIntersectBlock(p);
	}

	private void newCurrentLine(Vector3 p ){
		_currentLine = MakeLineObject();
		_currentLine.transform.parent = _panel.transform;
		_currentLine.transform.localScale = new Vector3(1,1,1);
		_currentLine.GetComponent<UISprite>().width  = 18;
		_currentLine.GetComponent<UISprite>().height = 0;
		_currentLine.transform.localPosition = p;
        SoundManager.Instance.PlaySE(_soundName[_soundIndex]);
        _soundIndex +=1;
        _soundIndex = _soundIndex % 7;
	}

	private void insertIntersectBlocksBetweenLines(BlockType blockType){
		int startPosX = _startBlockPosInBoard.x < _endBlockPosInBoard.x ? (int)_startBlockPosInBoard.x : (int)_endBlockPosInBoard.x;
		int stopPosX = _startBlockPosInBoard.x < _endBlockPosInBoard.x ? (int)_endBlockPosInBoard.x : (int)_startBlockPosInBoard.x;
		int startPosY = _startBlockPosInBoard.y < _endBlockPosInBoard.y ? (int)_startBlockPosInBoard.y : (int)_endBlockPosInBoard.y;
		int stopPosY = _startBlockPosInBoard.y < _endBlockPosInBoard.y ? (int)_endBlockPosInBoard.y : (int)_startBlockPosInBoard.y;
		OPDebug.Log("start:(" + startPosX + ";" + startPosY + ");stop:(" + stopPosX + ";" + stopPosY + "); line direction: " + _currentLine.gameObject.transform.localRotation.eulerAngles );
		for(int i = startPosX ; i <= stopPosX; i++){
			for(int j = startPosY ; j <= stopPosY; j++){
				Block b = _blocks[i,j];
				Block last = _stackBlock[_stackBlock.Count - 1];
				float dis = Vector2.Distance(last.transform.localPosition, b.transform.localPosition);
				//only add to line iff block has same type and intersected with current line
				if(b.blockType == blockType && !_stackBlock.Contains(b) && dis <= _blockDistanceLimit ){
					OPDebug.Log("add block(" + i + ";" + j + ") to stack");
					pushToStack(b);
					newCurrentLine(b.transform.localPosition);
				}
			}
		}
	}


	/// <summary>
	/// Updates the board.
	/// </summary>
	/// <param name="startFever">in fever mode or not.</param>
	/// <param name="callback">Callback when user get points</param>
	//	TODO : improve performance when use callback
	public void updateBoard(bool startFever, AttackEffectController.OnFinished callback, ref int destroyedBlock)
	{
		if (Input.GetMouseButton(0)){ // touch start or mouse clicked
			
			Vector3 p = screenTo2DPoint(Input.mousePosition);
			if( intersectNodeToPoint(_board,p) ){
				Vector2 idx = findIntersectBlock(p);
				if(_currentLine != null){
					drawConnectLine(p);
				}
				if(idx.x >= 0){
					Block _b = _blocks[(int)idx.x,(int)idx.y];
					if(_stackBlock.Count == 0){
						pushToStack(_b);
						newCurrentLine(_b.transform.localPosition);
					}else{
						Block last = _stackBlock[_stackBlock.Count - 1];
						if(last.blockType == _b.blockType){
							float dis = Vector2.Distance(last.transform.localPosition,_b.transform.localPosition);
							if(dis <= _blockDistanceLimit){
//								if(_stackBlock.Count > 1){
//									if(_stackBlock[_stackBlock.Count - 2] == _b){
//										popFromStack();
//									}
//								}
								if(!_stackBlock.Contains(_b)){
									pushToStack(_b);
									newCurrentLine(_b.transform.localPosition);
								}
							}else{
								// find intersect block that has the same type
								OPDebug.Log("draw line from " + _startBlockPosInBoard + " to " + _endBlockPosInBoard);
								insertIntersectBlocksBetweenLines(last.blockType);
							}
						}
					}
					if(startFever)
						addNeighborBlock2Stack(_b);
				}		
			}  		
		}else{ // touch end or mouse release
			releaseBlocks(startFever, callback, ref destroyedBlock);
		}	
	}

	private void releaseBlocks(bool startFever, AttackEffectController.OnFinished callback, ref int destroyedBlock)
	{
		if(_stackBlock.Count > 0) {
			if(_stackBlock.Count >= 3) {
				if(startFever)
					_stackBlock.AddRange(_neighborBlocks);
				destroyedBlock = destroyBlocks(_stackBlock, callback);
			} 
			foreach(Block b in _stackBlock)
				b.TouchUp();
			
			_stackBlock.Clear();
			dotLineDestroy();
		}
		_neighborBlocks.Clear();
	}
	#endregion effect
}

