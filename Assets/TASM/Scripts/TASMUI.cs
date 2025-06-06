using System.Collections.Generic;
using EmberAI;
using EmberAI.Attributes;
using EmberAI.Attributes.EmberAI.Attributes;
using EmberAI.Avatars;
using EmberAI.Core;
using EmberAI.Core.Util;
using EmberAI.Futureverse.AssetRegistry;
using EmberAI.Futureverse.FuturePass;
using EmberAI.UI;
using TMPro;
using UnityEngine;

namespace TASM
{
    public class TASMUI : EmberBehaviour
    {
        #region EVENTS /////////////////////////////////////////////////////////////////////////////////////////////////        

        #endregion

        #region ENUMS //////////////////////////////////////////////////////////////////////////////////////////////////

        #endregion

        #region FIELDS /////////////////////////////////////////////////////////////////////////////////////////////////

        private List<AssetItem> _assets;
        
        private Vector2 _avatarPanelOriginalPos, _brainPanelOriginalPos;

        private bool _panelsVisible;
        
        private const string Blurb = "TASM. The Altered State Machine. You are not ready....";

        [BoxGroup("Settings"), SerializeField]
        private AvatarConfig avatarConfigs;
        
        [BoxGroup("Settings"), SerializeField] 
        private Vector2 avatarPanelHidePosition, brainPanelHidePosition;
        
        [BoxGroup("UI"), SerializeField] 
        private ThumbnailSelector avatarSelector, brainSelector;
        
        [BoxGroup("UI"), SerializeField] 
        private TextMeshProUGUI headerTXT;
        
        [BoxGroup("Components"), SerializeField]
        private GLBBehaviour _GLBTarget;

        [BoxGroup("Components"), SerializeField]
        private CharacterControllerSystem _characterController;

        
        
        #endregion

        #region PROPERTIES /////////////////////////////////////////////////////////////////////////////////////////////           

        #endregion

        #region METHODS ////////////////////////////////////////////////////////////////////////////////////////////////

        #region Static .................................................................................................

        #endregion

        #region Inspector ..............................................................................................

        [ButtonGroup("Tests", "Show Panels")]
        private void ShowPanels()
        {
            SetPanelsVisible(true);
        }
        
        [ButtonGroup("Tests", "Hide Panels")]
        private void HidePanels()
        {
            SetPanelsVisible(false);
        }
        
        #endregion

        #region Initialization .........................................................................................

        
        
        #endregion

        #region MonoBehaviours .........................................................................................

        protected override void OnAwake()
        {
            base.OnAwake();
        
            SetHeaderText("Logging in to FuturePass...");

            _avatarPanelOriginalPos = avatarSelector.RectTransform.anchoredPosition;
            _brainPanelOriginalPos = brainSelector.RectTransform.anchoredPosition;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            SetPanelsVisible(!InputUtil.AnyCurrentInput());
            
        }

        private void OnEnable()
        {
            _GLBTarget.OnLoadComplete += GLBTargetOnOnLoadComplete;
            _GLBTarget.OnLoadError += GLBTargetOnLoadOnLoadError;
            
            avatarSelector.OnItemClicked += AvatarSelectorOnOnItemClicked;
            
            FPAuthManager.Instance.OnLoginComplete += OnLoggedIn;
            AssetRegistryManager.Instance.OnAssetsLoaded += OnARAssetsLoaded;
        }

        private void OnDisable()
        {
            _GLBTarget.OnLoadComplete -= GLBTargetOnOnLoadComplete;
            _GLBTarget.OnLoadError -= GLBTargetOnLoadOnLoadError;
            
            //AssetRegistryManager.Instance.OnAssetsLoaded -= OnARAssetsLoaded;
        }
        
        #endregion

        #region General ................................................................................................

        private void SetHeaderText(string message, int resetTime = 0)
        {
            headerTXT.text = $"<b><color=yellow>{message}</color></b>";
            
            if(resetTime > 0) CallbackManager.AddOneOff(this, resetTime, () => headerTXT.text = $"<color=white>{Blurb}</color>");
        }

        
        // move to ui behaviour
        public void SetPanelsVisible(bool visible, bool tween = true)
        {
            if (visible)
            {
                SetPanelPosition(avatarSelector.RectTransform, _avatarPanelOriginalPos, tween);
                SetPanelPosition(brainSelector.RectTransform, _brainPanelOriginalPos, tween);;
            }
            else
            {
                SetPanelPosition(avatarSelector.RectTransform, avatarPanelHidePosition, tween);
                SetPanelPosition(brainSelector.RectTransform, brainPanelHidePosition, tween);
            }
        }

        private void SetPanelPosition(RectTransform panel, Vector2 position, bool tween = true)
        {
            if (tween)
            {
                TweenUtil.TweenVector2(panel.anchoredPosition, position, 0.5f, position => { panel.anchoredPosition = position;}, () => { });
            }
            else
            {
                panel.anchoredPosition = position;
            }
        }

        #endregion

        #region Event Handlers .........................................................................................

        private void GLBTargetOnOnLoadComplete(string path)
        {
            SetHeaderText("Loaded", 1);
        }
        
        private void GLBTargetOnLoadOnLoadError(string error)
        {
            SetHeaderText(error, 5);
        }
        
        private void OnLoggedIn()
        {
            SetHeaderText("Retrieving FuturePass Assets...");
            
            AssetRegistryManager.Instance.GetAssets(AssetRegistryManager.AlteredStateCollectionID);
        }
        
        private void OnARAssetsLoaded(List<AssetItem> assets)
        {
            _assets = assets;
            
            foreach (AssetItem asset in assets)
            {
                avatarSelector.AddItem(asset.TokenID, asset.ImagePath);
            }
            
            SetHeaderText("Assets Loaded", 5);
        }
        
        private void AvatarSelectorOnOnItemClicked(string tokenID)
        {
            SetHeaderText("Loading...");
            
            string selectedPath = _assets.Find(i => i.TokenID == tokenID).GLBPath;
            
            _GLBTarget.avatarConfig = avatarConfigs;
            _GLBTarget.LoadGLB(selectedPath);
        }
        
        #endregion

        #endregion
        
    }
}