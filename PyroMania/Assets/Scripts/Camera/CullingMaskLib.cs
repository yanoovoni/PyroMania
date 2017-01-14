using UnityEngine;

public class CullingMaskLib : MonoBehaviour {
    
    Camera cam;
    
    void Start() {
        cam = gameObject.GetComponent<Camera>();
    }
    
    public void HideAllLayers() {
        cam.cullingMask = 0;
    }
    
    public void ShowAllLayers() {
        cam.cullingMask = 1;
    }
    
    public void LayerCullingShow(int layerMask) {
        cam.cullingMask |= layerMask;
    }
    
    public void LayerCullingShow(string layer) {
        LayerCullingShow(1 << LayerMask.NameToLayer(layer));
    }
    
    public void LayerCullingHide(int layerMask) {
        cam.cullingMask &= ~layerMask;
    }
    
    public void LayerCullingHide(string layer) {
        LayerCullingHide(1 << LayerMask.NameToLayer(layer));
    }
    
    public void LayerCullingToggle(int layerMask) {
        cam.cullingMask ^= layerMask;
    }
    
    public void LayerCullingToggle(string layer) {
        LayerCullingToggle(1 << LayerMask.NameToLayer(layer));
    }
    
    public bool LayerCullingIncludes(int layerMask) {
        return (cam.cullingMask & layerMask) > 0;
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