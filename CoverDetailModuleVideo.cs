using DG.Tweening;
using FasskerJSON;
using FNS.Global;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class CoverDetailModuleVideo : CoverDetailModuleParent
{
    public enum E_TYPE_ITEM
    {
        none = 0,
        web = 1,
        wiki = 2,
        cover = 3
    }

    [Header("UI_Video")]
    public VideoPlayer videoPlayer;
    public ModuleVideoSlider moSlider;
    public RawImage imgRender;
    public GameObject objSlider;
    public GameObject objNotice;
    public GameObject objPause;
    public GameObject objPlay;
    public GameObject objReplay;

    private List<CoverDetailModuleVideoItem> listItem;
    private int numItem;

    [Header("Options")]
    [SerializeField] private GameObject objLogo;
    [SerializeField] private GameObject objSound;
    [SerializeField] private Sprite imgLogoWhite;
    [SerializeField] private Sprite imgLogoBlack;
    [SerializeField] private List<Image> listImageOption;

    // 데이터를 받아와 모듈을 설정하는 함수입니다.
    public override void SetData(DataDetailModule _data)
    {
        // 비디오 초기화 함수 호출
        InitVideo(_data.youtube_url);
        // 부모 클래스의 SetData 함수 호출
        base.SetData(_data);
    }

    // 비디오 플레이어를 초기화하는 함수입니다.
    public void InitVideo(string _url)
    {
        imgRender.color = Color.gray;

        videoPlayer.isLooping = false;

        videoPlayer.prepareCompleted += VideoPlayer_started;
        videoPlayer.loopPointReached += FinishVideo;

        var youtubePlayer = videoPlayer.GetComponent<YoutubePlayer.YoutubePlayer>();
        youtubePlayer.youtubeUrl = _url;
        youtubePlayer.SetYoutubeLoadExceptionCallBack(SetOnNotice);

        videoPlayer.gameObject.SetActive(true);
    }

    // 모듈의 옵션을 설정하는 함수입니다.
    protected override void SetOption(List<DataDetailModule_Option> _listOption)
    {
        base.SetOption(_listOption);
        foreach (var option in _listOption)
        {
            switch (option.key)
            {
                case "video_autoplay": // Default On.
                    if (option.value == "Off")
                    {
                        videoPlayer.playOnAwake = false;
                        FasskerUtils.On(objPlay);
                    }
                    break;
                case "video_default_sound_button": // Default Off.
                    if (option.value == "On")
                    {
                        FasskerUtils.On(objSound);
                        videoPlayer.SetDirectAudioMute(0, true);
                    }
                    break;
                case "video_thumbnail_position": // Default Right.
                    if (option.value == "Left")
                    {
                        objLogo.GetComponent<RectTransform>().anchoredPosition = new Vector2(-967, -30);
                    }
                    break;
                case "color": // Default White.
                    if (option.value == "Black")
                    {
                        objLogo.GetComponent<Image>().sprite = imgLogoBlack;
                        foreach (var img in listImageOption)
                        {
                            img.color = Color.black;
                        }
                    }
                    break;
            }
        }
    }

    // 아이템을 생성하는 함수입니다.
    protected override void CreateItem(List<DataDetailModule> _listItem)
    {
        listItem = new List<CoverDetailModuleVideoItem>();
        numItem = 1;

        base.CreateItem(_listItem);
    }

    // 아이템 모듈을 생성하고 데이터를 설정하는 함수입니다.
    protected override void SetItemModule(GameObject _obj, DataDetailModule _data)
    {
        var objItem = Instantiate(_obj, trnRootItem);
        var item = objItem.GetComponent<CoverDetailModuleVideoItem>();
        item.SetData(_data, numItem++);
        item.callbackColor = ResetItemsMarker;
        item.callback = OnClickItem;
        listItem.Add(item);
    }

    // 아이템을 클릭했을 때 호출되는 함수입니다.
    public void OnClickItem(int _sec)
    {
        if (videoPlayer.time == 0 && videoPlayer.length == 0)
        {
            ResetItemsMarker();
            return;
        }

        moSlider.Seek(_sec);
    }

    bool isPrepare = false;
    // 비디오를 클릭했을 때 호출되는 함수입니다.
    public void OnClickVideo()
    {
        if (videoPlayer.isPlaying)
        {
            PauseVideo();
        }
        else
        {
            if (videoPlayer.time == 0 && videoPlayer.length == 0 && videoPlayer.playOnAwake == true)
                return;
            else if (videoPlayer.time == 0 && videoPlayer.length == 0 && isPrepare == false)
                LoadYouTube();
            else if (videoPlayer.time == videoPlayer.length)
                ReplayVideo();
            else
                PlayVideo();
        }
    }

    // YouTube 동영상을 로드하는 비동기 함수입니다.
    private async void LoadYouTube()
    {
        isPrepare = true;

        if (videoPlayer.GetComponent<YoutubePlayer.YoutubePlayer>() == false)
            return;

        objReplay.SetActive(false);
        objPause.SetActive(false);
        objPlay.SetActive(true);

        PlayAnim(objPlay);

        await videoPlayer.GetComponent<YoutubePlayer.YoutubePlayer>().PlayVideoAsync();

        videoPlayer.Play();

        imgRender.color = Color.white```
    }

    // 애니메이션 효과를 재생하는 함수입니다.
    public void PlayAnim(GameObject _obj)
    {
        _obj.transform.DOScale(0.8f, 0);
        _obj.transform.DOScale(1f, 0.5f).SetEase(Ease.InOutQuad);
        _obj.GetComponent<Image>().DOFade(1, 0);
        _obj.GetComponent<Image>().DOFade(0, 0.5f).SetEase(Ease.InOutQuad);

        if (_obj.transform.childCount == 0)
            return;
        _obj.transform.GetChild(0).DOScale(0.8f, 0);
        _obj.transform.GetChild(0).DOScale(1f, 0.5f).SetEase(Ease.InOutQuad);
        if (_obj.transform.GetChild(0).GetComponent<Image>() == false)
            return;
        _obj.transform.GetChild(0).GetComponent<Image>().DOFade(1, 0);
        _obj.transform.GetChild(0).GetComponent<Image>().DOFade(0, 0.5f).SetEase(Ease.InOutQuad);
    }

    // 비디오를 일시 정지하는 함수입니다.
    public void PauseVideo()
    {
        videoPlayer.Pause();

        objReplay.SetActive(false);
        objPlay.SetActive(false);
        objPause.SetActive(true);

        PlayAnim(objPause);

        imgRender.color = Color.gray;
    }

    // 비디오를 재생하는 함수입니다.
    public void PlayVideo()
    {
        objReplay.SetActive(false);
        objPause.SetActive(false);
        objPlay.SetActive(true);

        PlayAnim(objPlay);

        videoPlayer.Play();

        imgRender.color = Color.white;
    }

    // 비디오를 다시 재생하는 함수입니다.
    public void ReplayVideo()
    {
        videoPlayer.frame = 0;
        ResetItemsMarker();
    }

    // 비디오 플레이어가 시작되었을 때 호출되는 함수입니다.
    private void VideoPlayer_started(VideoPlayer source)
    {
        listImage[0].gameObject.SetActive(false);

        imgRender.color = Color.white;
    }

    // 비디오 재생이 완료되었을 때 호출되는 함수입니다.
    public void FinishVideo(VideoPlayer player)
    {
        videoPlayer.Pause();

        objReplay.SetActive(true);
        imgRender.color = Color.gray;
    }

    // 로딩 중에 예외가 발생할 때 호출되는 함수입니다.
    public void SetOnNotice()
    {
        objNotice.SetActive(true);
        objSlider.SetActive(false);
    }

    // 아이템의 마커를 초기화하는 함수입니다.
    public void ResetItemsMarker()
    {
        if (listItem == null || listItem.Count == 0)
            return;

        foreach (var item in listItem)
        {
            item.SetMarker(false);
        }
    }

    // 사운드를 무음으로 설정하는 함수입니다.
    public void MuteSound(bool _isMute)
    {
        videoPlayer.SetDirectAudioMute(0, _isMute);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
}
