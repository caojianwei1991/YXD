package com.kudospark.yxd;

import java.util.ArrayList;

import android.content.Context;
import android.util.Log;
import android.widget.TextView;

import com.sinovoice.hcicloudsdk.api.HciCloudSys;
import com.sinovoice.hcicloudsdk.api.hwr.HciCloudHwr;
import com.sinovoice.hcicloudsdk.common.HciErrorCode;
import com.sinovoice.hcicloudsdk.common.Session;
import com.sinovoice.hcicloudsdk.common.hwr.HwrConfig;
import com.sinovoice.hcicloudsdk.common.hwr.HwrInitParam;
import com.sinovoice.hcicloudsdk.common.hwr.HwrRecogResult;
import com.sinovoice.hcicloudsdk.common.hwr.HwrRecogResultItem;
import com.sinovoice.hcicloudsdk.common.hwr.HwrAssociateWordsResult;
import com.unity3d.player.UnityPlayer;

public class HciCloudFuncHelper extends HciCloudHelper{
    private static final String TAG = HciCloudFuncHelper.class.getSimpleName();

    /**
     * 显示结果集合
     * 
     * @param recogResult
     */
    private static void showRecogResultResult(HwrRecogResult recogResult) {
        String strResult = "";
        if (recogResult != null) {
            ArrayList<HwrRecogResultItem> recogItemList = recogResult.getResultItemList();
            for (int index = 0; index < recogItemList.size(); index++) {
                String strTmp = recogItemList.get(index).getResult();
                strResult = strResult.concat(strTmp);
                if(index != recogItemList.size() - 1)
                {
                	strResult = strResult.concat(",");
                }
            }
        }
        UnityPlayer.UnitySendMessage("UI Root", "ReceiveHWR", strResult);
        //ShowMessage(strResult);
    }
    
    private static void showAssociateResultResult(HwrAssociateWordsResult recogResult) {
        String strResult = "";
        if (recogResult != null) {
            ArrayList<String> recogItemList = recogResult
                    .getResultList();
            for (int index = 0; index < recogItemList.size(); index++) {
                //String strTmp = recogItemList.get(index).getResult();
                strResult = strResult.concat(recogItemList.get(index)).concat(";");
            }
        }
        ShowMessage(strResult);
    }

    /**
     * 开始识别，此方法为非实时识别
     * 
     * @param hwrConfig
     * @param strokes
     */
    public static void Recog(String capkey,HwrConfig recogConfig, short[] strokes) {
        int errCode = -1;
        HwrConfig sessionConfig = new HwrConfig();
        sessionConfig.addParam(HwrConfig.SessionConfig.PARAM_KEY_CAP_KEY, capkey);
        sessionConfig.addParam(HwrConfig.SessionConfig.PARAM_KEY_RES_PREFIX, "en_");
        Session session = new Session();
        ShowMessage("HciCloudHwr hciHwrSessionStart config " + sessionConfig.getStringConfig());
        // 开始会话
        errCode = HciCloudHwr.hciHwrSessionStart(sessionConfig.getStringConfig(), session);
        if (HciErrorCode.HCI_ERR_NONE != errCode) {
        	ShowMessage("hciHwrSessionStart error:" + HciCloudSys.hciGetErrorInfo(errCode));
            return;
        }
        ShowMessage("hciHwrSessionStart Success");

        Log.i(TAG,"HciCloudHwr HwrConfig: " + recogConfig.getStringConfig());
        // 开始识别
        int bRet = capkey.indexOf("hwr.local.associateword");
        if (bRet != -1)
        {
        	//开始联想
        	String associateWords = "en";
        	HwrAssociateWordsResult associateResult = new HwrAssociateWordsResult();
            errCode = HciCloudHwr.hciHwrAssociateWords(session, recogConfig.getStringConfig(), associateWords,
            		associateResult);
            if (HciErrorCode.HCI_ERR_NONE == errCode) {
            	ShowMessage("hciHwrAssociateWords Success");
            	showAssociateResultResult(associateResult);
            }
            else{
            	ShowMessage("hciHwrRecog error:" + HciCloudSys.hciGetErrorInfo(errCode));	
            }
            
            //联想词动态调整
            /*String adjustWords = "人民日报社";
            errCode = HciCloudHwr.hciHwrAssociateWordsAdjust(session, recogConfig.getStringConfig(), 
            		adjustWords);
            
            if (HciErrorCode.HCI_ERR_NONE == errCode) {
            	ShowMessage("hciHwrAssociateWordsAdjust Success");
            }
            else{
            	ShowMessage("hciHwrRecog error:" + HciCloudSys.hciGetErrorInfo(errCode));	
            }*/
            
          //再联想，查看调整结果
            /*HwrAssociateWordsResult associateResult2 = new HwrAssociateWordsResult();
            errCode = HciCloudHwr.hciHwrAssociateWords(session, recogConfig.getStringConfig(), associateWords,
            		associateResult2);
            if (HciErrorCode.HCI_ERR_NONE == errCode) {
            	ShowMessage("hciHwrAssociateWords Success");
            	showAssociateResultResult(associateResult2);
            }
            else{
            	ShowMessage("hciHwrRecog error:" + HciCloudSys.hciGetErrorInfo(errCode));	
            }*/
            
        }
        else
        {
        	HwrRecogResult recogResult = new HwrRecogResult();
            errCode = HciCloudHwr.hciHwrRecog(session, strokes, recogConfig.getStringConfig(),
                    recogResult);
            if (HciErrorCode.HCI_ERR_NONE == errCode) {
            	ShowMessage("hciHwrRecog Success");
            	showRecogResultResult(recogResult);
            }
            else{
            	ShowMessage("hciHwrRecog error:" + HciCloudSys.hciGetErrorInfo(errCode));	
            }
        }
        
                
        // 停止会话
        HciCloudHwr.hciHwrSessionStop(session);
        ShowMessage("hciHwrSessionStop");
    }
    
	public static void Func(Context context,String capkey,short[] traceData) {
		setContext(context);
		// HWR 初始化
        HwrInitParam hwrInitParam = new HwrInitParam();
        // 获取App应用中的lib的路径,使用lib下的资源文件,需要添加android_so的标记
        String hwrDirPath = context.getFilesDir().getAbsolutePath().replace("files", "lib");
        hwrInitParam.addParam(HwrInitParam.PARAM_KEY_DATA_PATH, hwrDirPath);
        hwrInitParam.addParam(HwrInitParam.PARAM_KEY_FILE_FLAG, HwrInitParam.VALUE_OF_PARAM_FILE_FLAG_ANDROID_SO);
        //hwrInitParam.addParam(HwrInitParam.PARAM_KEY_INIT_CAP_KEYS, AccountInfo.getInstance().getCapKey());
        int errCode = HciCloudHwr.hciHwrInit(hwrInitParam.getStringConfig());
        if (errCode != HciErrorCode.HCI_ERR_NONE) {
            ShowMessage("hciHwrInit error:" + HciCloudSys.hciGetErrorInfo(errCode));
            return;
        } else {
        	ShowMessage("hciHwrInit Success");
        }
        HwrConfig recogConfig = new HwrConfig();
		Recog(capkey,recogConfig,traceData);
		//HWR反初始化
		HciCloudHwr.hciHwrRelease();
		ShowMessage("hciHwrRelease");
	}
}
