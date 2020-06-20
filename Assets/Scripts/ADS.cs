using System.Collections.Generic;
using UnityEngine.Advertisements;
using System.Collections;
using UnityEngine;

public class ADS
    {
    #region ADS
    private string gameID = "3625139";
    private string bannerID = "TopBanner";
    
    private bool testMode = true;
    public IEnumerator ShowBannerWhenReady()
    {
        while (!Advertisement.IsReady(bannerID))
        {
            yield return new WaitForSeconds(0.5f);
        }
        Advertisement.Banner.SetPosition(BannerPosition.TOP_CENTER);
        Advertisement.Banner.Show(bannerID);
    }
    public ADS()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            //Advertisement.Initialize(gameID, testMode);
            //StartCoroutine(ShowBannerWhenReady());  
        }
    }
    #endregion
}

