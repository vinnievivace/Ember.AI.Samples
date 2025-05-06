using System;
using EmberAI;
using EmberAI.Attributes;
using EmberAI.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class SamplesUI : EmberBehaviour
{
    [BoxGroup("UI"), SerializeField] 
    private TMP_InputField urlInput;
    
    [BoxGroup("UI"), SerializeField] 
    private Button loadButton;

    [BoxGroup("Components"), SerializeField]
    private GLBBehaviour _GLBTarget;

    protected override void OnAwake()
    {
        base.OnAwake();
    
        urlInput.text = "https://ember-sdk.com/Samples/GLB/WalkerGenesis/5100.glb";
    }

    private void OnEnable()
    {
        loadButton.onClick.AddListener(OnClickLoad);
    }

    private void OnDisable()
    {
        loadButton.onClick.RemoveListener(OnClickLoad);
    }

    private void OnClickLoad()
    {
        _GLBTarget.LoadGLB(urlInput.text);
    }
}
