using PrimarchAssault.External;
using UnityEngine;
using Verse;

namespace RimworldModding
{
    public class HealthBarWindow: Window
    {
        public HealthBarWindow()
        {
            doWindowBackground = false;
            doCloseX = false;
            windowRect = new Rect(Current.Camera.scaledPixelWidth / (float)2 - 1000, 30, 2000, 150);
            absorbInputAroundWindow = false;
            closeOnAccept = false;
            closeOnCancel = false;
            closeOnClickedOutside = false;
            drawShadow = false;
            focusWhenOpened = false;
            preventCameraMotion = false;
            resizeable = false;
            layer = WindowLayer.GameUI;
        }

        public override Vector2 InitialSize => new Vector2(2000, 150);

        private float _healthPercent;
        private float _shieldPercent;
        public ChallengeDef ChallengeDef;
        public int CurrentPawn;

        private static readonly Color ShieldColor = new Color(.65f, .78f, 1f);
        private static readonly Color HealthColor = new Color(.81f, .24f, .12f);
        
        public override void DoWindowContents(Rect inRect)
        {

            Rect barRect = new Rect()
            {
                width = 1280,
                height = 128,
                center = inRect.center
            };
            /*{
                center = inRect.center
            };*/

            if (ChallengeDef.HealthBarIcon == null) return;
            GUI.DrawTexture(barRect, ChallengeDef.HealthBarIcon);
            Rect shieldBar = new Rect(barRect.xMin + 100, barRect.yMin + 75, 1080, 10);
            Rect healthBar = new Rect(barRect.xMin + 100, barRect.yMin + 93, 1080, 10);
            Widgets.DrawRectFast(shieldBar.LeftPart(_shieldPercent), ShieldColor);
            Widgets.DrawRectFast(healthBar.LeftPart(_healthPercent), HealthColor);
        }

        public void UpdateIfWilling(int championId, float healthPercent, float shieldPercent)
        {
            if (championId != CurrentPawn) return; 
            _healthPercent = healthPercent;
            _shieldPercent = shieldPercent;
        }
    }
}