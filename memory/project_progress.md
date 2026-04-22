---
name: GambleDefence 작업 진척도
description: 구현된 스크립트, 에셋 현황, 미완성 항목, 다음 작업 목록
type: project
---

## 구현 완료된 스크립트

### Core
- `GameManager.cs` — 기지 HP, 골드, 게임오버/클리어 이벤트
- `CardManager.cs` — 카드 선택 패널 관리
- `WaveManager.cs` — 웨이브 진행 제어
- `SceneLoader.cs` — 씬 전환 (MainMenu ↔ SampleScene), DontDestroyOnLoad

### UI
- `HUDManager.cs` — 인게임 HUD
- `CardSlotUI.cs` — 카드 슬롯 UI
- `CardSelectionPanel.cs` — 카드 선택 패널
- `CharacterSelectBar.cs` — 캐릭터 선택 바
- `GameResultPanel.cs` — 게임 결과 패널
- `CharacterButtonUI.cs` — 캐릭터 버튼
- `MainMenuUI.cs` — 메인화면 버튼 (게임 시작 / 종료)

### Character
- `PlacedCharacter.cs`, `PlacementManager.cs`, `PlacementTile.cs`

### Enemy
- `Enemy.cs`, `EnemySpawner.cs`, `EnemyHPBar.cs`

### Map
- `EnemyPath.cs`

### Combat
- `Projectile.cs`

### Data (ScriptableObject)
- `CardData.cs`, `CharacterData.cs`, `EnemyData.cs`, `WaveData.cs`

### Editor
- `SceneSetupWizard.cs`, `AssetCreator.cs`

## 씬 구성

| 씬 | Build Index | 설명 |
|---|---|---|
| MainMenu | 0 | 메인화면 (타이틀, 게임 시작, 종료 버튼) |
| SampleScene | 1 | 인게임 플레이 씬 |

### MainMenu 씬 Hierarchy
- Main Camera
- SceneLoader (DontDestroyOnLoad)
- EventSystem
- --- UI --- (Canvas + MainMenuUI)
  - Background
  - TitleText ("GambleDefence", 노란색 96pt)
  - SubtitleText ("카드로 싸워라, 운명을 도박하라")
  - StartButton → SceneLoader.LoadGame()
  - QuitButton → SceneLoader.QuitGame()

## 미완성 / 다음 작업 후보
- 메인화면 배경 아트 (현재 단색)
- 인게임 씬 ↔ 메인화면 복귀 버튼 (GameResultPanel에 추가 필요)
- 카드/캐릭터 데이터 ScriptableObject 인스턴스 생성
- 실제 웨이브 데이터 설정
