#!/usr/bin/python

import sys
import shutil
import os.path
from mod_pbxproj import XcodeProject

def log(x):
  with open('IOSBuildLogFile.txt','a') as f:
    f.write(x + "\n")

log('In iOsSetup.py\n'
    '---------------------------')

install_path = sys.argv[1]
curr_dir = sys.argv[2]

log('Install path: ' + install_path)

project = XcodeProject.Load(install_path + '/Unity-iPhone.xcodeproj/project.pbxproj')
log('Loaded project.pbxproj.')

frameworkGroup = project.get_or_create_group('Frameworks')
craftARSDKGroup = project.get_or_create_group('CraftARSDK')
project.add_folder(curr_dir + "/Assets/Plugins/CraftAR-iOS/.libs/CraftARResourcesAR.bundle", parent=craftARSDKGroup)
project.add_folder(curr_dir + "/Assets/Plugins/CraftAR-iOS/.libs/CraftARUnityARSDK.framework", parent=craftARSDKGroup)
project.add_folder(curr_dir + "/Assets/Plugins/CraftAR-iOS/.libs/Pods.framework", parent=craftARSDKGroup)
project.add_file('System/Library/Frameworks/Security.framework', parent=frameworkGroup, tree='SDKROOT')
project.add_file('System/Library/Frameworks/MobileCoreServices.framework', parent=frameworkGroup, tree='SDKROOT')
project.add_other_ldflags("-ObjC")

project.saveFormat3_2()
log('Saved project.\n'
    '---------------------------')

