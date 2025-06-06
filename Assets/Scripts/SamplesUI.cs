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
    
    private Vector2 _sidePanelOriginalPas, _bottomPanelOriginalPas;

    private bool _panelsVisible;
    
    private const string Blurb = "Modular framework providing tools for Metaverse / Blockchain / Extended Reality / AI experiences.";

    [BoxGroup("Settings"), SerializeField] 
    private Vector2 sidePanelHidePosition;
    
    [BoxGroup("Settings"), SerializeField] 
    private Vector2 bottomPanelHidePosition;
    
    [BoxGroup("UI"), SerializeField] 
    private RectTransform sidePanel;

    [BoxGroup("UI"), SerializeField] 
    private RectTransform bottomPanel;
    
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

    [BoxGroup("Components"), SerializeField]
    private CharacterControllerSystem _characterController;

    [BoxGroup("Avatar Configs"), SerializeField]
    private List<AvatarConfig> avatarConfigs;
    
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

        _sidePanelOriginalPas = sidePanel.anchoredPosition;
        _bottomPanelOriginalPas = bottomPanel.anchoredPosition;

        urlInput.text = "file:///Users/vinnievivace/Documents/Ember/Ember-SDK.com%20uploads/Samples/GLB/Ailu/1229.glb";

    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        SetPanelsVisible(!_characterController.HasInput);
        
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
        
        //AssetRegistryManager.Instance.OnAssetsLoaded -= OnARAssetsLoaded;
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

    public void SetPanelsVisible(bool visible, bool tween = true)
    {
        if (visible)
        {
            SetPanelPosition(sidePanel, _sidePanelOriginalPas, tween);
            SetPanelPosition(bottomPanel, _bottomPanelOriginalPas, tween);
        }
        else
        {
            SetPanelPosition(sidePanel, sidePanelHidePosition, tween);
            SetPanelPosition(bottomPanel, bottomPanelHidePosition, tween);
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
