# AvatarSettingUpdater

![Component](https://github.com/crestudio/AvatarSettingUpdater/blob/master/Image/VRSuya_AvatarSettingUpdate.jpg?raw=true)

## 다운로드

[VCC 패키지 추가](https://crestudio.notion.site/Avatar-Setting-Updater-628ff7f0bd2640a29e56cade96104f71)

[ZIP 다운로드](https://github.com/crestudio/AvatarSettingUpdater/releases)

---

## 개요

VRChat 아바타용 VRSuya 제품 자동 세팅 프로그램

- 설치되어 있는 VRSuya 에셋을 읽어서 Append 데이터 인식
- 설치 가능한 아바타 자동 인식
- 설치 가능한 VRSuya 제품 인식
- 필요한 에셋이 없는 경우 템플릿으로 자동 생성
- Prefab 자동 배치 및 내부 설정
- Animator Controller, VRC Menu, VRC Parameter 자동 세팅
- 모듈식 구성

---

## 사용방법

1. 사용 중인 Unity 프로젝트에 해당 패키지를 임포트합니다
1. **적용을 원하는 아바타 GameObject → Add Component → VRSuya AvatarSettingUpdater 추가**합니다
1. 적용 및 설치를 원하는 옵션과 아바타, VRSuya 제품을 선택합니다
1. 아바타 업데이트 버튼을 누릅니다
   - 만약 잘못 적용한 경우 실행취소(Undo)를 할 수 있습니다

---

## 작동조건

- [x] 적용하려는 아바타가 휴머노이드 타입이어야 합니다
- [x] VRCSDK Avatar 3.0 패키지가 설치 및 올바르게 작동하고 있어야 합니다
- [x] 아바타에 VRC 아바타 디스크립터가 설정되어 있어야 합니다
- [x] VRSuya의 컨텐츠의 에셋 GUID가 변경되지 않거나, 또는 폴더 이름과 파일명이 변경되지 않아야 합니다

---

## 알려진 버그

- [ ] 셀레스티아 Animator Controller의 Cheek 관련 레이어가 Weight 0이 되지 않는 버그

---

## 업데이트 로그

### 2023년 5월 31일 : 1.00

+ 릴리즈

---

## Contact

- Twitter : https://twitter.com/VRSuya
- Mail : vrsuya@gmail.com