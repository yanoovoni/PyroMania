using UnityEngine;

public class CullingMaskLib : MonoBehaviour {
    
    public void HideAllLayers() {
        gameObject.GetComponent<Camera>().cullingMask = 0;
    }
    
    public void ShowAllLayers() {
        gameObject.GetComponent<Camera>().cullingMask = int.MaxValue;
    }
    
    public void LayerCullingShow(int layerMask) {
        gameObject.GetComponent<Camera>().cullingMask |= layerMask;
    }
    
    public void LayerCullingShow(string layer) {
        LayerCullingShow(1 << LayerMask.NameToLayer(layer));
    }
    
    public void LayerCullingHide(int layerMask) {
        gameObject.GetComponent<Camera>().cullingMask &= ~layerMask;
    }
    
    public void LayerCullingHide(string layer) {
        LayerCullingHide(1 << LayerMask.NameToLayer(layer));
    }
    
    public void LayerCullingToggle(int layerMask) {
        gameObject.GetComponent<Camera>().cullingMask ^= layerMask;
    }
    
    public void LayerCullingToggle(string layer) {
        LayerCullingToggle(1 << LayerMask.NameToLayer(layer));
    }
    
    public bool LayerCullingIncludes(int layerMask) {
        return (gameObject.GetComponent<Camera>().cullingMask & layerMask) > 0;
    }
    
    public bool LayerCullingIncludes(string layer) {
        return LayerCullingIncludes(1 << LayerMask.NameToLayer(layer));
    }
    
    public void LayerCullingToggle(int layerMask, bool isOn) {
        bool included = LayerCullingIncludes(layerMask);
        if (isOn && !included) {
            LayerCullingShow(layerMask);
        } else if (!isOn && included) {
            LayerCullingHide(layerMask);
        }
    }
    
    public void LayerCullingToggle(string layer, bool isOn) {
        LayerCullingToggle(1 << LayerMask.NameToLayer(layer), isOn);
    }
}