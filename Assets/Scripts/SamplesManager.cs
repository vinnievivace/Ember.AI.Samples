using System.Collections.Generic;
using Core;
using EmberAI.Attributes;
using EmberAI.Futureverse.AssetRegistry;
using EmberAI.Futureverse.FuturePass;
using EmberAI.UI;
using UnityEngine;

namespace EmberAI.Samples
{
    public class SamplesManager : EmberSingleton<SamplesManager>
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
        private SamplesUI UI;
        
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
            AssetRegistryManager.Instance.OnError += OnErrorAssetRegistry;
        }

        private void OnDisable()
        {
            FPAuthManager.Instance.OnLoginComplete -= OnFPLoginComplete;
            AssetRegistryManager.Instance.OnAssetsLoaded -= OnAssetRegistryLoaded;
            AssetRegistryManager.Instance.OnError -= OnErrorAssetRegistry;
        }

        #endregion

        #region General ................................................................................................

        #endregion

        #region Event Handlers .........................................................................................

        private void OnFPLoginComplete()
        {
            UI.SetHeaderText("Retrieving FuturePass Assets...");
            
            AssetRegistryManager.Instance.GetAssets(FPAuthManager.Instance.WalletIDs, AssetRegistryManager.AlteredStateCollectionID);
            AssetRegistryManager.Instance.GetAssets(FPAuthManager.Instance.WalletIDs, AssetRegistryManager.ASMBrainCollectionID);
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
        
        private void OnErrorAssetRegistry(string error)
        {
            ModalPopup.Instance.Show("Asset Registry Error", error, "OK", () => { ModalPopup.Instance.Hide(); });
        }
        
        #endregion

#endregion
    }
}