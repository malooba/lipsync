;Copyright 2016 Malooba Ltd
;
;Licensed under the Apache License, Version 2.0 (the "License");
;you may not use this file except in compliance with the License.
;You may obtain a copy of the License at
;
;    http://www.apache.org/licenses/LICENSE-2.0
;
;Unless required by applicable law or agreed to in writing, software
;distributed under the License is distributed on an "AS IS" BASIS,
;WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
;See the License for the specific language governing permissions and
;limitations under the License.

; HM NIS Edit Wizard helper defines
!define PRODUCT_NAME "Malooba Lip Sync"
!define PRODUCT_VERSION "1.0"
!define PRODUCT_PUBLISHER "Malooba"
!define PRODUCT_WEB_SITE "http://www.malooba.com/"
!define PRODUCT_DIR_REGKEY "Software\Microsoft\Windows\CurrentVersion\App Paths\MaloobaLipSync.exe"
!define PRODUCT_UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}"
!define PRODUCT_UNINST_ROOT_KEY "HKLM"

; MUI 1.67 compatible ------
!include "MUI.nsh"

; MUI Settings
!define MUI_ABORTWARNING
!define MUI_ICON "${NSISDIR}\Contrib\Graphics\Icons\modern-install.ico"
!define MUI_UNICON "${NSISDIR}\Contrib\Graphics\Icons\modern-uninstall.ico"

; Welcome page
!insertmacro MUI_PAGE_WELCOME
; License page
!insertmacro MUI_PAGE_LICENSE "MaloobaLicense.txt"
; Components page
!insertmacro MUI_PAGE_COMPONENTS
; Directory page
!insertmacro MUI_PAGE_DIRECTORY
; Instfiles page
!insertmacro MUI_PAGE_INSTFILES
; Finish page
!define MUI_FINISHPAGE_RUN "$INSTDIR\MaloobaLipSync.exe"
!insertmacro MUI_PAGE_FINISH

; Uninstaller pages
!insertmacro MUI_UNPAGE_INSTFILES

; Language files
!insertmacro MUI_LANGUAGE "English"

; MUI end ------

OutFile "LipSync.exe"

Name "${PRODUCT_NAME} ${PRODUCT_VERSION}"
InstallDir "$PROGRAMFILES\Malooba Lip Sync"
InstallDirRegKey HKLM "${PRODUCT_DIR_REGKEY}" ""
ShowInstDetails show
ShowUnInstDetails show

Section "Lip Sync" SEC01
  SetOverwrite try
  SetOutPath "$INSTDIR"
  File "MaloobaLipSync\bin\Release\GalaSoft.MvvmLight.dll"
  File "MaloobaLipSync\bin\Release\GalaSoft.MvvmLight.Extras.dll"
  File "MaloobaLipSync\bin\Release\GalaSoft.MvvmLight.Platform.dll"
  File "MaloobaLipSync\bin\Release\MaloobaLipSync.exe.config"
  File "MaloobaLipSync\bin\Release\Microsoft.Expression.Interactions.dll"
  File "MaloobaLipSync\bin\Release\Microsoft.Practices.ServiceLocation.dll"
  File "MaloobaLipSync\bin\Release\Newtonsoft.Json.dll"
  File "MaloobaLipSync\bin\Release\System.Reactive.Core.dll"
  File "MaloobaLipSync\bin\Release\System.Reactive.Interfaces.dll"
  File "MaloobaLipSync\bin\Release\System.Reactive.Linq.dll"
  File "MaloobaLipSync\bin\Release\System.Reactive.PlatformServices.dll"
  File "MaloobaLipSync\bin\Release\System.Windows.Interactivity.dll"
  File "MaloobaLipSync\bin\Release\MaloobaLipSync.exe"
  File "Malooba Lip Sync.pdf"
  CreateDirectory "$SMPROGRAMS\Malooba Lip Sync"
  CreateShortCut "$SMPROGRAMS\Malooba Lip Sync\MaloobaLipSync.lnk" "$INSTDIR\MaloobaLipSync.exe"
  CreateShortCut "$DESKTOP\MaloobaLipSync.lnk" "$INSTDIR\MaloobaLipSync.exe"
SectionEnd

Section "Fingerprint" SEC02
  SetOutPath "$INSTDIR"
  File "MaloobaFingerprint\bin\Release\GalaSoft.MvvmLight.dll"
  File "MaloobaFingerprint\bin\Release\GalaSoft.MvvmLight.Extras.dll"
  File "MaloobaFingerprint\bin\Release\GalaSoft.MvvmLight.Platform.dll"
  File "MaloobaFingerprint\bin\Release\MaloobaFingerprint.exe.config"
  File "MaloobaFingerprint\bin\Release\Microsoft.Expression.Interactions.dll"
  File "MaloobaFingerprint\bin\Release\Microsoft.Practices.ServiceLocation.dll"
  File "MaloobaFingerprint\bin\Release\Newtonsoft.Json.dll"
  File "MaloobaFingerprint\bin\Release\System.Windows.Interactivity.dll"
  File "MaloobaFingerprint\bin\Release\MaloobaFingerprint.exe"
  CreateDirectory "$SMPROGRAMS\MaloobaLipSync"
  CreateShortCut "$SMPROGRAMS\MaloobaLipSync\MaloobaFingerprint.lnk" "$INSTDIR\MaloobaFingerprint.exe"
  CreateShortCut "$DESKTOP\MaloobaFingerprint.lnk" "$INSTDIR\MaloobaFingerprint.exe"
SectionEnd

Section -AdditionalIcons
  SetOutPath $INSTDIR
  CreateShortCut "$SMPROGRAMS\MaloobaLipSync\Uninstall.lnk" "$INSTDIR\uninst.exe"
SectionEnd

Section -Post
  WriteUninstaller "$INSTDIR\uninst.exe"
  WriteRegStr HKLM "${PRODUCT_DIR_REGKEY}" "" "$INSTDIR\MaloobaLipSync.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayName" "$(^Name)"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "UninstallString" "$INSTDIR\uninst.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayIcon" "$INSTDIR\MaloobaLipSync.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayVersion" "${PRODUCT_VERSION}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "URLInfoAbout" "${PRODUCT_WEB_SITE}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "Publisher" "${PRODUCT_PUBLISHER}"
SectionEnd

; Section descriptions
!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
  !insertmacro MUI_DESCRIPTION_TEXT ${SEC01} "Main Lip Sync application"
  !insertmacro MUI_DESCRIPTION_TEXT ${SEC02} "Fingerprint generator application"
!insertmacro MUI_FUNCTION_DESCRIPTION_END


Function un.onUninstSuccess
  HideWindow
  MessageBox MB_ICONINFORMATION|MB_OK "$(^Name) was successfully removed from your computer."
FunctionEnd

Function un.onInit
  MessageBox MB_ICONQUESTION|MB_YESNO|MB_DEFBUTTON2 "Are you sure you want to completely remove $(^Name) and all of its components?" IDYES +2
  Abort
FunctionEnd

Section Uninstall
  Delete "$INSTDIR\uninst.exe"
  Delete "$INSTDIR\XamlFingerprintAnalyser.vshost.exe.manifest"
  Delete "$INSTDIR\XamlFingerprintAnalyser.vshost.exe.config"
  Delete "$INSTDIR\XamlFingerprintAnalyser.vshost.exe"
  Delete "$INSTDIR\XamlFingerprintAnalyser.exe.config"
  Delete "$INSTDIR\XamlFingerprintAnalyser.exe"
  Delete "$INSTDIR\System.Windows.Interactivity.dll"
  Delete "$INSTDIR\Newtonsoft.Json.dll"
  Delete "$INSTDIR\Microsoft.Practices.ServiceLocation.dll"
  Delete "$INSTDIR\Microsoft.Expression.Interactions.dll"
  Delete "$INSTDIR\MaloobaFingerprint.exe.config"
  Delete "$INSTDIR\MaloobaFingerprint.exe"
  Delete "$INSTDIR\GalaSoft.MvvmLight.Platform.dll"
  Delete "$INSTDIR\GalaSoft.MvvmLight.Extras.dll"
  Delete "$INSTDIR\GalaSoft.MvvmLight.dll"
  Delete "$INSTDIR\System.Windows.Interactivity.dll"
  Delete "$INSTDIR\System.Reactive.PlatformServices.dll"
  Delete "$INSTDIR\System.Reactive.Linq.dll"
  Delete "$INSTDIR\System.Reactive.Interfaces.dll"
  Delete "$INSTDIR\System.Reactive.Core.dll"
  Delete "$INSTDIR\Newtonsoft.Json.dll"
  Delete "$INSTDIR\Microsoft.Practices.ServiceLocation.dll"
  Delete "$INSTDIR\Microsoft.Expression.Interactions.dll"
  Delete "$INSTDIR\MaloobaLipSync.exe.config"
  Delete "$INSTDIR\MaloobaLipSync.exe"
  Delete "$INSTDIR\GalaSoft.MvvmLight.Platform.dll"
  Delete "$INSTDIR\GalaSoft.MvvmLight.Extras.dll"
  Delete "$INSTDIR\GalaSoft.MvvmLight.dll"
  Delete "$SMPROGRAMS\MaloobaLipSync\Uninstall.lnk"
  Delete "$DESKTOP\MaloobaLipSync.lnk"
  Delete "$SMPROGRAMS\MaloobaLipSync\MaloobaLipSync.lnk"
  Delete "$DESKTOP\MaloobaFingerprint.lnk"
  Delete "$SMPROGRAMS\MaloobaLipSync\MaloobaFingerprint.lnk"
  RMDir "$SMPROGRAMS\MaloobaLipSync"
  RMDir "$INSTDIR"

  DeleteRegKey ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}"
  DeleteRegKey HKLM "${PRODUCT_DIR_REGKEY}"
  SetAutoClose true
SectionEnd