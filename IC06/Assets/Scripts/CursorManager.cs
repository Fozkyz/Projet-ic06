using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private List<Texture2D> cursors;

	private void Start()
	{
		Cursor.SetCursor(cursors[0], Vector2.one * cursors[0].height / 2, CursorMode.ForceSoftware);
	}
}
