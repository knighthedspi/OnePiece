//
//  ImobileSdkAdsIosUnityPluginImpl.mm
//
//  Copyright 2014 i-mobile Co.Ltd. All rights reserved.
//

#import "ImobileSdkAdsIosUnityPluginImpl.h"
#import "ImobileSdkAds/ImobileSdkAdsIconParams.h"

#ifdef __cplusplus
extern "C" {
#endif
	void UnitySendMessage(const char* obj, const char* method, const char* msg);
#ifdef __cplusplus
}
#endif


@interface ImobileSdkAdsIosUnityPluginImpl ()

@end


@implementation ImobileSdkAdsIosUnityPluginImpl

static const NSMutableSet *gameObjectNames = [NSMutableSet set];
static const NSMutableDictionary *adViewIdDictionary = [NSMutableDictionary dictionary];

extern UIViewController *UnityGetGLViewController();

// ----------------------------------------
#pragma mark - Call from inner C++
// ----------------------------------------

- (void)addObserver:(const char*)gameObjectName {
    [gameObjectNames addObject:[NSString stringWithUTF8String:gameObjectName]];
}

- (void)removeObserver:(const char*)gameObjectName {
    [gameObjectNames removeObject:[NSString stringWithUTF8String:gameObjectName]];
}

- (void)registerWithPublisherID:(const char*)publisherId MediaID:(const char*)mediaId SoptID:(const char*)soptId {
    [ImobileSdkAds registerWithPublisherID:[NSString stringWithUTF8String:publisherId]
                                   MediaID:[NSString stringWithUTF8String:mediaId]
                                    SpotID:[NSString stringWithUTF8String:soptId]];
    
    [ImobileSdkAds setSpotDelegate:[NSString stringWithUTF8String:soptId] delegate:self];
}

- (void)start {
    [ImobileSdkAds start];
}

- (void)stop {
    [ImobileSdkAds stop];
}

- (bool)startBySpotID:(const char*)spotId {
    return [ImobileSdkAds startBySpotID:[NSString stringWithUTF8String:spotId]];
}

- (bool)stopBySpotID:(const char*)spotId {
    return [ImobileSdkAds stopBySpotID:[NSString stringWithUTF8String:spotId]];
}

- (bool)showBySpotID:(const char*)spotId {
    return [ImobileSdkAds showBySpotID:[NSString stringWithUTF8String:spotId]];
}

- (bool)showBySpotID:(const char*)spotId PublisherID:(const char*)publisherId MediaID:(const char*)mediaId Left:(int)left Top:(int)top Width:(int)width Height:(int)height iconNumber:(int)iconNumber iconViewLayoutWidth:(int)iconViewLayoutWidth iconSize:(int)iconSize iconTitleEnable:(bool)iconTitleEnable iconTitleFontSize:(int)iconTitleFontSize iconTitleFontColor:(const char*)iconTitleFontColor iconTitleOffset:(int)iconTitleOffset iconTitleShadowEnable:(bool)iconTitleShadowEnable iconTitleShadowColor:(const char*)iconTitleShadowColor iconTitleShadowDx:(int)iconTitleShadowDx iconTitleShadowDy:(int)iconTitleShadowDy adViewId:(int)adViewId {
    
    // create iconParams
    ImobileSdkAdsIconParams *params = [[ImobileSdkAdsIconParams alloc] init];
    params.iconNumber = iconNumber;
    params.iconViewLayoutWidth = iconViewLayoutWidth;
    params.iconSize = iconSize;
    params.iconTitleEnable = iconTitleEnable;
    params.iconTitleFontSize = iconTitleFontSize;
    params.iconTitleFontColor = [NSString stringWithUTF8String:iconTitleFontColor];
    params.iconTitleOffset = iconTitleOffset;
    params.iconTitleShadowEnable = iconTitleShadowEnable;
    params.iconTitleShadowColor = [NSString stringWithUTF8String:iconTitleShadowColor];
    params.iconTitleShadowDx = iconTitleShadowDx;
    params.iconTitleShadowDy = iconTitleShadowDy;
    
    // resister
    [self registerWithPublisherID:publisherId MediaID:mediaId SoptID:spotId];
    
    // start
    [self startBySpotID:spotId];
    
    // show
    UIView *adContainerView = [[UIView alloc] initWithFrame:CGRectMake(left, top, width, height)];
    [adViewIdDictionary setObject:adContainerView forKey:[NSString stringWithFormat:@"%d", adViewId]];
    [UnityGetGLViewController().view addSubview:adContainerView];
    
    return [ImobileSdkAds showBySpotID:[NSString stringWithUTF8String:spotId] View:adContainerView IconPrams:params];
}

- (void)setAdOrientation:(ImobileSdkAdsAdOrientation)orientation {
    [ImobileSdkAds setAdOrientation:orientation];
}

- (void)setAdView:(int)adViewId visible:(bool)visible {
    UIView *adContainerView = [adViewIdDictionary objectForKey:[NSString stringWithFormat:@"%d", adViewId]];
    adContainerView.hidden = !visible;
}

- (void)setLegacyIosSdkMode:(bool)isLegacyMode {
    [ImobileSdkAds setLegacyIosSdkMode:isLegacyMode];
}

- (float)getScreenScale {
    float iOSVersion = [[[UIDevice currentDevice] systemVersion] floatValue];
    if (iOSVersion < 8) {
        return [UIScreen mainScreen].scale;
    } else {
        return [[[UIScreen mainScreen] valueForKey: @"nativeScale"] floatValue];
    }
}

// ----------------------------------------
#pragma mark Call from ImobileSdkAds
// ----------------------------------------

- (void)imobileSdkAdsSpot:(NSString *)spotId didReadyWithValue:(ImobileSdkAdsReadyResult)value {
    NSString *msg = [NSString stringWithFormat:@"%@", spotId];
    
    for (NSString *gameObjectName in gameObjectNames ) {
        UnitySendMessage([gameObjectName UTF8String],
                         [@"imobileSdkAdsSpotDidReady" UTF8String],
                         [msg UTF8String]);
    }
}

- (void)imobileSdkAdsSpot:(NSString *)spotId didFailWithValue:(ImobileSdkAdsFailResult)value {
    NSString *msg = [NSString stringWithFormat:@"%@,%d", spotId,value];
    
    for (NSString *gameObjectName in gameObjectNames ) {
        UnitySendMessage([gameObjectName UTF8String],
                         [@"imobileSdkAdsSpotDidFail" UTF8String],
                         [msg UTF8String]);
    }
}

- (void)imobileSdkAdsSpotIsNotReady:(NSString *)spotId {
    NSString *msg = [NSString stringWithFormat:@"%@", spotId];
    
    for (NSString *gameObjectName in gameObjectNames ) {
        UnitySendMessage([gameObjectName UTF8String],
                         [@"imobileSdkAdsSpotIsNotReady" UTF8String],
                         [msg UTF8String]);
    }
}

- (void)imobileSdkAdsSpotDidClick:(NSString *)spotId {
    NSString *msg = [NSString stringWithFormat:@"%@", spotId];
    
    for (NSString *gameObjectName in gameObjectNames ) {
        UnitySendMessage([gameObjectName UTF8String],
                         [@"imobileSdkAdsSpotDidClick" UTF8String],
                         [msg UTF8String]);
    }
}

- (void)imobileSdkAdsSpotDidClose:(NSString *)spotId {
    NSString *msg = [NSString stringWithFormat:@"%@", spotId];
    
    for (NSString *gameObjectName in gameObjectNames ) {
        UnitySendMessage([gameObjectName UTF8String],
                         [@"imobileSdkAdsSpotDidClose" UTF8String],
                         [msg UTF8String]);
    }
}


// ----------------------------------------
#pragma mark - Call from Unity
// ----------------------------------------

#ifdef __cplusplus
extern "C" {
#endif
    static const ImobileSdkAdsIosUnityPluginImpl *unityPlugin = NULL;
    
    void imobileAddObserver_(const char* gameObjectName) {
        if (!unityPlugin) {
            unityPlugin = [[ImobileSdkAdsIosUnityPluginImpl alloc] init];
        }
        [unityPlugin addObserver:gameObjectName];
    }
    
    void imobileRemoveObserver_(const char* gameObjectName) {
        if (!unityPlugin) {
            unityPlugin = [[ImobileSdkAdsIosUnityPluginImpl alloc] init];
        }
        [unityPlugin removeObserver:gameObjectName];
    }
    
    void imobileRegisterWithPublisherID_(const char* publisherId, const char* mediaId, const char* soptId) {
        if (!unityPlugin) {
            unityPlugin = [[ImobileSdkAdsIosUnityPluginImpl alloc] init];
        }
        [unityPlugin registerWithPublisherID:publisherId
                                     MediaID:mediaId
                                      SoptID:soptId];
    }
    
    void imobileStart_() {
        if (!unityPlugin) {
            unityPlugin = [[ImobileSdkAdsIosUnityPluginImpl alloc] init];
        }
        [unityPlugin start];
    }
    
    void imobileStop_() {
        if (!unityPlugin) {
            unityPlugin = [[ImobileSdkAdsIosUnityPluginImpl alloc] init];
        }
        [unityPlugin stop];
    }
    
    bool imobileStartBySpotID_(const char* spotId){
        if (!unityPlugin) {
            unityPlugin = [[ImobileSdkAdsIosUnityPluginImpl alloc] init];
        }
        return [unityPlugin startBySpotID:spotId];
    }
    
    bool imobileStopBySpotID_(const char* spotId) {
        if (!unityPlugin) {
            unityPlugin = [[ImobileSdkAdsIosUnityPluginImpl alloc] init];
        }
        return [unityPlugin stopBySpotID:spotId];
    }
    
    bool imobileShowBySpotID_(const char* spotId, int adViewId) {
        if (!unityPlugin) {
            unityPlugin = [[ImobileSdkAdsIosUnityPluginImpl alloc] init];
        }
        return [unityPlugin showBySpotID:spotId];
    }
    
    bool imobileShowBySpotIDWithPositionAndIconParams_(const char* spotId,
                                                       const char* publisherId,
                                                       const char* mediaId,
                                                       int left,
                                                       int top,
                                                       int width,
                                                       int height,
                                                       int iconNumber,
                                                       int iconViewLayoutWidth,
                                                       int iconSize,
                                                       bool iconTitleEnable,
                                                       int iconTitleFontSize,
                                                       const char* iconTitleFontColor,
                                                       int iconTitleOffset,
                                                       bool iconTitleShadowEnable,
                                                       const char* iconTitleShadowColor,
                                                       int iconTitleShadowDx,
                                                       int iconTitleShadowDy,
                                                       int adViewId) {
        if (!unityPlugin) {
            unityPlugin = [[ImobileSdkAdsIosUnityPluginImpl alloc] init];
        }
        return [unityPlugin showBySpotID:spotId
                             PublisherID:publisherId
                                 MediaID:mediaId
                                    Left:left
                                     Top:top
                                   Width:width
                                  Height:height
                              iconNumber:iconNumber
                     iconViewLayoutWidth:iconViewLayoutWidth
                                iconSize:iconSize
                         iconTitleEnable:iconTitleEnable
                       iconTitleFontSize:iconTitleFontSize
                      iconTitleFontColor:iconTitleFontColor
                         iconTitleOffset:iconTitleOffset
                   iconTitleShadowEnable:iconTitleShadowEnable
                    iconTitleShadowColor:iconTitleShadowColor
                       iconTitleShadowDx:iconTitleShadowDx
                       iconTitleShadowDy:iconTitleShadowDy
                                adViewId:adViewId];
    }
    
    void imobileSetAdOrientation_(ImobileSdkAdsAdOrientation orientation) {
        if (!unityPlugin) {
            unityPlugin = [[ImobileSdkAdsIosUnityPluginImpl alloc] init];
        }
        [unityPlugin setAdOrientation:orientation];
    }
    
    void imobileSetVisibility_(int adViewId, bool visible) {
        if (!unityPlugin) {
            unityPlugin = [[ImobileSdkAdsIosUnityPluginImpl alloc] init];
        }
        [unityPlugin setAdView:adViewId visible:visible];
    }
    
    void imobileSetLegacyIosSdkMode_(bool isLegacyMode) {
        if (!unityPlugin) {
            unityPlugin = [[ImobileSdkAdsIosUnityPluginImpl alloc] init];
        }
        [unityPlugin setLegacyIosSdkMode:isLegacyMode];
    }
    
    float getScreenScale_() {
        if (!unityPlugin) {
            unityPlugin = [[ImobileSdkAdsIosUnityPluginImpl alloc] init];
        }
        return [unityPlugin getScreenScale];
    }
    
#ifdef __cplusplus
}
#endif

@end