using System;
using System.Collections.Generic;
using EmberAI;
using EmberAI.Attributes;
using EmberAI.Attributes.EmberAI.Attributes;
using EmberAI.Avatars;
using EmberAI.Core;
using EmberAI.Futureverse.AssetRegistry;
using EmberAI.Futureverse.FuturePass;
using EmberAI.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SamplesUI : EmberBehaviour
{
    #region EVENTS /////////////////////////////////////////////////////////////////////////////////////////////////        

    #endregion

    #region ENUMS //////////////////////////////////////////////////////////////////////////////////////////////////

    #endregion

    #region FIELDS /////////////////////////////////////////////////////////////////////////////////////////////////

    private List<AssetItem> _assets;
    
    private const string Blurb = "Modular framework providing tools for Metaverse / Blockchain / Extended Reality / AI experiences.";

    [BoxGroup("UI"), SerializeField] 
    private ThumbnailSelector _thumbnailSelector;
    
    [BoxGroup("UI"), SerializeField] 
    private TextMeshProUGUI headerTXT;
    [BoxGroup("UI"), SerializeField] 
    private TMP_InputField urlInput;

    [BoxGroup("UI"), SerializeField] 
    private Button loadButton;

    [BoxGroup("Components"), SerializeField]
    private GLBBehaviour _GLBTarget;

    [BoxGroup("Avatar Configs"), SerializeField]
    private List<AvatarConfig> avatarConfigs;
    
    #endregion

    #region PROPERTIES /////////////////////////////////////////////////////////////////////////////////////////////           

    #endregion

    #region METHODS ////////////////////////////////////////////////////////////////////////////////////////////////

    #region Static .................................................................................................

    #endregion

    #region Inspector ..............................................................................................

    [ButtonGroup("Collections", "Load Goblins")]
    private void LoadGoblins()
    {
        AssetRegistryManager.Instance.GetAssets(AssetRegistryManager.GoblinCollectionID);
    }
    
    #endregion

    #region Initialization .........................................................................................

    
    
    #endregion

    #region MonoBehaviours .........................................................................................

    protected override void OnAwake()
    {
        base.OnAwake();
    
        SetHeaderText("Logging in to FuturePass...");
        
        
    }

    private void OnEnable()
    {
        loadButton.onClick.AddListener(OnClickLoad);
        
        _GLBTarget.OnLoadComplete += GLBTargetOnOnLoadComplete;
        _GLBTarget.OnLoadError += GLBTargetOnLoadOnLoadError;
        
        _thumbnailSelector.OnItemClicked += ThumbnailSelectorOnOnItemClicked;
        
        FPAuthManager.Instance.OnLoginComplete += OnLoggedIn;
        AssetRegistryManager.Instance.OnAssetsLoaded += OnARAssetsLoaded;
    }

    private void OnDisable()
    {
        loadButton.onClick.RemoveListener(OnClickLoad);
        
        _GLBTarget.OnLoadComplete -= GLBTargetOnOnLoadComplete;
        _GLBTarget.OnLoadError -= GLBTargetOnLoadOnLoadError;
        
        AssetRegistryManager.Instance.OnAssetsLoaded -= OnARAssetsLoaded;
    }
    
    #endregion

    #region General ................................................................................................

    private AvatarConfig GetAvatarConfig(string path)
    {
        foreach (AvatarConfig config in avatarConfigs)
        {
            if (path.ToLower().Contains(config.nameHint.ToLower())) return config;
        }

        throw new Exception("No Valid Avatar Config Found for " + path);
    }

    private void SetHeaderText(string message, int resetTime = 0)
    {
        headerTXT.text = $"<b><color=yellow>{message}</color></b>";
        
        if(resetTime > 0) CallbackManager.AddOneOff(this, resetTime, () => headerTXT.text = $"<color=white>{Blurb}</color>");
    }
    
    #endregion

    #region Event Handlers .........................................................................................

    private void OnClickLoad()
    {
        SetHeaderText("Loading...");
        
        _GLBTarget.avatarConfig = GetAvatarConfig(urlInput.text);
        _GLBTarget.LoadGLB(urlInput.text);
    }
    
    private void GLBTargetOnOnLoadComplete(string path)
    {
        SetHeaderText("Loaded", 1);
        urlInput.text = "";
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
            _thumbnailSelector.AddItem(asset.TokenID, asset.ImagePath);
        }
        
        SetHeaderText("Assets Loaded", 5);
    }
    
    private void ThumbnailSelectorOnOnItemClicked(string tokenID)
    {
        urlInput.text = _assets.Find(i => i.TokenID == tokenID).GLBPath;
        
        OnClickLoad();
    }
    
    #endregion

    #endregion
    
}
