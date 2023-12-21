using GameEngine.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace GameEngine.Core
{
	public class KeyPressVisual
	{
		public KeyCode Key;
		public RawImage Visual;
		public float PressTime;
	}

    public class KeyDrawer : MonoBehaviour
    {
		[SerializeField] private RawImage _keyUIPrefab;
		[SerializeField] private Transform _visualContainer;
		[SerializeField] private float _keySpacing = 25f;
		[SerializeField] private float _keyScaleRatio = 1f;

		private const string _resourcesKeyPath = "Keys/";
		// Holds keys that are pressed with press time.
		private List<KeyPressVisual> _keysPressed = new List<KeyPressVisual>();
		private bool _needsRedraw = false;

		private void Awake()
		{
			KeyListener.Instance.KeyGenericEvent.Register(OnKeyGenericTriggered);
		}

		private void Update()
		{
			if (_needsRedraw)
				Redraw();
		}

		private void OnKeyGenericTriggered(KeyEventValue keyEvent)
		{
			if (keyEvent.EventType == KeyEventType.KeyDown)
			{
				Debug.Log("Key Down: " + keyEvent.Key);

				var visual = Instantiate(_keyUIPrefab, _visualContainer);
				visual.texture = Resources.Load<Texture2D>(GetKeyResourcePath(keyEvent.Key));
				visual.SetNativeSize(_keyScaleRatio);
				_keysPressed.Add(new KeyPressVisual
				{
					Key = keyEvent.Key,
					PressTime = Time.time,
					Visual = visual
				});

				RequestRedraw();
			}
			else if (keyEvent.EventType == KeyEventType.KeyPress)
			{
				Debug.Log("Key Press: " + keyEvent.Key);
			}
			else if (keyEvent.EventType == KeyEventType.KeyUp)
			{
				Debug.Log("Key Up: " + keyEvent.Key);

				var index = _keysPressed.FindIndex(x => x.Key == keyEvent.Key);
				if (index >= 0)
				{
					var keyPressed = _keysPressed[index];
					_keysPressed.RemoveAt(index);

					Destroy(keyPressed.Visual.gameObject);

					RequestRedraw();
				}
			}
		}

		private void RequestRedraw()
		{
			_needsRedraw = true;
		}

		private void Redraw()
		{
			_needsRedraw = false;

			float lastEndPositionX = 0;
			float keySpacing = 0;
			for (int index = 0; index < _keysPressed.Count; index++)
			{
				var keyPressed = _keysPressed[index];
				var visualRT = keyPressed.Visual.GetComponent<RectTransform>();
				Vector2 newLocalPosition = new Vector2(lastEndPositionX - visualRT.sizeDelta.x * .5f - keySpacing, visualRT.sizeDelta.y * .5f);

				visualRT.transform.localPosition = newLocalPosition;

				lastEndPositionX -= visualRT.sizeDelta.x + keySpacing;
				keySpacing = _keySpacing * _keyScaleRatio;
			}
		}

		protected virtual string GetKeyResourcePath(KeyCode key)
		{
			return _resourcesKeyPath + GetKeyResourceName(key);
		}

		protected virtual string GetKeyResourceName(KeyCode key)
		{
			string resourceName = "KEY_" + key.ToString().ToUpper();
			return resourceName;
		}

		/*
		void OnGUI()
		{
			Event e = Event.current;
			if (e.isKey)
			{
				Debug.Log("Detected key code: " + e.keyCode);

				var texture = Resources.Load<Texture2D>(_resourcesKeyPath + "KEY_" + e.keyCode.ToString().ToUpper() + "");
				_testImage.texture = texture;
			}
		}
		*/
	}
}