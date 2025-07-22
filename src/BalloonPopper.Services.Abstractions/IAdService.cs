namespace BalloonPopper.Services.Abstractions;

public interface IAdService
{
    bool IsPremiumUser { get; }
    event EventHandler<bool>? PremiumStatusChanged;
    
    void ShowBannerAd();
    void HideBannerAd();
    Task<bool> ShowInterstitialAdAsync();
    Task<bool> ShowRewardedVideoAdAsync();
    Task<bool> PurchasePremiumAsync();
}
