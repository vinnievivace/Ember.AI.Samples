using System.Collections.Generic;
using EmberAI.Attributes;
using EmberAI.Avatars;
using EmberAI.Core;
using EmberAI.Core.Util;
using EmberAI.Futureverse.AssetRegistry;
using EmberAI.UI;
using Futureverse.AlteredState;
using TMPro;
using UnityEngine;

namespace EmberAI.Samples
{
    public class SamplesUI : EmberBehaviour
    {
        #region FIELDS /////////////////////////////////////////////////////////////////////////////////////////////////

        private const string Blurb = "TASM. The Altered State Machine. You are not ready....";

        [BoxGroup("Settings"), SerializeField]
        private AvatarConfig avatarConfigs;
       
        [BoxGroup("Settings"), SerializeField, Tooltip("Delay in seconds before toggling UI visible when no input is detected.")] 
        private float showUIDelay = 3;

        [BoxGroup("UI"), SerializeField]
        private ThumbnailSelector avatarSelector, brainSelector;

        [BoxGroup("UI"), SerializeField]
        private TextMeshProUGUI headerTXT;

        [BoxGroup("Components"), SerializeField]
        private GLBBehaviour _GLBTarget;

        [BoxGroup("Components"), SerializeField]
        private CharacterControllerSystem _characterController;
        
        [BoxGroup("Debug"), ReadOnly, SerializeField]
        private bool PanelsVisible;

        #endregion

        #region INSPECTOR TEST BUTTONS /////////////////////////////////////////////////////////////////////////////////

        #endregion

        #region MONOBEHAVIOURS /////////////////////////////////////////////////////////////////////////////////////////

        protected override void OnAwake()
        {
            base.OnAwake();

            SetHeaderText("Logging in to FuturePass...");
            
            // sometimes disable the canvas in edit mode for visibility, so ensure its enabled!
            GetComponent<Canvas>().enabled = true;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            
            bool shouldBeVisible = !InputUtil.AnyCurrentInput();
            bool tweenVisibility = true;

            if (ModalPopup.Instance.Active)
            {
                shouldBeVisible = false;
                tweenVisibility = false;
            } 

            if (shouldBeVisible != PanelsVisible)
            {
                // when we have no input, apply a delay before toggling the UI visible.
                if (shouldBeVisible)
                {
                    CallbackManager.AddOneOff(this, showUIDelay, () =>
                    {
                        avatarSelector.SetVisible(true);
                        brainSelector.SetVisible(true);
                    });
                }
                else
                {
                    avatarSelector.SetVisible(false, tweenVisibility);
                    brainSelector.SetVisible(false, tweenVisibility);
                }
                
                PanelsVisible = shouldBeVisible;
            }
        }

        private void OnEnable()
        {
            _GLBTarget.OnLoadComplete += GLBTargetOnOnLoadComplete;
            _GLBTarget.OnLoadError += GLBTargetOnLoadOnLoadError;

            avatarSelector.OnItemClicked += AvatarSelectorOnOnItemClicked;
            brainSelector.OnItemClicked += BrainSelectorOnOnItemClicked;
        }

        private void OnDisable()
        {
            _GLBTarget.OnLoadComplete -= GLBTargetOnOnLoadComplete;
            _GLBTarget.OnLoadError -= GLBTargetOnLoadOnLoadError;

            avatarSelector.OnItemClicked -= AvatarSelectorOnOnItemClicked;
            brainSelector.OnItemClicked -= BrainSelectorOnOnItemClicked;
        }

        #endregion

        #region PANEL & UI LOGIC ///////////////////////////////////////////////////////////////////////////////////////

        public void SetHeaderText(string message, int resetTime = 0)
        {
            headerTXT.text = $"<b><color=yellow>{message}</color></b>";

            if (resetTime > 0) CallbackManager.AddOneOff(this, resetTime, () => headerTXT.text = $"<color=white>{Blurb}</color>");
        }

        public void PopulateAvatarSelector(List<AssetItem> avatars)
        {
            foreach (AssetItem asset in avatars)
            {
                avatarSelector.AddItem(asset.TokenID, asset.ImagePath);
            }

            SetHeaderText("Assets Loaded", 5);
        }

        public void PopulateBrainSelector(List<AssetItem> brains)
        {
            foreach (AssetItem asset in brains)
            {
                brainSelector.AddItem(asset.TokenID, asset.TransparentImagePath);
            }

            SetHeaderText("Assets Loaded", 5);
        }

        #endregion

        #region EVENT HANDLERS /////////////////////////////////////////////////////////////////////////////////////////

        private void GLBTargetOnOnLoadComplete(string path)
        {
            SetHeaderText("Loaded", 1);
        }

        private void GLBTargetOnLoadOnLoadError(string error)
        {
            SetHeaderText(error, 5);
        }

        private void AvatarSelectorOnOnItemClicked(string tokenID)
        {
            SetHeaderText("Loading Avatar " + tokenID + "...");
            
            string selectedPath = SamplesManager.Instance.Avatars.Find(i => i.TokenID == tokenID).GLBPath;

            _GLBTarget.avatarConfig = avatarConfigs;
            _GLBTarget.LoadGLB(selectedPath);
        }
        
        private void BrainSelectorOnOnItemClicked(string tokenID)
        {
            SetHeaderText("Loading Brain " + tokenID + "...");
            
            //string selectedPath = TASMManager.Instance.Brains.Find(i => i.TokenID == tokenID).GLBPath;

            //Debug.Log(selectedPath);
            
            CallbackManager.AddOneOff(this, 1, () =>
            {
                GLBTargetOnOnLoadComplete("");

                ASAvatar avatar = FindAnyObjectByType<ASAvatar>();

                avatar.Brain = avatar.GetOrAddComponent<ASMBrain>();
                
                avatar.GetComponentInChildren<Animator>().SetBool("HasBrain", true);
                    
                
                
                Debug.LogError("fuck around, find out.");
            });
        }

        #endregion
    }
}
