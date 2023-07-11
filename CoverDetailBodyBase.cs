using FasskerJSON;
using FNS.Global;
using FNS.UI;
using FNS.Util;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoverDetailBodyBase : MonoBehaviour
{
    [Header("UI_Base")]
    public Transform trnModulesRoot;
    public ScrollRect sclMain;

    // 데이터와 타이틀을 받아와 화면에 표시하는 함수입니다.
    public virtual void SetData(List<DataDetailModule> _modules, string _txtTitleGNB, bool isHoldGNB = false)
    {
        // 블랙 컨텐츠 UI를 가져옵니다.
        var contentUI = UIManager.Instance.GetUI<BlackContentsUI>(E_UI.BLACK_DETAIL);
        // GNB(글로벌 네비게이션 바)를 생성합니다.
        contentUI.CreateGNB(_txtTitleGNB, isHoldGNB);

        // 모듈 리스트를 순회하며 각 모듈을 처리합니다.
        foreach (var module in _modules)
        {
            // daily_news_related_module인 경우 관련 모듈을 설정합니다.
            if (module.module_name == "daily_news_related_module")
            {
                contentUI.SetRelatedModule(trnModulesRoot, E_CONTENTS_TYPE.BLACK);
                continue;
            }
            // common_comment_module인 경우 댓글 모듈을 설정합니다.
            if (module.module_name == "common_comment_module")
            {
                contentUI.SetCommentModule(trnModulesRoot, E_CONTENTS_TYPE.BLACK, true);
                continue;
            }
            // todays_fassker_margin_item인 경우 처리하지 않고 넘어갑니다.
            if (module.module_name == "todays_fassker_margin_item")
                continue;
            
            try
            {
                // AssetBundle을 로드하여 아이템 모듈을 생성합니다.
                AssetBundleDownloadManager.Instance.
                    LoadAssetBundleFromModuleGUID(module.module_type_guid, module, CreateItemModule);
            }
            catch (System.Exception)
            {
                GlobalUIManager.Instance.HideLoading();
                LogSystem.Error("Cover Detail Item Module Instance Error :" + module.module_name);
                GlobalUIManager.Instance.ShowToastMessage(false, GlobalUIManager.Instance.GetStringToastRetry());
                ManageScene.Instance.CompleteDetailLoading();
                BNavigationMgr.Instance.CheckBackBtn();
                throw;
            }
        }

        ManageScene.Instance.CompleteDetailLoading();
        contentUI.scroll = sclMain;
    }

    // 아이템 모듈을 생성하고 데이터를 설정하는 함수입니다.
    private void CreateItemModule(GameObject _obj, DataDetailModule _data)
    {
        var objItem = Instantiate(_obj, trnModulesRoot);
        objItem.GetComponent<CoverDetailModuleBase>().SetData(_data);
    }
}