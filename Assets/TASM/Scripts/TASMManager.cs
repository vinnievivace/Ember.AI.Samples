using System.Collections.Generic;
using Core;
using EmberAI.Attributes;
using EmberAI.Core;
using EmberAI.Futureverse.AssetRegistry;
using EmberAI.Futureverse.FuturePass;
using EmberAI.UI;
using UnityEngine;

namespace TASM
{
    public class TASMManager : EmberSingleton<TASMManager>
    {
        #region EVENTS /////////////////////////////////////////////////////////////////////////////////////////////////        

        #endregion

        #region ENUMS //////////////////////////////////////////////////////////////////////////////////////////////////

        #endregion

        #region FIELDS /////////////////////////////////////////////////////////////////////////////////////////////////

        private Material _avatarMaterial;
        private Vector3 _lookatOffset;
        
        [BoxGroup("Settings"), SerializeField] 
        private Material _zombieAvatarMaterial;
        
        [BoxGroup("Settings"), SerializeField] 
        private Vector3 _zombieLookatOffset;
        
        [BoxGroup("Components"), SerializeField]
        private TASMUI UI;
        
        #endregion

        #region PROPERTIES /////////////////////////////////////////////////////////////////////////////////////////////           

        public List<AssetItem> Avatars { get; private set; }
        public List<AssetItem> Brains { get; private set; } 
        
        #endregion

        #region METHODS ////////////////////////////////////////////////////////////////////////////////////////////////

        #region Static .................................................................................................

        #endregion

        #region Inspector ..............................................................................................

        #endregion

        #region Initialization .........................................................................................

        #endregion

        #region MonoBehaviours .........................................................................................

        private void OnEnable()
        {
            FPAuthManager.Instance.OnLoginComplete += OnFPLoginComplete;
            AssetRegistryManager.Instance.OnAssetsLoaded += OnAssetRegistryLoaded;
        }

        private void OnDisable()
        {
            FPAuthManager.Instance.OnLoginComplete -= OnFPLoginComplete;
            AssetRegistryManager.Instance.OnAssetsLoaded -= OnAssetRegistryLoaded;
        }

        #endregion

        #region General ................................................................................................

        #endregion

        #region Event Handlers .........................................................................................

        private void OnFPLoginComplete()
        {
            UI.SetHeaderText("Retrieving FuturePass Assets...");
            
            AssetRegistryManager.Instance.GetAssets(AssetRegistryManager.AlteredStateCollectionID);
            AssetRegistryManager.Instance.GetAssets(AssetRegistryManager.ASMBrainCollectionID);
        }
        
        private void OnAssetRegistryLoaded(string collectionID, List<AssetItem> assets)
        {
            if(assets.Count == 0) return;
            
            if (collectionID == AssetRegistryManager.AlteredStateCollectionID)
            {
                Avatars = assets;
                
                UI.PopulateAvatarSelector(assets);
            }
            else if(collectionID == AssetRegistryManager.ASMBrainCollectionID)
            {
                Brains = assets;
                
                UI.PopulateBrainSelector(assets);
            }
            
        }
        
        #endregion

#endregion
    }
}