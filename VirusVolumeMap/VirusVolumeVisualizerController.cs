using UnityEngine;
using UnityEngine.Rendering;


class VirusVolumnVisualizerController:MonoBehaviour{

    public SpriteRenderer spriteRenderer;
    private int volume = 100;
    private float fadeRatioPerQ = 0.05f;

    private float maxVol = 100f;

    public void Start()
    {
        // this.gameObject.SetActive(false);
        TimeManager.AfterQuarterChanged += MaintainVolumeQ;
    }

    // public void Update(){
    //     // MaintainVolumeQ();
    // }

    public void SetVolume(int newVol){
        this.volume = newVol;
        // this.gameObject.SetActive(true);
    }
    public void MaintainVolumeQ((int,int) timeIn){
        
        if(volume == 0){return;}
        this.volume -= (int)(maxVol* fadeRatioPerQ);
        if(this.volume <= 0){
            this.volume = 0;
        }
        AdjustVisualizerAlpha();
    }
   public void AdjustVisualizerAlpha(){
        Color currentColor = spriteRenderer.color;
        float alpha = Mathf.Clamp01((volume / maxVol) * 0.5f);
        spriteRenderer.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
    }

}