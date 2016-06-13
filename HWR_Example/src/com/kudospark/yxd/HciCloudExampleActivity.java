package com.kudospark.yxd;

import java.io.File;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.HashMap;
import java.util.LinkedHashMap;
import java.util.Locale;

import org.json.JSONException;
import org.json.JSONObject;

import android.os.Bundle;
import android.os.Environment;
import android.text.TextUtils;
import android.util.Log;
import android.widget.Toast;

import com.iflytek.cloud.EvaluatorListener;
import com.iflytek.cloud.EvaluatorResult;
import com.iflytek.cloud.RecognizerListener;
import com.iflytek.cloud.RecognizerResult;
import com.iflytek.cloud.SpeechConstant;
import com.iflytek.cloud.SpeechError;
import com.iflytek.cloud.SpeechEvaluator;
import com.iflytek.cloud.SpeechRecognizer;
import com.iflytek.cloud.SpeechUtility;
import com.sinovoice.hcicloudsdk.api.HciCloudSys;
import com.sinovoice.hcicloudsdk.common.AuthExpireTime;
import com.sinovoice.hcicloudsdk.common.HciErrorCode;
import com.sinovoice.hcicloudsdk.common.InitParam;
import com.unity3d.player.UnityPlayer;
import com.unity3d.player.UnityPlayerActivity;


public class HciCloudExampleActivity extends UnityPlayerActivity  {
	
	private static final String TAG = "HciCloudExampleActivity";
    private AccountInfo mAccountInfo;
    private SpeechEvaluator mIse;
    private SpeechRecognizer mAsr;
 	private Toast mToast;
 	private String mLanguage;
 	private String mTotalScore;
 	private String speechRecognizerResult;
 	
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		
		mToast = Toast.makeText(this, "", Toast.LENGTH_SHORT);
		SpeechUtility.createUtility(this, "appid=5721ce11");
		
		mAccountInfo = AccountInfo.getInstance();
        boolean loadResult = mAccountInfo.loadAccountInfo(this);
        if (loadResult) {
            // 加载信息成功进入主界面
        	ShowTip("加载灵云账号成功");
        } else {
            // 加载信息失败，显示失败界面
        	ShowTip("加载灵云账号失败！请在assets/AccountInfo.txt文件中填写正确的灵云账户信息，账户需要从www.hcicloud.com开发者社区上注册申请。");
        	return;
        }
        
        // 加载信息,返回InitParam, 获得配置参数的字符串
        InitParam initParam = getInitParam();
        String strConfig = initParam.getStringConfig();
        Log.i(TAG,"hciInit config:" + strConfig);
        
        // 初始化
        int errCode = HciCloudSys.hciInit(strConfig, this);
        if (errCode != HciErrorCode.HCI_ERR_NONE && errCode != HciErrorCode.HCI_ERR_SYS_ALREADY_INIT) {
        	Log.e(TAG, "hciInit error: " + HciCloudSys.hciGetErrorInfo(errCode));
            return;
        } else {
        	Log.i(TAG, "hciInit success");
        }

        // 获取授权/更新授权文件 :
        errCode = checkAuthAndUpdateAuth();
        if (errCode != HciErrorCode.HCI_ERR_NONE) {
            // 由于系统已经初始化成功,在结束前需要调用方法hciRelease()进行系统的反初始化
        	Log.e(TAG, "CheckAuthAndUpdateAuth error: " + HciCloudSys.hciGetErrorInfo(errCode));
            HciCloudSys.hciRelease();
            return;
        }
	}
	
	public void StartIse(String language, String evaText)
	{
		mLanguage = language;
		mIse = SpeechEvaluator.createEvaluator(HciCloudExampleActivity.this, null);
		mIse.setParameter(SpeechConstant.LANGUAGE, mLanguage);
		mIse.setParameter(SpeechConstant.ISE_CATEGORY, "read_word");
		mIse.setParameter(SpeechConstant.TEXT_ENCODING, "utf-8");
		mIse.setParameter(SpeechConstant.ISE_AUDIO_PATH, Environment.getExternalStorageDirectory()+"/IseCache/ise.wav");
		mIse.setParameter(SpeechConstant.AUDIO_FORMAT,"wav");
//		mIse.setParameter(SpeechConstant.VAD_BOS, vad_bos);
//		mIse.setParameter(SpeechConstant.VAD_EOS, vad_eos);
//		mIse.setParameter(SpeechConstant.KEY_SPEECH_TIMEOUT, speech_timeout);
//		mIse.setParameter(SpeechConstant.RESULT_LEVEL, result_level);
		
		Log.i(TAG, "StartIse Result: " + mIse.startEvaluating(evaText, null, mEvaluatorListener));
	}
	
	public void StopIse()
	{
		mIse.stopEvaluating();
		Log.i(TAG, "StopIse");
	}
	
	void StartSpeechRecognizer()
	{
		speechRecognizerResult = "";
		mAsr = SpeechRecognizer.createRecognizer(HciCloudExampleActivity.this, null);
		mAsr.setParameter(SpeechConstant.DOMAIN, "iat"); 
		mAsr.setParameter(SpeechConstant.LANGUAGE, mLanguage);
		mAsr.setParameter(SpeechConstant.ACCENT, "mandarin"); 
		mAsr.setParameter(SpeechConstant.SAMPLE_RATE, "16000"); 
		// 设置标点符号,设置为"0"返回结果无标点,设置为"1"返回结果有标点
		mAsr.setParameter(SpeechConstant.ASR_PTT, "0");
		mAsr.setParameter(SpeechConstant.AUDIO_SOURCE, "-2");
		mAsr.setParameter(SpeechConstant.ASR_SOURCE_PATH, Environment.getExternalStorageDirectory()+"/IseCache/ise.wav");
		Log.i(TAG, "StartSpeechRecognizer Result: " + mAsr.startListening(mRecoListener));
	}
	
	public void StopSpeechRecognizer()
	{
		mAsr.stopListening();
		Log.i(TAG, "StopSpeechRecognizer");
	}
	
	// 评测监听接口
	private EvaluatorListener mEvaluatorListener = new EvaluatorListener() {
		
		@Override
		public void onResult(EvaluatorResult result, boolean isLast) {
			Log.d(TAG, "evaluator result :" + isLast);

			if (isLast) {
				String mLastResult = result.getResultString();
				ShowTip("评测结束");
				// 解析最终结果
				if (!TextUtils.isEmpty(mLastResult)) {
					XmlResultParser resultParser = new XmlResultParser();
					Result ret = resultParser.parse(mLastResult);
					if (null != ret) {
						mTotalScore = String.valueOf(ret.total_score * 20);
						StringBuilder sb = new StringBuilder();
						sb.append(ret.content);
						sb.append(",");
						sb.append(mTotalScore);
						UnityPlayer.UnitySendMessage("UI Root", "ReceiveIse", sb.toString());
						//StartSpeechRecognizer();
					} else {
						ShowTip("结析结果为空");
					}
				}
			}
		}

		@Override
		public void onError(SpeechError error) {
			if(error != null) {	
				ShowTip("error:"+ error.getErrorCode() + "," + error.getErrorDescription());
			} else {
				Log.d(TAG, "evaluator over");
			}
		}

		@Override
		public void onBeginOfSpeech() {
			// 此回调表示：sdk内部录音机已经准备好了，用户可以开始语音输入
			ShowTip("开始说话");
		}

		@Override
		public void onEndOfSpeech() {
			// 此回调表示：检测到了语音的尾端点，已经进入识别过程，不再接受语音输入
			ShowTip("结束说话");
		}

		@Override
		public void onVolumeChanged(int volume, byte[] data) {
			ShowTip("当前正在说话，音量大小：" + volume);
			Log.d(TAG, "返回音频数据："+data.length);
		}

		@Override
		public void onEvent(int eventType, int arg1, int arg2, Bundle obj) {
			// 以下代码用于获取与云端的会话id，当业务出错时将会话id提供给技术支持人员，可用于查询会话日志，定位出错原因
			//	if (SpeechEvent.EVENT_SESSION_ID == eventType) {
			//		String sid = obj.getString(SpeechEvent.KEY_EVENT_SESSION_ID);
			//		Log.d(TAG, "session id =" + sid);
			//	}
		}
		
	};
	
	/**
     * 识别监听器。
     */
    private RecognizerListener mRecoListener = new RecognizerListener() {
        
        @Override
        public void onVolumeChanged(int volume, byte[] data) {
        	ShowTip("当前正在说话，音量大小：" + volume);
        	Log.d(TAG, "返回音频数据："+data.length);
        }
        
        @Override
        public void onResult(final RecognizerResult result, boolean isLast) {
        	if (null != result) {
        		Log.d(TAG, "recognizer result：" + result.getResultString());
    			printResult(result);
        		// 显示
        		
        	} else {
        		Log.d(TAG, "recognizer result : null");
        	}	
        }
        
        @Override
        public void onEndOfSpeech() {
        	// 此回调表示：检测到了语音的尾端点，已经进入识别过程，不再接受语音输入
        	ShowTip("结束说话");
        }
        
        @Override
        public void onBeginOfSpeech() {
        	// 此回调表示：sdk内部录音机已经准备好了，用户可以开始语音输入
        	ShowTip("开始说话");
        }

		@Override
		public void onError(SpeechError error) {
			ShowTip("onError Code："	+ error.getErrorCode());
		}

		@Override
		public void onEvent(int eventType, int arg1, int arg2, Bundle obj) {
			// 以下代码用于获取与云端的会话id，当业务出错时将会话id提供给技术支持人员，可用于查询会话日志，定位出错原因
			// 若使用本地能力，会话id为null
			//	if (SpeechEvent.EVENT_SESSION_ID == eventType) {
			//		String sid = obj.getString(SpeechEvent.KEY_EVENT_SESSION_ID);
			//		Log.d(TAG, "session id =" + sid);
			//	}
		}

    };
    
    private void printResult(RecognizerResult results) {
		String text = JsonParser.parseIatResult(results.getResultString());

		String sn = null;
		// 读取json结果中的sn字段
		try {
			JSONObject resultJson = new JSONObject(results.getResultString());
			sn = resultJson.optString("sn");
		} catch (JSONException e) {
			e.printStackTrace();
		}

		HashMap<String, String> mIatResults = new LinkedHashMap<String, String>();
		mIatResults.put(sn, text);

		StringBuffer resultBuffer = new StringBuffer();
		for (String key : mIatResults.keySet()) {
			resultBuffer.append(mIatResults.get(key));
		}
		speechRecognizerResult += resultBuffer.toString();
		StringBuilder sb = new StringBuilder();
		sb.append(mTotalScore);
		sb.append(",");
		sb.append(speechRecognizerResult);
		UnityPlayer.UnitySendMessage("UI Root","ReceiveSpeechRecognizer",sb.toString());
	}
	
	public void StartHWR(String traceData, String capKey)
	{
		String strs[] = traceData.split(",");
		short trace[] = new short[strs.length];
		String s = "";
		for(int i = 0; i < trace.length; i++)
		{
			s += strs[i];
			trace[i] = Short.parseShort(strs[i]);
		}
		Log.i(TAG, "StartHWR.traceData=" + s);
		Log.i(TAG, "StartHWR.capKey=" + capKey);
		HciCloudFuncHelper.Func(this, capKey, trace);
	}
    
    
    /**
     * 加载初始化信息
     * 
     * @param context
     *            上下文语境
     * @return 系统初始化参数
     */
    private InitParam getInitParam() {
        String authDirPath = this.getFilesDir().getAbsolutePath();

        // 前置条件：无
        InitParam initparam = new InitParam();
        // 授权文件所在路径，此项必填
        initparam.addParam(InitParam.AuthParam.PARAM_KEY_AUTH_PATH, authDirPath);
        // 灵云云服务的接口地址，此项必填
        initparam.addParam(InitParam.AuthParam.PARAM_KEY_CLOUD_URL, AccountInfo
                .getInstance().getCloudUrl());
        // 开发者Key，此项必填，由捷通华声提供
        initparam.addParam(InitParam.AuthParam.PARAM_KEY_DEVELOPER_KEY, AccountInfo
                .getInstance().getDeveloperKey());
        // 应用Key，此项必填，由捷通华声提供
        initparam.addParam(InitParam.AuthParam.PARAM_KEY_APP_KEY, AccountInfo
                .getInstance().getAppKey());

        // 配置日志参数
        String sdcardState = Environment.getExternalStorageState();
        if (Environment.MEDIA_MOUNTED.equals(sdcardState)) {
            String sdPath = Environment.getExternalStorageDirectory()
                    .getAbsolutePath();
            String packageName = this.getPackageName();

            String logPath = sdPath + File.separator + "sinovoice"
                    + File.separator + packageName + File.separator + "log"
                    + File.separator;

            // 日志文件地址
            File fileDir = new File(logPath);
            if (!fileDir.exists()) {
                fileDir.mkdirs();
            }

            // 日志的路径，可选，如果不传或者为空则不生成日志
            initparam.addParam(InitParam.LogParam.PARAM_KEY_LOG_FILE_PATH, logPath);
        }

        return initparam;
    }

    /**
     * 获取授权
     * 
     * @return true 成功
     */
    private int checkAuthAndUpdateAuth() {
        
    	// 获取系统授权到期时间
        int initResult;
        AuthExpireTime objExpireTime = new AuthExpireTime();
        initResult = HciCloudSys.hciGetAuthExpireTime(objExpireTime);
        if (initResult == HciErrorCode.HCI_ERR_NONE) {
            // 显示授权日期,如用户不需要关注该值,此处代码可忽略
            Date date = new Date(objExpireTime.getExpireTime() * 1000);
            SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd",
                    Locale.CHINA);
            Log.i(TAG, "expire time: " + sdf.format(date));

            if (objExpireTime.getExpireTime() * 1000 > System
                    .currentTimeMillis()) {
                // 已经成功获取了授权,并且距离授权到期有充足的时间(>7天)
                Log.i(TAG, "checkAuth success");
                return initResult;
            }
            
        } 
        
        // 获取过期时间失败或者已经过期
        initResult = HciCloudSys.hciCheckAuth();
        if (initResult == HciErrorCode.HCI_ERR_NONE) {
            Log.i(TAG, "checkAuth success");
            return initResult;
        } else {
            Log.e(TAG, "checkAuth failed: " + initResult);
            return initResult;
        }
    }
    
    private void ShowTip(final String str) {
		mToast.setText(str);
		mToast.show();
	}
    
    @Override
    protected void onDestroy() {
        // 释放HciCloudSys，当其他能力全部释放完毕后，才能调用HciCloudSys的释放方法
        HciCloudSys.hciRelease();
        Log.i(TAG, "hciRelease");
        if(mAsr != null)
        {
        	mAsr.cancel();
            mAsr.destroy();
        }
        if(mIse != null)
        {
	        mIse.cancel();
	        mIse.destroy();
        }
        super.onDestroy();
    }

}
