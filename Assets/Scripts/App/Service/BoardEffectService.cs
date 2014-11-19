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
using System.Collections;
using System.Collections.Generic;

public partial class GamePlayService{
	#region fever_effect
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
		if(posX < blockNum.x - 1 && !_neighbors.Contains(block[posX + 1, posY]))
			_neighbors.Add(block[posX + 1, posY]);
		if(posY < blockNum.y - 1 && !_neighbors.Contains(block[posX, posY + 1]))
			_neighbors.Add(block[posX, posY + 1]);
		if(posX > 0 && !_neighbors.Contains(block[posX - 1, posY]))
			_neighbors.Add(block[posX - 1, posY]);
		if(posY > 0 && !_neighbors.Contains(block[posX, posY - 1]))
			_neighbors.Add(block[posX, posY - 1]);
		if(posX % 2 != 0) {
			if(posX < blockNum.x - 1 && posY > 0 && !_neighbors.Contains(block[posX + 1, posY - 1]))
				_neighbors.Add(block[posX + 1, posY - 1]);
			if(posX > 0 && posY > 0 && !_neighbors.Contains(block[posX - 1, posY - 1]))
				_neighbors.Add(block[posX - 1, posY - 1]);
		} else {
			if(posX < blockNum.x - 1 && posY < blockNum.y - 1 && !_neighbors.Contains(block[posX + 1, posY + 1]))
				_neighbors.Add(block[posX + 1, posY + 1]);
			if(posX > 0 && posY < blockNum.y - 1 && !_neighbors.Contains(block[posX - 1, posY + 1]))
				_neighbors.Add(block[posX - 1, posY + 1]);
		}
	}

	#endregion fever_effect

	#region check_touch_board
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

	/// <summary>
	/// check Intersects the node to point.
	/// </summary>
	/// <returns><c>true</c>, if node to point was intersected, <c>false</c> otherwise.</returns>
	/// <param name="node">Node.</param>
	/// <param name="pos">Position.</param>
	public bool intersectNodeToPoint(GameObject node,Vector3 pos)
	{
		UISprite spr = node.GetComponent<UISprite>();
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

	// 3d position to 2d position
	public Vector3 screenTo2DPoint(Vector3 pos, Camera camera, GameObject panel)
	{
		if(camera == null) {
			Debug.Log("You need set _camera value. recommand NGUI Camera");
			return new Vector3();
		}        
		if(panel == null) {
			Debug.Log("You need set _panel value. recommand NGUI Panel");
			return new Vector3();
		}
		Vector3 worldPos = camera.ScreenToWorldPoint(pos);
		return panel.transform.worldToLocalMatrix.MultiplyPoint3x4(worldPos);
	}

	#endregion check_touch_board

	#region block_position_calculator
	/// <summary>
	/// Calculate the position of block
	/// </summary>
	/// <returns>The position of block</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	/// <param name="deltaStartX">Delta start x.</param>
	/// <param name="deltaStartY">Delta start y.</param>
	/// <param name="blockSize">Size of block as pixels.</param>
	/// <param name="boardPadding">Board padding size</param>
	/// <param name="board">Board game object</param>
	public Vector2 blockPos(int x, int y, int deltaStartX, int deltaStartY, Vector2 blockSize, Vector2 boardPadding, Vector2 blockMargin, GameObject board)
	{
		if(x >= (int)blockNum.x || x < 0)
			return new Vector2(0, 0);
		if(y >= (int)blockNum.y || y < 0)
			return new Vector2(0, 0);
		
		float curve = x % 2 * blockNum.x * blockNum.y;
		
		Transform trans = board.transform;
		
		float width = board.GetComponent<UISprite>().width;
		float height = board.GetComponent<UISprite>().height;
		float startX = trans.localPosition.x - width / 2 + deltaStartX;
		float startY = trans.localPosition.y - height / 2 + deltaStartY;
		float posX = (x * blockSize.x);
		float posY = ((blockNum.x - y - 1) * (blockSize.y));
		
		posX = posX + startX + blockSize.x / 2 + boardPadding.x + blockMargin.x * x;
		posY = posY + startY + blockSize.y / 2 + boardPadding.y + blockMargin.y * y + curve;
		
		return new Vector2(posX, posY);
	}
	#endregion block_position_calculator
}

