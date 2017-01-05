using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
This example shows how to modify the frames from the SDK. In this case, we will draw the frames in grayscale.
 */
public class ExampleOverrideFrame : MonoBehaviour,CraftARSDK.CraftARFrameDrawer {
	
	Texture2D cameraTexture; //We will store here texture in which the camera draws the frames and the contents
	void Start () {
		bool keepDrawingFrames = false; //We will draw the frames, the SDK will not.
		CraftARSDK.instance.setCraftARFrameHandler (this, keepDrawingFrames);
	}

	//Frame events:
	void CraftARSDK.CraftARFrameDrawer.newFrame(byte[] frameData,int frameWidth,int frameHeight){
		if (cameraTexture != null) {
			VideoFrameSettings videoFrameSettings = CraftARSDK.instance.GetVideoFrameSettings();
			//Example: Convert the frame to grayscale
			// Note that this can be done more efficiently using other tools!)
			int B,G,R,mean;
			switch(videoFrameSettings.Format){
			case TextureFormat.RGBA32:
				//Android
				for(int pixel=0;pixel<frameWidth*frameHeight;pixel++){
					R = (int) frameData[4*pixel];
					G = (int) frameData[4*pixel + 1];
					B = (int) frameData[4*pixel + 2];
					mean = (R+G+B)/3;
					frameData[4*pixel]= frameData[4*pixel + 1] = frameData[4*pixel + 2] = (byte)mean;		
				}
				break;
			case TextureFormat.BGRA32:
				//iOS
				for(int pixel=0;pixel<frameWidth*frameHeight;pixel++){
					B = (int) frameData[4*pixel];
					G = (int) frameData[4*pixel + 1];
					R = (int) frameData[4*pixel + 2];
					mean = (B+G+R)/3;
					frameData[4*pixel]= frameData[4*pixel + 1] = frameData[4*pixel + 2] = (byte)mean;		
				}
				break;
			}
			//Apply the modified frame to the camera texture!
			cameraTexture.LoadRawTextureData(frameData);	
			cameraTexture.Apply();
		}
	}
	void CraftARSDK.CraftARFrameDrawer.textureReady(Texture2D cameraTexture){
		this.cameraTexture = cameraTexture;
	}
}
