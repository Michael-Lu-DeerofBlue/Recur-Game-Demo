// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers
{

    /// <summary>
    /// Methods to hide and show the cursor.
    /// </summary>
    public static class CursorControl
    {

        public static CursorLockMode cursorLockMode { get; set; } = CursorLockMode.Locked;

        public static bool isCursorActive
        {
            get { return isCursorVisible && !isCursorLocked; }
        }

        public static void SetCursorActive(bool value)
        {
            ShowCursor(value);
            LockCursor(!value);
        }

#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6

        public static bool isCursorVisible
        {
            get { return Screen.showCursor; }
        }

        public static bool isCursorLocked
        {
            get { return Screen.lockCursor; }
        }

        public static void ShowCursor(bool value)
        {
            Screen.showCursor = value;
        }

        public static void LockCursor(bool value)
        {
            Screen.lockCursor = value;
        }

#else
		
		public static bool isCursorVisible
		{
			get { return Cursor.visible; }
		}
		
		public static bool isCursorLocked
		{
			get { return Cursor.lockState != CursorLockMode.None; }
		}
		
		public static void ShowCursor(bool value) 
		{
			Cursor.visible = value;
		}
		
		public static void LockCursor(bool value) 
		{
			if (value == false && isCursorLocked) 
			{
				cursorLockMode = Cursor.lockState;
			}
			Cursor.lockState = value ? cursorLockMode : CursorLockMode.None;
		}
		
#endif

    }

}
