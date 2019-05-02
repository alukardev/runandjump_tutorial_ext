﻿using Assets.Scripts.Level;
using Assets.Tools.LevelCreator.Scripts;
using UnityEngine;

namespace RunAndJump {
	public partial class Level : MonoBehaviour
	{
	    [SerializeField]
	    private LevelSettings _settings;

	    public LevelSettings Settings
	    {
	        get { return _settings; }
	        set { _settings = value; }
	    }

        [Time]
		public int _totalTime = 60;

	    [SerializeField]
	    private int _totalColumns = 25;
	    [SerializeField]
	    private int _totalRows = 10;
	    public const float GridSize = 1.28f;
	    private readonly Color _normalColor = Color.grey;
	    private readonly Color _selectedColor = Color.yellow;

	    [SerializeField]
	    private LevelPiece[] _pieces;
	    public LevelPiece[] Pieces
	    {
	        get { return _pieces; }
	        set { _pieces = value; }
	    }

        public int TotalTime {
			get { return _totalTime; }
			set { _totalTime = value; }
		}

	    public int TotalColumns
	    {
	        get { return _totalColumns; }
	        set { _totalColumns = value; }
	    }
	    public int TotalRows
	    {
	        get { return _totalRows; }
	        set { _totalRows = value; }
	    }


	    private void GridFrameGizmo(int cols, int rows)
	    {
	        Gizmos.DrawLine(new Vector3(0, 0, 0), new Vector3(0, rows *
	                                                             GridSize, 0));
	        Gizmos.DrawLine(new Vector3(0, 0, 0), new Vector3(cols *
	                                                          GridSize, 0, 0));
	        Gizmos.DrawLine(new Vector3(cols * GridSize, 0, 0), new
	            Vector3(cols * GridSize, rows * GridSize, 0));
	        Gizmos.DrawLine(new Vector3(0, rows * GridSize, 0), new
	            Vector3(cols * GridSize, rows * GridSize, 0));
	    }
	    private void GridGizmo(int cols, int rows)
	    {
	        for (int i = 1; i < cols; i++)
	        {
	            Gizmos.DrawLine(new Vector3(i * GridSize, 0, 0), new Vector3(i
	                                                                         * GridSize, rows * GridSize, 0));
	        }
	        for (int j = 1; j < rows; j++)
	        {
	            Gizmos.DrawLine(new Vector3(0, j * GridSize, 0), new
	                Vector3(cols * GridSize, j * GridSize, 0));
	        }
	    }

	    private void OnDrawGizmos()
	    {
	        Matrix4x4 oldMatrix = Gizmos.matrix;
	        Color oldColor = Gizmos.color;

            Gizmos.matrix = transform.localToWorldMatrix;
	        Gizmos.color = _normalColor;
	        GridGizmo(_totalColumns, _totalRows);
	        GridFrameGizmo(_totalColumns, _totalRows);

	        Gizmos.color = oldColor;
	        Gizmos.matrix = transform.localToWorldMatrix;
	    }

	    private void OnDrawGizmosSelected()
	    {
	        Matrix4x4 oldMatrix = Gizmos.matrix;
	        Color oldColor = Gizmos.color;

            Gizmos.matrix = transform.localToWorldMatrix;
	        Gizmos.color = _selectedColor;
	        GridFrameGizmo(_totalColumns, _totalRows);

	        Gizmos.color = oldColor;
	        Gizmos.matrix = transform.localToWorldMatrix;
	    }

	    public Vector3 WorldToGridCoordinates(Vector3 point)
	    {
	        Vector3 gridPoint = new Vector3(
	            (int)((point.x - transform.position.x) / GridSize),
	            (int)((point.y - transform.position.y) / GridSize), 0.0f);
	        return gridPoint;
	    }
	    public Vector3 GridToWorldCoordinates(int col, int row)
	    {
	        Vector3 worldPoint = new Vector3(
	            transform.position.x + (col * GridSize + GridSize / 2.0f),
	            transform.position.y + (row * GridSize + GridSize / 2.0f),
	            0.0f);
	        return worldPoint;
	    }
	    public bool IsInsideGridBounds(Vector3 point)
	    {
	        float minX = transform.position.x;
	        float maxX = minX + _totalColumns * GridSize;
	        float minY = transform.position.y;
	        float maxY = minY + _totalRows * GridSize;
	        return (point.x >= minX && point.x <= maxX && point.y >= minY &&
	                point.y <= maxY);
	    }
	    public bool IsInsideGridBounds(int col, int row)
	    {
	        return (col >= 0 && col < _totalColumns && row >= 0 && row < _totalRows);
	    }
    }
}