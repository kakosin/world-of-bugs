﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackScreen : Bug {
    
    [Tooltip("Camera RenderTexture to apply the blackscreen to.")]
    public RenderTexture _cameraRenderTexture;
    [Tooltip("Bug Mask RenderTexture that will label black screen.")]
    public RenderTexture _bugMaskRenderTexture;
    
    public Vector2Int range = new Vector2Int(5, 50);
    protected int lastFrame = 0;
    protected int toWait = 0;

    protected FillScreen[] fillScreens;

    void Awake() { 
        fillScreens = new FillScreen[2];
        Camera[] cameras = null;
        cameras = CameraExtensions.GetCamerasByRenderTexture(_cameraRenderTexture);
        fillScreens[0] = cameras[0].gameObject.AddComponent<FillScreen>();
        fillScreens[0].color = new Color(0f,0f,0f,1f);
        
        cameras = CameraExtensions.GetCamerasByRenderTexture(_bugMaskRenderTexture);
        fillScreens[1] = cameras[0].gameObject.AddComponent<FillScreen>();
        fillScreens[1].color = (Color) gameObject.GetComponent<BugTag>().bugType;
    }

    void Update() { 
        if (Time.frameCount - lastFrame > toWait) {
            toWait = UnityEngine.Random.Range(range.x, range.y);
            lastFrame = Time.frameCount;
            toggle();
        }
    }

    void toggle() {
        foreach (FillScreen fs in fillScreens) {
            fs._enabled = ! fs._enabled;
        }
    }

    public override bool InView(Camera camera) { 
        return false;
    }

    public class FillScreen : MonoBehaviour {
        [HideInInspector]
        public bool _enabled = false;
        [HideInInspector]
        public Color color {
            get { return _color;}
            set { 
                _color = value;
                _solid = new Texture2D(1,1); // dont need a massive texture, let uv handle that!
                _solid.SetPixels(new Color[] { value });
                _solid.Apply();
            }
        }

        protected Color _color;
        protected Texture2D _solid;

        [HideInInspector]
        public RenderTexture renderTexture;
        
        public void OnRenderImage(RenderTexture src, RenderTexture dst) {
            if (_enabled) {
                Graphics.Blit(_solid, dst);
            }  else {
                Graphics.Blit(src, dst);
            } 
            
        }
    }
}
