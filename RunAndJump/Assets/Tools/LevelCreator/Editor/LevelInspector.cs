﻿using System.Collections.Generic;
using Assets.Scripts.Level;
using Assets.Tools.LevelCreator.Scripts;
using RunAndJump;
using UnityEditor;
using UnityEngine;

namespace Assets.Tools.LevelCreator.Editor
{
    [CustomEditor(typeof(Level))]
    public class LevelInspector : UnityEditor.Editor
    {
        private Level _myTarget;
        private int _newTotalColumns;
        private int _newTotalRows;

        private SerializedObject _mySerializedObject;
        private SerializedProperty _serializedTotalTime;

        private PaletteItem _itemSelected;
        private Texture2D _itemPreview;
        private LevelPiece _pieceSelected;

        private int _originalPosX;
        private int _originalPosY;

        private GUIStyle _titleStyle;

        public enum Mode
        {
            View,
            Paint,
            Edit,
            Erase,
        }

        private Mode _selectedMode;
        private Mode _currentMode;

        private void OnEnable()
        {
            Debug.Log("OnEnable was called...");
            _myTarget = (Level)target;

            InitLevel();
            ResetResizeValues();
            SubscribeEvents();
            InitStyles();
        }

        private void OnDisable()
        {
            UnsubscribeEvents();
        }

        private void OnDestroy()
        {
            Debug.Log("OnDestroy was called...");
        }

        public override void OnInspectorGUI()
        {
            //DrawDefaultInspector();
            DrawLevelDataGUI();
            DrawLevelSizeGUI();
            DrawPieceSelectedGUI();
            DrawInspectedItemGUI();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(_myTarget);
            }
        }

        private void OnSceneGUI()
        {
            DrawModeGUI();
            ModeHandler();
            EventHandler();
        }

        private void InitLevel()
        {
            _mySerializedObject = new SerializedObject(_myTarget);
            _serializedTotalTime = _mySerializedObject.FindProperty("_totalTime");

            _myTarget.transform.hideFlags = HideFlags.NotEditable;

            if (_myTarget.Pieces == null || _myTarget.Pieces.Length == 0)
            {
                Debug.Log("Initializing the Pieces array...");
                _myTarget.Pieces = new LevelPiece[_myTarget.TotalColumns *
                                                  _myTarget.TotalRows];
            }
        }
        private void InitStyles()
        {
            //_titleStyle = new GUIStyle();
            //_titleStyle.alignment = TextAnchor.MiddleCenter;
            //_titleStyle.fontSize = 16;

            //Texture2D titleBg = (Texture2D)Resources.Load("Color_Bg");
            //Font titleFont = (Font)Resources.Load("Oswald-Regular");
            //_titleStyle.normal.background = titleBg;
            //_titleStyle.normal.textColor = Color.white;
            //_titleStyle.font = titleFont;

            GUISkin skin = (GUISkin)Resources.Load("LevelCreatorSkin");
            _titleStyle = skin.label;
        }

        private void Move()
        {
            Vector3 gridPoint = _myTarget.WorldToGridCoordinates(_itemInspected.transform.position);
            int col = (int)gridPoint.x;
            int row = (int)gridPoint.y;

            if (col == _originalPosX && row == _originalPosY)
            {
                return;
            }

            if (!_myTarget.IsInsideGridBounds(col, row) || _myTarget.Pieces[col + row * _myTarget.TotalColumns] != null)
            {
                _itemInspected.transform.position = _myTarget.GridToWorldCoordinates(_originalPosX, _originalPosY);
            }
            else
            {
                _myTarget.Pieces[_originalPosX + _originalPosY * _myTarget.TotalColumns] = null;
                _myTarget.Pieces[col + row * _myTarget.TotalColumns] = _itemInspected.GetComponent<LevelPiece>();
                _myTarget.Pieces[col + row * _myTarget.TotalColumns].transform.position = _myTarget.GridToWorldCoordinates(col, row);
            }
        }

        private void DrawLevelDataGUI()
        {
            EditorGUILayout.LabelField("Data", _titleStyle);

            EditorGUILayout.BeginVertical("box");
                // _myTarget.TotalTime = EditorGUILayout.IntField("Total Time", Mathf.Max(0, _myTarget.TotalTime));
                EditorGUILayout.PropertyField(_serializedTotalTime);

                _myTarget.Settings = (LevelSettings)EditorGUILayout.ObjectField("Level Settings", _myTarget.Settings, typeof(LevelSettings), false);
                if (_myTarget.Settings != null)
                    UnityEditor.Editor.CreateEditor(_myTarget.Settings).OnInspectorGUI();
                else
                    EditorGUILayout.HelpBox("You must attach a LevelSettings asset!", MessageType.Warning);
            EditorGUILayout.EndVertical();

            _myTarget.SetGravity();
        }

        private void DrawLevelSizeGUI()
        {
            EditorGUILayout.LabelField("Size", _titleStyle);
            EditorGUILayout.BeginHorizontal("box");

                EditorGUILayout.BeginVertical();
                    _newTotalColumns = EditorGUILayout.IntField("Columns", Mathf.Max (1, _newTotalColumns));
                    _newTotalRows = EditorGUILayout.IntField("Rows", Mathf.Max(1, _newTotalRows));
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();
                    bool oldEnabled = GUI.enabled;
                    GUI.enabled = (_newTotalColumns != _myTarget.TotalColumns || _newTotalRows != _myTarget.TotalRows);
                    bool buttonResize = GUILayout.Button("Resize", GUILayout.Height(2 * EditorGUIUtility.singleLineHeight));

                    if (buttonResize)
                    {
                        if (EditorUtility.DisplayDialog(
                            "Level Creator",
                            "Are you sure you want to resize the level?\nThis action cannot be undone.",
                            "Yes",
                            "No"))
                        {
                            ResizeLevel();
                        }
                    }
                    bool buttonReset = GUILayout.Button("Reset");
                    if (buttonReset)
                    {
                        ResetResizeValues();
                    }
                    GUI.enabled = oldEnabled;
                EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        private void ResetResizeValues()
        {
            _newTotalColumns = _myTarget.TotalColumns;
            _newTotalRows = _myTarget.TotalRows;
        }

        private void ResizeLevel()
        {
            LevelPiece[] newPieces = new LevelPiece[_newTotalColumns * _newTotalRows];
            for (int col = 0; col < _myTarget.TotalColumns; ++col)
            {
                for (int row = 0; row < _myTarget.TotalRows; ++row)
                {
                    if (col < _newTotalColumns && row < _newTotalRows)
                    {
                        newPieces[col + row * _newTotalColumns] =
                            _myTarget.Pieces[col + row * _myTarget.
                                                 TotalColumns];
                    }
                    else
                    {
                        LevelPiece piece = _myTarget.Pieces[col + row * _myTarget.TotalColumns];
                        if (piece != null)
                        {
                            // we must to use DestroyImmediate in a Editor context
                            Object.DestroyImmediate(piece.gameObject);
                        }
                    }
                }
            }
            _myTarget.Pieces = newPieces;
            _myTarget.TotalColumns = _newTotalColumns;
            _myTarget.TotalRows = _newTotalRows;
        }

        private void UpdateCurrentPieceInstance(PaletteItem item, Texture2D preview)
        {
            _itemSelected = item;
            _itemPreview = preview;
            _pieceSelected = (LevelPiece)item.GetComponent<LevelPiece>();
            Repaint();
        }

        private void DrawPieceSelectedGUI()
        {
            EditorGUILayout.LabelField("Piece Selected", EditorStyles.boldLabel);
            if (_pieceSelected == null)
            {
                EditorGUILayout.HelpBox("No piece selected!", MessageType.
                    Info);
            }
            else
            {
                EditorGUILayout.BeginVertical("box");
                    EditorGUILayout.LabelField(new GUIContent(_itemPreview), GUILayout.Height(40));
                    EditorGUILayout.LabelField(_itemSelected.itemName);
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawModeGUI()
        {
            List<Mode> modes = EditorUtils.GetListFromEnum<Mode>();
            List<string> modeLabels = new List<string>();
            foreach (Mode mode in modes)
            {
                modeLabels.Add(mode.ToString());
            }
            Handles.BeginGUI();
            GUILayout.BeginArea(new Rect(10f, 10f, 360, 40f));
            _selectedMode = (Mode)GUILayout.Toolbar(
                (int)_currentMode,
                modeLabels.ToArray(),
                GUILayout.ExpandHeight(true));
            GUILayout.EndArea();
            Handles.EndGUI();
        }

        private void ModeHandler()
        {
            switch (_selectedMode)
            {
                case Mode.Paint:
                case Mode.Edit:
                case Mode.Erase:
                    UnityEditor.Tools.current = Tool.None;
                    break;
                case Mode.View:
                default:
                    UnityEditor.Tools.current = Tool.View;
                    break;
            }
            // Detect Mode change
            if (_selectedMode != _currentMode)
            {
                _currentMode = _selectedMode;

                _itemInspected = null;
                Repaint();
            }
            // Force 2D Mode!
            SceneView.currentDrawingSceneView.in2DMode = true;
        }

        private void EventHandler()
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            Camera camera = SceneView.currentDrawingSceneView.camera;
            Vector3 mousePosition = Event.current.mousePosition;
            mousePosition = new Vector2(mousePosition.x, camera.pixelHeight - mousePosition.y);
            //Debug.LogFormat("MousePos: {0}", mousePosition);
            Vector3 worldPos = camera.ScreenToWorldPoint(mousePosition);
            Vector3 gridPos = _myTarget.WorldToGridCoordinates(worldPos);
            int col = (int)gridPos.x;
            int row = (int)gridPos.y;


            switch (_currentMode)
            {
                case Mode.Paint:
                    if (Event.current.type == EventType.MouseDown ||
                        Event.current.type == EventType.MouseDrag)
                    {
                        Paint(col, row);
                    }
                    break;
                case Mode.Edit:
                    if (Event.current.type == EventType.MouseDown)
                    {
                        _originalPosX = col;
                        _originalPosY = row;
                        Edit(col, row);
                    }
                    if (Event.current.type == EventType.MouseUp || Event.current.type == EventType.Ignore)
                    {
                        if (_itemInspected != null)
                        {
                            Move();
                        }
                    }

                    if (_itemInspected != null)
                    {
                        _itemInspected.transform.position = Handles.FreeMoveHandle(
                                _itemInspected.transform.position,
                                _itemInspected.transform.rotation,
                                Level.GridSize / 2,
                                Level.GridSize / 2 * Vector3.one,
                                Handles.RectangleCap);
                    }
                    break;
                case Mode.Erase:
                    if (Event.current.type == EventType.MouseDown ||
                        Event.current.type == EventType.MouseDrag)
                    {
                        Erase(col, row);
                    }
                    break;
                case Mode.View:
                default:
                    break;
            }
        }

        private void Paint(int col, int row)
        {
            // Check out of bounds and if we have a piece selected
            if (!_myTarget.IsInsideGridBounds(col, row) || _pieceSelected == null)
                return;

            // Check if I need to destroy a previous piece
            if (_myTarget.Pieces[col + row * _myTarget.TotalColumns] != null)
            {
                DestroyImmediate(_myTarget.Pieces[col + row * _myTarget.TotalColumns].gameObject);
            }
            // Do paint !
            GameObject obj = PrefabUtility.InstantiatePrefab(_pieceSelected.gameObject) as GameObject;
            obj.transform.parent = _myTarget.transform;
            obj.name = string.Format("[{0},{1}][{2}]", col, row, obj.name);
            obj.transform.position = _myTarget.GridToWorldCoordinates(col, row);
            obj.hideFlags = HideFlags.HideInHierarchy;

            _myTarget.Pieces[col + row * _myTarget.TotalColumns] = obj.GetComponent<LevelPiece>();
        }
        private void Erase(int col, int row)
        {
            // Check out of bounds
            if (!_myTarget.IsInsideGridBounds(col, row))
            {
                return;
            }
            // Do Erase
            if (_myTarget.Pieces[col + row * _myTarget.TotalColumns] != null)
            {
                DestroyImmediate(_myTarget.Pieces[col + row * _myTarget.TotalColumns].gameObject);
            }
        }

        private PaletteItem _itemInspected;
        private void Edit(int col, int row)
        {
            // Check out of bounds
            if (!_myTarget.IsInsideGridBounds(col, row) || _myTarget.Pieces[col + row * _myTarget.TotalColumns] == null)
            {
                _itemInspected = null;
            }
            else
            {
                _itemInspected = _myTarget.Pieces[col + row * _myTarget.TotalColumns].GetComponent<PaletteItem>() as PaletteItem;
            }

            Repaint();
        }

        private void DrawInspectedItemGUI()
        {
            // Only show this GUI if we are in edit mode.
            if (_currentMode != Mode.Edit)
            {
                return;
            }
            //EditorGUILayout.LabelField ("Piece Edited", _titleStyle);
            EditorGUILayout.LabelField("Piece Edited", EditorStyles.boldLabel);
            if (_itemInspected != null)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Name: " + _itemInspected.name);
                UnityEditor.Editor.CreateEditor(_itemInspected.inspectedScript).OnInspectorGUI();
                EditorGUILayout.EndVertical();
            }
            else
            {
                EditorGUILayout.HelpBox("No piece to edit!",
                    MessageType.Info);
            }
        }

        private void SubscribeEvents()
        {
            PaletteWindow.ItemSelectedEvent += new PaletteWindow.itemSelectedDelegate(UpdateCurrentPieceInstance);
        }

        private void UnsubscribeEvents()
        {
            PaletteWindow.ItemSelectedEvent -= new PaletteWindow.itemSelectedDelegate(UpdateCurrentPieceInstance);
        }
    }
}